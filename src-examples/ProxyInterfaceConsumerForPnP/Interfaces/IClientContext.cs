using System;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using Speckle.ProxyGenerator;

namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [Speckle.ProxyGenerator.Proxy(typeof(ClientContext), ImplementationOptions.None)]
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
