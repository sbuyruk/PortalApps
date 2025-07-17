using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.EnvanterdenCikanTasinmazListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class EnvanterdenCikanTasinmazListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public EnvanterdenCikanTasinmazListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string SecilenIdQS
        {
            get
            {

                if (ViewState["SecilenId"] == null)
                {
                    if (Page.Request.QueryString["SecilenId"] != null)
                    {
                        ViewState["SecilenId"] = Page.Request.QueryString["SecilenId"];
                    }
                    else
                    {
                        ViewState["SecilenId"] = "0";
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<EnvanterdenCikanListesiListItem> list = GetDataList();


                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper();
                exceptionHelper.Exceptions.Add(exception);
                exceptionHelper.PublishException();
            }
            return jSon;
        }
        private List<EnvanterdenCikanListesiListItem> GetDataList()
        {
            DataTable dataTable = GetDataTable();

            List<EnvanterdenCikanListesiListItem> list = new List<EnvanterdenCikanListesiListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            foreach (DataRow row in dataTable.Rows)
            {
                string tasinmazId = row["TasinmazId"].ReturnZeroIfNull().ToString();
                string kullanimSekli = row["KullanimSekli"].ToString();
                string envanterdenCikmaSebebi = row["EnvanterdenCikmaSebebi"].ToString();
                string envanterdenCikmaYili = row["EnvanterdenCikmaYili"].ToString();
                string adresIlIlce = row["AdresIlIlce"].ToString();
                string aciklama = row["Aciklama"].ToString();


                EnvanterdenCikanListesiListItem envanterdenCikanListesiListItem = new EnvanterdenCikanListesiListItem();
                envanterdenCikanListesiListItem.TasinmazId = tasinmazId;
                envanterdenCikanListesiListItem.KullanimSekli = kullanimSekli;
                envanterdenCikanListesiListItem.EnvanterdenCikmaSebebi = envanterdenCikmaSebebi;
                envanterdenCikanListesiListItem.EnvanterdenCikmaYili = envanterdenCikmaYili;
                envanterdenCikanListesiListItem.AdresIlIlce = adresIlIlce;
                envanterdenCikanListesiListItem.Aciklama = aciklama;


                envanterdenCikanListesiListItem.Tasinmaz = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZ_GIRIS + "?EnvanterdeMi=0&DestinationApp=TD&SenderApp=STL&TasinmazId=" + tasinmazId + " class='btn btn-outline-info'>Taşınmaz</a>";
                envanterdenCikanListesiListItem.Duzenle = "<a href=" + ProjeConstants.PAGE_TASINMAZ_ENVANTERDEN_CIKARMA + "?EnvanterdeMi=0&DestinationApp=ECD&SenderApp=STL&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>";
                list.Add(envanterdenCikanListesiListItem);
            }
            return list;
        }

        private DataTable GetDataTable()
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            DataTable dataTable = tasinmaz.SelectAllEnvanterdenCikanReturnDataTable();
            return dataTable;
        }
        private string CreateDataTable(string jsonData)
        {
           
            string tableString = @"
            jQuery(document).ready(function () {

                    jQuery('#CustomDataTable').DataTable({
            'initComplete': function (settings, json) {//tablo yüklendiğinde
                var api = this.api();
                var row = api.row(function (idx, data, node) { //secilen satıra gider
                    return data['TasinmazId'] == " + SecilenIdQS + @";
                });
                if (row.length > 0) {
                    row.select()
                        .show()
                        .draw(false);
                }
            },

             data: " + jsonData + @",
            columns: [
                { data: 'TasinmazId' },
                { data: 'KullanimSekli' },
                { data: 'EnvanterdenCikmaSebebi' },
                { data: 'EnvanterdenCikmaYili' },
                { data: 'AdresIlIlce' , 'width':'20%'},
                { data: 'Aciklama' , 'width':'20%'},
                { data: 'Tasinmaz' },
                { data: 'Duzenle' },
            ],
            'order': [[2, 'desc']],//AdiSoyadi Sıralı
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            dom: 'Bfrtip',
            buttons:
            [
                {
            extend: 'print',
                    exportOptions:
                {
                columns: ':visible'
                    }
            },
                {
            extend: 'excel',
                    exportOptions:
                {
                columns: ':visible'
                    }
            },
                {
            extend: 'pdf',
                    exportOptions:
                {
                columns: ':visible'
                    }
            },
                {
            extend: 'copy',
                    exportOptions:
                {
                columns: ':visible'
                    }
            },
                , 'pageLength', 'colvis'
            ]
        });
        });
        ";
            return tableString;
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
            GridView GridView1 = new GridView
            {
                AllowPaging = false,

                DataSource = GetDataTable()
            };
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=EnvanterdenCikanTasinmazListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                //Apply text style to each Row
                GridView1.Rows[i].Attributes.Add("class", "textmode");
            }
            GridView1.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Page.Response.Write(style);
            Page.Response.Output.Write(sw.ToString());
            Page.Response.Flush();
            Page.Response.End();

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
        private class EnvanterdenCikanListesiListItem
        {
            public string TasinmazId { get; set; }
            public string KullanimSekli { get; set; }
            public string EnvanterdenCikmaSebebi { get; set; }
            public string EnvanterdenCikmaYili { get; set; }
            public string AdresIlIlce { get; set; }
            public string Aciklama { get; set; }
            public string Tasinmaz { get; set; }
            public string Duzenle { get; set; }

        }
    }
}
