using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TaahhutListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TaahhutListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TaahhutListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string AuthQS
        {
            get
            {

                if (ViewState["Auth"] == null || string.IsNullOrEmpty(ViewState["Auth"].ToString()))
                {
                    if (Page.Request.QueryString["Auth"] != null)
                    {
                        ViewState["Auth"] = Page.Request.QueryString["Auth"];
                    }
                    else
                    {
                        ViewState["Auth"] = string.Empty;
                    }
                }
                return ViewState["Auth"].ToString();
            }

            set
            {
                ViewState["Auth"] = value;
            }
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
                if (!Page.IsPostBack)
                {
                    BolgeQS = IKYSOrtak.PersonelinBolgesiniGetir(CurrentUserName);
                    if (!string.IsNullOrEmpty(BolgeQS))
                    {
                        TitleLbl.Text = "Taahhüt Listesi" + " (" + BolgeQS + " Bölgesi)";
                    }
                    TabloOlustur();
                }
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
                List<TasinmazTaahhutListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
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
        private List<TasinmazTaahhut> GetBagisciData()
        {
            TasinmazTaahhut ttDao = new TasinmazTaahhut();
            List<TasinmazTaahhut> ttlist = (string.IsNullOrEmpty(BolgeQS) ? 
                ttDao.SelectByFilters(false, false, false, ProjeConstants.BOLGE_HEPSI) : 
                ttDao.SelectByFilters(false, false, false,BolgeQS)); 
            return ttlist;
        }
        private List<TasinmazTaahhutListItem> GetDataList()
        {
            List<TasinmazTaahhutListItem> list = new List<TasinmazTaahhutListItem>();
            string pageUrl = ProjeConstants.PAGE_TASINMAZBAGISCI_GIRIS;
            List<string> bagisciTaahhutFormuDosyalari = UtilityHelper.GetFileNameListFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, ProjeConstants.DOSYA_TAAHHUT_FORMU);

            TasinmazBagisci tb= new TasinmazBagisci();
            List<TasinmazBagisci> tblist = (!string.IsNullOrEmpty(BolgeQS)? tb.SelectByBolge(BolgeQS):tb.SelectAll<TasinmazBagisci>());
            foreach (var item in tblist) 
            {
                string dosyaAdi = ProjeConstants.DOSYA_TAAHHUT_FORMU + item.Id + ".pdf";
                bool dosyaVarMi = bagisciTaahhutFormuDosyalari.Contains(dosyaAdi);
                if (dosyaVarMi)
                {
                    TasinmazTaahhutListItem tasinmazBagisciListItem = new TasinmazTaahhutListItem();
                    tasinmazBagisciListItem.TaahhutFormu = FormLinkiGetir(bagisciTaahhutFormuDosyalari, ProjeConstants.DOSYA_TAAHHUT_FORMU, item.Id.ToString(), "Taahhüt Formu", "btn btn-outline-secondary");

                    tasinmazBagisciListItem.Bagisci = (item.Adi + " " + item.Soyadi).Trim();
                    tasinmazBagisciListItem.AdiSoyadi = (item.Adi + " " + item.Soyadi).Trim();
                    tasinmazBagisciListItem.TCKimlikNo = item.TCKimlikNo;
                    tasinmazBagisciListItem.DogumTarihi = item.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                    tasinmazBagisciListItem.Sag_vefat = item.Sag_vefat;
                    tasinmazBagisciListItem.Bolge=BolgeGetir(item.Id);
                    tasinmazBagisciListItem.Duzenle = "<a href=" + pageUrl + @"?DestinationApp=TBD&BagisciId=" + item.Id + "  class='btn btn-outline-primary'>Düzenle</a>"; //"<a href=" + pageUrl + @"?DestinationApp=TBD&BagisciId=" + item.Id + "  class='btn btn-outline-primary'>Düzenle</a>";
                    tasinmazBagisciListItem.Secildi = SecilenIdQS.Equals(item.Id);
                    list.Add(tasinmazBagisciListItem);
                }
            }


            pageUrl = ProjeConstants.PAGE_TASINMAZBAGISCI_TAAHHUTLERI;
            List<TasinmazTaahhut> ttlist = GetBagisciData();
            foreach (var item in ttlist)
            {
                TasinmazTaahhutListItem tasinmazBagisciListItem = new TasinmazTaahhutListItem();
                tasinmazBagisciListItem.AdiSoyadi = (item.Adi + " " + item.Soyadi).Trim();
                tasinmazBagisciListItem.TCKimlikNo = item.TCKimlikNo;
                tasinmazBagisciListItem.DogumTarihi = item.DogumTarihi.ConvertToDatetimeEmptyIfNull();
                tasinmazBagisciListItem.Sag_vefat = item.Sag_vefat;
                TasinmazBilgileriGetir(tasinmazBagisciListItem, item);//adres,mulkiyetsekli
                tasinmazBagisciListItem.TaahhutAciklama = item.TaahhutAciklama;

                tasinmazBagisciListItem.TaahhutFormu = FormLinkiGetir(bagisciTaahhutFormuDosyalari, ProjeConstants.DOSYA_TAAHHUT_FORMU, item.BagisciId.ToString(), "Taahhüt Formu", "btn btn-outline-secondary");
                tasinmazBagisciListItem.Bolge = BolgeGetir(item.BagisciId);
                tasinmazBagisciListItem.Bagisci = BagisciBilgisiGetir(item.BagisciId);
                
                bool duzenleGorunsunMu = !string.IsNullOrEmpty(AuthQS) && AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM);
                if (duzenleGorunsunMu)
                {
                    tasinmazBagisciListItem.Duzenle = "<a href=" + pageUrl + @"?DestinationApp=TBD&BagisciId=" + item.BagisciId + "  class='btn btn-outline-primary'>Düzenle</a>";
                }

                tasinmazBagisciListItem.Secildi = SecilenIdQS.Equals(item.Id);
                //Listede yoksa ekle
                //Varsa güncelle

                var listedekiKayit= list.FirstOrDefault(o => o.TCKimlikNo != 0 && o.TCKimlikNo== tasinmazBagisciListItem.TCKimlikNo);
                if (listedekiKayit!=null)
                {
                    list.Remove(listedekiKayit);
                }
                list.Add(tasinmazBagisciListItem);
            }


            return list;
        }
        private string BolgeGetir(int bagisciId)
        {
            string retval = string.Empty;
            TasinmazBagisci tasinmazBagisci = new TasinmazBagisci();
            tasinmazBagisci = tasinmazBagisci.Select<TasinmazBagisci>(bagisciId);
            if (tasinmazBagisci != null)
            {
                Il ili = new Il();
                ili = ili.SelectByIlAdi(tasinmazBagisci.Ili.ReturnEmptyIfNull().ToString());
                if (ili != null)
                {
                    retval = ili.Bolge;
                }
            }
            return retval;
        }
        private string BagisciBilgisiGetir(int bagisciId)
        {
            TasinmazBagisci bagisci = new TasinmazBagisci();
            bagisci=bagisci.Select<TasinmazBagisci>(bagisciId);
            return bagisci != null ? (bagisci.Adi + " " + bagisci.Soyadi).Trim() : string.Empty;
        }

        private void TasinmazBilgileriGetir(TasinmazTaahhutListItem tasinmazBagisciListItem, TasinmazTaahhut taahhut)
        {
            Tasinmaz tasinmaz = new Tasinmaz();
            tasinmaz = tasinmaz.Select(taahhut.TasinmazId);
            tasinmazBagisciListItem.TasinmazAdresi = tasinmaz != null ? (tasinmaz.Adres + " " + tasinmaz.Ilcesi + " " + tasinmaz.Ili).Trim() : string.Empty;
            tasinmazBagisciListItem.MulkiyetSekli = tasinmaz != null ? tasinmaz.MulkiyetSekli: string.Empty;
        }

        private string CreateDataTable(string jsonData)
        {
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM)
                ? "{ targets:10, visible:false}," 
                : "{ targets:10, visible:true},";
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
                { data: 'Bagisci' },
                { data: 'AdiSoyadi' },
                { data: 'TCKimlikNo' },
                { data: 'DogumTarihi' },
                { data: 'Sag_vefat' },
                { data: 'TasinmazAdresi' },
                { data: 'MulkiyetSekli' },
                { data: 'Bolge' },
                { data: 'TaahhutAciklama', 'width': '30%' },
                { data: 'TaahhutFormu' },                
                { data: 'Duzenle' },

            ],
            'order': [[0, 'asc']],//Sırala
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
        protected void TasinmazBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_TASINMAZBAGISCI_LIST );
                
        }

        private class TasinmazTaahhutListItem
        {
            public string AdiSoyadi { get; set; }
            public long TCKimlikNo { get; set; }
            public string DogumTarihi { get; set; }
            public string Sag_vefat { get; set; }
            public string TasinmazAdresi { get; set; }
            public string MulkiyetSekli { get; set; }
            public string TaahhutAciklama { get; set; }
            public string TaahhutFormu { get; set; }
            public string Bagisci { get; set; }
            public string Bolge { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }

            
        }

    }
}
