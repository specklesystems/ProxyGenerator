using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientRuntimeContext), ImplementationOptions.None)]
    public partial interface IClientRuntimeContext { }
}
