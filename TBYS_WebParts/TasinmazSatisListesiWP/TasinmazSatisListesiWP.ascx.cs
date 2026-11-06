using System;
using System.ComponentModel;
using Model.Ortak;
using Model.Services.TBYS;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using TBYS_WebParts.Logging;
using Utility.Logging;
using System.Web.UI.WebControls.WebParts;

namespace TBYS_WebParts.TasinmazSatisListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazSatisListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazSatisListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var context = PortalLogContext.FromCurrentRequest("TBYS", "TasinmazSatisListesiWP");
            try
            {
                if (!Page.IsPostBack) TbysWebPartLog.RecordOpened(context);
                DataTable table = new TasinmazService(context).GetSalesList();
                var rows = new List<Dictionary<string, object>>();
                foreach (DataRow row in table.Rows)
                {
                    var item = new Dictionary<string, object>();
                    foreach (DataColumn column in table.Columns)
                    {
                        object value = row[column];
                        if (value == DBNull.Value) value = null;
                        if (value != null && column.ColumnName == "SatisTarihi")
                            value = Convert.ToDateTime(value, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        item[column.ColumnName] = value;
                    }
                    rows.Add(item);
                }
                var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
                // Prevent stored text from terminating the script element.
                string json = HttpUtility.JavaScriptStringEncode(serializer.Serialize(rows), true);
                string languageUrl = HttpUtility.JavaScriptStringEncode(UtilityHelper.TurkishTxtURLGetir(), true);
                UtilityHelper.ScriptCalistir(CreateTableScript(json, languageUrl));
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(context, "TBYS.TasinmazSatisListesiWP.Page_Load", error);
            }
        }

        private string CreateTableScript(string json, string languageUrl)
        {
            return @"
jQuery(function ($) {
    var rows = JSON.parse(" + json + @");
    $('.tasinmaz-satis-listesi').each(function () {
        var root = $(this), element = root.find('[data-role=satis-tablosu]');
        if ($.fn.dataTable.isDataTable(element[0])) return;
        var text = $.fn.dataTable.render.text();
        var table = element.DataTable({
            data: rows,
            columns: [
                { data: 'Adres', render: text }, { data: 'Ili', render: text },
                { data: 'Ilcesi', render: text }, { data: 'AdaNo', render: text },
                { data: 'ParselNo', render: text }, { data: 'Yuzolcumu', render: text },
                { data: 'VakifHissesi', render: text },
                { data: 'SatisBedeli', render: function (value, type) {
                    if (value === null) return '';
                    return type === 'display' || type === 'filter'
                        ? Number(value).toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) : value;
                } },
                { data: 'SatisTarihi', render: function (value, type) {
                    if (!value) return '';
                    return type === 'display' || type === 'filter' ? value.split('-').reverse().join('.') : value;
                } },
                { data: 'Aciklama', render: text }
            ],
            columnDefs: [{ targets: '_all', defaultContent: '' }],
            order: [[8, 'desc']], responsive: true,
            language: { url: " + languageUrl + @", decimal: ',', thousands: '.' },
            dom: 'Bfrtip',
            buttons: [
                { extend: 'excel', text: 'Excel’e Aktar', title: 'Taşınmaz Satış Listesi',
                  exportOptions: { columns: ':visible', modifier: { search: 'applied' } } },
                { extend: 'print', text: 'Yazdır', title: 'Taşınmaz Satış Listesi',
                  exportOptions: { columns: ':visible', modifier: { search: 'applied' } } },
                { extend: 'pageLength', text: 'Sayfa Uzunluğu' },
                { extend: 'colvis', text: 'Sütunlar' }
            ]
        });
        function filter(name) { return root.find('[data-filter=' + name + ']'); }
        function fillOptions(name, values) {
            var select = filter(name).empty().append($('<option>').val('').text('Tümü'));
            values.sort(function (a, b) { return a.localeCompare(b, 'tr'); });
            $.each(values, function (_, value) { select.append($('<option>').val(value).text(value)); });
        }
        function uniqueValues(column, il) {
            var values = [];
            $.each(rows, function (_, row) {
                var value = row[column];
                if ((!il || row.Ili === il) && value && $.inArray(value, values) < 0) values.push(value);
            });
            return values;
        }
        fillOptions('il', uniqueValues('Ili'));
        fillOptions('ilce', uniqueValues('Ilcesi'));
        function applyFilters() {
            var il = filter('il').val(), ilce = filter('ilce').val();
            var ada = $.trim(filter('ada').val()), parsel = $.trim(filter('parsel').val());
            var start = filter('baslangic').val(), end = filter('bitis').val();
            var invalid = start && end && start > end;
            root.find('[data-role=hata]').text(invalid ? 'Başlangıç tarihi bitiş tarihinden sonra olamaz.' : '').toggle(!!invalid);
            var filtered = $.grep(rows, function (row) {
                return !invalid && (!il || row.Ili === il) && (!ilce || row.Ilcesi === ilce)
                    && (!ada || row.AdaNo === ada) && (!parsel || row.ParselNo === parsel)
                    && (!start || (row.SatisTarihi && row.SatisTarihi >= start))
                    && (!end || (row.SatisTarihi && row.SatisTarihi <= end));
            });
            table.clear().rows.add(filtered).draw();
        }
        filter('il').on('change', function () {
            fillOptions('ilce', uniqueValues('Ilcesi', $(this).val()));
        });
        root.find('[data-filter]').on('input change', applyFilters);
        root.find('[data-action=temizle]').on('click', function () {
            root.find('[data-filter]').val('');
            fillOptions('ilce', uniqueValues('Ilcesi'));
            table.search('').columns().search('');
            applyFilters();
        });
    });
});";
        }
    }
}
