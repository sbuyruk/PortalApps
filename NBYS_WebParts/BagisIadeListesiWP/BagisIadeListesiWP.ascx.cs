using Model.NBYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.BagisIadeListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BagisIadeListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BagisIadeListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    AyDDLDoldur();
                    YilDDLDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void AyDDLDoldur()
        {
            AyDDL.Items.Add(new ListItem(ProjeConstants.HEPSI, ProjeConstants.HEPSI_INT.ToString()));
            AyDDL.Items.Add(new ListItem("Ocak", "1"));
            AyDDL.Items.Add(new ListItem("Şubat", "2"));
            AyDDL.Items.Add(new ListItem("Mart", "3"));
            AyDDL.Items.Add(new ListItem("Nisan", "4"));
            AyDDL.Items.Add(new ListItem("Mayıs", "5"));
            AyDDL.Items.Add(new ListItem("Haziran", "6"));
            AyDDL.Items.Add(new ListItem("Temmuz", "7"));
            AyDDL.Items.Add(new ListItem("Ağustos", "8"));
            AyDDL.Items.Add(new ListItem("Eylül", "9"));
            AyDDL.Items.Add(new ListItem("Ekim", "10"));
            AyDDL.Items.Add(new ListItem("Kasım", "11"));
            AyDDL.Items.Add(new ListItem("Aralık", "12"));

            // Mevcut aya seç
            string ay = DateTime.Today.Month.ToString();
            ListItem item = AyDDL.Items.FindByValue(ay);
            if (item != null)
                AyDDL.SelectedValue = item.Value;
        }

        private void YilDDLDoldur()
        {
            int year = DateTime.Now.Year;
            for (int i = year; i >= 2005; i--)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            YilDDL.SelectedValue = year.ToString();
        }

        private void TabloOlustur()
        {
            string jsonData = TabloJson();
            string jsString = CreateDataTable(jsonData);
            UtilityHelper.ScriptCalistir(jsString);
        }

        private string TabloJson()
        {
            string jSon = string.Empty;
            try
            {
                NakitBagisHareket nbh = new NakitBagisHareket();
                jSon = nbh.SelectIadeEdilenBagislarReturnJson(AyDDL.SelectedItem.Value, YilDDL.SelectedItem.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }

        private DataTable GetIadeEdilenBagislarDataTable()
        {
            NakitBagisHareket nbh = new NakitBagisHareket();
            return nbh.SelectIadeEdilenBagislarReturnDataTable(AyDDL.SelectedItem.Value, YilDDL.SelectedItem.Value);
        }

        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
        jQuery(document).ready(function () {
            jQuery('#CustomDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'IadeTarihi' },
                    { data: 'IadeMiktari', 'className': 'text-end' },
                    { data: 'IadeSebebi' },
                    { data: 'BagisciAdiSoyadi' },
                    { data: 'BagisTarihi' },
                    { data: 'Banka' },
                    { data: 'Adres' },
                    { data: 'Ili' },
                    { data: 'Ilcesi' }
                ],
                'order': [[0, 'desc']],
                columnDefs: [
                    {
                        targets: [0, 4], render: function(data) {
                            return moment(data).format('DD.MM.YYYY');
                        }
                    }
                ],
                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                responsive: true,
                dom: 'Bfrtip',
                buttons: [
                    { extend: 'print', exportOptions: { columns: ':visible' } },
                    {
                        extend: 'excel',
                        exportOptions: {
                            columns: ':visible',
                            format: {
                                body: function(data, row, column, node) {
                                    data = $('<p>' + data + '</p>').text();
                                    return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                                }
                            }
                        }
                    },
                    { extend: 'pdf', exportOptions: { columns: ':visible' } },
                    { extend: 'copy', exportOptions: { columns: ':visible' } },
                    'pageLength', 'colvis'
                ]
            });
        });
        ";
            return tableString;
        }

        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        protected void ExportToExcel()
        {
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetIadeEdilenBagislarDataTable();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;

            string filename = "IadeEdilenBagisListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }

            GridView1.RenderControl(hw);

            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();
        }
    }
}
