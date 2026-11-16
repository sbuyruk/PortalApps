using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Portal_WebParts.Jobs;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Portal_WebParts.Features.ToplantiBildirim
{
    [Guid("11afc3dc-1175-457d-8a81-8bad87cf36e6")]
    public sealed class ToplantiBildirimEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication application = properties.Feature.Parent as SPWebApplication;
            // Validate the selected application's configuration before registering the job.
            ToplantiBildirimJob.ReadConnectionString(application);
            SPJobDefinition existing = application.JobDefinitions.FirstOrDefault(
                job => job.Name == ToplantiBildirimJob.JobName);
            if (existing != null)
            {
                if (!(existing is ToplantiBildirimJob))
                    throw new InvalidOperationException("Aynı adla tanımlanmış farklı bir zamanlanmış görev var.");
                return;
            }

            var definition = new ToplantiBildirimJob(application);
            definition.Schedule = new SPMinuteSchedule { BeginSecond = 0, EndSecond = 59, Interval = 1 };
            definition.Update();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication application = properties.Feature.Parent as SPWebApplication;
            if (application != null) RemoveJobs(application);
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            foreach (SPWebApplication application in SPWebService.ContentService.WebApplications)
                RemoveJobs(application);
        }

        private static void RemoveJobs(SPWebApplication application)
        {
            // Snapshot before deleting persisted definitions. Queue rows remain intact.
            foreach (SPJobDefinition job in application.JobDefinitions
                .Where(job => job.Name == ToplantiBildirimJob.JobName).ToList())
                job.Delete();
        }
    }
}
