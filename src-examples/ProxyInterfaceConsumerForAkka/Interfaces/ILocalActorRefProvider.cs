using Akka.Actor;

namespace ProxyInterfaceConsumerForAkka.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(LocalActorRefProvider))]
    public partial interface ILocalActorRefProvider { }
}
