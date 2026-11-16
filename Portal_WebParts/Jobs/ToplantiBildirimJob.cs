using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Model.Services.Portal;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Configuration;

namespace Portal_WebParts.Jobs
{
    [Guid("31a1210c-fd92-4a84-b153-3ff34d57926e")]
    public sealed class ToplantiBildirimJob : SPJobDefinition
    {
        public const string JobName = "portalapps-toplanti-bildirim";
        private static readonly SPDiagnosticsCategory Category = new SPDiagnosticsCategory(
            "PortalApps Toplantı Bildirimleri", TraceSeverity.Unexpected, EventSeverity.Error);

        // SharePoint needs a public parameterless constructor to restore persisted jobs.
        public ToplantiBildirimJob() { }

        public ToplantiBildirimJob(SPWebApplication application)
            : base(JobName, application, null, SPJobLockType.Job)
        {
            Title = "PortalApps Toplantı Bildirimleri";
        }

        public override string Description
        {
            get { return "Toplantı bildirim kuyruğundaki bekleyen e-postaları gönderir."; }
        }

        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                string connectionString = ReadConnectionString(WebApplication);
                var result = PortalToplantiOutboxDispatcher.ProcessPending(connectionString, 20,
                    (outboxId, operationId, error) => WriteFailure(outboxId, operationId, error));
                if (result.Processed > 0)
                {
                    SPDiagnosticsService.Local.WriteTrace(0, Category,
                        result.Incomplete > 0 ? TraceSeverity.Unexpected : TraceSeverity.Medium,
                        "WebApplicationId={0}; Processed={1}; Sent={2}; Incomplete={3}",
                        new object[] { WebApplication.Id, result.Processed, result.Sent, result.Incomplete });
                }
                if (result.Incomplete > 0)
                    throw new InvalidOperationException(
                        "Bazı toplantı bildirimleri gönderilemedi. Bildirim kuyruğunu ve ULS kayıtlarını kontrol edin.");
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                WriteFailure(0, Guid.Empty, error);
                // Do not expose configuration values or raw SMTP/SQL errors in timer job history.
                throw new InvalidOperationException(
                    "Toplantı bildirim görevi tamamlanamadı. Bildirim kuyruğunu, servis hesabının erişimini ve ULS kayıtlarını kontrol edin.");
            }
        }

        internal static string ReadConnectionString(SPWebApplication application)
        {
            if (application == null || application.IsAdministrationWebApplication)
                throw new InvalidOperationException("Toplantı bildirimleri için içerik web uygulamasını seçin.");
            if (!application.IisSettings.ContainsKey(SPUrlZone.Default))
                throw new InvalidOperationException("Web uygulamasının varsayılan bölge ayarları bulunamadı.");

            // OWSTIMER has no HTTP context. Read this application's configuration on every run;
            // never persist credentials in the SharePoint configuration database.
            string directory = application.IisSettings[SPUrlZone.Default].Path.FullName;
            if (!File.Exists(Path.Combine(directory, "web.config")))
                throw new InvalidOperationException("Web uygulamasının web.config dosyası bu sunucuda bulunamadı.");
            var map = new WebConfigurationFileMap();
            map.VirtualDirectories.Add("/", new VirtualDirectoryMapping(directory, true));
            Configuration configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, "/");
            ConnectionStringSettings connection = configuration.ConnectionStrings.ConnectionStrings["TSKGV_Connection"];
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
                throw new ConfigurationErrorsException("Web uygulamasında TSKGV_Connection bağlantısı bulunamadı.");
            return connection.ConnectionString;
        }

        private void WriteFailure(int outboxId, Guid operationId, Exception error)
        {
            try
            {
                // No recipient addresses, message bodies, connection strings or exception messages.
                SPDiagnosticsService.Local.WriteTrace(0, Category, TraceSeverity.Unexpected,
                    "WebApplicationId={0}; OutboxId={1}; OperationId={2}; ExceptionType={3}; HResult={4}",
                    new object[] { WebApplication == null ? Guid.Empty : WebApplication.Id,
                        outboxId, operationId, error.GetType().FullName, error.HResult });
            }
            catch (Exception)
            {
                // Diagnostics must not interrupt delivery or queue persistence.
            }
        }
    }
}
