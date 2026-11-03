using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using Utility.Logging;

namespace Portal_WebParts.Logging
{
    // Uses the page's authenticated, event-validated callback channel; no full postback.
    internal sealed class PortalModalActivity
    {
        private const string TargetsKey = "PortalApps.Portal.ModalActivityTargets";
        private readonly string project;
        private readonly string webPart;
        private readonly Control owner;
        private readonly StateBag viewState;
        private bool clientScriptRegistered;
        private string result = "retry";

        private PortalModalActivity(Control owner, PortalLogContext context, StateBag viewState)
        {
            this.owner = owner;
            this.viewState = viewState;
            project = context.Project;
            webPart = context.WebPart;
        }

        internal static PortalModalActivity Register(Control owner, PortalLogContext context,
            string selector, StateBag viewState)
        {
            // ASCX inline code makes the root Controls collection read-only.
            // The existing WebPart handles callbacks without adding any child controls.
            var activity = new PortalModalActivity(owner, context, viewState);
            var targets = HttpContext.Current.Items[TargetsKey] as HashSet<string>;
            if (targets == null)
            {
                targets = new HashSet<string>(StringComparer.Ordinal);
                HttpContext.Current.Items[TargetsKey] = targets;
            }
            targets.Add(owner.UniqueID);
            owner.PreRender += delegate { activity.RegisterClientScript(selector); };
            // Event validation binds to serialized ViewState, which is available only
            // after saving state. Use the page event even on partial postbacks.
            owner.Page.SaveStateComplete += delegate
            {
                if (activity.clientScriptRegistered)
                    owner.Page.ClientScript.RegisterForEventValidation(owner.UniqueID, activity.VisitToken);
            };
            return activity;
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
                const string key = "Portal.ModalActivity.VisitToken";
                if (viewState[key] == null) viewState[key] = Guid.NewGuid().ToString("N");
                return (string)viewState[key];
            }
        }

        private void RegisterClientScript(string selector)
        {
            string callback = owner.Page.ClientScript.GetCallbackEventReference(owner,
                Quote(VisitToken), "completed", "null", "failed", true);
            string script = @"(function () {
    var key = " + Quote(owner.UniqueID + ":" + VisitToken) + @";
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
                owner.UniqueID, script, true);
            clientScriptRegistered = true;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            owner.Page.ClientScript.ValidateEvent(owner.UniqueID, eventArgument);
            HttpContext http = HttpContext.Current;
            if (!string.Equals(eventArgument, VisitToken, StringComparison.Ordinal)
                || http == null || http.User == null || http.User.Identity == null
                || !http.User.Identity.IsAuthenticated) return;
            if (!Microsoft.SharePoint.Utilities.SPUtility.ValidateFormDigest()) return;

            PortalLogContext context = PortalLogContext.FromCurrentRequest(project, webPart);
            string cacheKey = "PortalApps.Portal.ModalOpened:" + context.UserLogin + ":" + owner.UniqueID + ":" + VisitToken;
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
