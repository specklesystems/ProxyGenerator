using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Speckle.ProxyGenerator.Extensions;
using Speckle.ProxyGenerator.Types;

namespace Speckle.ProxyGenerator.SyntaxReceiver;

internal static class AttributeArgumentListParser
{
    public static ProxyInterfaceGeneratorAttributeArguments ParseAttributeArguments(
        AttributeArgumentListSyntax? argumentList,
        SemanticModel semanticModel
    )
    {
        if (argumentList is null || argumentList.Arguments.Count is < 1 or > 4)
        {
            throw new ArgumentException("The ProxyAttribute requires 1, 2, 3 or 4 arguments.");
        }

        ProxyInterfaceGeneratorAttributeArguments result;
        if (
            TryParseAsType(
                argumentList.Arguments[0].Expression,
                semanticModel,
                out var fullyQualifiedDisplayString,
                out var metadataName
            )
        )
        {
            result = new ProxyInterfaceGeneratorAttributeArguments(
                fullyQualifiedDisplayString,
                metadataName
            );
        }
        else
        {
            throw new ArgumentException(
                "The first argument from the ProxyAttribute should be a Type."
            );
        }

        foreach (var argument in argumentList.Arguments.Skip(1))
        {
            if (TryParseAsStringArray(argument.Expression, out var membersToIgnore))
            {
                result = result with { MembersToIgnore = membersToIgnore };
                continue;
            }
            if (TryParseAsEnum<ProxyClassAccessibility>(argument.Expression, out var accessibility))
            {
                result = result with { Accessibility = accessibility };
            }

            if (TryParseAsEnum<ImplementationOptions>(argument.Expression, out var options))
            {
                result = result with { Options = options };
            }
        }

        return result;
    }

    private static bool TryParseAsType(
        ExpressionSyntax expressionSyntax,
        SemanticModel semanticModel,
        [NotNullWhen(true)] out string? fullyQualifiedDisplayString,
        [NotNullWhen(true)] out string? metadataName
    )
    {
        fullyQualifiedDisplayString = null;
        metadataName = null;

        if (expressionSyntax is TypeOfExpressionSyntax typeOfExpressionSyntax)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeOfExpressionSyntax.Type);
            var typeSymbol = typeInfo.Type!;
            metadataName = typeSymbol.GetFullMetadataName();
            fullyQualifiedDisplayString = typeSymbol.ToFullyQualifiedDisplayString();

            return true;
        }

        return false;
    }

    private static bool TryParseAsEnum<TEnum>(ExpressionSyntax expressionSyntax, out TEnum value)
        where TEnum : struct, Enum
    {
        var enumAsString = expressionSyntax.ToString();
        value = default;
        if (!enumAsString.Contains(typeof(TEnum).Name))
        {
            return false;
        }
        var splitter = new[] { $"{typeof(TEnum).Name}." };
        var vals = enumAsString
            .Split(splitter, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.TrimEnd(' ', '|'));

        long l = 0;
        foreach (var v in vals)
        {
            if (Enum.TryParse<TEnum>(v, out var e))
            {
                l |= Convert.ToInt64(e);
            }
        }
        value = (TEnum)Enum.ToObject(typeof(TEnum), l);
        ;
        return true;
    }

    private static bool TryParseAsStringArray(ExpressionSyntax expressionSyntax, out string[] value)
    {
        if (
            expressionSyntax
            is ImplicitArrayCreationExpressionSyntax lmplicitArrayCreationExpressionSyntax
        )
        {
            var strings = new List<string>();
            foreach (
                var expression in lmplicitArrayCreationExpressionSyntax.Initializer.Expressions
            )
            {
                if (expression.GetFirstToken().Value is string s)
                {
                    strings.Add(s);
                }
            }
            value = strings.ToArray();
            return true;
        }
        value = [];
        return false;
    }
}
