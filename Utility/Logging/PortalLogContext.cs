using Microsoft.SharePoint;
using System;
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

        public string Project { get; private set; }
        public string WebPart { get; private set; }
        public string UserLogin { get; private set; }
        public string PagePath { get; private set; }
        public Guid OperationId { get; private set; }

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
