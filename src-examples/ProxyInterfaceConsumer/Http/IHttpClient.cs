using System.Net.Http;
using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumer.Http;

[Speckle.ProxyGenerator.Proxy(typeof(HttpClient), ImplementationOptions.ProxyBaseClasses)]
public partial interface IHttpClient : IHttpMessageInvoker { }

[Speckle.ProxyGenerator.Proxy(typeof(HttpMessageInvoker), ImplementationOptions.None)]
public partial interface IHttpMessageInvoker { }
