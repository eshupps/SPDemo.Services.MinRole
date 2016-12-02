using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SharePoint.Administration;
using SPDemo.Services.MinRole.Utilities;

namespace SPDemo.Services.MinRole
{
    [Guid("5e08ecba-db93-4916-862b-0d4a2d30f1fa")]
    public class DemoService : SPService
    {
        internal static string DefaultName = "Demo Custom MinRole Service";
        internal static TraceLevel DefaultTraceLevel = TraceLevel.Verbose;
        private static DemoService m_local = null;

        [Persisted]
        private TraceLevel m_traceLevel;

        public DemoService() : base()
        {
        }

        public DemoService(string name, SPFarm farm) 
            : base(name, farm)
        {
            Status = SPObjectStatus.Offline;
            TraceLevel = DefaultTraceLevel;
        }

        public override string TypeName
        {
            get { return "Demo Custom MinRole Service"; }
        }

        public static DemoService Local
        {
            get
            {
                if (m_local == null)
                {
                    m_local = GetService();
                }
                return m_local;
            }
        }

        public TraceLevel TraceLevel
        {
            get { return m_traceLevel; }
            set { m_traceLevel = value; }
        }

        internal static DemoService[] GetAllServices()
        {
            List<DemoService> demoServices = new List<DemoService>();

            foreach (SPService service in SPFarm.Local.Services)
            {
                if (service is DemoService)
                {
                    demoServices.Add(service as DemoService);
                }
            }

            return demoServices.ToArray();
        }

        internal static DemoService GetService()
        {
            return GetService(SPFarm.Local);
        }

        internal static DemoService GetService(SPFarm farm)
        {
            if (farm == null)
            {
                return null;
            }
            return farm.Services.GetValue<DemoService>(DemoService.DefaultName);
        }

        private void EnsureServiceInstances()
        {
            SPFarm farm = SPFarm.Local;

            if (farm == null)
            {
                throw new Exception("This server is not part of a farm.");
            }

            foreach (SPServer server in farm.Servers)
            {
                if (DemoServiceInstance.IsSupportedServer(server))
                {
                    DemoServiceInstance serviceInstance = DemoServiceInstance.GetServiceInstance(server);

                    if (serviceInstance == null)
                    {
                        serviceInstance = new DemoServiceInstance(Guid.NewGuid().ToString(), server, this);
                        serviceInstance.Update();

                    }
                }
            }
        }

        public override void Provision()
        {
            EnsureServiceInstances();

            ProvisioningUtility.ProvisionServiceInstances(Instances, false);
            ProvisioningUtility.EnableTimerJobs(JobDefinitions);

            Status = SPObjectStatus.Online;
            AutoProvision = true;

            this.Update();
        }

        public override void Unprovision()
        {
            ProvisioningUtility.DisableTimerJobs(JobDefinitions);
            ProvisioningUtility.UnprovisionServiceInstances(Instances, true);

            Status = SPObjectStatus.Disabled;
            this.Update();

            base.Unprovision();
        }

        public override void Delete()
        {
            ProvisioningUtility.RemoveTimerJobs(JobDefinitions);
            ProvisioningUtility.RemoveServiceInstances(Instances);
            // Call the base method
            base.Delete();
        }
    }
}
