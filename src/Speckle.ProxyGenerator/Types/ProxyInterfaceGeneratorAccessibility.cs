namespace Speckle.ProxyGenerator.Types;

[Flags]
internal enum ProxyClassAccessibility
{
    Public = 0,

    Internal = 1
}

[Flags]
internal enum ImplementationOptions
{
    None = 0,

   ProxyBaseClasses = 1,

   ProxyInterfaces = 2,

    UseExtendedInterfaces = 4
}
