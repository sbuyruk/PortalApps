using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraArtisYazisiWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraArtisYazisiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraArtisYazisiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private string SecilenAyQS
        {
            get
            {

                if (ViewState["SecilenAy"] == null)
                {
                    if (Page.Request.QueryString["SecilenAy"] != null)
                    {
                        ViewState["SecilenAy"] = Page.Request.QueryString["SecilenAy"];
                    }
                    else
                    {
                        ViewState["SecilenAy"] = string.Empty;
                    }
                }
                return ViewState["SecilenAy"].ToString();
            }

            set
            {
                ViewState["SecilenAy"] = value;
            }
        }
        private string SecilenYilQS
        {
            get
            {

                if (ViewState["SecilenYil"] == null)
                {
                    if (Page.Request.QueryString["SecilenYil"] != null)
                    {
                        ViewState["SecilenYil"] = Page.Request.QueryString["SecilenYil"];
                    }
                    else
                    {
                        ViewState["SecilenYil"] = string.Empty;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        private string SecilenBolgeQS
        {
            get
            {

                if (ViewState["SecilenBolge"] == null)
                {
                    if (Page.Request.QueryString["SecilenBolge"] != null)
                    {
                        ViewState["SecilenBolge"] = Page.Request.QueryString["SecilenBolge"];
                    }
                    else
                    {
                        ViewState["SecilenBolge"] = string.Empty;
                    }
                }
                return ViewState["SecilenBolge"].ToString();
            }

            set
            {
                ViewState["SecilenBolge"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDownList();
                SetDDLValues(); //ay ve yılı querystringden al
                ParametreleriDoldur();
                TabloOlustur();
            }
            
        }
        private void FillDropDownList()
        {
            AyDDLDoldur();
            YilDDLDoldur();
            BolgeDDLDoldur();
        }
        private void BolgeDDLDoldur()
        {
            BolgeDDL.Items.Clear();
            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_GENELMUDURLUK);
            System.Web.UI.WebControls.ListItem li1 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_ISTANBUL);
            System.Web.UI.WebControls.ListItem li2 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_IZMIR);
            System.Web.UI.WebControls.ListItem li3 = new System.Web.UI.WebControls.ListItem(ProjeConstants.BOLGE_MERSIN);
            BolgeDDL.Items.Add(li);
            BolgeDDL.Items.Add(li1);
            BolgeDDL.Items.Add(li2);
            BolgeDDL.Items.Add(li3);
            SecilenBolgeQS = string.IsNullOrEmpty(SecilenBolgeQS) ? BolgeDDL.SelectedItem.Text : SecilenBolgeQS;
        }
        //private void AyDDLDoldur()
        //{

        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ocak", "1"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Şubat", "2"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Mart", "3"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Nisan", "4"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Mayıs", "5"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Haziran", "6"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Temmuz", "7"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ağustos", "8"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Eylül", "9"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Ekim", "10"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Kasım", "11"));
        //    AyDDL.Items.Add(new System.Web.UI.WebControls.ListItem("Aralık", "12"));

        //}
        protected void AyYilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabloOlustur();
        }
        private void AyDDLDoldur()
        {
            AyDDL.Items.Clear();
            System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem(DateTime.Today.ToString("MMMM"), DateTime.Today.AddMonths(-1).ToString("MM"));//DİKKAT Bir önceki ay
            System.Web.UI.WebControls.ListItem li1 = new System.Web.UI.WebControls.ListItem(DateTime.Today.AddMonths(1).ToString("MMMM"), DateTime.Today.ToString("MM"));
            AyDDL.Items.Add(li);
            AyDDL.Items.Add(li1);
        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year; i++)
            {
                YilDDL.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SetDDLValues()
        {
            try
            {
                #region tarih
                //acilista ay ve yili querystring ile gelen ay ve yıla eşitle boş geldiyse gecen aya/yila eşitle               
                //ay
                string ay = !string.IsNullOrEmpty(SecilenAyQS) ? SecilenAyQS : DateTime.Today.Month.ToString();
                System.Web.UI.WebControls.ListItem AyItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(ay))
                    AyItem = AyDDL.Items.FindByValue(ay);

                if (AyItem != null)
                {
                    AyDDL.SelectedValue = AyItem.Value;
                    SecilenAyQS = AyItem.Value;
                }

                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                System.Web.UI.WebControls.ListItem YilItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }
                #endregion tarih
                #region bolge
                //bolge
                System.Web.UI.WebControls.ListItem bolgeItem = new System.Web.UI.WebControls.ListItem();
                if (!string.IsNullOrEmpty(SecilenBolgeQS))
                    bolgeItem = BolgeDDL.Items.FindByValue(SecilenBolgeQS);

                if (bolgeItem != null)
                {
                    BolgeDDL.SelectedValue = bolgeItem.Value;
                    SecilenBolgeQS = bolgeItem.Value;
                }
                #endregion
            }
            catch (Exception)
            {

                //TODO
            }
        }
        private void ParametreleriDoldur()
        {
            DateTime bugun = DateTime.Today;

            Parafe1Txt.Text = @"…./" + bugun.ToString("MM") + @"/" + bugun.Year + " Eml.Ynt.Kd.Uzm.Z.ÇALIŞ";
            Parafe2Txt.Text = @"…./" + bugun.ToString("MM") + @"/" + bugun.Year + " İnş.Eml.Ynt.Ş.Md.H.ŞENEL";
            KoordineTxt.Text = @"…./" + bugun.ToString("MM") + @"/" + bugun.Year + " Huk.Müş.E.ŞENGÜL";
            ImzalayanTxt.Text = @"Zeki YAĞCI";
            ImzalayanMakamTxt.Text = @"Vakıf Hiz.Grp. Bşk.";
            EvrakTarihiTxt.Text = bugun.ToString("dd") + " " + bugun.ToString("MMMM") + " " + bugun.Year;

            EvrakSayisiYiliTxt.Text = bugun.ToString("yy");

        }
        private void TabloOlustur()
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime tarih = new DateTime(DateTime.Today.Year, ay, 1);
            decimal tufe = SecilenAyIcinTufeBul(tarih);
            TufeTxt.Text = tufe.ToString("N", culturInfo);
            var jsonData = TabloJson(); 
            UtilityHelper.ScriptCalistir("setDataSet(" + jsonData + ");");
        }
        private string TabloJson()
        {
            string jSon = string.Empty;

            try
            {
                List<KiraArtisListItem> list = GetDataList();
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
        private string KiraArtisJson()
        {
            string jSon = string.Empty;

            List<KiraArtisListItem> list = GetDataList();
            TableDataLbl.Text = "Toplam " + list.Count + " kayıt bulundu";
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private List<KiraArtisListItem> GetDataList()
        {

            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime tarih = new DateTime(DateTime.Today.Year, ay, 1);
            KiraSozlesme kiraSozlesmeDao = new KiraSozlesme();

            DataTable dataTable = kiraSozlesmeDao.SelectKiraArtisiGelenSozlesmelerReturnDT(SecilenBolgeQS,tarih);
            int SiraNo = 1;

            List<KiraArtisListItem> list = new List<KiraArtisListItem>();
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DataView dataView = new DataView(dataTable);
            foreach (DataRowView row in dataView)
            {
                string kiraciId = row["KiraciId"].ToString();
                string kiraciAdi = row["KiraciAdi"].ToString();
                string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                DateTime ilkSozlesmeTar = row["IlkSozlesmeTar"].ConvertToDatetime();
                string ilkSozlesmeTarStr = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                string artisAyi = row["ArtisAyi"].ToString();
                bool aktif = row["Aktif"].ReturnFalseIfNull().ConvertToBool();

                decimal tufe = TufeTxt.Text.ConvertToDecimal();//GelecekAyIcinTufeBul();
                DateTime bugun = DateTime.Today;
                decimal yeniKiraBedeli = kiraBedeli + Math.Round(kiraBedeli * tufe / 100);
                //bool sozlesmeYenilendiMi = sozBitTar.ConvertToDatetime() > new DateTime( bugun.Year, bugun.AddMonths(2).Month,1); //--new DateTime(bugun.AddYears(1).Year, bugun.AddMonths(1).Month,1);
                //if (sozlesmeYenilendiMi)
                //{
                //    KiraSozlesme oncekiKiraSozlesme = new KiraSozlesme();
                //    oncekiKiraSozlesme = oncekiKiraSozlesme.SelectByKiraciIdTarih(kiraciId.ConvertToInt(), sozBitTar.ConvertToDatetime().AddMonths(-1));
                //    if (oncekiKiraSozlesme != null)
                //    {
                //        kiraBedeli = oncekiKiraSozlesme.KiraBedeli;
                //    }

                //}
                string artisOrani = "%" + tufe.ToString("N", culturInfo) + " (TÜFE)";
                //DateTime bastar = string.IsNullOrEmpty(kiraSozlesmeDao.SozBasTar.ConvertToDatetimeEmptyIfNull()) ? DateTime.Today : kiraSozlesmeDao.SozBasTar;
                //DateTime yenibastar = bastar.AddYears(1);
                //if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                //   (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                //   kiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                //{
                //    yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100);
                //    artisOrani = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString("N", culturInfo) + " (6098 Say.Kanun)";
                //}

                DateTime bastar = string.IsNullOrEmpty(sozBasTar) ? DateTime.Today : sozBasTar.ConvertToDatetime();
                DateTime yenibastar = bastar.AddYears(1);
                if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                   (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                   kiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                {
                    yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100);
                    artisOrani = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString("N", culturInfo) + " (6098 Say.Kanun)";
                }
                else
                {
                    yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * tufe / 100);
                }

                string adres = row["Adres"].ToString();

                string ili = row["Ili"].ToString();
                string ilcesi = row["Ilcesi"].ToString();
                string bolge = row["bolge"].ToString();

                KiraArtisListItem kiraArtisListItem = new KiraArtisListItem();
                kiraArtisListItem.Sirano = SiraNo++.ToString();
                kiraArtisListItem.SozlesmeId = kiraSozlesmeId.ToString();
                kiraArtisListItem.KiraciId = kiraciId;
                kiraArtisListItem.KiraciAdi = kiraciAdi + " " + kiraciSoyadi;
                kiraArtisListItem.SozlesmeTarihi = ilkSozlesmeTarStr;

                int kiraSuresi = Math.Round(DateTime.Today.AddMonths(1).Subtract(ilkSozlesmeTar).TotalDays / 365).ConvertToInt();
                kiraArtisListItem.KiraSuresi = kiraSuresi + " Yıl";
                kiraArtisListItem.BesYil = kiraSuresi >= 5 ? "True" : "False";
                kiraArtisListItem.OnYil = kiraSuresi >= 10 ? "True" : "False";
                kiraArtisListItem.SozlesmeBasTar = sozBasTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.SozlesmeBitTar = sozBitTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.TarihAraligi = sozBasTar.ConvertToDatetimeEmptyIfNull() + "-" + sozBitTar.ConvertToDatetimeEmptyIfNull();
                kiraArtisListItem.KiralamaAmaci = kiralamaAmaci;
                kiraArtisListItem.KiraBedeli = kiraBedeli.ToString("N", culturInfo);
                kiraArtisListItem.Tufe = artisOrani;
                kiraArtisListItem.YeniKiraBedeli = yeniKiraBedeli.ToString("N", culturInfo);
                kiraArtisListItem.Adres = "- " + adres;
                kiraArtisListItem.TamAdres = "- " + adres + " " + ilcesi + "-" + ili;
                kiraArtisListItem.Ilcesi = ilcesi;
                kiraArtisListItem.Ili = ili;
                kiraArtisListItem.Bolge = bolge;
                kiraArtisListItem.ArtisAyi = new DateTime(DateTime.Today.Year, artisAyi.ConvertToInt(), 1).ToString("MMMM");
                kiraArtisListItem.YenilendiMi = aktif ? "Yenilenecek" : "Yenilendi";
                list.Add(kiraArtisListItem);
            }
            return list;
        }
        private decimal SecilenAyIcinTufeBul(DateTime tarih)
        {
            decimal tufe = 1M;
            YasalFaiz yasalFaiz = new YasalFaiz();
            tarih = tarih.AddMonths(1);//bir önceki ay geliyor
            //DateTime gelecekAy = DateTime.Today.AddMonths(1);
            yasalFaiz = yasalFaiz.SelectByYilAy(tarih.Year, tarih.Month);
            if (yasalFaiz != null)
            {
                tufe = yasalFaiz.Tufe;
            }
            return tufe;
        }
        protected void AyDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenAyQS = AyDDL.SelectedItem.Value.ToString();
            //KayitGetir();
            TabloOlustur();
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            //KayitGetir();
            TabloOlustur();
        }
        protected void BolgeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenBolgeQS = BolgeDDL.SelectedItem.Text;
            //KayitGetir();
            TabloOlustur();
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void YaziyiOlusturBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Dosya adları 
                string zaman = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
                string yaziDosyaAdi = "Kira-Artis-" + SecilenBolgeQS + "-(" + zaman + ").docx";
                string etiketDosyaAdi = "Adres-EtiketiKA-" + SecilenBolgeQS + "-(" + zaman + ").docx";
                bool isYaziOlusturuldu = YeniYaziOlustur(yaziDosyaAdi);
                if (isYaziOlusturuldu)
                {
                    YeniAdresEtiketDosyasiOlustur(etiketDosyaAdi);
                    MessageHelper.PublishMessage("Dosyalar hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz", ProjeConstants.MESAJ_BASARILI, 2000);
                }
                else
                    MessageHelper.PublishMessage("Hata Oluştu", ProjeConstants.MESAJ_HATA);
                TabloOlustur();
            }
            catch (Exception ex)
            {
                TabloOlustur();
                Exception ex1 = new Exception("Yazı ve Adres oluşturmada hata");
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
        }
        private bool YeniAdresEtiketDosyasiOlustur(string dosyaAdi)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream("AdresEtiketiTemplate.docx");
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);

                MemoryStream destinationStream = AddAdresEtiketData2DestinationStream(templateStream, templateParagraphs);

                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);

                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.TBYSBELGELERI_LIB + @"/" + dosyaAdi;

                string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                int index = sourceString.IndexOf(removeString);
                string rootUrl = (index < 0)
                    ? sourceString
                    : sourceString.Remove(index, removeString.Length);

                AdresEtiketLnk.Text = dosyaAdi;
                AdresEtiketLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;

                AdresEtiketLnk.Visible = true;
                isYaziOlusturuldu = true;
            }
            catch (Exception ex)
            {
                isYaziOlusturuldu = false;
                ExceptionHelper exh = new ExceptionHelper(ex);
                exh.PublishException();
            }
            return isYaziOlusturuldu;
        }
        /// <summary>
        /// Template dosyasını aç
        /// Paragrafları kopyala
        /// Yeni bir stream içine koy
        /// Yeni stream içinde veritabanından gelen verilere göre yeni paragraflar oluştur AddData2DestinationStream()
        /// SharePointe kaydet
        /// Yeni dosyalar için bağlantı oluştur
        /// </summary>
        /// <param name="dosyaAdi"></param>
        /// <returns></returns>
        private bool YeniYaziOlustur(string dosyaAdi)
        {
            bool isYaziOlusturuldu = false;
            try
            {
                MemoryStream templateStream = GetTemplateStream(ProjeConstants.TBYS_KIRAARTIS_TEMPLATE);
                IEnumerable<Paragraph> templateParagraphs = GetTemplateParagraphs(templateStream);

                MemoryStream destinationStream = AddData2DestinationStream(templateStream, templateParagraphs);

                destinationStream.Seek(0, SeekOrigin.Begin);
                destinationStream.Position = 0;

                AddToSharePoint(destinationStream, dosyaAdi);
                string dosyaUrl = SPContext.Current.Web.Url + @"/" + ProjeConstants.TBYSBELGELERI_LIB + @"/" + dosyaAdi;

                string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                string removeString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;

                int index = sourceString.IndexOf(removeString);
                string rootUrl = (index < 0)
                    ? sourceString
                    : sourceString.Remove(index, removeString.Length);

                DosyaLnk.Text = dosyaAdi;
                DosyaLnk.NavigateUrl = rootUrl + "/_layouts/15/download.aspx?SourceUrl=" + dosyaUrl;
                DosyaLnk.Visible = true;
                isYaziOlusturuldu = true;
            }
            catch (Exception ex)
            {
                isYaziOlusturuldu = false;
                throw ex;
            }
            return isYaziOlusturuldu;
        }
        private MemoryStream GetTemplateStream(string templateFileName)
        {
            string newFileUrl = string.Empty;

            string siteUrl = SPContext.Current.Web.Url;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                Console.WriteLine("Querying for Test.docx");
                SPList list = SPContext.Current.Web.Lists[ProjeConstants.TBYSBELGELERI_LIB];
                SPQuery query = new SPQuery();
                query.ViewFields = @"<FieldRef Name='FileLeafRef' />";
                query.Query =
                  @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + templateFileName + @"</Value>
                          </Eq>
                        </Where>";
                SPListItemCollection collection = list.GetItems(query);
                MemoryStream memStr = new MemoryStream();
                if (collection.Count > 0)
                {
                    SPFile file = collection[0].File;
                    byte[] byteArray = file.OpenBinary();
                    memStr.Write(byteArray, 0, byteArray.Length);
                }
                return memStr;
            }
        }
        private MemoryStream AddData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime tarih = new DateTime(DateTime.Today.Year, ay, 1);
            MemoryStream destinationStream = null;
            DateTime bugun = DateTime.Today;
            DateTime gecenAySonGun = new DateTime(bugun.Year, bugun.Month, 1).AddDays(-1);
            DateTime gecenAyIlkGun = new DateTime(bugun.Year, bugun.AddMonths(-1).Month, 1);
            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            DataTable dataTable = kiraSozlesme.SelectKiraArtisiGelenSozlesmelerReturnDT(SecilenBolgeQS, tarih);
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string kiraciId = row["KiraciId"].ToString();
                    string kiraciAdi = row["KiraciAdi"].ToString();
                    string kiraciSoyadi = row["KiraciSoyadi"].ToString();
                    string ilkSozlesmeTar = row["IlkSozlesmeTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    string sozBasTar = row["SozBasTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    string sozBitTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetimeEmptyIfNull();
                    string yeniSozBasTar = row["SozBitTar"].ReturnEmptyIfNull().ConvertToDatetime().AddDays(1).ConvertToDatetimeEmptyIfNull();
                    string kiralamaAmaci = row["KiralamaAmaci"].ToString();
                    decimal kiraBedeli = row["KiraBedeli"].ConvertToDecimal();
                    int kiraSozlesmeId = row["KiraSozlesmeId"].ConvertToInt();
                    string artisAyi = row["ArtisAyi"].ToString();
                    string odemeSekli = row["OdemeSekli"].ToString();

                    string adres = row == null ? "" : row["Adres"].ToString();
                    string semt = row == null ? "" : row["Semt"].ToString();
                    string ili = row == null ? "" : row["Ili"].ToString();
                    string ilcesi = row == null ? "" : row["Ilcesi"].ToString();
                    string semtIlIlce = (string.IsNullOrEmpty(semt) ? "" : semt + @"-") + ilcesi + @"/" + ili;

                    decimal tufe = TufeTxt.Text.ConvertToDecimal();// GelecekAyIcinTufeBul();
                    decimal yeniKiraBedeli = kiraBedeli + Math.Round(kiraBedeli * tufe / 100);
                    adres = row["Adres"].ToString();

                    ili = row["Ili"].ToString();
                    ilcesi = row["Ilcesi"].ToString();

                    // %25 olayı
                    string artisOraniVar = "Tüketici Fiyat Endeksi (TÜFE) on iki aylık ortalamalara göre %" +tufe;
                    string artisOrani = "%" + TufeTxt.Text + " (TÜFE)";
                    DateTime bastar = string.IsNullOrEmpty(sozBasTar) ? DateTime.Today : sozBasTar.ConvertToDatetime();
                    DateTime yenibastar = bastar.AddYears(1);
                    if ((yenibastar >= ProjeConstants.SINIRLIKIRAARTISI_BASLAMATARIHI) &&
                       (yenibastar <= ProjeConstants.SINIRLIKIRAARTISI_BITISTARIHI) &&
                       kiralamaAmaci.Equals(ProjeConstants.SINIRLIKIRAARTISI_UYGULANACAKTASINMAZCINSI))
                    {
                        yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * ProjeConstants.SINIRLIKIRAARTISI_ORANI / 100);
                        artisOrani = "%" + ProjeConstants.SINIRLIKIRAARTISI_ORANI.ToString("N", culturInfo) ;
                        artisOraniVar = "6098 sayılı Türk Borçlar Kanununa eklenen geçici madde kapsamında yeni dönem kiranızın " + artisOrani;
                    }
                    else
                    {
                        yeniKiraBedeli = Math.Round(kiraBedeli + kiraBedeli * tufe / 100);
                    }
                    
                    //

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("EvrakYilVar", EvrakSayisiYiliTxt.Text);
                    keyValues.Add("EvrakTarihiVar", EvrakTarihiTxt.Text);
                    keyValues.Add("AdSoyadVar", kiraciAdi + " " + kiraciSoyadi);
                    keyValues.Add("AdresVar", adres);
                    keyValues.Add("SemtIlceIlVar", semtIlIlce);

                    keyValues.Add("IlkSozlesmeTarihiVar", ilkSozlesmeTar);
                    keyValues.Add("SozBitTarVar", sozBitTar);
                    keyValues.Add("YeniSozBasTarVar", yeniSozBasTar);
                    keyValues.Add("KiraBedeliVar", kiraBedeli.ToString("N", culturInfo) + " TL ");
                    keyValues.Add("YeniBedelVar", yeniKiraBedeli.ToString("N", culturInfo) + " TL ");
                    keyValues.Add("TufeVar", "%" + TufeTxt.Text);
                    keyValues.Add("OdemeSekliVar", odemeSekli.ToLower());
                    keyValues.Add("SonOdemeVar", odemeSekli.Equals(ProjeConstants.KIRA_ODMSEKLI_YILLIK)? "sözleşmede belirlenen ayın son mesai gününe": "her ayın en son mesai günü akşamına");
                    keyValues.Add("ArtisOraniVar", artisOraniVar);

                    keyValues.Add("ImzaVar", ImzalayanTxt.Text);
                    keyValues.Add("UnvanVar", ImzalayanMakamTxt.Text);
                    keyValues.Add("Paraf1Var", Parafe1Txt.Text);
                    keyValues.Add("Paraf2Var", Parafe2Txt.Text);
                    keyValues.Add("KoordineBaslikVar", "KOORDİNE:");
                    keyValues.Add("KoordineVar", KoordineTxt.Text);

                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);

                    keyValues.Remove("Paraf1Var");
                    keyValues.Remove("Paraf2Var");
                    keyValues.Remove("KoordineBaslikVar");
                    keyValues.Remove("KoordineVar");
                    keyValues.Add("Paraf1Var", "");
                    keyValues.Add("Paraf2Var", "");
                    keyValues.Add("KoordineBaslikVar", "");
                    keyValues.Add("KoordineVar", "");

                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);
                }
            }


            return destinationStream;
        }

        private MemoryStream AddParagraph2DestinationStream(MemoryStream destinationStream, IEnumerable<Paragraph> templateParagraphs)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(destinationStream, true))
            {
                //boş sayfa ekle
                Paragraph PageBreakParagraph = new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                wordDoc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);

                //template yaziyi ekle
                foreach (var paragraph in templateParagraphs)
                {
                    Paragraph newPara = (Paragraph)paragraph.CloneNode(true);// new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break() { Type = BreakValues.Page }));
                    wordDoc.MainDocumentPart.Document.Body.Append(newPara);
                }
            }
            return destinationStream;
        }
        private MemoryStream AddAdresEtiketData2DestinationStream(MemoryStream templateStream, IEnumerable<Paragraph> templateParagraphs)
        {
            int ay = AyDDL.SelectedItem.Value.ConvertToInt();
            DateTime tarih = new DateTime(DateTime.Today.Year, ay, 1);

            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            MemoryStream destinationStream = null;

            KiraSozlesme kiraSozlesme = new KiraSozlesme();
            DataTable dataTable = kiraSozlesme.SelectKiraArtisiGelenSozlesmelerReturnDT(SecilenBolgeQS, tarih);
            if (dataTable != null)
            {
                int index = 1;
                List<string> uzunAdresliler = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    string kiraciAdi = row["KiraciAdi"].ToString();
                    string adres = row == null ? "" : row["Adres"].ToString();
                    string semt = row == null ? "" : row["Semt"].ToString();
                    string ili = row == null ? "" : row["Ili"].ToString();
                    string ilcesi = row == null ? "" : row["Ilcesi"].ToString();
                    string semtIlIlce = (string.IsNullOrEmpty(semt) ? "" : semt + @"-") + ilcesi + @"/" + ili;

                    //create key value pair, key represents words to be replace and 
                    //values represent values in document in place of keys.
                    Dictionary<string, string> keyValues = new Dictionary<string, string>();
                    keyValues.Add("AdSoyad" + index + "Var", kiraciAdi);
                    keyValues.Add("Adres" + index + "Var", string.IsNullOrEmpty(adres) ? "Adresi yok" : adres.Substring(0, adres.Length > 110 ? 110 : adres.Length - 1));
                    keyValues.Add("SemtIlceIl" + index + "Var", semtIlIlce);
                    index++;
                    destinationStream = SearchAndReplace(templateStream, keyValues);
                    if (index >= 21)//sayfa bitti yeni sayfa ekle
                    {
                        destinationStream = AddParagraph2DestinationStream(destinationStream, templateParagraphs);
                        index = 1;
                    }
                    if (!string.IsNullOrEmpty(adres) && adres.Trim().Length > 110)
                    {
                        uzunAdresliler.Add(kiraciAdi);
                    }
                }
                if (index < 21)
                {

                    for (int i = index; i < 22; i++)
                    {
                        Dictionary<string, string> keyValues = new Dictionary<string, string>();
                        keyValues.Add("AdSoyad" + i + "Var", "");
                        keyValues.Add("Adres" + i + "Var", "");
                        keyValues.Add("SemtIlceIl" + i + "Var", "");
                        destinationStream = SearchAndReplace(templateStream, keyValues);
                    }

                }
                if (uzunAdresliler.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, uzunAdresliler);
                    MessageHelper.PublishMessage(message + Environment.NewLine +
                        " adresi çok uzun olduğundan kesilerek kısaltıldı. Lütfen etiketini kontrol ediniz. ", ProjeConstants.MESAJ_BILGI);
                }
            }


            return destinationStream;
        }
        private IEnumerable<Paragraph> GetTemplateParagraphs(MemoryStream templateStream)
        {
            //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
            //{
            WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true);
            var body = wordDoc.MainDocumentPart.Document.Body;
            var paras = body.Elements<Paragraph>();
            return paras;
            //}

        }
        public MemoryStream SearchAndReplace(MemoryStream templateStream, Dictionary<string, string> dict)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                foreach (KeyValuePair<string, string> item in dict)
                {
                    Regex regexText = new Regex(item.Key);
                    docText = regexText.Replace(docText, item.Value);
                }

                using (StreamWriter sw = new StreamWriter(
                          wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
                return templateStream;
            }
        }
        protected void AddToSharePoint(MemoryStream memStream, string fileName)
        {
            //string url = System.Web.HttpContext.Current.Request.Url.ToString();
            using (SPSite spSite = new SPSite(SPContext.Current.Web.Url))
            {
                //Get the document library object
                SPList docLib = SPContext.Current.Web.Lists[ProjeConstants.TBYSBELGELERI_LIB];
                SPFile file = docLib.RootFolder.Files.Add(fileName, memStream, true);
                file.Update();
            }
        }

        private class KiraArtisListItem
        {
            public string Sirano { get; set; }
            public string ArtisAyi { get; set; }
            public string SozlesmeId { get; set; }
            public string KiraciAdi { get; set; }
            public string KiraciId { get; set; }
            public string KiralamaAmaci { get; set; }
            public string SozlesmeTarihi { get; set; }
            public string SozlesmeBasTar { get; set; }
            public string SozlesmeBitTar { get; set; }
            public string TarihAraligi { get; set; }
            public string KiraBedeli { get; set; }
            public string Tufe { get; set; }
            public string YeniKiraBedeli { get; set; }
            public string Ili { get; set; }
            public string Ilcesi { get; set; }
            public string Bolge { get; set; }
            public string Adres { get; set; }
            public string TamAdres { get; set; }
            public string YenilendiMi { get; set; }
            public string BesYil { get; set; }
            public string OnYil { get; set; }
            public string KiraSuresi { get; set; }

        }
    }
}