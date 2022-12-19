using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace TBYS_WebParts.KiraOdemeDosyasiYuklemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class KiraOdemeDosyasiYuklemeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public KiraOdemeDosyasiYuklemeWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        bool isVakifBank2Aktarildi = false;
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
                NextBtn.Visible = isVakifBank2Aktarildi;
                if (!Page.IsPostBack)
                {
                    
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
        protected void KaydetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Vakifbank2Save();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void Vakifbank2Save()
        {
            if (!isVakifBank2Aktarildi && Vakifbank2FU.HasFile)
            {
                var exceptionHelper = SaveVakifBank2File(Vakifbank2FU.FileContent, DateTime.Now, CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    Vakifbank2FU.Enabled = true;
                    exceptionHelper.PublishException();
                }
                else
                {
                    Vakifbank2FU.Enabled = false;
                }

            }
        }
        public ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            CultureInfo culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            try
            {

                var vakifBank2Data = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.BANKA_VAKIF2_ILKKACSATIRHARIC, ProjeConstants.BANKA_VAKIF2_SONKACSATIRHARIC, true);
                if (vakifBank2Data != null)
                {
                    foreach (DataRow row in vakifBank2Data.Rows)
                    {
                        KiraEkstreAktarma kiraEkstreAktarma = new KiraEkstreAktarma();
                        var hesapNo = row[0].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
                        var islem = row[5].ReturnEmptyIfNull().ToString();
                        var tutar = row[6].ReturnEmptyIfNull().ToString().Replace(".", ",").ConvertToDecimal();//row[6].ReturnEmptyIfNull().ToString();
                        var detay = row[16].ReturnEmptyIfNull().ToString();
                        if (string.IsNullOrEmpty(hesapNo))// ilk kolon boş ise dosya bitti çık
                        {
                            break;
                        }
                        else if (islem.Contains("Batch Yatan")//Kart ile bağıştan gelen toplam miktar, zaten ayrıca girişi yapılıyor, dikkate alma
                           || islem.Contains("Batch Komisyonu")
                           || islem.Contains("Otomatik Süpürme İşlemleri Virman")
                           || islem.Contains("Valor İşlemi İçin Para Çek ve Yatır")
                           || islem.Contains("Yatırım Fonu Satış")
                           || detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafından TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafına gelen")//ziraat bank
                           || detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME V tarafından TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafına gelen") //işbank
                           || detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇL tarafından TÜRK SILAHLI KUVVETLERINI GÜÇLENDIRME VAKFI tarafına gelen ")//garanti bank
                           || detay.Contains(" virman ")
                           || detay.Contains(" VİRMAN ")
                           || hesapNo.Equals("HESAP NO")
                           || tutar < 0)
                            continue;
                        try
                        {
                            var borc_alacak = row[15].ReturnEmptyIfNull().ToString();//borc satırı için işlem yapma
                            if (borc_alacak.Equals("B"))
                            {
                                continue;
                            }
                            //dummy rows
                            //var hesapno = row[0].ReturnEmptyIfNull().ToString();
                            //var fisnono = row[1].ReturnEmptyIfNull().ToString();
                            var hareketTar = row[2].ReturnEmptyIfNull().ToString();
                            //

                            //var odemeTarihi = row[3].ReturnEmptyIfNull().ToString();
                            //dummy rows
                            //var kartno = row[4].ReturnEmptyIfNull().ToString();
                            //

                            //dummy rows
                            //var bakiye = row[7].ReturnEmptyIfNull().ToString();
                            var kanal = row[8].ReturnEmptyIfNull().ToString();
                            var islemno = row[9].ReturnEmptyIfNull().ToString();
                            //var referans = row[10].ReturnEmptyIfNull().ToString();
                            //var havale = row[11].ReturnEmptyIfNull().ToString();
                            //var refno = row[12].ReturnEmptyIfNull().ToString();
                            //var tckn = row[13].ReturnEmptyIfNull().ToString();
                            //var vkn = row[14].ReturnEmptyIfNull().ToString();
                            var b_a = row[15].ReturnEmptyIfNull().ToString();
                            //
                            //var detay = row[16].ReturnEmptyIfNull().ToString(); //yukarıda
                            var aciklama = detay.ToString();
                            var adi = string.Empty;
                            var tel1 = string.Empty;
                            var adres = string.Empty;//adres bilgisi yok

                            var splitText = new string[] { " sorgu numaralı ", " tarafından " };
                            var holder = detay.Split(splitText, StringSplitOptions.None);
                            if (holder.Length > 2)
                            {
                                adi = holder[1].ReturnEmptyIfNull().ToString().Trim();

                            }
                            else
                            {

                                var splitTextM = new string[] { " nolu ", " hesabından " };
                                holder = detay.Split(splitTextM, StringSplitOptions.None);

                                if (holder.Length > 2)
                                {
                                    adi = holder[1].ReturnEmptyIfNull().ToString().Trim();

                                }
                                else
                                {
                                    var splitTextA = new string[] { " TRF YT " };
                                    holder = detay.Split(splitTextA, StringSplitOptions.None);
                                    if (holder.Length > 1)
                                    {
                                        adi = holder[0].ReturnEmptyIfNull().ToString().Trim();

                                    }
                                    else
                                    {
                                        var splitTextB = new string[] { "A.Ş. ", " hesabından " };
                                        holder = detay.Split(splitTextB, StringSplitOptions.None);
                                        if (holder.Length > 2)
                                        {
                                            adi = holder[1].ReturnEmptyIfNull().ToString().Trim();
                                        }
                                        else
                                        {
                                            var splitTextC = new string[] { "Adına işlem Yapan: " };
                                            holder = detay.Split(splitTextC, StringSplitOptions.None);
                                            if (holder.Length > 1)
                                            {
                                                adi = holder[1].ReturnEmptyIfNull().ToString().Trim();
                                            }
                                            else
                                            {
                                                var splitTextD = new string[] { "Kiracı Adı:" };
                                                holder = detay.Split(splitTextD, StringSplitOptions.None);
                                                if (holder.Length > 2)
                                                    adi = holder[2].ReturnEmptyIfNull().ToString().Trim();
                                            }
                                        }
                                    }
                                }
                            }

                            //işlem numarası varsa kaydı atla
                            KiraEkstreAktarma keaDao = new KiraEkstreAktarma();
                            List<KiraEkstreAktarma> bulunanIslemNolar = keaDao.SelectByIslemNo(islemno);
                            if (bulunanIslemNolar.Count > 0)
                            {
                                Exception kayitVar = new Exception("İşlem Numarası Çakışıyor. (" + adi + ", " + tutar + "TL, " + hareketTar);
                                exceptionHelper.Exceptions.Add(kayitVar);
                                continue;
                            }
                            int kiraciId = 0;
                            bool buIsimdeBirdenCokKiraciVarMi = BuIsimdeBirdenCokKiraciVarMi(adi);
                            if (buIsimdeBirdenCokKiraciVarMi)
                            {
                                kiraEkstreAktarma.Uyari = true;
                            }
                            else
                            {
                                // SB DosyaNo ile konu çözülemiyor! o yüzden kapandı
                                //bool birdenCokSozlesmesiVarMi = BirdenCokSozlesmesiVarMi(adi);
                                //if (birdenCokSozlesmesiVarMi)
                                //{
                                //    kiraEkstreAktarma.Uyari = true;
                                //}
                                //else
                                {
                                    kiraciId= KiraciOtomatikEslestir(adi);
                                }
                            }

                            kiraEkstreAktarma.KiraciId = kiraciId;

                            kiraEkstreAktarma.Adi = adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            kiraEkstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel1.ReturnEmptyIfNull().ToString());
                            kiraEkstreAktarma.Tutar = tutar.ConvertToDecimal();
                            kiraEkstreAktarma.OdemeTarihi = hareketTar.ConvertToDatetime();
                            kiraEkstreAktarma.Aciklama = aciklama;
                            kiraEkstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(culturInfo); ;
                            kiraEkstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF2;
                            kiraEkstreAktarma.IslemTarihi = processTime;
                            kiraEkstreAktarma.IslemNo = islemno;
                            kiraEkstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                            kiraEkstreAktarma.Olusturan = currentUser;

                            kiraEkstreAktarma.Save();
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF, ""), ex);
                            exceptionHelper.Exceptions.Add(exception);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHelper.Exceptions.Add(ex);
            }
            return exceptionHelper;
        }
        private int KiraciOtomatikEslestir(string adi)
        {
            int kiraciId = 0;
            if (!string.IsNullOrEmpty(adi))
            {
                //kiraEkstreAktarma Listesinde kiraciId>0 olan kayıtları ada göre sorgulasın 
                KiraEkstreAktarma keaDao = new KiraEkstreAktarma();
                List<KiraEkstreAktarma> keaList = keaDao.SelectKiraciIdByAdi(adi);
                foreach (var item in keaList)
                {
                    if (item.KiraciId > 0)
                    {
                        kiraciId = item.KiraciId;
                        break;
                    }
                }
                //kiraci tablosundaki kayıtları isme göre sorgulasın
                if (kiraciId < 1)
                {
                    Kiraci kiraciDao = new Kiraci();
                    List<Kiraci> kiraciList = kiraciDao.SelectByAdi(adi);
                        foreach (var item in kiraciList)
                        {
                            if (item.Id > 0)
                            {
                                kiraciId = item.Id;
                                break;
                            }
                        }         
                }
            }
            return kiraciId;
        }
        private bool BuIsimdeBirdenCokKiraciVarMi(string adi)
        {
            bool varMi = false;
            if (!string.IsNullOrEmpty(adi))
            {
                //kiraci tablosundaki kayıtları isme göre sorgulasın
                Kiraci kiraciDao = new Kiraci();
                List<Kiraci> kiraciList = kiraciDao.SelectByAdi(adi);
                if (kiraciList.Count > 1)
                {
                    varMi = true;
                }
            }
            return varMi;
        }
        private bool BirdenCokSozlesmesiVarMi(string adi)
        {
            bool varMi = false;
            if (!string.IsNullOrEmpty(adi))
            {
                //kiraSozlesme tablosundaki kayıtları isme göre sorgulasın
                KiraSozlesme kirasozlesmeDao = new KiraSozlesme();
                List<KiraSozlesme> sozlesmeList = kirasozlesmeDao.SelectByKiraciAdi(adi);
                if (sozlesmeList.Count > 1)
                {
                    varMi = true;
                }
            }
            return varMi;
        }
        protected void NextBtn_Click(object sender, EventArgs e)
        {
            RedirectWithTarih(ProjeConstants.PAGE_KIRAEKSTRE_LIST);
        }
        private void RedirectWithTarih(string pageUrl)
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
    }
}
