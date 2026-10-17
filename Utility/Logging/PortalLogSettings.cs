using System;
using System.Configuration;

namespace Utility.Logging
{
    /// <summary>
    /// Reads the host application's web.config. An empty configuration disables the new pipeline.
    /// Example: PortalLogging.EnabledWebParts = IKYS:EgitimGirisiWP;IKYS:YoklamaListesiWP
    /// After a whole project has migrated, add IKYS to PortalLogging.EnabledProjects.
    /// </summary>
    public static class PortalLogSettings
    {
        private const string ProjectsKey = "PortalLogging.EnabledProjects";
        private const string WebPartsKey = "PortalLogging.EnabledWebParts";

        public static bool IsEnabled(PortalLogContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Log bağlamı gereklidir.");
            return Contains(ConfigurationManager.AppSettings[ProjectsKey], context.Project)
                || (!string.IsNullOrWhiteSpace(context.WebPart)
                    && Contains(ConfigurationManager.AppSettings[WebPartsKey], context.Project + ":" + context.WebPart));
        }

        private static bool Contains(string entries, string requested)
        {
            if (string.IsNullOrWhiteSpace(entries)) return false;
            foreach (string entry in entries.Split(';'))
                if (string.Equals(entry.Trim(), requested, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }
    }
}
