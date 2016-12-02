using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPDemo.Services.MinRole.Utilities
{
    public class ProvisioningUtility
    {
        public static int GetTimerJobProgress(int current, int count)
        {
            double progress; 

            double percent = Math.Min(100.0, current * 100 / count);

            progress = Math.Min(percent, 100);
            progress = Math.Max(percent, 0);

            return Convert.ToInt32(progress);
        }

        public static SPJobDefinition GetJobNoThrow(SPJobDefinitionCollection jobDefinitions, string name)
        {
            if (jobDefinitions == null)
            {
                throw new ArgumentException("jobDefinitions");
            }

            foreach (SPJobDefinition jobDefinition in jobDefinitions)
            {
                if (jobDefinition.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return jobDefinition;
                }
            }
            return null;
        }

        public static void EnableTimerJobs(SPJobDefinitionCollection jobDefinitions)
        {
            if (jobDefinitions == null && jobDefinitions.Count == 0)
            {
                return;
            }

            foreach (SPJobDefinition jobDefinition in jobDefinitions)
            {
                if (jobDefinition.IsDisabled)
                {
                    jobDefinition.IsDisabled = false;
                    jobDefinition.Update();
                }
            }
        }

        public static void DisableTimerJobs(SPJobDefinitionCollection jobDefinitions)
        {
            if (jobDefinitions == null && jobDefinitions.Count == 0)
            {
                return;
            }

            foreach (SPJobDefinition jobDefinition in jobDefinitions)
            {
                if (!jobDefinition.IsDisabled)
                {
                    jobDefinition.IsDisabled = true;
                    jobDefinition.Update();
                }
            }
        }

        public static void RemoveTimerJobs(SPJobDefinitionCollection jobDefinitions)
        {
            if (jobDefinitions == null && jobDefinitions.Count == 0)
            {
                return;
            }

            foreach (SPJobDefinition jobDefinition in jobDefinitions)
            {
                jobDefinition.Delete();
            }
        }

        public static bool ContainsServiceInstance(SPServiceInstanceCollection serviceInstances, string typeName, SPObjectStatus status)
        {
            if (serviceInstances == null)
            {
                throw new ArgumentNullException("server");
            }

            foreach (SPServiceInstance serviceInstance in serviceInstances)
            {
                if (serviceInstance.Status == status && serviceInstance.TypeName.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ProvisionServiceInstances(SPServiceInstanceDependencyCollection instances, bool wait)
        {
            if (instances == null && instances.Count == 0)
            {
                return;
            }

            foreach (SPServiceInstance instance in instances)
            {
                ProvisionServiceInstance(instance, wait);
            }
        }

        public static void ProvisionServiceInstance(SPServiceInstance instance, bool wait)
        {
            if (instance.Status == SPObjectStatus.Offline || instance.Status == SPObjectStatus.Disabled)
            {
                if (instance.Server.Id == SPServer.Local.Id)
                {
                    instance.Provision();
                }
                else
                {
                    SPServiceInstanceJobDefinition definition = new SPServiceInstanceJobDefinition(instance, true);
                    definition.Schedule = new SPOneTimeSchedule(DateTime.Now);
                    definition.Update();
                }

                if (wait)
                {
                    WaitForServiceInstanceStatus(instance, SPObjectStatus.Online, new TimeSpan(0, 1, 0));
                }
            }
        }

        public static void UnprovisionServiceInstances(SPServiceInstanceDependencyCollection instances, bool wait)
        {
            if (instances == null && instances.Count == 0)
            {
                return;
            }

            foreach (SPServiceInstance instance in instances)
            {
                UnprovisionServiceInstance(instance, wait);
            }
        }

        public static void UnprovisionServiceInstance(SPServiceInstance instance, bool wait)
        {
            if (instance.Server.Id == SPServer.Local.Id)
            {
                instance.Unprovision();
            }
            else
            {
                SPServiceInstanceJobDefinition definition = new SPServiceInstanceJobDefinition(instance, false);
                definition.Schedule = new SPOneTimeSchedule(DateTime.Now);
                definition.Update();
            }

            if (wait)
            {
                WaitForServiceInstanceStatus(instance, SPObjectStatus.Disabled, new TimeSpan(0, 1, 0));
            }
        }

        public static void RemoveServiceInstances(SPServiceInstanceDependencyCollection instances)
        {
            if (instances == null && instances.Count == 0)
            {
                return;
            }

            foreach (SPServiceInstance instance in instances)
            {
                instance.Delete();
            }
        }

        private static void WaitForServiceInstanceStatus(SPServiceInstance instance, SPObjectStatus status, TimeSpan timeout)
        {
            if (instance == null || instance.Status == status)
            {
                return;
            }

            DateTime timeoutDateTime = DateTime.Now + timeout;

            while (timeoutDateTime > DateTime.Now)
            {
                SPServiceInstance latestInstance = instance.Farm.GetObject(instance.Id) as SPServiceInstance;

                if (latestInstance == null || latestInstance.Status == status)
                {
                    return;
                }

                System.Threading.Thread.Sleep(1000);
            }

            throw new TimeoutException();
        }
    }
}
