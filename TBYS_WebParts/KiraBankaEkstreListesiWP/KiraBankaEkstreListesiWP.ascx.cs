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

namespace TBYS_WebParts.KiraBankaEkstreListesiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraBankaEkstreListesiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraBankaEkstreListesiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string IslemTarihiQS
        {
            get
            {

                if (ViewState["IslemTarihi"] == null)
                {
                    if (Page.Request.QueryString["IslemTarihi"] != null)
                    {
                        ViewState["IslemTarihi"] = Page.Request.QueryString["IslemTarihi"];
                    }
                    else
                    {
                        ViewState["IslemTarihi"] = string.Empty;
                    }
                }
                return ViewState["IslemTarihi"].ToString();
            }

            set
            {
                ViewState["IslemTarihi"] = value;
            }
        }
        private string IslemQS
        {
            get
            {
                if (ViewState["Islem"] == null)
                {
                    if (Page.Request.QueryString["Islem"] != null)
                    {
                        ViewState["Islem"] = Page.Request.QueryString["Islem"];
                    }
                    else
                    {
                        ViewState["Islem"] = string.Empty;
                    }
                }
                return ViewState["Islem"].ToString();
            }

            set
            {
                ViewState["Islem"] = value;
            }
        }
        private string AktarilanlarHaricQS
        {
            get
            {
                if (ViewState["AktarilanlarHaric"] == null)
                {
                    if (Page.Request.QueryString["AktarilanlarHaric"] != null)
                    {
                        ViewState["AktarilanlarHaric"] = Page.Request.QueryString["AktarilanlarHaric"];
                    }
                    else
                    {
                        ViewState["AktarilanlarHaric"] = AktarilanlarHaricChk.Checked;
                    }
                }
                return ViewState["AktarilanlarHaric"].ToString();
            }

            set
            {
                ViewState["AktarilanlarHaric"] = value;
            }
        }
        private string KiraTeminatDigerQS
        {
            get
            {
                if (ViewState["KiraTeminatDiger"] == null)
                {
                    if (Page.Request.QueryString["KiraTeminatDiger"] != null)
                    {
                        ViewState["KiraTeminatDiger"] = Page.Request.QueryString["KiraTeminatDiger"];
                    }
                    else
                    {
                        ViewState["KiraTeminatDiger"] = KiraTeminatDigerChk.Checked;
                    }
                }
                return ViewState["KiraTeminatDiger"].ToString();
            }

            set
            {
                ViewState["KiraTeminatDiger"] = value;
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
        private readonly IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SayfaGirisKontrolu())
            {
                RedirectToPage(ProjeConstants.PAGE_HOME + "?Mesaj=true&Text=Sayfada düzenleme yapılmaktadir.Lütfen daha sonra tekrar deneyiniz.");
            }
            else
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        if (!string.IsNullOrEmpty(MesajQS))
                        {
                            if (IslemQS.ToLower().Equals("kaydet"))
                            {
                                if (MesajQS.ToLower().Equals("true"))
                                    MessageHelper.PublishMessage("Aktarma tamamlandı.", ProjeConstants.MESAJ_BASARILI, 2000);
                                else
                                    MessageHelper.PublishMessage("Aktarma yapılamadı.", ProjeConstants.MESAJ_HATA);
                            }
                            MesajQS = string.Empty;
                        }
                        TabloOlustur();
                        AktarilanlarHaricChk.Checked = AktarilanlarHaricQS.ConvertToBool();
                        KiraTeminatDigerChk.Checked = KiraTeminatDigerQS.ConvertToBool();

                    }

                }
                catch (Exception ex)
                {
                    ExceptionHelper exHelper = new ExceptionHelper(ex);
                    exHelper.PublishException();
                }
            }

        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson(); //veri çekilip json a çeviriliyor
            ScriptCalistir("setDataSet(" + jsonData + ");");            
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraEkstreAktarmaListItem> list = GetDataList();
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
        private bool SayfaGirisKontrolu()
        {
            return true;
        }
        protected void SecilenListeyiKaydet()
        {
            string value = ParamKaydedilecekArray.Value;

            string currentUser = UtilityHelper.GetCurrentUserLoginName();
            if (!string.IsNullOrEmpty(value))
            {
                KiraEkstreAktarma eaDao = new KiraEkstreAktarma();
                int rowCount = 0;
                List<KiraEkstreAktarma> aktarilmayanlar = eaDao.SelectByEkstreIdList(value, ref rowCount);
                if (aktarilmayanlar.Count > 0)
                {
                    var exceptionHelper = OdemeIslemleriniYap(aktarilmayanlar, currentUser); //seçilenler diger tablolara dagitiliyor
                    if (exceptionHelper.Exceptions.Count > 0)
                    {
                        ScriptCalistir("CloseModalOnay();");
                        exceptionHelper.PublishException();
                    }
                    else
                    {
                        RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?Mesaj=true&Islem=kaydet&basarili=true&IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&KiraTeminatDiger=" + KiraTeminatDigerQS);
                    }
                }
            }
            else
            {
                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
            }
        }
        public ExceptionHelper OdemeIslemleriniYap(List<KiraEkstreAktarma> listEkstreAktarma, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();


            foreach (KiraEkstreAktarma ekstreAktarma in listEkstreAktarma)
            {

                try
                {
                    DateTime odemeTarihi = ekstreAktarma.OdemeTarihi;
                    decimal odemeTutari = ekstreAktarma.Tutar;

                    KiraSozlesme kiraSozlesme = new KiraSozlesme();
                    kiraSozlesme = kiraSozlesme.SelectByKiraciIdTarih(ekstreAktarma.KiraciId, odemeTarihi);
                    if (kiraSozlesme == null)
                    {
                        kiraSozlesme = new KiraSozlesme();
                        kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(ekstreAktarma.KiraciId, odemeTarihi);

                    }
                    if (kiraSozlesme != null)
                    {
                        if (!ekstreAktarma.AktarildiMi)
                        {
                            int odemeId = 0;
                            bool odemeYapildiMi = TBYSOrtak.OdemeYap(kiraSozlesme, odemeTarihi, odemeTutari, " Banka Ekstresinden Aktarma ", ref odemeId);
                            if (odemeYapildiMi)
                            {
                                ekstreAktarma.AktarildiMi = true;
                                ekstreAktarma.Update();

                            }
                        }
                        else
                        {
                            throw new Exception("Ödeme Zaten Kaydedilmiş!");
                        }
                    }
                    else
                    {
                        throw new Exception("Sözleşme Bulunamadı");
                    }

                }
                catch (Exception e)
                {
                    exceptionHelper.Exceptions.Add(e);
                }

            }

            return exceptionHelper;
        }

        private List<KiraEkstreAktarmaListItem> GetDataList()
        {

            KiraEkstreAktarma ea = new KiraEkstreAktarma();
            int rowCount = 0;
            DataTable dataTable = ea.SelectYuklenenKayit(ref rowCount, AktarilanlarHaricQS.ConvertToBool(), KiraTeminatDigerQS.ConvertToBool());

            RowCountLbl.Text = "Kayıt Sayısı : " + rowCount.ToString();
            List<KiraEkstreAktarmaListItem> returnlist = new List<KiraEkstreAktarmaListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    KiraEkstreAktarmaListItem ekstreAktarmaListItem = new KiraEkstreAktarmaListItem();


                    try
                    {
                        ekstreAktarmaListItem.KiraEkstreAktarmaId = dataRow["KiraEkstreAktarmaId"].ToString();
                        ekstreAktarmaListItem.IslemTarihi = dataRow["IslemTarihi"].ToString();

                        ekstreAktarmaListItem.KiraciId = dataRow["KiraciId"].ToString();
                        ekstreAktarmaListItem.KiraciAdi = dataRow["KiraciAdi"].ToString();
                        ekstreAktarmaListItem.IslemNo = dataRow["IslemNo"].ToString();

                        ekstreAktarmaListItem.BankaAdi = dataRow["BankaAdi"].ToString();
                        ekstreAktarmaListItem.TCKimlikNo = dataRow["TCKimlikNo"].ToString();

                        ekstreAktarmaListItem.Adi = dataRow["Adi"].ToString();
                        ekstreAktarmaListItem.Soyadi = dataRow["Soyadi"].ToString();
                        ekstreAktarmaListItem.AdiSoyadi = dataRow["AdiSoyadi"].ToString();
                        ekstreAktarmaListItem.Telefon = dataRow["Telefon"].ToString();
                        ekstreAktarmaListItem.Telefon1 = dataRow["Telefon1"].ToString();
                        ekstreAktarmaListItem.Telefon2 = dataRow["Telefon2"].ToString();
                        ekstreAktarmaListItem.Aciklama = dataRow["Aciklama"].ToString();
                        DateTime odemeTarihi = dataRow["OdemeTarihi"].ConvertToDatetime();
                        ekstreAktarmaListItem.OdemeTarihi = odemeTarihi.ToString("dd.MM.yyyy HH:mm");
                        decimal tutar = dataRow["Tutar"].ConvertToDecimal();
                        string dovizCinsi = dataRow["DovizCinsi"].ToString();
                        ekstreAktarmaListItem.Tutar = tutar > 0 ? tutar.ToString("N", culturInfo) + " " + dovizCinsi : "";
                        bool aktarildiMi = dataRow["AktarildiMi"].ConvertToBool();
                        bool uyari = dataRow["Uyari"].ConvertToBool();
                        ekstreAktarmaListItem.AktarildiMi = aktarildiMi.ToString();
                        ekstreAktarmaListItem.Uyari = uyari.ToString();

                        ekstreAktarmaListItem.OdemeSebebiId = dataRow["OdemeSebebiId"].ConvertToInt();
                        ekstreAktarmaListItem.OdemeSebebi = dataRow["OdemeSebebi"].ToString();
                        ekstreAktarmaListItem.Secildi = SecilenIdQS.Equals(ekstreAktarmaListItem.KiraEkstreAktarmaId);
                        if (aktarildiMi)
                        {
                            ekstreAktarmaListItem.CakismaVarMi = "False";

                            ekstreAktarmaListItem.SecKaydet = string.Empty;
                            ekstreAktarmaListItem.Eslestir = string.Empty;
                            ekstreAktarmaListItem.OdemeAyristir = string.Empty;
                        }
                        else
                        {
                            ekstreAktarmaListItem.CakismaVarMi = CakismaKontrolu(ekstreAktarmaListItem, tutar, odemeTarihi).ToString();
                            if (ekstreAktarmaListItem.CakismaVarMi.ConvertToBool())
                            {
                                ekstreAktarmaListItem.SecKaydet = string.Empty;
                                ekstreAktarmaListItem.Eslestir = string.Empty;
                                ekstreAktarmaListItem.OdemeAyristir = string.Empty;
                            }
                            else
                            {

                                {
                                    if (ekstreAktarmaListItem.OdemeSebebiId==ProjeConstants.ODEMESEBEBI_KIRA_INT ||
                                        ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT ||
                                        ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT ||
                                        ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT
                                        )
                                    {
                                        if (uyari)
                                        {
                                            ekstreAktarmaListItem.SecKaydet = string.Empty;
                                        }
                                        else
                                        {
                                            ekstreAktarmaListItem.SecKaydet = SecCheckBoxLinki(ProjeConstants.KAYDET,
                                                aktarildiMi, ekstreAktarmaListItem.KiraciId.ConvertToInt(), ekstreAktarmaListItem.KiraEkstreAktarmaId.ConvertToInt());
                                        }
                                            

                                        ekstreAktarmaListItem.Eslestir = "<a href=" + ProjeConstants.PAGE_KIRACI_ESLESTIR +
                                            "?KiraEkstreAktarmaId=" + ekstreAktarmaListItem.KiraEkstreAktarmaId + " class='btn btn-warning'>Eslestir</a>";

                                        ekstreAktarmaListItem.OdemeAyristir = OdemeAyristirBtnLinki(ekstreAktarmaListItem, aktarildiMi); 
                                    }
                                    
                                    if (ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT ||
                                        ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT ||
                                        ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT)
                                    {
                                        ekstreAktarmaListItem.SecKaydet = string.Empty;
                                    }else if (ekstreAktarmaListItem.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_DIGER_INT)
                                    {
                                        ekstreAktarmaListItem.Eslestir = "<a href=" + ProjeConstants.PAGE_KIRACI_ESLESTIR +
                                            "?KiraEkstreAktarmaId=" + ekstreAktarmaListItem.KiraEkstreAktarmaId + " class='btn btn-warning'>Eşleştir</a>";
                                    }

                                    if (ekstreAktarmaListItem.KiraciId.ConvertToInt() > 0)
                                    {
                                        ekstreAktarmaListItem.KiraciAdi = "<a href=# class='btn btn-link text-left' style='white-space:normal'  onclick=OdemePlaniModalAc(" + ekstreAktarmaListItem.KiraEkstreAktarmaId + "," + ekstreAktarmaListItem.KiraciId + ",\'" + odemeTarihi.ConvertToDatetimeEmptyIfNull() + "\');>" + ekstreAktarmaListItem.KiraciAdi + "</a>";

                                    }
                                }

                            }

                        }

                    }
                    catch (Exception)
                    {
                        MessageHelper.PublishMessage("İşlem Tamamlanamadı. EkstreAktarmaId=" + ekstreAktarmaListItem.KiraEkstreAktarmaId, ProjeConstants.MESAJ_HATA);
                    }
                    returnlist.Add(ekstreAktarmaListItem);
                }
            }
            return returnlist;
        }
        private string OdemeAyristirBtnLinki( KiraEkstreAktarmaListItem ekstreAktarmaListItem, bool aktarildiMi)
        {
            string retVal = ProjeConstants.ODEMESEBEBI_DIGER_INT.ToString();
            if (ekstreAktarmaListItem!=null)
            {
                string odemeAyristirBtnId = "OdemeAyristirBtn" + ekstreAktarmaListItem.KiraEkstreAktarmaId;
                int secilenOdemeSebebi = ekstreAktarmaListItem.OdemeSebebiId>0? ekstreAktarmaListItem.OdemeSebebiId: ProjeConstants.ODEMESEBEBI_DIGER_INT;
                string visibility = (
                    secilenOdemeSebebi == ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT || 
                    secilenOdemeSebebi == ProjeConstants.ODEMESEBEBI_KIRA_INT ||
                    secilenOdemeSebebi == ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT ||
                    secilenOdemeSebebi == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT) ?
                    string.Empty : " style='display:none'";
                string classofbutton = ekstreAktarmaListItem.Uyari.ConvertToBool() ? " class='btn btn-danger'" : " class='btn btn-warning'";

                string link = "<a id=" + odemeAyristirBtnId + visibility +
                    " href=" + ProjeConstants.PAGE_ODEMEYI_KIRA_TEMINAT_AYRISTIR +
                    "?KiraEkstreAktarmaId=" + ekstreAktarmaListItem.KiraEkstreAktarmaId + classofbutton + " >Ödemeyi Ayrıştır ve Kaydet</a>";
               
                if (aktarildiMi)
                {
                    retVal = ekstreAktarmaListItem.OdemeSebebi;
                }
                else if (ekstreAktarmaListItem.KiraciId.ConvertToInt() < 1)
                {
                    //Kiraci belli degilse odeme Sebebi listesinde Kira+Teminat bulunmasin
                    retVal = ekstreAktarmaListItem.OdemeSebebi;
                }
                else
                {
                    retVal = link;

                } 
            }

            return retVal;
        }

        private string SecCheckBoxLinki(string kaydetSil, bool aktarildiMi, int kiraciId, int kiraEkstreAktarmaId)
        {
            string retVal;
            if (aktarildiMi)
            {
                
                if (kaydetSil.Equals(ProjeConstants.KAYDET))
                {
                    retVal = "<input id=chk type=checkbox checked disabled class=ekstre-aktarildi />";
                    
                }
                else
                {
                    retVal = string.Empty;
                }
            }
            else if (kiraciId < 1)
            {
               
                if (kaydetSil.Equals(ProjeConstants.KAYDET))
                {
                    retVal = string.Empty;
                }
                else
                {
                    retVal = @"<input id=chk class=ekstre-aktarilmadi onchange=addRemoveEkstreIdToDeleteList(" + kiraEkstreAktarmaId + ",this); type=checkbox />";
                }
            }
            else
            {

                if (kaydetSil.Equals(ProjeConstants.KAYDET))
                {
                    retVal = @"<input id=chk class=ekstre-aktarilmadi onchange=addRemoveEkstreIdToList(" + kiraEkstreAktarmaId + ",this); type=checkbox />";
                }
                else
                {
                    retVal = @"<input id=chk class=ekstre-aktarilmadi onchange=addRemoveEkstreIdToDeleteList(" + kiraEkstreAktarmaId + ",this); type=checkbox />";
                }
                
            }
            return retVal;
        }

        private void ScriptCalistir(string script)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), script, true);
        }


        private bool CakismaKontrolu(KiraEkstreAktarmaListItem ekstreAktarmaListItem, decimal tutar, DateTime odemeTarihi)
        {
            bool isConflict = false;

            KiraEkstreAktarma keDao = new KiraEkstreAktarma();
            List<KiraEkstreAktarma> list = keDao.SelectByIslemNo(ekstreAktarmaListItem.IslemNo);
            if (list.Count > 1)
            {
                isConflict = true;

            }
            else
            {
                list = keDao.SelectByColumns(ekstreAktarmaListItem.Adi, ekstreAktarmaListItem.Soyadi, tutar, odemeTarihi);
                if (list.Count > 1) // TC Kimlik numarasi var konflict yok.. // TCKIMLIKNO geçerli mi diye kontrol etmek gerekir mi?
                {
                    isConflict = true;
                }
            }

            return isConflict;
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
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
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
            List<TasinmazBagisci> list = new List<TasinmazBagisci>();
            GridView1.DataSource = GetDataList(); ;
            GridView1.DataBind();

            Page.Response.Clear();
            Page.Response.Buffer = true;
            Page.Response.AddHeader("content-disposition",
             "attachment;filename=KiraEkstreAktarmaListesi_" + IslemTarihiQS.ConvertToDatetimeEmptyIfNull() + ".xls");
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
        [Serializable]
        private class KiraEkstreAktarmaListItem
        {
            public string KiraEkstreAktarmaId { get; set; }
            public string IslemTarihi { get; set; }
            public string KiraciId { get; set; }
            public string KiraciAdi { get; set; }
            public string TCKimlikNo { get; set; }
            public string Telefon { get; set; }
            public string Telefon1 { get; set; }
            public string Telefon2 { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string AdiSoyadi { get; set; }
            public string Adres { get; set; }
            public string Aciklama { get; set; }
            public string OdemeTarihi { get; set; }
            public string BankaAdi { get; set; }
            public string Tutar { get; set; }
            public string AktarildiMi { get; set; }
            public string IslemNo { get; set; }
            public string CakismaVarMi { get; set; }
            public string Uyari { get; set; }
            public string SecKaydet { get; set; }
            public string Eslestir { get; set; }
            public string OdemeAyristir { get; set; }           
            public int OdemeSebebiId { get; set; }
            public string OdemeSebebi { get; set; }
            public bool Secildi { get; set; }
        }
        protected void AktarilanlarHaricChk_CheckedChanged(object sender, EventArgs e)
        {
            AktarilanlarHaricQS = AktarilanlarHaricChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&KiraTeminatDiger=" + KiraTeminatDigerQS);
        }
        protected void KiraTeminatDigerChk_CheckedChanged(object sender, EventArgs e)
        {
            KiraTeminatDigerQS = KiraTeminatDigerChk.Checked.ToString();
            RedirectToPage(ProjeConstants.PAGE_KIRAEKSTRE_LIST + "?IslemTarihi=" + IslemTarihiQS + "&AktarilanlarHaric=" + AktarilanlarHaricQS + "&KiraTeminatDiger=" + KiraTeminatDigerQS);
        }
        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
        {

            string value = ParamKaydedilecekArray.Value;
            string[] idList = value.Split(',');

            if (idList.Length < 1)
            {
                MessageHelper.PublishMessage("Kaydetmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
            }
            else
            {
                ModalTitleLbl.Text = "Seçilen Kayıtlar Aktarılacak";
                ModalSubTitleLbl.Text = idList.Length + " Adet satırı kaydetmek için seçtiniz.";
                UyariMesajiLbl.Text = "Lütfen kaydetmeden önce dikkatle inceleyiniz.";
                KaydetNowBtn.Visible = true;
                var openPopup = "OpenModalOnay();";
                ScriptCalistir(openPopup);
            }
        }
        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SecilenListeyiKaydet();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void OdemePlaniModalAcBtn_Click(object sender, EventArgs e)
        {
            int kiraciId = ParamKiraciIdLbl.Value.ConvertToInt();
            DateTime odemeTarihi = ParamOdemeTarihiLbl.Value.ConvertToDatetime();
            Kiraci kiraci = new Kiraci();
            kiraci = kiraci.Select<Kiraci>(kiraciId);
            if (kiraci != null)//bu kiraci varsa
            {
                KiraSozlesme kiraSozlesme = new KiraSozlesme();
                kiraSozlesme = kiraSozlesme.SelectEnYakinTarihliSozlesmeByKiraciIdTarih(kiraci.Id, odemeTarihi);
                if (kiraSozlesme != null) //bu sozlesme varsa
                {
                    OdemePlani odemePlani = new OdemePlani();
                    bool odemePlaniVarMi = odemePlani.OdemePlaniVarMi(kiraSozlesme.Id);
                    if (odemePlaniVarMi)
                    {
                        OdemePlaniGoruntule(kiraSozlesme, kiraci);
                    }
                }
            }
        }
        private void OdemePlaniGoruntule(KiraSozlesme kiraSozlesme, Kiraci kiraci)
        {
            TitleLbl.Text = " Kiraci : " + kiraci.Adi + " " + kiraci.Soyadi;
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);

            OdemePlani odemePlaniDao = new OdemePlani();
            List<OdemePlani> list = odemePlaniDao.SelectBySozlesmeId(kiraSozlesme.Id);
            if (list.Count > 0)
            {
                DevirLbl.Text = "Devir Anapara : " + kiraSozlesme.DevirAnaPara.ToString("N", culturInfo) + "      "
                    + "Devir Faiz : " + kiraSozlesme.DevirFaizTutari.ToString("N", culturInfo)
                    + "Devir FaizliBakiye : " + kiraSozlesme.DevirFaizliBakiye.ToString("N", culturInfo);
            }
            foreach (OdemePlani odemePlani in list)
            {
                if (odemePlani.Sira == 0)
                {
                    continue;
                }
                TableRow row = new TableRow();
                TableCell SiraNoCell = new TableCell();
                SiraNoCell.Text = odemePlani.Sira.ToString(); ;
                row.Controls.Add(SiraNoCell);

                TableCell YilCell = new TableCell();
                YilCell.CssClass = "text-end";
                YilCell.Text = odemePlani.Yil.ToString();
                row.Controls.Add(YilCell);

                TableCell AyCell = new TableCell();
                AyCell.Text = odemePlani.Ay.ToString();
                row.Controls.Add(AyCell);

                TableCell KiraBedeliCell = new TableCell();
                KiraBedeliCell.CssClass = "text-end";
                KiraBedeliCell.Text = odemePlani.KiraBedeli.ToString("N", culturInfo);
                row.Controls.Add(KiraBedeliCell);

                TableCell OdenenTutarCell = new TableCell();
                OdenenTutarCell.CssClass = "text-end";
                OdenenTutarCell.Text = odemePlani.OdenenTutar.ToString("N", culturInfo);
                row.Controls.Add(OdenenTutarCell);

                OdemePlaniTable.Controls.Add(row);
            }
            var jsString = @"         
                var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OdemePlaniModal'));
                myModalInstance.show();";
            ScriptCalistir(jsString);

        }

    }
}