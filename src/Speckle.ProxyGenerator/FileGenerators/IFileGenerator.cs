using Speckle.ProxyGenerator.Models;

namespace Speckle.ProxyGenerator.FileGenerators;

internal interface IFileGenerator
{
    FileData GenerateFile(List<ProxyMapItem> proxyMapItems, bool supportsNullable);
}
