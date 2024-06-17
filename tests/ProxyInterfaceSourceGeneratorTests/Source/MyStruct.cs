namespace ProxyInterfaceSourceGeneratorTests.Source;

public struct MyStruct
{
    public int Id { get; set; }

    public bool TryGetMyStruct2(int i, out MyStruct2 x, double z)
    {
        x = default;
        return true;
    }
}

public struct MyStruct2
{
    public int Id { get; set; }
}
