using Speckle.ProxyGenerator.Types;

namespace Speckle.ProxyGenerator.Models;

internal class ProxyData
{
    public string Namespace { get; }

    public string NamespaceDot { get; }

    public string ShortInterfaceName { get; }

    private string _fullInterfaceName;
    public string FullInterfaceName => FullQualifiedMappedTypeName ?? _fullInterfaceName;

    public string FullQualifiedTypeName { get; }

    public string? FullQualifiedMappedTypeName { get; set; }

    public string ShortMetadataName { get; }

    public string FullMetadataTypeName { get; }

    public List<string> Usings { get; }

    public ImplementationOptions Options { get; }
    public ProxyClassAccessibility Accessibility { get; }
    public string[] MembersToIgnore { get; }

    public ProxyData(
        string @namespace,
        string namespaceDot,
        string shortInterfaceName,
        string fullInterfaceName,
        string fullQualifiedTypeName,
        string shortMetadataTypeName,
        string fullMetadataTypeName,
        List<string> usings,
        ImplementationOptions options,
        ProxyClassAccessibility accessibility,
        string[] membersToIgnore
    )
    {
        Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        NamespaceDot = namespaceDot ?? throw new ArgumentNullException(nameof(namespaceDot));
        ShortInterfaceName =
            shortInterfaceName ?? throw new ArgumentNullException(nameof(shortInterfaceName));
        _fullInterfaceName =
            fullInterfaceName ?? throw new ArgumentNullException(nameof(fullInterfaceName));
        FullQualifiedTypeName =
            fullQualifiedTypeName ?? throw new ArgumentNullException(nameof(fullQualifiedTypeName));
        ShortMetadataName =
            shortMetadataTypeName ?? throw new ArgumentNullException(nameof(shortMetadataTypeName));
        FullMetadataTypeName =
            fullMetadataTypeName ?? throw new ArgumentNullException(nameof(fullMetadataTypeName));
        Usings = usings ?? throw new ArgumentNullException(nameof(usings));
        Options = options;
        Accessibility = accessibility;
        MembersToIgnore = membersToIgnore;
    }
}
