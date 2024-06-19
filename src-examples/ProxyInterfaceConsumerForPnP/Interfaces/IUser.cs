using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(Microsoft.SharePoint.Client.User), ImplementationOptions.None)]
    public partial interface IUser { }
}
