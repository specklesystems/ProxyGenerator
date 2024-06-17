using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy(
        typeof(Address),
        ImplementationOptions.ProxyBaseClasses,
        ProxyClassAccessibility.Public,
        new[] { "Weird" }
    )]
    public partial interface IAddress
    {
        public void Weird()
        {
            _Instance.Weird2();
        }
    }
}
