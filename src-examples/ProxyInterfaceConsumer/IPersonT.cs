using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumer
{
    [Speckle.ProxyGenerator.Proxy(typeof(ProxyInterfaceConsumer.PersonT<>), ImplementationOptions.None)]
    public partial interface IPersonT //<T> where T : struct
    { }
}
