using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using Utility.Logging;

namespace Portal_WebParts.Logging
{
    // Uses the page's authenticated, event-validated callback channel; no full postback.
    internal sealed class PortalModalActivity : Control, ICallbackEventHandler
    {
        private const string TargetsKey = "PortalApps.Portal.ModalActivityTargets";
        private readonly string project;
        private readonly string webPart;
        private string result = "retry";

        private PortalModalActivity(PortalLogContext context)
        {
            ID = "PortalModalActivity";
            project = context.Project;
            webPart = context.WebPart;
        }

        internal static void Register(Control owner, PortalLogContext context, string selector)
        {
            var control = new PortalModalActivity(context);
            owner.Controls.Add(control);
            var targets = HttpContext.Current.Items[TargetsKey] as HashSet<string>;
            if (targets == null)
            {
                targets = new HashSet<string>(StringComparer.Ordinal);
                HttpContext.Current.Items[TargetsKey] = targets;
            }
            targets.Add(control.UniqueID);
            control.PreRender += delegate { control.RegisterClientScript(owner, selector); };
        }

        internal static bool IsLoggingCallback(HttpContext http)
        {
            var page = http == null ? null : http.Handler as Page;
            var targets = http == null ? null : http.Items[TargetsKey] as HashSet<string>;
            return page != null && page.IsCallback && targets != null
                && targets.Contains(http.Request.Form["__CALLBACKID"] ?? string.Empty);
        }

        private string VisitToken
        {
            get
            {
                if (ViewState["VisitToken"] == null) ViewState["VisitToken"] = Guid.NewGuid().ToString("N");
                return (string)ViewState["VisitToken"];
            }
        }

        private void RegisterClientScript(Control owner, string selector)
        {
            string callback = Page.ClientScript.GetCallbackEventReference(this,
                Quote(VisitToken), "completed", "null", "failed", true);
            string script = @"(function () {
    var key = " + Quote(UniqueID + ":" + VisitToken) + @";
    var states = window.portalModalActivityStates || (window.portalModalActivityStates = {});
    if (states[key]) return;
    var state = states[key] = { pending: false, logged: false };
    function completed(value) { state.pending = false; state.logged = value === 'ok'; }
    function failed() { state.pending = false; }
    function record(event) {
        if (!event.target || !event.target.matches(" + Quote(selector) + @")) return;
        if (state.pending || state.logged) return;
        state.pending = true;
        try {
            // Refresh ViewState and the SharePoint digest after partial postbacks.
            __theFormPostData = '';
            __theFormPostCollection = [];
            WebForm_InitCallback();
            " + callback + @";
        } catch (error) { failed(); }
    }
    document.addEventListener('shown.bs.modal', record);
})();";
            // Client blocks run before startup scripts that can automatically show a modal.
            ScriptManager.RegisterClientScriptBlock(owner, typeof(PortalModalActivity),
                UniqueID, script, true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterForEventValidation(UniqueID, VisitToken);
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            Page.ClientScript.ValidateEvent(UniqueID, eventArgument);
            if (!string.Equals(eventArgument, VisitToken, StringComparison.Ordinal)
                || Context.User == null || Context.User.Identity == null
                || !Context.User.Identity.IsAuthenticated) return;
            if (!Microsoft.SharePoint.Utilities.SPUtility.ValidateFormDigest()) return;

            PortalLogContext context = PortalLogContext.FromCurrentRequest(project, webPart);
            string cacheKey = "PortalApps.Portal.ModalOpened:" + context.UserLogin + ":" + UniqueID + ":" + VisitToken;
            // The token survives normal/partial postbacks in ViewState. Cache.Add is atomic,
            // so concurrent callbacks and repeat openings do not multiply activity rows.
            if (HttpRuntime.Cache.Add(cacheKey, true, null, DateTime.UtcNow.AddHours(8),
                Cache.NoSlidingExpiration, CacheItemPriority.Normal, null) != null)
            {
                result = "ok";
                return;
            }
            if (new DAO.Repositories.Ortak.PortalLogRepository().TryRecordWebPartOpened(context))
                result = "ok";
            else
                HttpRuntime.Cache.Remove(cacheKey);
        }

        public string GetCallbackResult() { return result; }

        private static string Quote(string value)
        {
            return "\"" + HttpUtility.JavaScriptStringEncode(value) + "\"";
        }
    }
}
