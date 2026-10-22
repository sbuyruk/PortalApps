using Model.NBYS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.Logging
{
    internal static class NbysSmsTable
    {
        internal static void Fill(Table table, IList<SMSAylikBagis> rows, int year, bool showTotals,
            Func<SMSAylikBagis, Control> editControl = null)
        {
            // Dynamic rows must not be restored/appended again on each postback.
            for (int index = table.Rows.Count - 1; index >= 0; index--)
                if (!(table.Rows[index] is TableHeaderRow)) table.Rows.RemoveAt(index);
            CultureInfo culture = new CultureInfo(ProjeConstants.CULTUREINFO);
            long[] totals = new long[3];
            decimal[] amounts = new decimal[3];
            foreach (SMSAylikBagis item in rows.OrderBy(item => item.Ay))
            {
                TableRow row = new TableRow { EnableViewState = false };
                Add(row, new DateTime(year, item.Ay, 1).ToString("MMMM", culture), false);
                long[] counts = { item.TurkcellSMSAdedi, item.VodafoneSMSAdedi, item.TurkTelekomSMSAdedi };
                for (int index = 0; index < counts.Length; index++)
                {
                    decimal amount = counts[index] * item.SMSTutari;
                    totals[index] += counts[index];
                    amounts[index] += amount;
                    Add(row, counts[index].ToString(culture));
                    Add(row, amount.ToString("N", culture));
                }
                long monthCount = counts.Sum();
                Add(row, monthCount.ToString(culture));
                Add(row, (monthCount * item.SMSTutari).ToString("N", culture));
                if (editControl != null)
                {
                    TableCell cell = new TableCell();
                    cell.Controls.Add(editControl(item));
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }
            if (rows.Count == 0)
            {
                TableRow row = new TableRow { EnableViewState = false };
                row.Cells.Add(new TableCell { ColumnSpan = editControl == null ? 9 : 10, Text = "Seçilen yıl için kayıt bulunamadı." });
                table.Rows.Add(row);
                return;
            }
            if (rows.Count < 12)
            {
                string[] missing = Enumerable.Range(1, 12).Where(month => !rows.Any(item => item.Ay == month))
                    .Select(month => new DateTime(year, month, 1).ToString("MMMM", culture)).ToArray();
                TableRow note = new TableRow { EnableViewState = false };
                note.Cells.Add(new TableCell { ColumnSpan = editControl == null ? 9 : 10,
                    Text = "Kayıt bulunmayan aylar: " + string.Join(", ", missing) + ". Toplamlar yalnızca mevcut kayıtları kapsar." });
                table.Rows.Add(note);
            }
            if (!showTotals) return;
            TableRow summary = new TableRow { EnableViewState = false, CssClass = "fw-bold" };
            Add(summary, "Toplam");
            for (int index = 0; index < totals.Length; index++)
            {
                Add(summary, totals[index].ToString(culture));
                Add(summary, amounts[index].ToString("N", culture));
            }
            Add(summary, totals.Sum().ToString(culture));
            Add(summary, amounts.Sum().ToString("N", culture));
            if (editControl != null) Add(summary, string.Empty);
            table.Rows.Add(summary);
        }

        private static void Add(TableRow row, string value, bool numeric = true)
        {
            row.Cells.Add(new TableCell { Text = value, CssClass = numeric ? "text-end" : string.Empty });
        }
    }
}
