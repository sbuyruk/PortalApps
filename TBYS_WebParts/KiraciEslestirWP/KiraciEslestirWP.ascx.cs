using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraciEslestirWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraciEslestirWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraciEslestirWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string KiraEkstreAktarmaIdQS
        {
            get
            {
                if (ViewState["KiraEkstreAktarmaId"] == null)
                {
                    if (Page.Request.QueryString["KiraEkstreAktarmaId"] != null)
                    {
                        ViewState["KiraEkstreAktarmaId"] = Page.Request.QueryString["KiraEkstreAktarmaId"];
                    }
                    else
                    {
                        ViewState["KiraEkstreAktarmaId"] = string.Empty;
                    }
                }
                return ViewState["KiraEkstreAktarmaId"].ToString();
            }

            set
            {
                ViewState["KiraEkstreAktarmaId"] = value;
            }
        }
        private string SenderAppQS
        {
            get
            {

                if (ViewState["SenderApp"] == null)
                {
                    if (Page.Request.QueryString["SenderApp"] != null)
                    {
                        ViewState["SenderApp"] = Page.Request.QueryString["SenderApp"];
                    }
                    else
                    {
                        ViewState["SenderApp"] = string.Empty;
                    }
                }
                return ViewState["SenderApp"].ToString();
            }

            set
            {
                ViewState["SenderApp"] = value;
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
        private IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(KiraEkstreAktarmaIdQS))
                    {
                        KiraEkstreAktarma ekstreAktarma = new KiraEkstreAktarma();
                        ekstreAktarma = ekstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                        KiraciAraTxt.Text = ekstreAktarma != null ? ekstreAktarma.Adi : "";
                        TabloOlustur();
                    }
                }
                
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }        
        #region customdatatable
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); 
            var jsString = CreateDataTable(jsonData); 
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraciListItem> list = GetDataList();
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
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen kayda gider
                                return data['Secildi'] == true;
                            });
                            if (row.length > 0) {
                                row.select()
                                    .show()
                                    .draw(false);
                            }
                        },
                        data: " + jsonData + @",
                        columns: [
                            { data: 'KiraciId' },
                            { data: 'AdiUnvani' },
                            { data: 'Adres' },
                            { data: 'Bolge' },
                            { data: 'IlceIl' },
                            { data: 'SozBasTar' },
                            { data: 'SozBitTar' },
                            { data: 'KiraBedeli' },
                            { data: 'Sec' },               
                        ],
                        'order': [[5, 'desc']],//SozBasTar Sıralı
                        'columnDefs': [
                            { 'width': '20%', 'targets': 1 },
                            { 'width': '25%', 'targets': 2 }

                        ],
                        'language': {
                             'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        destroy: true,
                        dom: 'frtip',
                        
                        'createdRow': function(row, data, dataIndex) {
                            if ((!data.Aktif)&&(!data.Secildi))
                            {
                                $(row).addClass('pasif-kiraci');

                            }
                        },//set row color
                        });
                    });

            ";

            return tableString;
        }
        private List<KiraciListItem> GetDataList()
        {
            KiraEkstreAktarma kea = new KiraEkstreAktarma();
            kea = kea.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
            if (kea != null)
            {
                GelenOdemeLbl.Text = "Ödenen Tutar :" + kea.Tutar.ToString("N", culturInfo) + " TL";
            }
            
            Kiraci kiraci = new Kiraci();
            DataTable dataTable = kiraci.SelectByFilterReturnDataTable(KiraciAraTxt.Text);

            List<KiraciListItem> list = new List<KiraciListItem>();
            
            if (dataTable!=null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string kiraciId = row["KiraciId"].ToString();
                    string sozlesmeId = row["SozlesmeId"].ToString();
                    string adi = row["Adi"].ToString();
                    string soyadi = row["Soyadi"].ToString();

                    string bolge = row["Bolge"].ToString();
                    string ilIlce = row["Ili"].ToString() + " " + row["Ilcesi"].ToString();
                    string adres = row["Adres"].ToString();
                    string kiraBedeli = row["KiraBedeli"].ConvertToDecimal().ToString("N", culturInfo);
                    string sozBasTar = row["SozBasTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string sozBitTar = row["SozBitTar"].ConvertToDatetime().ConvertToDatetimeEmptyIfNull();
                    string sozlesme = sozBasTar + " - " + sozBitTar;
                    bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

                    KiraciListItem kiraciItem = new KiraciListItem();
                    kiraciItem.KiraciId = kiraciId;
                    kiraciItem.AdiUnvani = (adi + " " + soyadi).Trim();

                    kiraciItem.Bolge = bolge;
                    kiraciItem.IlceIl = ilIlce;
                    kiraciItem.Adres = adres;
                    kiraciItem.KiraBedeli = kiraBedeli;
                    kiraciItem.Sozlesme = sozlesme;
                    kiraciItem.SozBasTar= sozBasTar;
                    kiraciItem.SozBitTar = sozBitTar;
                    KiraSozlesme ks = new KiraSozlesme();

                    if (!aktif)
                    {
                        ks = ks.SelectBitenSozlesmeByKiraciId(kiraciId.ConvertToInt());
                    }
                    else
                    {
                        ks = ks.SelectAktifSozlesmeByKiraciId(kiraciId.ConvertToInt());
                    }

                    int sonSozlesmeId = ks != null ? ks.Id : sozlesmeId.ConvertToInt();
                    kiraciItem.Sec = "<a href=# onclick=KiraciSec(" + kiraciId + ","+sozlesmeId+"); class='btn btn-outline-primary \'>Seç</a>";

                    kiraciItem.Aktif = aktif;
                    list.Add(kiraciItem);
                } 
            }
            return list;
        }
        #endregion
        #region class 
        private class KiraciListItem
        {
            public string KiraciId { get; set; }
            public string AdiUnvani { get; set; }
            public string Adres { get; set; }
            public string Bolge { get; set; }
            public string IlceIl { get; set; }
            public string Sozlesme { get; set; }
            public string SozBasTar { get; set; }
            public string SozBitTar { get; set; }
            public string KiraBedeli { get; set; }
            public string Teminat { get; set; }
            public bool Aktif { get; set; }
            public string Sec { get; set; }
        }
        #endregion
        protected void KiraciAraTxt_TextChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }

        protected void KiraciAraBtn_Click(object sender, EventArgs e)
        {
            TabloOlustur();
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
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
                KiraEkstreAktarma ekstreAktarma = new KiraEkstreAktarma();
                ekstreAktarma = ekstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                if ((ekstreAktarma != null) && (SenderAppQS.Equals("EkstreListesi")))//ekstrelistesinden'dan geldiyse
                {
                    newUrl += "/" + ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?SecilenId=" + KiraEkstreAktarmaIdQS ;
                }
                else
                {
                    newUrl += "/" + ProjeConstants.PAGE_KIRAEKSTRE_LIST;
                }
                Page.Response.Redirect(newUrl, true);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void EkstreListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?SecilenId=" + KiraEkstreAktarmaIdQS);
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
        protected void KiraciSecBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Kiraci kiraci = new Kiraci();
                kiraci = kiraci.Select(paramKiraciIdLbl.Value.ConvertToInt());
                if (kiraci != null)
                {
                    KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                    kiraEkstreAktarma = kiraEkstreAktarma.Select<KiraEkstreAktarma>(KiraEkstreAktarmaIdQS.ConvertToInt());
                    if (kiraEkstreAktarma != null)
                    {
                        kiraEkstreAktarma.KiraciId = kiraci.Id;
                        kiraEkstreAktarma.OdemeSebebiId = kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_DIGER_INT ? ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT : kiraEkstreAktarma.OdemeSebebiId;
                        kiraEkstreAktarma.Update();
                        RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST+ "?SecilenId="+ KiraEkstreAktarmaIdQS);
                    }
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();
            }
        }
    }
}

