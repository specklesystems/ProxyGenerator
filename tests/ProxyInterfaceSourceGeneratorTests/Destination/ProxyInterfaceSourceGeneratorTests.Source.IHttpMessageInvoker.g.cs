//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/specklesystems/ProxyGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public partial interface IHttpMessageInvoker
    {
        global::System.Net.Http.HttpMessageInvoker _Instance { get; }

        [System.Runtime.Versioning.UnsupportedOSPlatformAttribute("browser")]
        global::System.Net.Http.HttpResponseMessage Send(global::System.Net.Http.HttpRequestMessage request, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> SendAsync(global::System.Net.Http.HttpRequestMessage request, global::System.Threading.CancellationToken cancellationToken);

        void Dispose();
    }
}
#nullable restore