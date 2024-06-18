namespace ProxyInterfaceSourceGeneratorTests.Source;

public partial interface IFooNotNullable
{
    public void Test(FooEnumImplement z = FooEnumImplement.Y, IEnumerable<string>? v = null) { }
}
