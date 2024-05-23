using Akka.Actor;
using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForAkka.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(LocalActorRefProvider), ImplementationOptions.ProxyInterfaces)]
    public partial interface ILocalActorRefProvider { }
}
