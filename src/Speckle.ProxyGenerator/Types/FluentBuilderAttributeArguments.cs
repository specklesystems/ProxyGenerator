namespace Speckle.ProxyGenerator.Types;

internal record ProxyInterfaceGeneratorAttributeArguments(
    string FullyQualifiedDisplayString,
    string MetadataName
)
{
    public ImplementationOptions Options { get; set; }

    public ProxyClassAccessibility Accessibility { get; set; }
    public string[] MembersToIgnore { get; set; } = [];
}
