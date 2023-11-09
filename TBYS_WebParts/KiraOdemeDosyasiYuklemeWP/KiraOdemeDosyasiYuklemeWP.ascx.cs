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
        private CultureInfo cultureInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
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
            string aciklamaStr = string.Empty;
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
                        var detay = row[16].ReturnEmptyIfNull().ToString().Replace("'","");
                        aciklamaStr = detay;
                        if (string.IsNullOrEmpty(hesapNo))// ilk kolon boş ise dosya bitti çık
                        {
                            break;
                        }
                        else if (
                           detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafından TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafına gelen")//ziraat bank
                           || detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME V tarafından TÜRK SİLAHLI KUVVETLERİNİ GÜÇLENDİRME VAKFI tarafına gelen") //işbank
                           || detay.Contains("TÜRK SİLAHLI KUVVETLERİNİ GÜÇL tarafından TÜRK SILAHLI KUVVETLERINI GÜÇLENDIRME VAKFI tarafına gelen ")//garanti bank
                           || detay.Contains(" virman ")
                           || detay.Contains(" VİRMAN ")
                           || hesapNo.Equals("HESAP NO")
                           || tutar < 0) //Bu borç demek değil mi?
                        {
                            continue;
                        }
                        
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

                                var splitTextM = new string[] { "nolu ", " hesabından " };
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
                            adi = adi.ToUpper().Replace("KUVEYT TÜRK KATILIM BANKASI A.Ş.", "")
                                   .Replace("QNB FİNANSBANK A.Ş.", "")
                                   .Replace("YAPI VE KREDİ BANKASI A.Ş.", "")
                                   .Replace("TÜRKİYE GARANTİ BANKASI A.Ş.", "")
                                   .Replace("AKBANK T.A.Ş.", "")
                                   .Replace("TÜRKİYE GARANTİ BANKASI A.Ş.", "")
                                   .Replace("TÜRKİYE İŞ BANKASI A.Ş.", "")
                                   .Replace("TÜRKİYE CUMHURİYETİ ZİRAAT BANKASI A.Ş.", "")
                                   .Replace("TÜRKİYE HALK BANKASI A.Ş.", "")
                                   .Replace("TÜRK EKONOMİ BANKASI A.Ş.", "").Trim();

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
                            if (true)// Borç Alacak Ayrımı için
                            {
                                kiraEkstreAktarma.Aciklama = aciklama;
                                kiraEkstreAktarma.OdemeSebebiId = OdemeSebebiBelirle(kiraEkstreAktarma,islem);
                                if (kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KIRA_INT //Diger, Kira veya Teminat ise
                                    || kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT
                                    || kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT 
                                    || kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_KIRA_TEMINAT_INT
                                    || kiraEkstreAktarma.OdemeSebebiId == ProjeConstants.ODEMESEBEBI_DIGER_INT

                                   )
                                {

                                    bool buIsimdeBirdenCokKiraciVarMi = BuIsimdeBirdenCokKiraciVarMi(adi, ref kiraciId);
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
                                            kiraciId = KiraciOtomatikEslestir(adi);
                                        }
                                    }

                                    kiraEkstreAktarma.KiraciId = kiraciId;

                                    kiraEkstreAktarma.Adi = adi.ReturnEmptyIfNull().ToString().Trim().ToUpper(cultureInfo); ;
                                    kiraEkstreAktarma.Telefon1 = UtilityHelper.TelefonFormatla(tel1.ReturnEmptyIfNull().ToString());
                                    kiraEkstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    kiraEkstreAktarma.OdemeTarihi = hareketTar.ConvertToDatetime();
                                   
                                    kiraEkstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(cultureInfo); ;
                                    kiraEkstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF2;
                                    kiraEkstreAktarma.IslemTarihi = processTime;
                                    kiraEkstreAktarma.IslemNo = islemno;
                                    kiraEkstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    kiraEkstreAktarma.Olusturan = currentUser;
                                    
                                    kiraEkstreAktarma.Save();  
                                }else if (true)//Kira veya Teminat değilse 
                                {

                                    kiraEkstreAktarma.Adi = aciklama ;
                                    kiraEkstreAktarma.Tutar = tutar.ConvertToDecimal();
                                    kiraEkstreAktarma.OdemeTarihi = hareketTar.ConvertToDatetime();
                                    
                                    kiraEkstreAktarma.Adres = adres.ReturnEmptyIfNull().ToString().Trim().ToUpper(cultureInfo); ;
                                    kiraEkstreAktarma.BankaAdi = ProjeConstants.BANKA_VAKIF2;
                                    kiraEkstreAktarma.IslemTarihi = processTime;
                                    kiraEkstreAktarma.IslemNo = islemno;
                                    kiraEkstreAktarma.DovizCinsi = ProjeConstants.DOVIZ_TL;
                                    kiraEkstreAktarma.Olusturan = currentUser;
                                    kiraEkstreAktarma.Save();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.BANKA_VAKIF, aciklamaStr), ex);
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

        private int OdemeSebebiBelirle(KiraEkstreAktarma kiraEkstreAktarma, string islem)
        {
            islem = string.IsNullOrEmpty(islem) ? string.Empty : islem;


            int odemeSebebiId = ProjeConstants.ODEMESEBEBI_DIGER_INT;

            if ((islem.Contains(ProjeConstants.ODEMESEBEBI_BATCH_YATAN)) ||
                   islem.ToUpper(cultureInfo).Contains("BATCH YATAN") ||
                   islem.ToLower(cultureInfo).Contains("Batch Yatan"))
            {
                odemeSebebiId = ProjeConstants.ODEMESEBEBI_BATCH_YATAN_INT;
            }
            else if ((islem.Contains(ProjeConstants.ODEMESEBEBI_BATCH_KOMISYONU)) ||
               islem.ToUpper(cultureInfo).Contains("BATCH KOMİSYONU") ||
               islem.ToLower(cultureInfo).Contains("Batch Komisyonu"))
            {
                odemeSebebiId = ProjeConstants.ODEMESEBEBI_BATCH_KOMISYONU_INT;
            }
            else if ((islem.Contains(ProjeConstants.ODEMESEBEBI_OTOMATIK_SUPURME)) ||
               islem.ToUpper(cultureInfo).Contains("OTOMATİK SÜPÜRME İŞLEMLERİ VİRMAN") ||
               islem.ToLower(cultureInfo).Contains("Otomatik Süpürme İşlemleri Virman"))
            {
                odemeSebebiId = ProjeConstants.ODEMESEBEBI_OTOMATIK_SUPURME_INT;
            }
            else if ((islem.Contains(ProjeConstants.ODEMESEBEBI_VALOR_CEK_YATIR)) ||
               islem.ToUpper(cultureInfo).Contains("VALOR İŞLEMİ İÇİN PARA ÇEK VE YATIR") ||
               islem.ToLower(cultureInfo).Contains("Valor İşlemi İçin Para Çek ve Yatır"))
            {
                odemeSebebiId = ProjeConstants.ODEMESEBEBI_VALOR_CEK_YATIR_INT;
            }
            else if ((islem.Contains(ProjeConstants.ODEMESEBEBI_YATIRIM_FONU_SATIS)) ||
               islem.ToUpper(cultureInfo).Contains("YATIRIM FONU SATIŞ") ||
               islem.ToLower(cultureInfo).Contains("Yatırım Fonu Satış"))
            {
                odemeSebebiId = ProjeConstants.ODEMESEBEBI_YATIRIM_FONU_SATIS_INT;
            }
            //yoksa açıklamaya bak
            else if (kiraEkstreAktarma != null && kiraEkstreAktarma.Aciklama != null)
            {
                if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_GECICITEMINAT)) ||
                        kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("GEÇİCİ TEMİNAT") ||
                        kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("geçici teminat"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_GECICITEMINAT_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_KESINTEMINAT) ||
                    (kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.TEMINAT_ODEMESI)) ||
                    kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("TEMİNAT")) ||
                    kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("teminat"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_KESINTEMINAT_INT;
                }
                else if (kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_KIRA))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_KIRA_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("KİRA BEDELİ")) ||
                    (kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("kira bedeli")) ||
                    (kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("kira")) ||
                    (kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("KİRA")))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_KIRA_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_AIDAT)) ||
                    kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("AİDAT") ||
                    (kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("aidat")))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_AIDAT_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_SIGORTA)) ||
                        (kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("SİGORTA")) ||
                    (kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("sigorta")))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_SIGORTA_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_AVUKATLIKUCRETI)) ||
                    kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("AVUKATLIK") ||
                    kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("avukatlık"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_AVUKATLIKUCRETI_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_YARGILAMAUCRETI)) ||
                    kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("YARGILAMA") ||
                    kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("yargılama"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_YARGILAMAUCRETI_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_AVANSIADESI)) ||
                        (kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("AVANS")) ||
                    kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("avans"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_AVANSIADESI_INT;
                }
                else if ((kiraEkstreAktarma.Aciklama.Contains(ProjeConstants.ODEMESEBEBI_SATIS)) ||
                        (kiraEkstreAktarma.Aciklama.ToUpper(cultureInfo).Contains("SATIŞ")) ||
                    kiraEkstreAktarma.Aciklama.ToLower(cultureInfo).Contains("satış"))
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_SATIS_INT;
                }
            }
            else
            {
                if (kiraEkstreAktarma.KiraciId.ConvertToInt() > 0)
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_KIRA_INT;
                }
                else
                {
                    odemeSebebiId = ProjeConstants.ODEMESEBEBI_DIGER_INT;
                }
            }
            return odemeSebebiId;
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
        private bool BuIsimdeBirdenCokKiraciVarMi(string adi, ref int kiraciId)
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
                    kiraciId = kiraciList[0].Id;
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
