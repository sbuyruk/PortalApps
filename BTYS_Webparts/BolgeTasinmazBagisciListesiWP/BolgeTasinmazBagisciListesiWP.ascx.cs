using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace BTYS_Webparts.BolgeTasinmazBagisciListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class BolgeTasinmazBagisciListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BolgeTasinmazBagisciListesiWP()
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
                        ViewState["SecilenId"] = string.Empty;
                    }
                }
                return ViewState["SecilenId"].ToString();
            }

            set
            {
                ViewState["SecilenId"] = value;
            }
        }
        private string BolgeQS
        {
            get
            {

                if (ViewState["Bolge"] == null)
                {
                    if (Page.Request.QueryString["Bolge"] != null)
                    {
                        ViewState["Bolge"] = Page.Request.QueryString["Bolge"];
                    }
                    else
                    {
                        ViewState["Bolge"] = string.Empty;
                    }
                }
                return ViewState["Bolge"].ToString();
            }

            set
            {
                ViewState["Bolge"] = value;
            }
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                if (!string.IsNullOrEmpty(BolgeQS))
                {
                    TitleLbl.Text = "Bağışçı Listesi" + " (" + BolgeQS + " Bölgesi)";

                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
            }
        }
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl;
                Page.Response.Redirect(newUrl);
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
                List<TasinmazBagisciListItem> list = GetDataList();
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

        private List<TasinmazBagisciListItem> GetDataList()
        {

            DataTable dataTable = GetBagisciData();
            List<string> bagisciBilgiFormuDosyalari = UtilityHelper.GetFileNameListFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU);
            List<string> bagisciTaahhutFormuDosyalari = UtilityHelper.GetFileNameListFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, ProjeConstants.DOSYA_TAAHHUT_FORMU);
            List<TasinmazBagisciListItem> list = new List<TasinmazBagisciListItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                string tasinmazBagisciId = row["TasinmazBagisciId"].ToString();
                string adiSoyadi = row["AdiSoyadi"].ToString();
                string toplamBagisAdedi = row["ToplamBagisAdedi"].ToString();
                string sagVefat = row["Sag_vefat"].ToString();
                string bolge = row["Bolge"].ToString();
                string ilIlce = row["IlIlce"].ToString();
                string foto = row["Foto"].ToString();

                string pageUrl = ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS;

                TasinmazBagisciListItem tasinmazBagisciListItem = new TasinmazBagisciListItem();
                tasinmazBagisciListItem.TasinmazBagisciId = tasinmazBagisciId;
                tasinmazBagisciListItem.AdiSoyadi = adiSoyadi.Trim();
                tasinmazBagisciListItem.ToplamBagisAdedi = toplamBagisAdedi;
                tasinmazBagisciListItem.Sag_vefat = sagVefat;
                tasinmazBagisciListItem.Bolge = bolge;
                tasinmazBagisciListItem.IlIlce = ilIlce.Trim();

                tasinmazBagisciListItem.TasinmazBagisciKarti = "<a target='_blank' href=" + ProjeConstants.PAGE_TASINMAZBAGISCI_KARTI + "?SenderApp=TBL&BagisciId=" + tasinmazBagisciId + " class='btn btn-outline-info'>Bağışçı Kartı</a>";

                tasinmazBagisciListItem.BagisciBilgiFormu = FormLinkiGetir(bagisciBilgiFormuDosyalari, ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU, tasinmazBagisciId, "Bağışçı Bilgi ve Talep Formu", "btn btn-outline-secondary");
                tasinmazBagisciListItem.TaahhutFormu = FormLinkiGetir(bagisciTaahhutFormuDosyalari, ProjeConstants.DOSYA_TAAHHUT_FORMU, tasinmazBagisciId, "Taahhut Formu", "btn btn-outline-secondary");
                tasinmazBagisciListItem.Duzenle = "<a href=" + pageUrl + @"?DestinationApp=TBD&BagisciId=" + tasinmazBagisciId + "  class='btn btn-outline-primary'>Düzenle</a>";
                tasinmazBagisciListItem.Secildi = SecilenIdQS.Equals(tasinmazBagisciListItem.TasinmazBagisciId);
                list.Add(tasinmazBagisciListItem);
            }
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(BolgeQS) ? "{ targets:9, visible:true}," : "{ targets:9, visible:false},";
            string tableString = @"
            jQuery(document).ready(function() {

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function(settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //secilen Id'ye gider
                            return data['Secildi'] == true;
                        });
                        if (row.length > 0)
                        {
                            row.select()
                                .show()
                                .draw(false);
                        }
                    },
            data: " + jsonData + @",
            columns:
                    [
                { data: 'TasinmazBagisciId' },
                { data: 'AdiSoyadi' },
                { data: 'ToplamBagisAdedi' },
                { data: 'Sag_vefat' },
                { data: 'Bolge' },
                { data: 'IlIlce', 'width': '14%' },
                { data: 'BagisciBilgiFormu' },
                { data: 'TaahhutFormu' },
                { data: 'TasinmazBagisciKarti' },
                { data: 'Duzenle' },

            ],
            'order': [[0, 'asc']],//AdiSoyadi Sıralı
            columnDefs:
                [
                " + duzenleGorunsun + @"
                ],
            'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            autoWidth: false,
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
            });";
            return tableString;
        }
        private string FormLinkiGetir(List<string> list, string form, string tasinmazBagisciId, string linkText, string classString)
        {
            string belgePdfLink = string.Empty;
            string dosyaAdi = form + tasinmazBagisciId + ".pdf";
            string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
            bool dosyaVarMi = list.Contains(dosyaAdi);
            if (dosyaVarMi)
            {
                belgePdfLink = @"<a class='" + classString + "' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @">" + linkText + "</a>";
            }
            return belgePdfLink;
        }
        //private string BagisciBilgiFormuGetir( string tasinmazBagisciId)
        //{
        //    string belgePdfLink = string.Empty;
        //    string dosyaAdi = ProjeConstants.DOSYA_BAGISBILGIVETALEP_FORMU + tasinmazBagisciId + ".pdf";
        //    string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
        //    bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir() , ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
        //    if (dosyaVarMi)
        //    {
        //        belgePdfLink = @"<a class='btn btn-outline-secondary' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @">Bağışçı Bilgi ve Talep Formu</a>";
        //    }
        //    return belgePdfLink;
        //}
        private string TaahhutFormuGetir(string tasinmazBagisciId)
        {
            string belgePdfLink = string.Empty;

            string dosyaAdi = ProjeConstants.DOSYA_TAAHHUT_FORMU + tasinmazBagisciId + ".pdf";
            string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
            bool dosyaVarMi = UtilityHelper.DosyaVarMi(UtilityHelper.TbysBelgelerURLGetir(), ProjeConstants.TBYSBELGELERI_LIB, dosyaAdi);
            if (dosyaVarMi)
            {
                belgePdfLink = @"<a class='btn btn-outline-secondary' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @">Taahhüt Formu</a>";
            }
            return belgePdfLink;
        }

        private DataTable GetBagisciData()
        {
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            DataTable dataTable = tasinmazBagisci.SelectAllCountBagisAdediReturnDataTable(BolgeQS);
            return dataTable;
        }
        protected void YeniKayitBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS);
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

            GridView1.DataSource = GetBagisciData();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazBagisciRaporu" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        private class TasinmazBagisciListItem
        {
            public string TasinmazBagisciId { get; set; }
            public string AdiSoyadi { get; set; }
            public string ToplamBagisAdedi { get; set; }
            public string Sag_vefat { get; set; }
            public string Bolge { get; set; }
            public string IlIlce { get; set; }
            public string BagisciBilgiFormu { get; set; }
            public string TaahhutFormu { get; set; }
            public string TasinmazBagisciKarti { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
        }

    }
}
