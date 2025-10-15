using Microsoft.SharePoint.JSGrid;
using Model.NBYS;
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

namespace NBYS_WebParts.DuzenliBagisciDosyaYuklemeWP
{
    [ToolboxItemAttribute(false)]
    public partial class DuzenliBagisciDosyaYuklemeWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public DuzenliBagisciDosyaYuklemeWP()
        {
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        bool dosyaAktarildi = false;
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
                if (!Page.IsPostBack)
                {
                    YonergeLnk.HRef = UtilityHelper.YonergeURLGetir(ProjeConstants.PARAM_NBYSYONERGE, ProjeConstants.NBYSBELGELERI_LIB, ProjeConstants.PAGE_DUZENLIBAGISCI_LIST);
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
                Page.Response.Redirect(newUrl,false);
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
                DosyayiKaydet();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        protected void DuzenliBagisciListesiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                RedirectToPage(ProjeConstants.PAGE_DUZENLIBAGISCI_LIST);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private void DosyayiKaydet()
        {
            if (!dosyaAktarildi && DosyaFU.HasFile)
            {
                var exceptionHelper = Kaydet(DosyaFU.FileContent, DateTime.Now, CurrentUserName);
                if (exceptionHelper.Exceptions.Count > 0)
                {
                    DosyaFU.Enabled = false;
                    KaydetBtn.Enabled = false;
                    exceptionHelper.PublishException();
                }
                else
                {
                    DosyaFU.Enabled = false;
                    KaydetBtn.Enabled = false;
                    MessageHelper.PublishMessage("Dosya başarıyla yüklendi.", ProjeConstants.MESAJ_BASARILI,2000);
                }

            }
        }
        public ExceptionHelper Kaydet(Stream fileStream, DateTime processTime, string currentUser)
        {
            ExceptionHelper exceptionHelper = new ExceptionHelper();
            
            try
            {

                var data = ExcelHelper.ReadXLSXAsDataTable(fileStream, ProjeConstants.DUZENLIBAGISCI_ILKKACSATIRHARIC, ProjeConstants.DUZENLIBAGISCI_SONKACSATIRHARIC, true);
                if (data != null)
                {

                    foreach (DataRow row in data.Rows)
                    {
                        string aciklamaStr = string.Empty;
                        var tutar = row[6].ReturnZeroIfNull().ToString().Replace("₺","").ConvertToDecimal();
                        if (tutar < 1)
                        {
                            continue; //tutar 1 den azsa atla
                        }
                        var aciklama = row[0].ReturnEmptyIfNull().ToString().Replace("'", "");
                        var adi = row[1].ReturnEmptyIfNull().ToString();
                        var eposta = row[2].ReturnEmptyIfNull().ToString();
                        var telefon = row[3].ReturnEmptyIfNull().ToString();
                        var TCKimlikNo = row[4].ReturnEmptyIfNull().ConvertToLong();
                        var adres = row[5].ReturnEmptyIfNull().ConvertToLong();
                        var bagisadedi = row[7].ReturnZeroIfNull().ConvertToInt();
                        var toplambagis = row[8].ReturnZeroIfNull().ToString().Replace("₺", "").ConvertToDecimal();
                        var baslamaTarihi = row[9].ReturnEmptyIfNull().ConvertToDatetime();

                        try
                        {
                            int bagisciId = 0;
                            string eslesmeBilgisi = string.Empty;
                            NakitBagisci bagisci = new NakitBagisci();
                            bagisci = bagisci.SelectByTcKimlikno(TCKimlikNo);
                            if (bagisci==null)
                            {
                                bagisci = new NakitBagisci();
                                bagisci = bagisci.SelectByAdAndTelefon(adi.Trim(), telefon);

                                if (bagisci == null)
                                {
                                    bagisci = new NakitBagisci();
                                    bagisci = bagisci.SelectByTelefon(telefon);
                                    if (bagisci == null)
                                    {
                                        bagisci = new NakitBagisci();
                                        bagisci = bagisci.SelectByEposta(eposta);
                                        if (bagisci == null)
                                        {


                                            //Bağışçı bulunamadı
                                            //Bu durumda yeni bağışçı oluşturmak doğru değil, çünkü 12 aydır bağış yaptığına göre mutlaka bir kaydı vardır.
                                            bagisciId = -1;
                                            eslesmeBilgisi = " # Bağışçı Bulunamadı ";
                                        }
                                        else
                                        {
                                            bagisciId = bagisci.Id;
                                            eslesmeBilgisi = " # Epostadan Bulundu";
                                        }


                                    }
                                    else
                                    {
                                        bagisciId = bagisci.Id;
                                        eslesmeBilgisi = " # Telefondan Bulundu";
                                    }

                                }
                                else
                                {
                                    bagisciId = bagisci.Id;
                                    eslesmeBilgisi = " # Ad ve Telefondan Bulundu";
                                }
                            }
                            else
                            {
                                bagisciId = bagisci.Id;
                                eslesmeBilgisi = " # TCKN ile bulundu";
                            }

                            DuzenliNakitBagisci duzenliNakitBagisci = new DuzenliNakitBagisci();
                            duzenliNakitBagisci = duzenliNakitBagisci.SelectByBagisciId(bagisciId);
                            bool saved = false;
                            bool updated = false;   

                            //Aktif düzenli bağışçı kaydı yoksa
                            if (duzenliNakitBagisci == null)
                            {
                                duzenliNakitBagisci = new DuzenliNakitBagisci();
                                duzenliNakitBagisci.BagisciId = bagisciId;
                                duzenliNakitBagisci.TCKimlikNo = TCKimlikNo;
                                duzenliNakitBagisci.BagisciAdi = adi;
                                duzenliNakitBagisci.Tutar = tutar;
                                duzenliNakitBagisci.BaslamaTarihi = baslamaTarihi;
                                duzenliNakitBagisci.Aktif = true;
                                duzenliNakitBagisci.Telefon = telefon;
                                duzenliNakitBagisci.EPosta = eposta;
                                duzenliNakitBagisci.EslesmeBilgisi = eslesmeBilgisi;
                                duzenliNakitBagisci.Aciklama = aciklama;
                                duzenliNakitBagisci.BagisAdedi = bagisadedi;
                                duzenliNakitBagisci.BagisToplami = toplambagis;
                                int id = duzenliNakitBagisci.Save();
                                if (id < 1)
                                {
                                    aciklamaStr= string.IsNullOrEmpty(aciklama) ? " Yeni Düzenli Bağışçı yaratılamadı -> " + adi : aciklama;
                                    Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} -> Düzenli bağışçı kaydı oluşturulamadı.", ProjeConstants.PAGE_DUZENLIBAGISCI_YUKLEME, aciklamaStr));
                                    exceptionHelper.Exceptions.Add(exception);
                                }
                            }
                            else
                            {
                                duzenliNakitBagisci.BagisciId = bagisciId;
                                duzenliNakitBagisci.TCKimlikNo = TCKimlikNo;
                                duzenliNakitBagisci.BagisciAdi = adi;
                                duzenliNakitBagisci.Tutar = tutar;
                                duzenliNakitBagisci.BaslamaTarihi = baslamaTarihi;
                                duzenliNakitBagisci.Aktif = true;
                                duzenliNakitBagisci.Telefon = telefon;
                                duzenliNakitBagisci.EPosta = eposta;
                                duzenliNakitBagisci.EslesmeBilgisi = eslesmeBilgisi;
                                duzenliNakitBagisci.Aciklama = aciklama;
                                duzenliNakitBagisci.BagisAdedi = bagisadedi;
                                duzenliNakitBagisci.BagisToplami = toplambagis;
                                updated=duzenliNakitBagisci.Update();

                            }

                        }
                        catch (Exception ex)
                        {
                            aciklamaStr = string.IsNullOrEmpty(aciklama) ? adi : aciklama;
                            Exception exception = new Exception(string.Format("HATA SATIRI {0}:{1} ->", ProjeConstants.PAGE_DUZENLIBAGISCI_YUKLEME, aciklamaStr), ex);
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
        
        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                int index = currentUrl.LastIndexOf("?") < 0 ? currentUrl.Length : currentUrl.LastIndexOf("?");

                string rawUrl = currentUrl.Substring(0, index);
                string newUrl = rawUrl.Substring(0, rawUrl.LastIndexOf("/")) + "/" + pageUrl;
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
