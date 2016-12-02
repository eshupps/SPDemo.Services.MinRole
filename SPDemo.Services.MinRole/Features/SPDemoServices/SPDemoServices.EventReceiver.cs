using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SPDemo.Services.MinRole.Features.SPDemoServices
{
    [Guid("44747afd-88bf-4d23-befb-1acfb5322e6f")]
    public class SPDemoServicesEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            DemoService demoService = DemoService.GetService();

            if (demoService == null)
            {
                demoService = new DemoService(DemoService.DefaultName, SPFarm.Local);
                demoService.Update(true);
            }

            if (demoService.Status != SPObjectStatus.Online)
            {
                demoService.Provision();
            }
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            DemoService demoService = DemoService.GetService();

            if (demoService != null)
            {
                demoService.Delete();
            }
        }
    }
}
