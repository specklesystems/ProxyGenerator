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

namespace ProxyInterfaceSourceGeneratorTests.Source.PnP
{
    public partial interface IClientContext
    {
        global::Microsoft.SharePoint.Client.ClientContext _Instance { get; }

        global::ProxyInterfaceSourceGeneratorTests.Source.PnP.IWeb Web { get; }

        global::Microsoft.SharePoint.Client.Site Site { get; }

        global::Microsoft.SharePoint.Client.RequestResources RequestResources { get; }

        global::System.Version ServerVersion { get; }

        global::Microsoft.SharePoint.Client.FormDigestInfo GetFormDigestDirect();

        void ExecuteQuery();

        global::System.Threading.Tasks.Task ExecuteQueryAsync();
    }
}
#nullable restore