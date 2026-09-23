using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciAdresListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class NakitBagisciAdresListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public NakitBagisciAdresListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string BagisciSayisiQS
        {
            get
            {

                if (ViewState["BagisciSayisi"] == null)
                {
                    if (Page.Request.QueryString["BagisciSayisi"] != null)
                    {
                        ViewState["BagisciSayisi"] = Page.Request.QueryString["BagisciSayisi"];
                    }
                    else
                    {
                        ViewState["BagisciSayisi"] = string.Empty;
                    }
                }
                return ViewState["BagisciSayisi"].ToString();
            }

            set
            {
                ViewState["BagisciSayisi"] = value;
            }
        }
        private string BasTarQS
        {
            get
            {

                if (ViewState["BasTar"] == null)
                {
                    if (Page.Request.QueryString["BasTar"] != null)
                    {
                        ViewState["BasTar"] = Page.Request.QueryString["BasTar"];
                    }
                    else
                    {
                        ViewState["BasTar"] = string.Empty;
                    }
                }
                return ViewState["BasTar"].ToString();
            }

            set
            {
                ViewState["BasTar"] = value;
            }
        }
        private string BitTarQS
        {
            get
            {

                if (ViewState["BitTar"] == null)
                {
                    if (Page.Request.QueryString["BitTar"] != null)
                    {
                        ViewState["BitTar"] = Page.Request.QueryString["BitTar"];
                    }
                    else
                    {
                        ViewState["BitTar"] = string.Empty;
                    }
                }
                return ViewState["BitTar"].ToString();
            }

            set
            {
                ViewState["BitTar"] = value;
            }
        }
        private string UlasilamayanlarHaricQS
        {
            get
            {

                if (ViewState["UlasilamayanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["UlasilamayanlarHaric"] != null)
                    {
                        ViewState["UlasilamayanlarHaric"] = Page.Request.QueryString["UlasilamayanlarHaric"];
                    }
                    else
                    {
                        ViewState["UlasilamayanlarHaric"] = string.Empty;
                    }
                }
                return ViewState["UlasilamayanlarHaric"].ToString();
            }

            set
            {
                ViewState["UlasilamayanlarHaric"] = value;
            }
        }
        private string BelgeIstemeyenlerHaricQS
        {
            get
            {

                if (ViewState["BelgeIstemeyenlerHaric"] == null)
                {
                    if (Page.Request.QueryString["BelgeIstemeyenlerHaric"] != null)
                    {
                        ViewState["BelgeIstemeyenlerHaric"] = Page.Request.QueryString["BelgeIstemeyenlerHaric"];
                    }
                    else
                    {
                        ViewState["BelgeIstemeyenlerHaric"] = string.Empty;
                    }
                }
                return ViewState["BelgeIstemeyenlerHaric"].ToString();
            }

            set
            {
                ViewState["BelgeIstemeyenlerHaric"] = value;
            }
        }
        private string AdresiBosOlanlarHaricQS
        {
            get
            {

                if (ViewState["AdresiBosOlanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["AdresiBosOlanlarHaric"] != null)
                    {
                        ViewState["AdresiBosOlanlarHaric"] = Page.Request.QueryString["AdresiBosOlanlarHaric"];
                    }
                    else
                    {
                        ViewState["AdresiBosOlanlarHaric"] = string.Empty;
                    }
                }
                return ViewState["AdresiBosOlanlarHaric"].ToString();
            }

            set
            {
                ViewState["AdresiBosOlanlarHaric"] = value;
            }
        }
        private string PostadanIadeEdilenlerHaricQS
        {
            get
            {

                if (ViewState["PostadanIadeEdilenlerHaric"] == null)
                {
                    if (Page.Request.QueryString["PostadanIadeEdilenlerHaric"] != null)
                    {
                        ViewState["PostadanIadeEdilenlerHaric"] = Page.Request.QueryString["PostadanIadeEdilenlerHaric"];
                    }
                    else
                    {
                        ViewState["PostadanIadeEdilenlerHaric"] = string.Empty;
                    }
                }
                return ViewState["PostadanIadeEdilenlerHaric"].ToString();
            }

            set
            {
                ViewState["PostadanIadeEdilenlerHaric"] = value;
            }
        }
        private string DergiGonderilmeyeceklerHaricQS
        {
            get
            {

                if (ViewState["DergiGonderilmeyeceklerHaric"] == null)
                {
                    if (Page.Request.QueryString["DergiGonderilmeyeceklerHaric"] != null)
                    {
                        ViewState["DergiGonderilmeyeceklerHaric"] = Page.Request.QueryString["DergiGonderilmeyeceklerHaric"];
                    }
                    else
                    {
                        ViewState["DergiGonderilmeyeceklerHaric"] = string.Empty;
                    }
                }
                return ViewState["DergiGonderilmeyeceklerHaric"].ToString();
            }

            set
            {
                ViewState["DergiGonderilmeyeceklerHaric"] = value;
            }
        }
        private string SadeceYeniBagiscilarQS
        {
            get
            {

                if (ViewState["SadeceYeniBagiscilar"] == null)
                {
                    if (Page.Request.QueryString["SadeceYeniBagiscilar"] != null)
                    {
                        ViewState["SadeceYeniBagiscilar"] = Page.Request.QueryString["SadeceYeniBagiscilar"];
                    }
                    else
                    {
                        ViewState["SadeceYeniBagiscilar"] = string.Empty;
                    }
                }
                return ViewState["SadeceYeniBagiscilar"].ToString();
            }

            set
            {
                ViewState["SadeceYeniBagiscilar"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BagisciSayisiTxt.Text = "1000";
                    DateTime today = DateTime.Today;
                    DateTime basyil = today.AddYears(-1);
                    BasTarTxt.Text = basyil.ConvertToDatetimeEmptyIfNull();
                    BitTarTxt.Text = today.ConvertToDatetimeEmptyIfNull();
                    BagisciSayisiQS = string.IsNullOrEmpty(BagisciSayisiQS) ? BagisciSayisiTxt.Text : BagisciSayisiQS;
                    BasTarQS = string.IsNullOrEmpty(BasTarQS) ? BasTarTxt.Text : BasTarQS;
                    BitTarQS = string.IsNullOrEmpty(BitTarQS) ? BitTarTxt.Text : BitTarQS;
                    UlasilamayanlarHaricQS = string.IsNullOrEmpty(UlasilamayanlarHaricQS) ? true.ToString() : UlasilamayanlarHaricQS;
                    BelgeIstemeyenlerHaricQS = string.IsNullOrEmpty(BelgeIstemeyenlerHaricQS) ? true.ToString() : BelgeIstemeyenlerHaricQS;
                    AdresiBosOlanlarHaricQS = string.IsNullOrEmpty(AdresiBosOlanlarHaricQS) ? true.ToString() : AdresiBosOlanlarHaricQS;
                    PostadanIadeEdilenlerHaricQS = string.IsNullOrEmpty(PostadanIadeEdilenlerHaricQS) ? true.ToString() : PostadanIadeEdilenlerHaricQS;
                    DergiGonderilmeyeceklerHaricQS = string.IsNullOrEmpty(DergiGonderilmeyeceklerHaricQS) ? true.ToString() : DergiGonderilmeyeceklerHaricQS;
                    SadeceYeniBagiscilarQS = string.IsNullOrEmpty(SadeceYeniBagiscilarQS) ? false.ToString() : SadeceYeniBagiscilarQS;
                    BagisciSayisiTxt.Text = BagisciSayisiQS;
                    BasTarTxt.Text = BasTarQS;
                    BitTarTxt.Text = BitTarQS;
                    UlasilamayanlarHaricChk.Checked = UlasilamayanlarHaricQS.ConvertToBool();
                    BelgeIstemeyenlerHaricChk.Checked = BelgeIstemeyenlerHaricQS.ConvertToBool();
                    AdresiBosOlanlarHaricChk.Checked = AdresiBosOlanlarHaricQS.ConvertToBool();
                    BelgesiPostadanIadeEdilenlerHaricChk.Checked = PostadanIadeEdilenlerHaricQS.ConvertToBool();
                    DergiGonderilmesinlerHaricChk.Checked = DergiGonderilmeyeceklerHaricQS.ConvertToBool();
                    SadeceYeniBagiscilarChk.Checked = SadeceYeniBagiscilarQS.ConvertToBool();
                    TabloOlustur();
                }
               
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
       
        protected void CloseBtn_Click(object sender, EventArgs e)
        {

        }
        
        protected void BagisTarihiTxt_TextChanged(object sender, EventArgs e)
        {
            BagisciSayisiQS = BagisciSayisiTxt.Text;
            BasTarQS = BasTarTxt.Text.ConvertToDatetimeEmptyIfNull();
            BitTarQS = BitTarTxt.Text.ConvertToDatetimeEmptyIfNull();
            UlasilamayanlarHaricQS = UlasilamayanlarHaricChk.Checked.ToString();
            BelgeIstemeyenlerHaricQS = BelgeIstemeyenlerHaricChk.Checked.ToString();
            AdresiBosOlanlarHaricQS = AdresiBosOlanlarHaricChk.Checked.ToString();
            PostadanIadeEdilenlerHaricQS = BelgesiPostadanIadeEdilenlerHaricChk.Checked.ToString();
            DergiGonderilmeyeceklerHaricQS = DergiGonderilmesinlerHaricChk.Checked.ToString();
            SadeceYeniBagiscilarQS = SadeceYeniBagiscilarChk.Checked.ToString();

            TabloOlustur();
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

            //
            GridView GridView1 = new GridView();
            GridView1.AllowPaging = false;
            GridView1.DataSource = GetDataList();
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=BagisciListesi_" + BasTarQS.ConvertToDatetimeEmptyIfNull() + "_" + BitTarQS.ConvertToDatetimeEmptyIfNull() + ".xls");

            //Türkçe karakter sorunu düzeltmek için
            //buradan başladı
            //Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            //Page.Response.Charset = "ISO-8859-9";//"windows-1254"
            //Page.Response.ContentType = "application/vnd.ms-excel";

            Page.Response.ContentType = "application/ms-excel";
            Page.Response.ContentEncoding = System.Text.Encoding.Unicode;
            Page.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            ///buraya kadar değişti 
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
        protected void DergiGondermeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SecilenIdQS = paramNakitBagisciIdLbl.Value;
                NakitBagisciService service = new NakitBagisciService();
                NakitBagisci nb = service.GetById(paramNakitBagisciIdLbl.Value.ConvertToInt());
                if (nb != null)
                {
                    nb.Degistiren = UtilityHelper.GetCurrentUserLoginName();
                    if (isDergiGonderLbl.Value.Equals("gonder"))
                    {
                        nb.DergiGonderilmesin = ProjeConstants.DERGI_GONDERILSIN;
                    }
                    else
                    {
                        nb.DergiGonderilmesin = ProjeConstants.DERGI_GONDERILMESIN;
                    }

                    if (nb.Update())
                    {
                        MessageHelper.PublishMessage(nb.Adi + " Adlı bağışçıya dergi gönderilmeyecek.", ProjeConstants.MESAJ_BASARILI, 2000);

                        string paramStr = "&BagisciSayisi=" + BagisciSayisiQS + "&BasTar=" + BasTarQS + "&BitTar=" + BitTarQS + "&UlasilamayanlarHaric=" + UlasilamayanlarHaricQS +
                            "&BelgeIstemeyenlerHaric=" + BelgeIstemeyenlerHaricQS + "&AdresiBosOlanlarHaric=" + AdresiBosOlanlarHaricQS +
                            "&PostadanIadeEdilenlerHaric=" + PostadanIadeEdilenlerHaricQS + "&DergiGonderilmeyeceklerHaric=" + DergiGonderilmeyeceklerHaricQS + "&SadeceYeniBagiscilar=" + SadeceYeniBagiscilarQS + "&SecilenId=" + SecilenIdQS;
                        RedirectToPage(ProjeConstants.PAGE_NAKITBAGISCI_ADRESLIST + "?Mesaj=true" + paramStr);
                    }
                    else
                    {
                        MessageHelper.PublishMessage(" Bağışçı bilgisi değiştirilemedi.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage(" Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exh = new ExceptionHelper();
                exh.Exceptions.Add(new Exception("Dergi Gönderme durumu değiştirilemedi. "));
                exh.Exceptions.Add(ex);
                exh.PublishException();
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
        #region Bagisci CustomDataTable
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json'a çeviriliyor
            var jsString = CreateDataTable(jsonData); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            List<NakitBagisciListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private List<NakitBagisciListItem> GetDataList()
        {
            string paramsStr = "&BagisciSayisi=" + BagisciSayisiQS + "&BasTar=" + BasTarQS + "&BitTar=" + BitTarQS + "&UlasilamayanlarHaric=" + UlasilamayanlarHaricQS + "&BelgeIstemeyenlerHaric=" + BelgeIstemeyenlerHaricQS +
                "&AdresiBosOlanlarHaric=" + AdresiBosOlanlarHaricQS + "&PostadanIadeEdilenlerHaric=" + PostadanIadeEdilenlerHaricQS +
                "&DergiGonderilmeyeceklerHaric=" + DergiGonderilmeyeceklerHaricQS + "&SadeceYeniBagiscilar=" + SadeceYeniBagiscilarQS + "&SecilenId=" + SecilenIdQS;

            NakitBagisci bagisci = new NakitBagisci();
            DateTime today = DateTime.Today;
            DateTime basTar = BasTarTxt.Text.ConvertToDatetime();
            DateTime bitTar = BitTarTxt.Text.ConvertToDatetime();
            int bagisciSayisi = BagisciSayisiTxt.Text.ConvertToInt();
            int rowCount = 0;

            DataTable dataTable = bagisci.SelectByBagisTarihiBagisSayisi(basTar, bitTar, bagisciSayisi, ref rowCount,
                BelgeIstemeyenlerHaricChk.Checked, AdresiBosOlanlarHaricChk.Checked, BelgesiPostadanIadeEdilenlerHaricChk.Checked,
                DergiGonderilmesinlerHaricChk.Checked, UlasilamayanlarHaricChk.Checked, SadeceYeniBagiscilarChk.Checked);
            int SiraNo = 0;
            List<NakitBagisciListItem> list = new List<NakitBagisciListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {

                decimal bagisMiktari = row["BagisMiktari"].ConvertToDecimal();
                string nakitBagisciId = row["NakitBagisciId"].ToString();
                string adi = row["Adi"].ToString();
                string telefon1 = row["Telefon1"].ToString();
                string telefon2 = row["Telefon2"].ToString();

                string adres = row["Adres"].ToString();
                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string tuzelKisi = row["TuzelKisi"].ToString();
                bool dergiGonderilmesin = row["DergiGonderilmesin"].ReturnFalseIfNull().ConvertToBool();

                NakitBagisciListItem nakitBagisciItem = new NakitBagisciListItem();
                nakitBagisciItem.Sirano = SiraNo++.ToString();
                nakitBagisciItem.BagisMiktari = bagisMiktari.ToString("N", culturInfo);
                nakitBagisciItem.NakitBagisciId = nakitBagisciId.ToString();
                nakitBagisciItem.Adi = "<a href=# onclick=OpenModal(" +nakitBagisciId+"); class='btn-link  text-primary'>" + adi+"</a>";
                nakitBagisciItem.Telefon1 = telefon1;
                nakitBagisciItem.Telefon2 = telefon2.ConvertToDatetimeEmptyIfNull();
                nakitBagisciItem.Adres = "- " + adres;
                nakitBagisciItem.Ilcesi = ilcesi;
                nakitBagisciItem.Ili = ili;
                nakitBagisciItem.DergiGonderilmesin = dergiGonderilmesin.ConvertToBool().ToString();
                nakitBagisciItem.TuzelKisi = tuzelKisi.ConvertToBool() ? "Tüzel" : "Özel";
                nakitBagisciItem.Duzenle = "<a class='btn btn-outline-primary' href=NakitBagisciEdit.aspx?SenderApp=NBAL&NakitBagisciId=" + nakitBagisciId + paramsStr + " >Düzenle</a>";
                string dergiStr = dergiGonderilmesin ?
                    "<a href=# onclick=CallButtonClick(" + nakitBagisciId + ",'gonder'); class='btn btn-outline-success'>Dergi Gönder</a>" :
                    "<a href=# onclick=CallButtonClick(" + nakitBagisciId + ",'gonderme'); class='btn btn-outline-danger'>Dergi Gönderme</a>";
                nakitBagisciItem.DergiGonderilmesin = dergiStr;
                nakitBagisciItem.Secildi = SecilenIdQS.Equals(nakitBagisciItem.NakitBagisciId); ;
                list.Add(nakitBagisciItem);
            }
            return list;
        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
            jQuery(document).ready(function() {

                jQuery('#CustomDataTable').DataTable({
                    'initComplete': function(settings, json) {//tablo yüklendiğinde
                        var api = this.api();
                        var row = api.row(function(idx, data, node) { //seçilen Id'ye gider
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
                { data: 'Adi' },
                { data: 'TuzelKisi' },
                { data: 'BagisMiktari' },
                { data: 'Ili' },
                { data: 'Ilcesi' },
                { data: 'Telefon1', 'width': '14%' },
                { data: 'Adres' },
                { data: 'Duzenle' },
                { data: 'DergiGonderilmesin' },

            ],
            'order': [[0, 'asc']],//AdiSoyadi Sirali
            columnDefs:
                [
                ],
            'language': {
                'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            autoWidth: false,
            dom: 'frtip',
            
                });
            });";
            return tableString;
        }
        #endregion
        #region Modal
        private void TabloModalOlustur(string nakitBagisciId)
        {
            var jsonData = GetModalDataJson(nakitBagisciId); //veri çekilip json'a çeviriliyor
            var jsString = CreateModalDataTable(jsonData, nakitBagisciId.ConvertToInt()); //javascript kodu hazırlanıyor.
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string CreateModalDataTable(string jsonData, int nakitBagisciId)
        {
            NakitBagisciService service = new NakitBagisciService();
            NakitBagisci nb = service.GetById(nakitBagisciId);
            string bagisciAdi = nb != null ? (nb.Adi + nb.Soyadi).ReplaceTrChars() : "Bagisci";
            string filename = bagisciAdi + "-" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
            string tableString = @"
        jQuery(document).ready(function () {
            if ( jQuery.fn.DataTable.isDataTable('#CustomModalDataTable') ) {
                jQuery('#CustomModalDataTable').DataTable().destroy();
            }
            jQuery('#CustomModalDataTable tbody').empty();

            jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
            jQuery('#CustomModalDataTable').DataTable({
                data: " + jsonData + @",
                columns: [
                    { data: 'BagisTarihi' },
                    { data: 'BagisMiktari', 'width': '10%', 'className': 'text-end' },
                    { data: 'Banka' },
                    { data: 'Armagan' },
                    { data: 'ArmaganTutari' },
                    { data: 'Durum' },
                ],
                'order': [[0, 'desc']],

                'language': {
                    'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                    'decimal': ',',
                    'thousands': '.'
                },
                columnDefs:[
                    {targets:0, render:function(data){
                        return moment(data).format('DD.MM.YYYY');
                    }},
                ],
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
                      title:'" + filename + @"',
                      exportOptions: {
                          columns: ':visible',
                          format: {
                              body: function(data, row, column, node) {
                                  data = $('<p>' + data + '</p>').text();
                                    
                                  return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                              }
                          }
                      },
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
        private string GetModalDataJson(string nakitBagisciId)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            NakitBagisHareket nbh = new NakitBagisHareket();
            int rowCount = 0;
            var json = nbh.SelectByBagisciIdReturnJSon(nakitBagisciId, ref rowCount);
            decimal toplamTutar = nbh.GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(
                ProjeConstants.BAGIS_SORGU_BASTAR.ConvertToDatetime(), DateTime.Today, nakitBagisciId.ConvertToInt());
            BagisBilgileriLbl.Text = rowCount < 1 ? "Bağış bulunmamaktadır" :
                "Bağışçının " + rowCount + " defada yaptığı toplam " + toplamTutar.ToString("N", culturInfo) + "TL bağışı bulunmaktadır";
            return json;
        }
        protected void ModalDoldurBtn_Click(object sender, EventArgs e)
        {
            try
            {

                TabloModalOlustur(paramNakitBagisciIdLbl.Value);
                NakitBagisciFormunuDoldur(paramNakitBagisciIdLbl.Value);
            }
            catch (Exception exception)
            {
                ExceptionHelper ex = new ExceptionHelper(exception);
                ex.PublishException();

            }
        }
        private void NakitBagisciFormunuDoldur(string nakitBagisciIdStr)
        {
            if (!string.IsNullOrEmpty(nakitBagisciIdStr))
            {
                int nakitBagisciId = nakitBagisciIdStr.ConvertToInt();

                NakitBagisciService service = new NakitBagisciService();
                NakitBagisci nakitBagisci = service.GetById(nakitBagisciId);
                if (nakitBagisci != null)
                {
                    //NakitBagisciIdLbl.Text = nakitBagisciId.ToString();
                    TableRow row = new TableRow();
                    TableCell AdiCell = new TableCell();
                    TableCell TCKimlikNoCell = new TableCell();
                    TableCell AdresCell = new TableCell();
                    TableCell IlIlceCell = new TableCell();
                    TableCell TelefonCell = new TableCell();
                    TableCell TuzelKisiCell = new TableCell();

                    AdiCell.Text = nakitBagisci.Adi.ReturnEmptyIfNull().ToString();
                    TCKimlikNoCell.Text = nakitBagisci.TCKimlikNo.ReturnEmptyIfNull().ToString();
                    AdresCell.Text = nakitBagisci.Adres.ReturnEmptyIfNull().ToString();

                    int ilId = nakitBagisci.Ili.ConvertToInt();
                    Il il = new Il();
                    il = il.Select<Il>(ilId);
                    if (il != null)
                    {

                        IlIlceCell.Text = il.IlAdi.ReturnEmptyIfNull().ToString();
                    }
                    int ilceId = nakitBagisci.Ilcesi.ConvertToInt();
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ilceId);
                    if (ilce != null)
                    {

                        IlIlceCell.Text += " " + ilce.IlceAdi.ReturnEmptyIfNull().ToString();
                    }
                    TelefonCell.Text = nakitBagisci.Telefon1.ReturnEmptyIfNull().ToString();
                    TuzelKisiCell.Text = nakitBagisci.TuzelKisi.ConvertToBool() ? "Evet" : "Hayır";
                    row.Controls.Add(AdiCell);
                    row.Controls.Add(TCKimlikNoCell);
                    row.Controls.Add(AdresCell);
                    row.Controls.Add(IlIlceCell);
                    row.Controls.Add(TelefonCell);
                    row.Controls.Add(TuzelKisiCell);
                    BagisciTable.Controls.Add(row);

                }
            }
        }
        #endregion
        private class NakitBagisciListItem
        {
            public string Sirano { get; set; }
            public string BagisMiktari { get; set; }
            public string NakitBagisciId { get; set; }
            public string Adi { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Adres { get; set; }
            public string TuzelKisi { get; set; }
            public string Duzenle { get; set; }
            public string DergiGonderilmesin { get; set; }
            public bool Secildi { get; set; }
        }
    }
}
