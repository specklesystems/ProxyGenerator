using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(Microsoft.SharePoint.Client.Web), ImplementationOptions.None)]
    public partial interface IWeb : ISecurableObject { }
}
