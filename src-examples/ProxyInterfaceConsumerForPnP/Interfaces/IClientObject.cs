using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientObject), ImplementationOptions.None)]
    public partial interface IClientObject { }
}
