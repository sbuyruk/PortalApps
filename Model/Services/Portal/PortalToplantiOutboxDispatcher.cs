using DAO.Repositories.Portal;
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Model.Services.Portal
{
    internal static class PortalToplantiOutboxPayload
    {
        // Preserve SQL wall-clock DateTimes. JavaScriptSerializer converts them to UTC on a round trip.
        internal static string Serialize(PortalToplantiChange change)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                new XmlSerializer(typeof(PortalToplantiChange)).Serialize(writer, change);
                string payload = writer.ToString();
                if (payload.Length > 8388608) throw new InvalidOperationException("Toplantı bildirim içeriği çok uzun.");
                return payload;
            }
        }

        internal static PortalToplantiChange Deserialize(string payload)
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
                MaxCharactersInDocument = 8388608 };
            using (var input = new StringReader(payload))
            using (var reader = XmlReader.Create(input, settings))
                return (PortalToplantiChange)new XmlSerializer(typeof(PortalToplantiChange)).Deserialize(reader);
        }
    }

    public sealed class PortalToplantiDispatchResult
    {
        public int Processed { get; internal set; }
        public int Sent { get; internal set; }
        public int Incomplete { get; internal set; }
    }

    public static class PortalToplantiOutboxDispatcher
    {
        // Explicit connection information makes this usable outside an HTTP/SharePoint request.
        public static PortalToplantiDispatchResult ProcessPending(string connectionString, int batchSize = 20)
        {
            if (batchSize < 1 || batchSize > 100) throw new ArgumentOutOfRangeException("batchSize", "Parti büyüklüğü 1 ile 100 arasında olmalıdır.");
            var repository = new PortalToplantiOutboxRepository(connectionString);
            var result = new PortalToplantiDispatchResult();
            for (int index = 0; index < batchSize; index++)
            {
                PortalToplantiOutboxItem item = repository.Claim();
                if (item == null) break;
                bool complete = false;
                string error = "Bazı alıcılara bildirim gönderilemedi. Alıcı adreslerini, bildirim gruplarını ve SMTP erişimini kontrol edin.";
                try
                {
                    var change = PortalToplantiOutboxPayload.Deserialize(item.Payload);
                    if (change == null || (change.Before == null && change.After == null)
                        || change.PreviousParticipants == null || change.Participants == null)
                        throw new InvalidOperationException("Bildirim içeriği geçersiz.");
                    complete = new PortalToplantiNotificationService(repository, item).Send(change);
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception failure)
                {
                    // Never persist raw recipient data or credentials from exception messages.
                    error = "Bildirim işlenemedi. Hata türü: " + failure.GetType().Name;
                }
                // A persistence failure must stop the worker; the lease allows recovery on the next run.
                repository.Finish(item, complete, error);
                result.Processed++;
                if (complete) result.Sent++; else result.Incomplete++;
            }
            return result;
        }
    }
}
