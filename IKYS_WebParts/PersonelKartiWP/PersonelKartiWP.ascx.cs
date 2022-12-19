using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.PersonelKartiWP
{
    [ToolboxItemAttribute(false)]
    public partial class PersonelKartiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public PersonelKartiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string PersonelIdQS
        {
            get
            {

                if (ViewState["PersonelId"] == null)
                {
                    if (Page.Request.QueryString["PersonelId"] != null)
                    {
                        ViewState["PersonelId"] = Page.Request.QueryString["PersonelId"];
                    }
                    else
                    {
                        ViewState["PersonelId"] = string.Empty;
                    }
                }
                return ViewState["PersonelId"].ToString();
            }

            set
            {
                ViewState["PersonelId"] = value;
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
            BilgiFormunuDoldur();
        }
        private Personel PersonelGetir()
        {
            Personel personel = new Personel();

            if (!string.IsNullOrEmpty(PersonelIdQS))
            {
                personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            }
            else
            {
                string userName = CurrentUserName.Substring(CurrentUserName.LastIndexOf("\\") + 1, CurrentUserName.Length - CurrentUserName.LastIndexOf("\\") - 1);
                personel = personel.SelectByUserName(userName);
            }
            PersonelIdQS = personel == null ? "0" : personel.Id.ToString();
            return personel;
        }
        private void BilgiFormunuDoldur()
        {
            #region personel
            Personel personel = PersonelGetir();

            if (personel != null)
            {
                AdiCell.Text = personel.Adi;
                SoyadiCell.Text = personel.Soyadi;
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string hostUrl = currentUrl.Substring(0, currentUrl.LastIndexOf(System.Web.HttpContext.Current.Request.Url.AbsolutePath));

                string fotostr = string.IsNullOrEmpty(personel.KullaniciAdi) ? personel.Adi.ReturnEmptyIfNull().ToString().Substring(0, 1) + personel.Soyadi.ReturnEmptyIfNull().ToString() : personel.KullaniciAdi.ReturnEmptyIfNull().ToString();
                string imgUrl = hostUrl + ProjeConstants.PATH_RESIMLER_PERSONEL + fotostr.ReplaceTrChars() + ".jpg";

                DisplayImage.ImageUrl = imgUrl;
            }
            #endregion

            #region kimlik bilgileri
            Kimlik kimlik = new Kimlik();
            kimlik = kimlik.SelectByPersonelId(personel.Id);
            if (kimlik != null)
            {
                TCKimlikNOCell.Text = kimlik.TCKimlikNo;
                Ilce dilce = new Ilce(kimlik.DogumYeri.ConvertToInt());
                DogumYeriCell.Text = dilce.IlceAdi + "/" + dilce.IlAdi;

                DogumTarihiCell.Text = kimlik.DogumTar.ConvertToDatetimeEmptyIfNull();
                MedeniHaliCell.Text = kimlik.MedeniHali.Equals("1") ? "Evli" : "Bekar"; ;
                EvlilikTarihiCell.Text = kimlik.EvlilikTar.ConvertToDatetimeEmptyIfNull();
            }

            #endregion

            #region İş bilgileri
            IsBilgileri isBilgisi = new IsBilgileri();
            isBilgisi = isBilgisi.SelectByPersonelId(personel.Id);
            if (isBilgisi != null)
            {
                GorevTanim gt = new GorevTanim();
                gt = gt.Select<GorevTanim>(isBilgisi.GorevId);
                UnvaniCell.Text = gt.Adi;
                IseGirisTarihiCell.Text = isBilgisi.BaslamaTar.ConvertToDatetimeEmptyIfNull();
            }
            #endregion

            #region Aile bilgileri
            Aile aileDao = new Aile();
            List<Aile> aileList = aileDao.SelectByPersonelId(personel.Id);
            int sayac = 0;
            foreach (Aile item in aileList)
            {
                string yakinlikDerecesi = item.YakinlikDerecesi.ToString();
                if (yakinlikDerecesi.Equals("1"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_ES;
                else if (yakinlikDerecesi.Equals("2"))
                    yakinlikDerecesi = ProjeConstants.PER_YAKINLIKDERECESI_COCUK;
                Meslek meslek = new Meslek();
                meslek = meslek.Select<Meslek>(item.Meslek);
                AddRow(sayac++, yakinlikDerecesi, item.Adi, item.Soyadi, item.DogumTar.ConvertToDatetimeEmptyIfNull(), meslek == null ? "" : meslek.Adi);
                
            }
            if (aileList.Count < 1)
            {
                AddRow(sayac++, "-", "-", "-", "-", "-");
            }
            #endregion

            #region İletişim Bilgileri
            IletisimBilgileri ib = new IletisimBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            Ilce ilce = new Ilce(ib.Ilcesi.ConvertToInt());
            AdresCell.Text = ib.Adres + " " + ilce.IlceAdi + "/" + ilce.IlAdi;
            CepTelCell.Text = ib.CepTelefonu + " " + ib.CepTelefonu2;
            EvTelCell.Text = ib.EvTelefonu;
            #endregion

            #region Eğitim Bilgileri
            Egitim egitim = new Egitim();
            List<Egitim> egitimList = egitim.SelectByPersonelId(personel.Id);
            string liseOkul = string.Empty;
            string liseMezuniyet = string.Empty;
            string lisansOkul = string.Empty;
            string lisansMezuniyet = string.Empty;
            string onLisansMezuniyet = string.Empty;
            string onLisansOkul = string.Empty;
            string ylisansOkul = string.Empty;
            string ylisansMezuniyet = string.Empty;
            string doktoraOkul = string.Empty;
            string doktoraMezuniyet = string.Empty;
            foreach (Egitim item in egitimList)
            {
                switch (item.Seviye)
                {
                    case "4"://lise
                        {
                            liseOkul = item.Okul;
                            liseMezuniyet = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }

                    case "5"://önlisans
                        {
                            onLisansOkul = item.Okul;
                            onLisansMezuniyet = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }

                    case "6":
                        {
                            lisansOkul = item.Okul;
                            lisansMezuniyet = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }
                    case "7":
                        {
                            ylisansOkul = item.Okul;
                            ylisansMezuniyet = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }
                    case "8":
                        {
                            doktoraOkul = item.Okul;
                            doktoraMezuniyet = item.MezuniyetTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }
                }
                LiseOkulCell.Text = liseOkul;
                LiseMezuniyetTarCell.Text = liseMezuniyet;
                OnLisansOkulCell.Text = onLisansOkul;
                OnLisansMezuniyetTarCell.Text = onLisansMezuniyet;
                LisansOkulCell.Text = lisansOkul;
                LisansMezuniyetTarCell.Text = lisansMezuniyet;
                YLisansOkulCell.Text = ylisansOkul;
                YLisansMezuniyetTarCell.Text = ylisansMezuniyet;
                DoktoraOkulCell.Text = doktoraOkul;
                DoktoraMezuniyetTarCell.Text = doktoraMezuniyet;
            }
            #endregion

            #region İş Deneyimi
            IsTecrube isTecrubeDao = new IsTecrube();
            List<IsTecrube> isTecrubeList = isTecrubeDao.SelectByPersonelId(personel.Id);
            sayac = 0;
            foreach (IsTecrube item in isTecrubeList)
            {
                switch (sayac++)
                {
                    case 0://iş 1
                        {
                            IsyeriCell.Text = item.Isyeri;
                            PozisyonCell.Text = item.Gorevi;
                            DonemCell.Text = item.BasTar.ConvertToDatetimeEmptyIfNull() + " - " + item.BitTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }

                    case 1://iş 2
                        {
                            Isyeri1Cell.Text = item.Isyeri;
                            Pozisyon1Cell.Text = item.Gorevi;
                            Donem1Cell.Text = item.BasTar.ConvertToDatetimeEmptyIfNull() + " - " + item.BitTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }
                    case 2://iş 3
                        {
                            Isyeri2Cell.Text = item.Isyeri;
                            Pozisyon2Cell.Text = item.Gorevi;
                            Donem2Cell.Text = item.BasTar.ConvertToDatetimeEmptyIfNull() + " - " + item.BitTar.ConvertToDatetimeEmptyIfNull();
                            break;
                        }

                }

            }
            #endregion

            #region Yabancı Dil
            YabanciDil yabanciDilDao = new YabanciDil();
            List<YabanciDil> yabanciDilList = yabanciDilDao.SelectByPersonelId(personel.Id);
            sayac = 0;
            foreach (YabanciDil item in yabanciDilList)
            {
                switch (sayac++)
                {
                    case 0://iş 1
                        {
                            YabanciDilCell.Text = item.Dil;
                            YabanciDilNotuCell.Text = item.SinavAdi + " Notu:" + item.SinavNotu + " (" + item.SinavTarihi.ConvertToDatetimeEmptyIfNull() + ")";
                            break;
                        }

                    case 1://iş 2
                        {
                            YabanciDil1Cell.Text = item.Dil;
                            YabanciDilNotu1Cell.Text = item.SinavAdi + " Notu:" + item.SinavNotu + " (" + item.SinavTarihi.ConvertToDatetimeEmptyIfNull() + ")";
                            break;
                        }
                    case 2://iş 3
                        {
                            YabanciDil2Cell.Text = item.Dil;
                            YabanciDilNotu2Cell.Text = item.SinavAdi + " Notu:" + item.SinavNotu + " (" + item.SinavTarihi.ConvertToDatetimeEmptyIfNull() + ")";
                            break;
                        }

                }

            }
            #endregion
        }

        private void AddRow(int sira,string cell0, string cell1, string cell2, string cell3, string cell4)
        {
            TableRow row = new TableRow();
            TableCell col0 = new TableCell();
            TableCell col1 = new TableCell();
            TableCell col2 = new TableCell();
            TableCell col3 = new TableCell();
            TableCell col4 = new TableCell();
            col0.BackColor = ColorTranslator.FromHtml("#D7F2FF");
            col0.BorderStyle = BorderStyle.Solid;
            col0.BorderColor = Color.Black;
            col0.Attributes["style"] = "vertical-align: middle;";
            col0.Text = cell0;

            col1.BorderStyle = BorderStyle.Solid;
            col1.BorderColor = Color.Black;
            col1.Attributes["style"] = "vertical-align: middle;";
            col1.Text = cell1;

            col2.BorderStyle = BorderStyle.Solid;
            col2.BorderColor = Color.Black;
            col2.Attributes["style"] = "vertical-align: middle;";
            col2.Text = cell2;

            col3.BorderStyle = BorderStyle.Solid;
            col3.BorderColor = Color.Black;
            col3.Attributes["style"] = "vertical-align: middle;";
            col3.Text = cell3;

            col4.BorderStyle = BorderStyle.Solid;
            col4.BorderColor = Color.Black;
            col4.Attributes["style"] = "vertical-align: middle;";
            col4.Text = cell4;

            row.Controls.Add(col0);
            row.Controls.Add(col1);
            row.Controls.Add(col2);
            row.Controls.Add(col3);
            row.Controls.Add(col4);
            PersonelTable.Rows.AddAt(sira+13,row);
        }

        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_HOME);
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
        protected void PersonelListesiBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_PERSONEL_LIST + "?SecilenId=" + PersonelIdQS);
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }
        protected void ExportToExcel()
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelIdQS.ConvertToInt());

            string filename = "PersonelKarti" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xlsx";
            if (personel != null)
            {
                filename = personel.Adi.ReplaceTrChars() + personel.Soyadi.ReplaceTrChars() + "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            }
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            PersonelTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }

    }
}
