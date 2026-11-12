using Microsoft.SharePoint;
using System;
using System.Globalization;
using System.Web;

namespace Utility.Logging
{
    /// <summary>
    /// One operation id is shared by all WebParts participating in the same HTTP request.
    /// Background work must pass its own user and operation id explicitly.
    /// </summary>
    public sealed class PortalLogContext
    {
        private const string OperationIdItemKey = "PortalApps.Logging.OperationId";
        private const string ErrorEntityKey = "PortalApps.Logging.ErrorEntity";

        public string Project { get; private set; }
        public string WebPart { get; private set; }
        public string UserLogin { get; private set; }
        public string PagePath { get; private set; }
        public Guid OperationId { get; private set; }
        public string EntityName { get; private set; }
        public string EntityId { get; private set; }

        // Return a separate context so another WebPart or lookup cannot change this record's identity.
        public PortalLogContext WithEntity(string entityName, int entityId)
        {
            if (string.IsNullOrWhiteSpace(entityName) || entityName.Length > 128)
                throw new ArgumentException("Geçerli bir kayıt türü gereklidir.", "entityName");
            return new PortalLogContext(Project, WebPart, UserLogin, PagePath, OperationId)
            {
                EntityName = entityName,
                EntityId = entityId > 0 ? entityId.ToString(CultureInfo.InvariantCulture) : null
            };
        }

        // Keep the failing operation's identity on the exception, even after transaction rollback.
        private void AttachEntityToError(Exception error)
        {
            if (error == null || string.IsNullOrEmpty(EntityId)) return;
            for (Exception current = error; current != null; current = current.InnerException)
                if (current.Data[ErrorEntityKey] is PortalLogContext) return;
            error.Data[ErrorEntityKey] = this;
        }

        public static T ExecuteForEntity<T>(PortalLogContext context, string entityName,
            Func<int> entityId, Func<T> action)
        {
            try { return action(); }
            catch (Exception error)
            {
                // Enrichment is best effort and must not replace the original exception.
                try
                {
                    if (context != null) context.WithEntity(entityName, entityId()).AttachEntityToError(error);
                }
                catch (Exception) { }
                throw;
            }
        }

        public static void AttachEntityToError(PortalLogContext context, string entityName, int entityId, Exception error)
        {
            // Support workflows with ref/out parameters without changing their transaction or return behavior.
            try
            {
                if (context != null) context.WithEntity(entityName, entityId).AttachEntityToError(error);
            }
            catch (Exception) { }
        }

        internal static PortalLogContext ErrorEntity(Exception error, PortalLogContext fallback)
        {
            for (Exception current = error; current != null; current = current.InnerException)
            {
                PortalLogContext entity = current.Data[ErrorEntityKey] as PortalLogContext;
                if (entity != null) return entity;
            }
            return fallback;
        }

        public PortalLogContext(string project, string webPart, string userLogin, string pagePath, Guid operationId)
        {
            if (string.IsNullOrWhiteSpace(project)) throw new ArgumentException("Proje bilgisi gereklidir.", "project");
            Project = project;
            WebPart = webPart ?? string.Empty;
            UserLogin = userLogin ?? string.Empty;
            PagePath = pagePath == null ? string.Empty
                : (pagePath.Length > 1024 ? pagePath.Substring(0, 1024) : pagePath);
            OperationId = operationId == Guid.Empty ? Guid.NewGuid() : operationId;
        }

        public static PortalLogContext FromCurrentRequest(string project, string webPart)
        {
            HttpContext http = HttpContext.Current;
            if (http == null)
                throw new InvalidOperationException("HTTP isteği bulunamadı. Arka plan işlemleri için log bağlamını açıkça oluşturun.");

            Guid operationId;
            object existing = http.Items[OperationIdItemKey];
            if (existing is Guid)
                operationId = (Guid)existing;
            else
            {
                operationId = Guid.NewGuid();
                http.Items[OperationIdItemKey] = operationId;
            }

            string userLogin = http.User != null && http.User.Identity != null
                ? http.User.Identity.Name : string.Empty;
            try
            {
                SPContext sharePoint = SPContext.Current;
                if (sharePoint != null && sharePoint.Web != null && sharePoint.Web.CurrentUser != null)
                    userLogin = sharePoint.Web.CurrentUser.LoginName;
            }
            catch (Exception)
            {
                // Keep the authenticated HTTP identity when SharePoint context is unavailable.
            }

            string pagePath = http.Request != null && http.Request.Url != null
                ? http.Request.Url.AbsolutePath : string.Empty;
            return new PortalLogContext(project, webPart, userLogin, pagePath, operationId);
        }
    }
}
