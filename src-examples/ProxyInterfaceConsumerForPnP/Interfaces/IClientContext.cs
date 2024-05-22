using System;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(ClientContext))]
    public partial interface IClientContext : IClientRuntimeContext
    {
        void Load<TSource, TTarget>(
            IClientObject clientObject,
            params Expression<Func<TSource, object>>[] retrievals
        )
            where TSource : IClientObject
            where TTarget : ClientObject;
    }
}
