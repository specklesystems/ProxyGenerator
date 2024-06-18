namespace ProxyInterfaceSourceGeneratorTests.Source;

#nullable disable
public class FooNotNullable
{
    public void Test(FooEnum z = FooEnum.Y, IEnumerable<string> v = null) { }
}

public enum FooEnum
{
    X,
    Y
}
#nullable restore
