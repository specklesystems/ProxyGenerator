using CSharp.SourceGenerators.Extensions;
using CSharp.SourceGenerators.Extensions.Models;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGeneratorTests.Source.Disposable;
using Speckle.ProxyGenerator;

namespace ProxyInterfaceSourceGeneratorTests;
[Flags]
public enum ImplementationOptions
{
    None = 0,

    ProxyBaseClasses = 1,

    ProxyInterfaces = 2,

    UseExtendedInterfaces = 4,

    ProxyForBaseInterface = 8
}
public class InheritedInterfaceTests
{
    private const string Namespace = "ProxyInterfaceSourceGeneratorTests.Source.Disposable";
    private const string OutputPath = "../../../Destination/Disposable/";
    private readonly ProxyInterfaceCodeGenerator _sut;

    public InheritedInterfaceTests()
    {
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
        _sut = new ProxyInterfaceCodeGenerator();
    }

    [Theory]
    [InlineData(ImplementationOptions.None, false)]
    [InlineData(ImplementationOptions.ProxyBaseClasses | ImplementationOptions.ProxyInterfaces, true)]
    public void GenerateFiles_InheritedInterface_InheritFromBaseClass(
        ImplementationOptions options,
        bool inheritBaseInterface
    )
    {
        var name = "Child";
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames = [$"{Namespace}.{interfaceName}.g.cs", $"{Namespace}.{proxyName}.g.cs"];
        var path = $"./Source/Disposable/{interfaceName}.cs";
        SourceFile sourceFile = CreateSourceFile(path, name,options);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var interfaceIndex = 0;
        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();

        // Assert
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList;
        bool didWeInherit = baseList is not null;
        Assert.Equal(didWeInherit, inheritBaseInterface);
    }

    [Theory]
    [InlineData("Parent")]
    [InlineData("Child")]
    public void GenerateFiles_InheritedInterface_Should_InheritTheInterface(string name)
    {
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames = [$"{Namespace}.{interfaceName}.g.cs", $"{Namespace}.{proxyName}.g.cs"];

        var path = $"./Source/Disposable/{interfaceName}.cs";
        SourceFile sourceFile = CreateSourceFile(path, name, ImplementationOptions.ProxyInterfaces | ImplementationOptions.ProxyBaseClasses | ImplementationOptions.UseExtendedInterfaces);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var interfaceIndex = 0;
        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();

        // Assert
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList;
        Assert.Equal(2, baseList?.Types.Count);
        var type1 = (QualifiedNameSyntax)baseList!.Types[0].Type;
        var type2 = (QualifiedNameSyntax)baseList.Types[1].Type;
        Assert.Equal(nameof(IDisposable), type1.Right.Identifier.Text);
        Assert.Equal(nameof(IUpdate<string>), type2.Right.Identifier.Text);
    }

    [Fact]
    public void GenerateFiles_InheritedInterface_Should_InheritTheInterfaceAndNotNew()
    {
        var className1 = "LocationPoint";
        var interfaceName1 = "IRevitLocationPointProxy";
        var proxyName1 =  $"{className1}Proxy";

        // Arrange
        var path1 = $"./Source/Disposable/{interfaceName1}.cs";

        var sourceFile1 = CreateSourceFile(path1, className1, ImplementationOptions.ProxyForBaseInterface | ImplementationOptions.UseExtendedInterfaces);
        var className2 = "Location";
        var interfaceName2 = "IRevitLocationProxy";
        var proxyName2 =  $"{className2}Proxy";

        // Arrange
        var path2 = $"./Source/Disposable/{interfaceName2}.cs";
        var sourceFile2 = CreateSourceFile(path2, className2, ImplementationOptions.ProxyForBaseInterface | ImplementationOptions.UseExtendedInterfaces);

        // Act
        var result = _sut.Execute([sourceFile1, sourceFile2]);
        string[] fileNames = [$"{Namespace}.{interfaceName1}.g.cs", $"{Namespace}.{proxyName1}.g.cs",
            $"{Namespace}.{interfaceName2}.g.cs", $"{Namespace}.{proxyName2}.g.cs"];

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);

        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index + 1]; // +1 means skip the attribute
            File.WriteAllText($"{OutputPath}{fileName.fileName}", builder.Text);
        }
    }
    [Fact]
    public void GenerateFiles_InheritedInterface_Should_Not_InheritExplicitImplementedInterfaces()
    {
        var name = "Explicit";
        var interfaceName = "I" + name;
        var proxyName = name + "Proxy";

        // Arrange
        string[] fileNames = [$"{Namespace}.{interfaceName}.g.cs", $"{Namespace}.{proxyName}.g.cs"];
        var path = $"./Source/Disposable/{interfaceName}.cs";
        SourceFile sourceFile = CreateSourceFile(path, name, ImplementationOptions.UseExtendedInterfaces);

        // Act
        var result = _sut.Execute([sourceFile]);

        result.Valid.Should().BeTrue();
        result.Files.Should().HaveCount(fileNames.Length + 1);
        WriteFiles(fileNames, result);

        var interfaceIndex = 0;
        var tree = result.Files[interfaceIndex].SyntaxTree;
        var root = tree.GetRoot();
        var interfaceDeclarations = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>();

        // Assert
        //This actually could work, we just need to implenent the logic inside the Proxy (and interface).
        //⚠ Dispose is not a public member of the 'Explicit' class and also not of the Proxy.
        //e.g. new Explicit().Dipose() is not possible.
        Assert.Single(interfaceDeclarations);
        var baseList = interfaceDeclarations.First().BaseList;
        bool noInterfaceImplementationFound = baseList is null;
        Assert.True(noInterfaceImplementationFound);
    }

    private static SourceFile CreateSourceFile(string path, string name,  ImplementationOptions options)
    {
        var o = string.Empty;
        foreach (var val in Enum.GetValues<ImplementationOptions>())
        {
            if (val == ImplementationOptions.None || !options.HasFlag(val))
            {
                continue;
            }
            if (o.Length > 0)
            {
                o += " | ";
            }

            o += "ImplementationOptions." + val;
        }
        if (o.Length == 0)
        {
            o  = "ImplementationOptions.None";
        }
        return new SourceFile
        {
            Path = path,
            Text = File.ReadAllText(path),
            AttributeToAddToInterface = new ExtraAttribute
            {
                Name = "Speckle.ProxyGenerator.Proxy",
                ArgumentList = $"typeof({Namespace}.{name}), {o}"
            }
        };
    }

    private static void WriteFiles(string[] fileNames, ExecuteResult result)
    {
        foreach (var fileName in fileNames.Select((fileName, index) => new { fileName, index }))
        {
            var builder = result.Files[fileName.index]; // attribute is always last
            builder.Path.Should().EndWith(fileName.fileName);
            File.WriteAllText($"{OutputPath}{fileName.fileName}", builder.Text);
            builder.Text.Should().Be(File.ReadAllText($"{OutputPath}{fileName.fileName}"));
        }
    }
}

