using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;
using SPDemo.Services.MinRole.Utilities;

namespace SPDemo.Services.MinRole
{
    [Guid("892ad9bc-a187-4d7f-940c-a9656fe97007")]
    public class DemoServiceInstance : SPServiceInstance
    {
        internal static string ServiceInstanceNameFormat = "Demo Service Instance ({0})";

        public DemoServiceInstance()
        {
        }

        public DemoServiceInstance(string name, SPServer server, DemoService service)
            : base(name, server, service)
        {
        }

        public override bool ShouldProvision(SPServerRole serverRole)
        {
            return SPServerRole.SingleServerFarm == serverRole || SPServerRole.Application == serverRole || SPServerRole.WebFrontEnd == serverRole;
        }

        public override string TypeName
        {
            get { return "Demo Custom MinRole Service"; }
        }

        internal static DemoServiceInstance GetServiceInstance()
        {
            return GetServiceInstance(SPServer.Local);
        }

        internal static DemoServiceInstance GetServiceInstance(SPServer server)
        {
            if (server == null)
            {
                return null;
            }
            return server.ServiceInstances.GetValue<DemoServiceInstance>();
        }

        internal static DemoServiceInstance GetServiceInstance(SPServer server, Guid instanceId)
        {
            if (server == null || instanceId == Guid.Empty)
            {
                return null;
            }
            return server.ServiceInstances.GetValue<DemoServiceInstance>(instanceId);
        }

        internal static bool IsSupportedServer(SPServer server)
        {
            // Must provide a server
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            if (server.Role == SPServerRole.WebFrontEnd || server.Role == SPServerRole.SingleServerFarm || server.Role == SPServerRole.Application || server.Role == SPServerRole.Custom)
            {
                return true;
            }

            // No service instances
            if (server.ServiceInstances != null)
            {
                if (ProvisioningUtility.ContainsServiceInstance(server.ServiceInstances, "Microsoft SharePoint Foundation Web Application", SPObjectStatus.Online))
                {
                    return true;
                }

                if (ProvisioningUtility.ContainsServiceInstance(server.ServiceInstances, "Central Administration", SPObjectStatus.Online))
                {
                    return true;
                }
            }

            return false;
        }

        public override SPActionLink ManageLink
        {
            get
            {
                return new SPActionLink(string.Format(
                    "/_admin/SPDemo/Services/ManageServiceInstance.aspx?ServerId={0}&&InstanceId={1}",
                    Server.Id,
                    Id));
            }
        }
    }
}
