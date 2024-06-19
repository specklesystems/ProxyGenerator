using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(Microsoft.SharePoint.Client.SecurableObject), ImplementationOptions.None)]
    public partial interface ISecurableObject : IClientObject
    {
        // public virtual void X();
    }
}
