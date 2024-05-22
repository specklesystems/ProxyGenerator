using Akka.Remote;

// namespace ProxyInterfaceConsumerForAkka.Interfaces;     <-- no namespace

[Speckle.ProxyGenerator.Proxy(typeof(AddressUid))]
// ReSharper disable once CheckNamespace
public partial interface IAddressUid { }
