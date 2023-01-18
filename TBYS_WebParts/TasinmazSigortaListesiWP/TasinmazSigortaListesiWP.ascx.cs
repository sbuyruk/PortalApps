using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.TasinmazSigortaListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class TasinmazSigortaListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public TasinmazSigortaListesiWP()
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
        private string MesajQS
        {
            get
            {

                if (ViewState["Mesaj"] == null)
                {
                    if (Page.Request.QueryString["Mesaj"] != null)
                    {
                        ViewState["Mesaj"] = Page.Request.QueryString["Mesaj"];
                    }
                    else
                    {
                        ViewState["Mesaj"] = string.Empty;
                    }
                }
                return ViewState["Mesaj"].ToString();
            }

            set
            {
                ViewState["Mesaj"] = value;
            }
        }
        private string SigortaCinsiQS
        {
            get
            {

                if (ViewState["SigortaCinsi"] == null)
                {
                    if (Page.Request.QueryString["SigortaCinsi"] != null)
                    {
                        ViewState["SigortaCinsi"] = Page.Request.QueryString["SigortaCinsi"];
                    }
                    else
                    {
                        ViewState["SigortaCinsi"] = string.Empty;
                    }
                }
                return ViewState["SigortaCinsi"].ToString();
            }

            set
            {
                ViewState["SigortaCinsi"] = value;
            }
        }
        private string DepremQS
        {
            get
            {

                if (ViewState["Deprem"] == null)
                {
                    if (Page.Request.QueryString["Deprem"] != null)
                    {
                        ViewState["Deprem"] = Page.Request.QueryString["Deprem"];
                    }
                    else
                    {
                        ViewState["Deprem"] = string.Empty;
                    }
                }
                return ViewState["Deprem"].ToString();
            }

            set
            {
                ViewState["Deprem"] = value;
            }
        }
        private string YanginQS
        {
            get
            {

                if (ViewState["Yangin"] == null)
                {
                    if (Page.Request.QueryString["Yangin"] != null)
                    {
                        ViewState["Yangin"] = Page.Request.QueryString["Yangin"];
                    }
                    else
                    {
                        ViewState["Yangin"] = string.Empty;
                    }
                }
                return ViewState["Yangin"].ToString();
            }

            set
            {
                ViewState["Yangin"] = value;
            }
        }
        private string Makine5000QS
        {
            get
            {

                if (ViewState["Makine5000"] == null)
                {
                    if (Page.Request.QueryString["Makine5000"] != null)
                    {
                        ViewState["Makine5000"] = Page.Request.QueryString["Makine5000"];
                    }
                    else
                    {
                        ViewState["Makine5000"] = string.Empty;
                    }
                }
                return ViewState["Makine5000"].ToString();
            }

            set
            {
                ViewState["Makine5000"] = value;
            }
        }
        private string Makine100000QS
        {
            get
            {

                if (ViewState["Makine100000"] == null)
                {
                    if (Page.Request.QueryString["Makine100000"] != null)
                    {
                        ViewState["Makine100000"] = Page.Request.QueryString["Makine100000"];
                    }
                    else
                    {
                        ViewState["Makine100000"] = string.Empty;
                    }
                }
                return ViewState["Makine100000"].ToString();
            }

            set
            {
                ViewState["Makine100000"] = value;
            }
        }
        private string JeneratorQS
        {
            get
            {

                if (ViewState["Jenerator"] == null)
                {
                    if (Page.Request.QueryString["Jenerator"] != null)
                    {
                        ViewState["Jenerator"] = Page.Request.QueryString["Jenerator"];
                    }
                    else
                    {
                        ViewState["Jenerator"] = string.Empty;
                    }
                }
                return ViewState["Jenerator"].ToString();
            }

            set
            {
                ViewState["Jenerator"] = value;
            }
        }
        private string AsansorQS
        {
            get
            {

                if (ViewState["Asansor"] == null)
                {
                    if (Page.Request.QueryString["Asansor"] != null)
                    {
                        ViewState["Asansor"] = Page.Request.QueryString["Asansor"];
                    }
                    else
                    {
                        ViewState["Asansor"] = string.Empty;
                    }
                }
                return ViewState["Asansor"].ToString();
            }

            set
            {
                ViewState["Asansor"] = value;
            }
        }
        private string KazanQS
        {
            get
            {

                if (ViewState["Kazan"] == null)
                {
                    if (Page.Request.QueryString["Kazan"] != null)
                    {
                        ViewState["Kazan"] = Page.Request.QueryString["Kazan"];
                    }
                    else
                    {
                        ViewState["Kazan"] = string.Empty;
                    }
                }
                return ViewState["Kazan"].ToString();
            }

            set
            {
                ViewState["Kazan"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(MesajQS))
                {
                    MessageHelper.PublishMessage("Sigorta Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    MesajQS = string.Empty;
                }
                if (!Page.IsPostBack)
                {
                    if (SigortaCinsiQS.Equals("Deprem İhtiyari"))
                        SigortaCinsiQS = ProjeConstants.SIGORTA_DEPREM_IHTIYARI;
                    else if (SigortaCinsiQS.Equals("DASK İhtiyari"))
                        SigortaCinsiQS = ProjeConstants.SIGORTA_DASK_IHTIYARI;
                    else if (SigortaCinsiQS.Equals(ProjeConstants.SIGORTA_YOK))
                        SigortaCinsiQS = ProjeConstants.SIGORTA_YOK;
                    else
                        SigortaCinsiQS = ProjeConstants.HEPSI;
                    SigortaCinsiDDLDoldur();
                    SigortaCinsiDDL.SelectedValue = string.IsNullOrEmpty(SigortaCinsiQS) ? ProjeConstants.HEPSI : SigortaCinsiQS;
                    TeminatSecimleriniDoldur();
                    TabloOlustur();
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void TeminatSecimleriniDoldur()
        {

            DepremChk.Checked = string.IsNullOrEmpty(DepremQS) ? true : DepremQS.ConvertToBool();
            YanginChk.Checked = string.IsNullOrEmpty(YanginQS) ? true : YanginQS.ConvertToBool(); ;
            Makine100000Chk.Checked = string.IsNullOrEmpty(Makine100000QS) ? true : Makine100000QS.ConvertToBool(); ;
            Makine5000Chk.Checked = string.IsNullOrEmpty(Makine5000QS) ? true : Makine5000QS.ConvertToBool(); ;
            JeneratorChk.Checked = string.IsNullOrEmpty(JeneratorQS) ? true : JeneratorQS.ConvertToBool(); ;
            AsansorChk.Checked = string.IsNullOrEmpty(AsansorQS) ? true : AsansorQS.ConvertToBool(); ;
            KazanChk.Checked = string.IsNullOrEmpty(KazanQS) ? true : KazanQS.ConvertToBool(); ;

            DepremQS = DepremChk.Checked.ToString();
            YanginQS = YanginChk.Checked.ToString();
            Makine100000QS = Makine100000Chk.Checked.ToString();
            Makine5000QS = Makine5000Chk.Checked.ToString();
            JeneratorQS = JeneratorChk.Checked.ToString();
            AsansorQS = AsansorChk.Checked.ToString();
            KazanQS = KazanChk.Checked.ToString();
        }
        private void SigortaCinsiDDLDoldur()
        {
            SigortaCinsiDDL.Items.Clear();
            SigortaCinsiDDL.Items.Add(ProjeConstants.HEPSI);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DASK_IHTIYARI);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_DEPREM_IHTIYARI);
            SigortaCinsiDDL.Items.Add(ProjeConstants.SIGORTA_YOK);
        }
        private DataTable SigortaListesiGetirDT()
        {
            Sigorta sigorta = new Sigorta();
            //DataTable dataTable = sigorta.SelectAllReturnDataTable();
            DataTable dataTable = sigorta.SelectByTeminatSigortaCinsiReturnDataTable(SigortaCinsiQS, VadesiGelenlerChk.Checked, DepremQS.ConvertToBool(), YanginQS.ConvertToBool(), Makine100000QS.ConvertToBool(),
                Makine5000QS.ConvertToBool(), JeneratorQS.ConvertToBool(), AsansorQS.ConvertToBool(), KazanQS.ConvertToBool(), AuthQS);
            return dataTable;
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

            GridView1.DataSource = SigortaListesiGetirDT();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=TasinmazListesi" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls");
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
        protected void DepremChk_CheckedChanged(object sender, EventArgs e)
        {
            TeminatSecimiDegisti();
            RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Auth="+AuthQS+"&SigortaCinsi=" + SigortaCinsiQS +
                "&Deprem=" + DepremQS + "&Yangin=" + YanginQS +
                "&Makine100000=" + Makine100000QS + "&Makine5000=" + Makine5000QS +
                "&Jenerator=" + JeneratorQS + "&Asansor=" + AsansorQS + "&Kazan=" + KazanQS);
        }
        private void TeminatSecimiDegisti()
        {
            SigortaCinsiQS = SigortaCinsiDDL.SelectedValue.ToString();
            DepremQS = DepremChk.Checked ? "true" : "false";
            YanginQS = YanginChk.Checked ? "true" : "false";
            Makine5000QS = Makine5000Chk.Checked ? "true" : "false";
            Makine100000QS = Makine100000Chk.Checked ? "true" : "false";
            JeneratorQS = JeneratorChk.Checked ? "true" : "false";
            AsansorQS = AsansorChk.Checked ? "true" : "false";
            KazanQS = KazanChk.Checked ? "true" : "false";
        }
        protected void SigortaCinsiDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TeminatSecimiDegisti();
            RedirectToPage(ProjeConstants.PAGE_TASINMAZSIGORTA_LIST + "?Auth=" + AuthQS + "&SigortaCinsi=" + SigortaCinsiQS +
                "&Deprem=" + DepremQS + "&Yangin=" + YanginQS +
                "&Makine100000=" + Makine100000QS + "&Makine5000=" + Makine5000QS +
                "&Jenerator=" + JeneratorQS + "&Asansor=" + AsansorQS + "&Kazan=" + KazanQS);
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

        protected void VadesiGelenlerChk_CheckedChanged(object sender, EventArgs e)
        {
            TabloOlustur();
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
                List<SigortaListItem> list = GetDataList();
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
            string duzenleGorunsun = string.IsNullOrEmpty(AuthQS) || !AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM)
                ? "{ targets:10, visible:false},"
                : "{ targets:10, visible:true},";

            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                    jQuery('#CustomModalDataTable').DataTable().destroy();
                }
                jQuery('#CustomModalDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
                jQuery(document).ready(function () {
                    jQuery('#CustomDataTable').DataTable({
                        'initComplete': function (settings, json) {//tablo yüklendiğinde
                            var api = this.api();
                            var row = api.row(function (idx, data, node) { //secilen toplantıya gider
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
                            { data: 'Bolge' },
                            { data: 'SigortaCinsi' },
                            { data: 'AdresKodu' },
                            { data: 'PoliceNo' },
                            { data: 'KullanimSekli' },
                            { data: 'Adres' },
                            { data: 'TeminatListesi' },
                            { data: 'SigortaBitTar' },
                            { data: 'Police' },
                            { data: 'TasinmazKarti' },
                            { data: 'Duzenle' },               
                        ],
                        'columnDefs': [
                            " + duzenleGorunsun + @"
                            { 'width': '25%', 'targets': 5 }
                        ],
                        'language': {
                            'url': 'http://tskgv-portal/OrtakBelgeler/Turkish.txt',
                            'decimal': ',',
                            'thousands': '.'
                        },
                        responsive: true,
                        dom: 'Bfrtip',
                        buttons: [
                            {
                                extend: 'print',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'excel',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'pdf',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            {
                                extend: 'copy',
                                exportOptions: {
                                    columns: ':visible'
                                }
                            },
                            , 'pageLength', 'colvis'
                        ],
                        'createdRow': function(row, data, dataIndex) {
                            if (data.Renkli)
                            {
                                $(row).addClass('renkli');

                            }
                        },//set row color


                        });
                    });

            ";

            return tableString;
        }
        private List<SigortaListItem> GetDataList()
        {
            List<string> policeDosyalari = UtilityHelper.GetFileNameListFromSharePointLib(ProjeConstants.PATH_TBYS_URL, ProjeConstants.TBYSBELGELERI_LIB, ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK);

            Sigorta sigorta = new Sigorta();
            //DataTable dataTable = sigorta.SelectAllReturnDataTable();

            DataTable dataTable = sigorta.SelectByTeminatSigortaCinsiReturnDataTable(SigortaCinsiQS, VadesiGelenlerChk.Checked, DepremQS.ConvertToBool(), YanginQS.ConvertToBool(), Makine100000QS.ConvertToBool(),
                Makine5000QS.ConvertToBool(), JeneratorQS.ConvertToBool(), AsansorQS.ConvertToBool(), KazanQS.ConvertToBool(),AuthQS);

            int SiraNo = 1;

            List<SigortaListItem> list = new List<SigortaListItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                string sigortaId = row["SigortaId"].ToString();
                DateTime sigortaBitTar = row["SigortaBitTar"].ConvertToDatetime();
                string bolge = row["SorumluBolge"].ToString();
                string sigortaCinsi = row["SigortaCinsi"].ToString();
                string adresKodu = row["AdresKodu"].ToString();
                string policeNo = row["PoliceNo"].ToString();

                string kullanimSekli = row["KullanimSekli"].ToString();
                string tamAdres = row["TamAdres"].ToString();
                string teminatListesi = row["TeminatListesi"].ToString();
                string tasinmazId = row["TasinmazId"].ToString();

                SigortaListItem sigortaItem = new SigortaListItem();
                sigortaItem.Sirano = SiraNo++.ToString();
                sigortaItem.Bolge = bolge;
                sigortaItem.SigortaCinsi =sigortaCinsi;
                if (sigortaBitTar < DateTime.Today.AddMonths(1))
                    sigortaItem.Renkli = true;
                else
                    sigortaItem.Renkli = false;
                sigortaItem.SigortaBitTar = sigortaBitTar<=DateTime.MinValue?string.Empty:sigortaBitTar.ToString("dd.MM.yyyy");
                sigortaItem.AdresKodu = adresKodu;
                sigortaItem.PoliceNo= policeNo;
                sigortaItem.KullanimSekli = kullanimSekli;
                sigortaItem.TeminatListesi = teminatListesi;
                sigortaItem.Adres = tamAdres;
                sigortaItem.Police = FormLinkiGetir(policeDosyalari, ProjeConstants.DOSYA_SIGORTAPOLICESI_DASK, sigortaId,"Poliçe", "btn btn-outline-secondary");
                sigortaItem.TasinmazKarti = "<a href=" + ProjeConstants.PAGE_TASINMAZ_KARTI + "?DestinationApp=TD&SenderApp=OL&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Taşınmaz Kartı</a>";
                bool duzenleGorunsunMu = !string.IsNullOrEmpty(AuthQS) && AuthQS.Equals(ProjeConstants.TBYS_YETKILI_BIRIM);
                if (duzenleGorunsunMu)
                {
                    sigortaItem.Duzenle = "<a href=" + ProjeConstants.PAGE_TASINMAZSIGORTA_GIRIS + "?DestinationApp=SigortaD&SenderApp=SigortaL&SigortaId=" + sigortaId + "&TasinmazId=" + tasinmazId + " class='btn btn-outline-primary'>Düzenle</a>";
                }
                sigortaItem.Secildi = SecilenIdQS.Equals(sigortaItem.SigortaId);
                list.Add(sigortaItem);
            }
            return list;
        }
        private string FormLinkiGetir(List<string> list, string form, string sigortaId, string linkText, string classString)
        {
            string belgePdfLink = string.Empty;
            string dosyaAdi = form + sigortaId + ".pdf";
            string dosyaUrl = UtilityHelper.TbysBelgelerURLGetir() + "/" + dosyaAdi;
            bool dosyaVarMi = list.Contains(dosyaAdi);
            if (dosyaVarMi)
            {
                belgePdfLink = @"<a class='" + classString + "' data-fancybox data-type=pdf data-width=960 data-height=720 href=" + dosyaUrl + @">" + linkText + "</a>";
            }
            return belgePdfLink;
        }
        private class SigortaListItem
        {
            public string Sirano { get; set; }
            public string SigortaId { get; set; }
            public string SigortaBitTar { get; set; }
            public string Bolge { get; set; }
            public string SigortaCinsi { get; set; }
            public string AdresKodu { get; set; }
            public string PoliceNo { get; set; }
            public string KullanimSekli { get; set; }
            public string Adres { get; set; }
            public string TeminatListesi { get; set; }
            public string TasinmazKarti { get; set; }
            public string Duzenle { get; set; }
            public bool Secildi { get; set; }
            public bool Renkli { get; set; }
            public string Police { get; set; }
        }
    }
}
