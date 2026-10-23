using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Utility.HelperClasses;
using Utility.Logging;

namespace MFYS_WebParts.Logging
{
    internal sealed class MfysInputException : Exception
    {
        internal MfysInputException(string message) : base(message) { }
    }

    internal static class MfysWebPartLog
    {
        internal static bool HasFailed(PortalLogContext context)
        {
            return HttpContext.Current != null && object.Equals(HttpContext.Current.Items["PortalApps.MFYS.Failed." + context.WebPart], true);
        }

        internal static void ExecuteAction(PortalLogContext context, string operation, Action action, Action clear)
        {
            if (HasFailed(context)) return;
            try { action(); }
            catch (ThreadAbortException) { throw; }
            catch (MfysInputException warning)
            {
                Fail(context); clear(); Messages.Warning(warning.Message);
            }
            catch (Exception error)
            {
                Fail(context); clear();
                PortalTechnicalLog.WriteError(context, "MFYS." + context.WebPart + "." + operation, error);
                Messages.Error("Rapor hazırlanamadı. Lütfen yeniden deneyin.", referenceCode: context.OperationId.ToString("N"));
            }
        }

        private static void Fail(PortalLogContext context)
        {
            if (HttpContext.Current != null) HttpContext.Current.Items["PortalApps.MFYS.Failed." + context.WebPart] = true;
        }

        internal static void RecordOpened(PortalLogContext context) { new PortalLogRepository().TryRecordWebPartOpened(context); }

        internal static string Json(object value)
        {
            return new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(value)
                .Replace("<", "\\u003c").Replace(">", "\\u003e").Replace("&", "\\u0026")
                .Replace("\u2028", "\\u2028").Replace("\u2029", "\\u2029");
        }

        internal static string DataTableScript(string data, string[] columns, int[] numericColumns, string title, string fileName, int pageLength)
        {
            return @"(function() {
                var table = jQuery('#CustomDataTable');
                if (!table.length) return;
                if (jQuery.fn.DataTable.isDataTable(table[0])) table.DataTable().destroy();
                table.find('tbody').empty();
                table.show();
                if (jQuery.fn.dataTable.moment) jQuery.fn.dataTable.moment('DD.MM.YYYY');
                var columns = " + Json(columns) + @".map(function(name) {
                    return { data: name, render: jQuery.fn.dataTable.render.text() };
                });
                table.DataTable({
                    data: " + data + @",
                    columns: columns,
                    columnDefs: [{ targets: " + Json(numericColumns) + @", className: 'dt-body-right' }],
                    language: { url: " + Json(UtilityHelper.TurkishTxtURLGetir()) + @", decimal: ',', thousands: '.' },
                    responsive: true,
                    pageLength: " + pageLength.ToString(CultureInfo.InvariantCulture) + @",
                    dom: 'Bfrtip',
                    buttons: [
                        { extend: 'print', title: " + Json(title) + @", exportOptions: { columns: ':visible' } },
                        { extend: 'excel', title: " + Json(title) + @", filename: " + Json(fileName) + @", exportOptions: { columns: ':visible' } },
                        { extend: 'pdf', title: " + Json(title) + @", filename: " + Json(fileName) + @", exportOptions: { columns: ':visible' } },
                        { extend: 'copy', exportOptions: { columns: ':visible' } }, 'pageLength', 'colvis'
                    ]
                });
            })();";
        }

        internal static void ReadPeriod(DropDownList years, DropDownList months, out DateTime from, out DateTime until)
        {
            int year, month;
            if (!int.TryParse(years.SelectedValue, out year) || year < 1753 || year > 9998
                || !int.TryParse(months.SelectedValue, out month) || month < 0 || month > 12)
                throw new MfysInputException("Geçerli bir yıl ve ay seçin.");
            from = new DateTime(year, month == 0 ? 1 : month, 1);
            until = month == 0 ? from.AddYears(1) : from.AddMonths(1);
        }

        internal static DateTime ReadDay(string value)
        {
            DateTime day;
            if (!DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out day)
                || day < new DateTime(1753, 1, 1) || day.Date == DateTime.MaxValue.Date)
                throw new MfysInputException("Geçerli bir bağış tarihi girin (gg.aa.yyyy).");
            return day.Date;
        }

        internal static string RequireSelection(DropDownList list, string message, bool allowEmpty = false)
        {
            if (list.SelectedItem == null || (!allowEmpty && string.IsNullOrWhiteSpace(list.SelectedValue))) throw new MfysInputException(message);
            return list.SelectedValue;
        }

        internal static void FillMonths(DropDownList list)
        {
            list.Items.Clear();
            for (int month = 1; month <= 12; month++)
                list.Items.Add(new ListItem(new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.GetCultureInfo("tr-TR")), month.ToString(CultureInfo.InvariantCulture)));
            list.Items.Add(new ListItem("Tüm Aylar", "0"));
            list.SelectedValue = DateTime.Today.Month.ToString(CultureInfo.InvariantCulture);
        }

        internal static void FillYears(DropDownList list)
        {
            list.Items.Clear();
            for (int offset = 0; offset < 2; offset++) list.Items.Add((DateTime.Today.Year - offset).ToString(CultureInfo.InvariantCulture));
        }
    }
}
