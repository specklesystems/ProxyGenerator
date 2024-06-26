using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Speckle.ProxyGenerator.Builders;
using Speckle.ProxyGenerator.Enums;
using Speckle.ProxyGenerator.Extensions;
using Speckle.ProxyGenerator.Models;

namespace Speckle.ProxyGenerator.FileGenerators;

internal abstract class BaseGenerator
{
    protected const string globalPrefix = "global::";
    protected readonly Context Context;
    protected readonly bool SupportsNullable;

    protected BaseGenerator(Context context, bool supportsNullable)
    {
        Context = context;
        SupportsNullable = supportsNullable;
    }

    protected FixedType GetPropertyType(IPropertySymbol property, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, null, out isReplaced);
    }

    protected FixedType GetParameterType(IParameterSymbol property, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, property.GetDefaultValue(), out isReplaced);
    }

    protected bool TryFindProxyDataByTypeName(
        string type,
        [NotNullWhen(true)] out ProxyData? proxyData
    )
    {
        proxyData = Context.Candidates.Values.FirstOrDefault(x =>
            x.FullQualifiedMappedTypeName == type
        );
        if (proxyData is not null)
        {
            return true;
        }
        proxyData = Context.Candidates.Values.FirstOrDefault(x => x.FullQualifiedTypeName == type);
        return proxyData != null;
    }

    protected string GetWhereStatementFromMethod(IMethodSymbol method)
    {
        if (!method.IsGenericMethod)
        {
            return string.Empty;
        }

        var list = new List<string>();
        foreach (var typeParameterSymbol in method.TypeParameters)
        {
            if (TryGetWhereConstraints(typeParameterSymbol, false, out var constraint))
            {
                list.Add(constraint.ToString());
            }
        }

        return string.Concat(list);
    }

    protected string ResolveInterfaceNameWithOptionalTypeConstraints(
        INamedTypeSymbol namedTypeSymbol,
        string interfaceName
    )
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return interfaceName;
        }

        var str = new StringBuilder(
            $"{interfaceName}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(ta => ta.Name))}>"
        );

        foreach (
            var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>()
        )
        {
            if (TryGetWhereConstraints(typeParameterSymbol, false, out var constraint))
            {
                str.Append(constraint);
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// https://www.codeproject.com/Articles/871704/Roslyn-Code-Analysis-in-Easy-Samples-Part-2
    /// </summary>
    public bool TryGetWhereConstraints(
        ITypeParameterSymbol typeParameterSymbol,
        bool replaceIt,
        [NotNullWhen(true)] out ConstraintInfo? constraint
    )
    {
        var constraints = new List<string>();
        if (typeParameterSymbol.HasReferenceTypeConstraint)
        {
            constraints.Add("class");
        }

        if (typeParameterSymbol.HasValueTypeConstraint)
        {
            constraints.Add("struct");
        }

        foreach (
            var namedTypeSymbol in typeParameterSymbol.ConstraintTypes.OfType<INamedTypeSymbol>()
        )
        {
            if (replaceIt)
            {
                var (_, type) = GetReplacedTypeAsString(namedTypeSymbol, null, out _);
                constraints.Add(type);
            }
            else
            {
                constraints.Add(namedTypeSymbol.GetFullType());
            }
        }

        // The new() constraint must be the last constraint specified.
        if (typeParameterSymbol.HasConstructorConstraint)
        {
            constraints.Add("new()");
        }

        if (constraints.Count > 0)
        {
            constraint = new(typeParameterSymbol.Name, constraints);
            return true;
        }

        constraint = null;
        return false;
    }

    internal readonly SymbolDisplayFormat NullableDisplayFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    protected FixedType GetReplacedTypeAsString(
        ITypeSymbol typeSymbol,
        string? defaultValue,
        out bool isReplaced
    )
    {
        isReplaced = false;

        var typeSymbolAsString = typeSymbol.ToFullyQualifiedDisplayString();
        var nullableTypeSymbolAsString = typeSymbol.ToDisplayString(
            NullableFlowState.None,
            NullableDisplayFormat
        );

        if (TryFindProxyDataByTypeName(typeSymbolAsString, out var existing))
        {
            if (!Context.ReplacedTypes.ContainsKey(typeSymbolAsString))
            {
                Context.ReplacedTypes.Add(typeSymbolAsString, existing.FullInterfaceName);
            }

            isReplaced = true;
            return FixType(existing.FullInterfaceName, typeSymbol.NullableAnnotation, defaultValue);
        }

        ITypeSymbol[] typeArguments;
        if (typeSymbol is INamedTypeSymbol namedTypedSymbol1)
        {
            typeArguments = namedTypedSymbol1.TypeArguments.ToArray();
        }
        else if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            typeArguments = [arrayTypeSymbol.ElementType];
        }
        else
        {
            return FixType(typeSymbolAsString, typeSymbol.NullableAnnotation, defaultValue);
        }

        var propertyTypeAsStringToBeModified = nullableTypeSymbolAsString;
        foreach (var typeArgument in typeArguments)
        {
            var typeArgumentAsString = typeArgument.ToFullyQualifiedDisplayString();

            if (TryFindProxyDataByTypeName(typeArgumentAsString, out var existingTypeArgument))
            {
                isReplaced = true;

                if (!Context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                {
                    Context.ReplacedTypes.Add(
                        typeArgumentAsString,
                        existingTypeArgument.FullInterfaceName
                    );
                }

                propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(
                    typeArgumentAsString,
                    existingTypeArgument.FullInterfaceName
                );
            }
        }

        return FixType(
            propertyTypeAsStringToBeModified,
            typeSymbol.NullableAnnotation,
            defaultValue
        );
    }

    protected bool TryGetNamedTypeSymbolByFullName(
        TypeKind[] kind,
        string name,
        IEnumerable<string> usings,
        [NotNullWhen(true)] out ClassSymbol? classSymbol
    )
    {
        classSymbol = default;
        if (name.StartsWith(globalPrefix, StringComparison.Ordinal))
        {
            name = name.Substring(globalPrefix.Length);
        }

        // The GetTypeByMetadataName method returns null if no type matches the full name or if 2 or more types (in different assemblies) match the full name.
        var symbol = Context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(name);

        if (symbol is not null && kind.Contains(symbol.TypeKind))
        {
            classSymbol = new ClassSymbol(
                symbol,
                symbol.GetBaseTypes(),
                symbol.AllInterfaces.ToList()
            );
            return true;
        }

        foreach (var @using in usings)
        {
            symbol = Context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(
                $"{@using}.{name}"
            );
            if (symbol is not null && kind.Contains(symbol.TypeKind))
            {
                classSymbol = new ClassSymbol(
                    symbol,
                    symbol.GetBaseTypes(),
                    symbol.AllInterfaces.ToList()
                );
                return true;
            }
        }

        return false;
    }

    protected IReadOnlyList<string> GetMethodParameters(
        ImmutableArray<IParameterSymbol> parameterSymbols,
        bool includeType
    )
    {
        var methodParameters = new List<string>();
        foreach (var parameterSymbol in parameterSymbols)
        {
            string? type = null;
            if (includeType)
            {
                if (parameterSymbol.GetTypeEnum() == TypeEnum.Complex)
                {
                    (_, type) = GetParameterType(parameterSymbol, out _);
                }
                else
                {
                    (_, type) = FixType(
                        parameterSymbol.Type.ToFullyQualifiedDisplayString(),
                        parameterSymbol.NullableAnnotation,
                        parameterSymbol.GetDefaultValue()
                    );
                }
            }

            methodParameters.Add(MethodParameterBuilder.Build(parameterSymbol, type));
        }

        return methodParameters;
    }

    protected IReadOnlyList<ProxyData> GetExtendsProxyData(
        ClassSymbol targetClassSymbol,
        bool useFullQualifiedMappedTypeName
    )
    {
        var extendsProxyClasses = new List<ProxyData>();
        foreach (var baseType in targetClassSymbol.BaseTypes)
        {
            var candidate = Context.Candidates.Values.FirstOrDefault(ci =>
                ci.FullQualifiedTypeName == baseType.ToFullyQualifiedDisplayString()
            );
            if (useFullQualifiedMappedTypeName)
            {
                //is a candidate and overrides
            }
            else
            {
                if (candidate != null)
                {
                    extendsProxyClasses.Add(candidate);
                    break;
                }
            }
        }
        return extendsProxyClasses;
    }

    internal FixedType FixType(
        string type,
        NullableAnnotation nullableAnnotation,
        string? defaultValue
    )
    {
        var na = nullableAnnotation;
        if (SupportsNullable && defaultValue == " = null")
        {
            na = NullableAnnotation.Annotated;
        }
        if (na == NullableAnnotation.Annotated && !type.EndsWith("?", StringComparison.Ordinal))
        {
            return new(true, $"{type}?");
        }
        return new(false, type);
    }
}

internal record FixedType(bool Fixed, string Type);
