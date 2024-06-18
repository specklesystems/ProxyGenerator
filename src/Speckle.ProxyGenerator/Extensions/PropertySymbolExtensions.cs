using Microsoft.CodeAnalysis;
using Speckle.ProxyGenerator.Enums;
using Speckle.ProxyGenerator.FileGenerators;

namespace Speckle.ProxyGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static TypeEnum GetTypeEnum(this IPropertySymbol p) => p.Type.GetTypeEnum();
}
