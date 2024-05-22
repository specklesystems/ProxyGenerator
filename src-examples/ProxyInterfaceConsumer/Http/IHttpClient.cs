using System.Net.Http;

namespace ProxyInterfaceConsumer.Http;

[Speckle.ProxyGenerator.Proxy(typeof(HttpClient), true)]
public partial interface IHttpClient : IHttpMessageInvoker { }

[Speckle.ProxyGenerator.Proxy(typeof(HttpMessageInvoker))]
public partial interface IHttpMessageInvoker { }
