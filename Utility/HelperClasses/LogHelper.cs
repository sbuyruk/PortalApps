using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;

namespace Utility.HelperClasses
{
    public class LogHelper : SPDiagnosticsServiceBase
    {
        public static string MaventionDiagnosticAreaName = "TSKGV NBYS";
        private const string PortalLoggingCategoryName = "PortalApps Logging";
        private static LogHelper _Current;
        public static LogHelper Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new LogHelper();
                }

                return _Current;
            }
        }

        private LogHelper() : base(MaventionDiagnosticAreaName, SPFarm.Local)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(MaventionDiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory(MaventionDiagnosticAreaName, TraceSeverity.Unexpected, EventSeverity.Error),
                    new SPDiagnosticsCategory(PortalLoggingCategoryName, TraceSeverity.Medium, EventSeverity.Error)
                })
            };

            return areas;
        }


        public static void WriteTrace(Exception ex)
        {
            SPDiagnosticsCategory category = LogHelper.Current.Areas[MaventionDiagnosticAreaName].Categories[MaventionDiagnosticAreaName];
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                LogHelper.Current.WriteTrace(0, category, TraceSeverity.Unexpected, "{0}", new object[] { ex });
            });
        }

        public static void WritePortalTrace(TraceSeverity severity, string message)
        {
            SPDiagnosticsCategory category = Current.Areas[MaventionDiagnosticAreaName]
                .Categories[PortalLoggingCategoryName];
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                Current.WriteTrace(0, category, severity, "{0}", new object[] { message });
            });
        }
    }
}
