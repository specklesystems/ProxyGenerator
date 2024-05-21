using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy(typeof(Address), false, ProxyClassAccessibility.Public, new[] { "Weird" })]
    public partial interface IAddress
    {
        public void Weird()
        {
            _Instance.Weird2();
        }
    }
}
