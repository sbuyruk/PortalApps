using Model.IKYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.MazereteMahsupWP
{
    [ToolboxItemAttribute(false)]
    public partial class MazereteMahsupWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public MazereteMahsupWP()
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
        private string DestinationAppQS
        {
            get
            {

                if (ViewState["DestinationApp"] == null)
                {
                    if (Page.Request.QueryString["DestinationApp"] != null)
                    {
                        ViewState["DestinationApp"] = Page.Request.QueryString["DestinationApp"];
                    }
                    else
                    {
                        ViewState["DestinationApp"] = string.Empty;
                    }
                }
                return ViewState["DestinationApp"].ToString();
            }

            set
            {
                ViewState["DestinationApp"] = value;
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
        private string MahsupMesajiQS
        {
            get
            {

                if (ViewState["MahsupMesaji"] == null)
                {
                    if (Page.Request.QueryString["MahsupMesaji"] != null)
                    {
                        ViewState["MahsupMesaji"] = Page.Request.QueryString["MahsupMesaji"];
                    }
                    else
                    {
                        ViewState["MahsupMesaji"] = string.Empty;
                    }
                }
                return ViewState["MahsupMesaji"].ToString();
            }

            set
            {
                ViewState["MahsupMesaji"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(MahsupMesajiQS))
                    {
                        MessageHelper.PublishMessage("Mahsup Tamamlandi", ProjeConstants.MESAJ_BASARILI, 2000);
                        MahsupMesajiQS = string.Empty;
                    }
                }

                FillIzinTable(false);

            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        private void RedirectToPage(string pageUrl)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + pageUrl + "?MahsupMesaji=true";
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private bool BirGunMahsupEt()
        {
            bool isAllSaved = false;
            bool isIzinHareketSaved = false;
            bool isUcretliIzinDonemiGuncellendi = false;
            bool isMazeretIzinDonemiGuncellendi = false;
            Mahsup mahsupRB = null;
            int personelId = paramPersonelIdLbl.Value.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                IzinHareket izinHareket = new IzinHareket();

                DateTime today = DateTime.Today;
                IzinDonem ucretliIzinDonemi = new IzinDonem();
                IzinDonem mazeretIzinDonemi = new IzinDonem();

                IzinDonem ucretliIzinDonemiRB = new IzinDonem();
                IzinDonem mazeretIzinDonemiRB = new IzinDonem();

                ucretliIzinDonemi = ucretliIzinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                mazeretIzinDonemi = mazeretIzinDonemi.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT, today);

                if (mazeretIzinDonemi == null)
                {
                    MessageHelper.PublishMessage("Mahsup islemi yapilamaz, Izin dönemi bulunamadi!", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    //ücretli izin hareketi yarat 
                    //ücretli izin dönemini güncelle
                    //mazeret iznine 9 saat ekle
                    //her üçü de ok ise islem tamamlandi
                    //eger herhangi biri tamamlanmadi ize rollback yap

                    if (ucretliIzinDonemi == null)
                    {
                        ucretliIzinDonemi = ucretliIzinDonemi.IzinDonemiOlustur(personel, ProjeConstants.IZINTIPI_UCRETLI_INT, today, CurrentUserName);
                    }
                    if (ucretliIzinDonemi != null)
                    {

                        ucretliIzinDonemiRB = ucretliIzinDonemi;
                        DateTime izinBastar = ucretliIzinDonemi.BaslangicTarihi;
                        ResmiTatil resmiTatil = new ResmiTatil();
                        bool resmiTatilMi = false;
                        while (resmiTatilMi)
                        {
                            izinBastar = izinBastar.AddDays(1);
                            resmiTatilMi = resmiTatil.ResmiTatilMi(izinBastar);
                        }
                        izinHareket.BaslangicTarihi = izinBastar;
                        izinHareket.BitisTarihi = izinBastar;
                        izinHareket.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                        izinHareket.Adres = "#Mazeret Iznine mahsup edilmistir. Mazeret Izin DonemId=" + mazeretIzinDonemi.Id;
                        izinHareket.Aciklama = AciklamaTxt.Text;
                        izinHareket.Sure = "1";
                        izinHareket.PersonelId = personel.Id;
                        izinHareket.IzinDonemId = ucretliIzinDonemi.Id;
                        izinHareket.IzinTalepId = 0;
                        izinHareket.IzinTipi = ProjeConstants.IZINTIPI_UCRETLI_INT;
                        izinHareket.Mahsup = true;
                        izinHareket.Olusturan = CurrentUserName;
                        int izinHareketId = izinHareket.Save();
                        isIzinHareketSaved = izinHareketId > 0 ? true : false;
                        if (isIzinHareketSaved)
                        {
                            string mahsupAciklama = "Ücretli izinden Mazerete mahsup edildi.";
                            Mahsup mahsup = MahsupTablosunaEkle(izinHareket, ProjeConstants.IZINTIPI_UCRETLI_INT, mazeretIzinDonemi.Id, ucretliIzinDonemi.Id, mahsupAciklama);
                            mahsupRB = mahsup;
                            int ucretliKullanilanIzinInt = ucretliIzinDonemi.KullanilanIzin.ConvertToInt() + 1;
                            ucretliIzinDonemi.KullanilanIzin = ucretliKullanilanIzinInt.ToString();
                            int ucretliKalanIzinInt = ucretliIzinDonemi.KalanIzin.ConvertToInt() - 1;
                            ucretliIzinDonemi.KalanIzin = ucretliKalanIzinInt.ToString();
                            ucretliIzinDonemi.Aciklama += System.Environment.NewLine + "#Mazerete Mahsup, IzinHareketId=" + izinHareket.Id + " MazeretIzinDonemi=" + mazeretIzinDonemi.Id;
                            ucretliIzinDonemi.Degistiren = CurrentUserName;
                            isUcretliIzinDonemiGuncellendi = ucretliIzinDonemi.Update();
                            if (isUcretliIzinDonemiGuncellendi)
                            {
                                TimeSpan birGunTs = ProjeConstants.BIRGUN_MAZERET_SAAT_TS;
                                TimeSpan mazeretIzinHakkiTS = mazeretIzinDonemi.IzinHakki.ConvertToTimeSpan();
                                TimeSpan mazeretKullanilanIzinTS = mazeretIzinDonemi.KullanilanIzin.ConvertToTimeSpan();
                                TimeSpan mazeretKalanIzinTS = mazeretIzinDonemi.KalanIzin.ConvertToTimeSpan();

                                mazeretIzinHakkiTS = mazeretIzinHakkiTS + birGunTs;
                                mazeretKalanIzinTS = mazeretIzinHakkiTS - mazeretKullanilanIzinTS;
                                mazeretIzinDonemi.Aciklama += System.Environment.NewLine + "#Mahsup, IzinHareketId=" + izinHareket.Id + " UcretliIzinDonemi=" + ucretliIzinDonemi.Id;
                                mazeretIzinDonemi.IzinHakki = mazeretIzinHakkiTS.ToString();
                                mazeretIzinDonemi.KalanIzin = mazeretKalanIzinTS.ToString();
                                mazeretIzinDonemi.Degistiren = CurrentUserName;
                                isMazeretIzinDonemiGuncellendi = mazeretIzinDonemi.Update();
                            }
                        }

                    }
                }

                isAllSaved = isIzinHareketSaved && isUcretliIzinDonemiGuncellendi && isMazeretIzinDonemiGuncellendi;
                if (!isAllSaved)
                {
                    bool isMahsupRB = (mahsupRB == null) ? true : mahsupRB.Delete();
                    bool isIzinHareketRB = izinHareket.Delete();
                    bool isUcretliIzinRB = ucretliIzinDonemiRB.Update();
                    bool isMazeretIzinRB = mazeretIzinDonemiRB.Update();
                    string message = "Ücretli izinden Mazeret Iznine Mahsup sirasinda sorunlarla karsilasildi. Geri alma isleminde: "
                        + " Izin Hareketi geri alma : " + (isIzinHareketRB ? "Basarili. " : "Basarisiz. ")
                        + " Ücretli Izin Dönemi geri alma : " + (isUcretliIzinRB ? "Basarili. " : " Basarisiz")
                        + (isUcretliIzinRB ? "Basarili. " : " Basarisiz.");
                    MessageHelper.PublishMessage(message, ProjeConstants.MESAJ_HATA);
                }
            }
            return isAllSaved;
        }

        private Mahsup MahsupTablosunaEkle(IzinHareket izinHareket, int asilIzinTipi, int kullanildigiDonem, int mahsupDonemi, string aciklama)
        {
            Mahsup mahsup = new Mahsup();
            mahsup.PersonelId = izinHareket.PersonelId;
            mahsup.IzinHareketId = izinHareket.Id;
            mahsup.IzinTipi = asilIzinTipi;
            mahsup.KullanildigiDonemId = kullanildigiDonem;
            mahsup.MahsupDonemId = mahsupDonemi;
            mahsup.Aciklama = aciklama;
            mahsup.Olusturan = CurrentUserName;
            int mahsupid = mahsup.Save();
            return mahsup;
        }

        private void FillIzinTable(bool isForExcel)
        {
            IzinTableHeaders(isForExcel);


            Personel personelDao = new Personel();
            DataTable dataTable = personelDao.SelectCalisanPersonelReturnDT();
            int birimIdTemp = 0;
            int sira = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {

                int personelId = dataRow["PersonelId"].ConvertToInt();
                Personel personel = new Personel();
                personel = personel.Select<Personel>(personelId);
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                string gorev = dataRow["Gorev"].ToString();
                string izinDonemiBasTar = dataRow["IzinDonemiBasTar"].ConvertToDatetimeEmptyIfNull();
                string birim = dataRow["Birim"].ToString();
                int birimId = dataRow["BirimId"].ConvertToInt();
                TableCell birimCell = new TableCell();
                if (birimIdTemp != birimId)
                {
                    birimIdTemp = birimId;
                    TableRow rowBirim = new TableRow();

                    birimCell.ColumnSpan = 9;
                    birimCell.Text = birim;
                    birimCell.Font.Bold = true;
                    birimCell.BackColor = System.Drawing.Color.DarkGray;
                    rowBirim.Controls.Add(birimCell);
                    IzinTable.Controls.Add(rowBirim);
                }


                TableRow row = new TableRow();

                TableCell siraCell = new TableCell();
                siraCell.Text = (++sira).ToString();
                row.Controls.Add(siraCell);

                TableCell adiSoyadiCell = new TableCell();
                adiSoyadiCell.Text = adiSoyadi;
                row.Controls.Add(adiSoyadiCell);
                //izin dönemleri yoksa olustur
                IzinDonem mazeretIzinDonemi = new IzinDonem();
                DateTime now = DateTime.Now;
                mazeretIzinDonemi = mazeretIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_MAZERET_INT, now);

                IzinDonem ucretliIzinDonemi = new IzinDonem();
                ucretliIzinDonemi = ucretliIzinDonemi.SelectByIzinTarihi(personelId, ProjeConstants.IZINTIPI_UCRETLI_INT, now);

                if (ucretliIzinDonemi == null)
                {
                    ucretliIzinDonemi = new IzinDonem();
                    ucretliIzinDonemi.IzinDonemiOlustur(personel, ProjeConstants.IZINTIPI_UCRETLI_INT, now, CurrentUserName);
                }
                if (mazeretIzinDonemi == null)
                {
                    mazeretIzinDonemi = new IzinDonem();
                    mazeretIzinDonemi.IzinDonemiOlustur(personel, ProjeConstants.IZINTIPI_MAZERET_INT, now, CurrentUserName);
                }

                string ucretliIzinDonemiStr = string.Empty;
                string izinHakki = string.Empty;
                string kullanilanIzin = string.Empty;
                string mazeretKalanIzin = string.Empty;
                int mazaretKalanIzinInt = 27;
                if (mazeretIzinDonemi != null)
                {
                    ucretliIzinDonemiStr = ucretliIzinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + ucretliIzinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                    if (mazeretIzinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        TimeSpan izinHakkiTS = new TimeSpan(0, 0, 0);
                        izinHakkiTS = mazeretIzinDonemi.IzinHakki.ConvertToTimeSpan();
                        izinHakki = mazeretIzinDonemi.IzinHakki.ConvertToTimeSpanReturnInHHmm();
                        kullanilanIzin = mazeretIzinDonemi.KullanilanIzin.ConvertToTimeSpanReturnInHHmm();
                        mazeretKalanIzin = mazeretIzinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                        TimeSpan kalanaIzinTs = mazeretIzinDonemi.KalanIzin.ConvertToTimeSpan();
                        mazaretKalanIzinInt = (int)kalanaIzinTs.TotalHours;
                    }

                }

                TableCell IzinHakkiCell = new TableCell();
                IzinHakkiCell.Text = izinHakki;
                row.Controls.Add(IzinHakkiCell);

                TableCell KullanilanIzinCell = new TableCell();

                KullanilanIzinCell.Text = kullanilanIzin;
                row.Controls.Add(KullanilanIzinCell);

                TableCell MazeretKalanIzinCell = new TableCell();
                MazeretKalanIzinCell.Text = mazeretKalanIzin;
                if (mazeretKalanIzin.Contains("-"))
                {
                    MazeretKalanIzinCell.ForeColor = System.Drawing.Color.Red;
                    //MazeretKalanIzinCell.Font.Bold = true;
                }
                row.Controls.Add(MazeretKalanIzinCell);

                TableCell UcretliIzinDonemiCell = new TableCell();
                int ucretliKalanIzin = ucretliIzinDonemi.KalanIzin.ConvertToInt();
                UcretliIzinDonemiCell.Text = ucretliIzinDonemiStr;

                TableCell UcretliKalanIzinCell = new TableCell();
                UcretliKalanIzinCell.Text = ucretliKalanIzin + " " + ucretliIzinDonemi.Birim;

                bool isMahsup = true;
                TableCell MahsupCell = new TableCell();
                TableCell DilekceCell = new TableCell();
               
                if (ucretliKalanIzin <= 0)
                {
                    //SB 07.10.2022 Yillik izni bitmis de olsa mazerete mahsup edilebilsin
                    
                    //UcretliKalanIzinCell.Font.Bold = true;
                    //isMahsup = false;
                    //MahsupCell.Text = "Mahsup edilemez";
                    MahsupCell.ForeColor = System.Drawing.Color.Red;
                    UcretliKalanIzinCell.ForeColor = System.Drawing.Color.Red;
                }
                row.Controls.Add(UcretliIzinDonemiCell);
                row.Controls.Add(UcretliKalanIzinCell);
                if (!isForExcel)
                {
                    if ((mazaretKalanIzinInt < 8) && (isMahsup))//kalan izin süresi 8 saatten azsa mahsup et çiksin
                    {

                        LinkButton MahsupBtn = new LinkButton();
                        MahsupBtn.Text = "Mahsup Islemi";
                        MahsupBtn.ID = "MahsupBtn" + sira;
                        TableUpdatePanel.ContentTemplateContainer.Controls.Add(MahsupBtn);
                        MahsupBtn.CssClass = "btn btn-outline-danger";
                        MahsupBtn.Click += delegate
                        {
                            try
                            {
                                var openPopup = "OpenModal(" + personelId + ");";
                                System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);

                            }
                            catch (Exception exception)
                            {
                                ExceptionHelper ex = new ExceptionHelper(exception);
                                ex.PublishException();
                            }
                        };
                        MahsupCell.Controls.Add(MahsupBtn);
                        //dilekçe butonu
                        LinkButton DilekceBtn = new LinkButton();
                        DilekceBtn.Text = "Dilekçe";
                        DilekceBtn.ID = "DilekceBtn" + sira;
                        TableUpdatePanel.ContentTemplateContainer.Controls.Add(DilekceBtn);
                        DilekceBtn.CssClass = "btn btn-outline-primary";
                        DilekceBtn.Click += delegate
                        {
                            try
                            {

                                if (mazeretIzinDonemi != null)
                                {
                                    string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                                    string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                                    int index = currentUrl.IndexOf(rawUrl);
                                    string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);

                                    TimeSpan kalanIzinTs = mazeretIzinDonemi.KalanIzin.ConvertToTimeSpan();
                                    double kalanIzinSaat = Math.Round(Math.Abs(kalanIzinTs.TotalHours) + 0.5);
                                    int mahsupGun = (int)Math.Round((kalanIzinSaat / ProjeConstants.BIRGUN_MAZERET_SAAT_INT + 0.5));
                                    var fullUrl = string.Format("{0}?IzinDonemId={1}&MahsupGun={2}", rootUrl + ProjeConstants.RAPOR_MAZERETEMAHSUPDILEKCE, mazeretIzinDonemi.Id, mahsupGun);
                                    ResponseHelper.Redirect(fullUrl, "_blank", "");
                                }
                            }
                            catch (Exception exception)
                            {
                                ExceptionHelper ex = new ExceptionHelper(exception);
                                ex.PublishException();
                            }
                        };
                        DilekceCell.Controls.Add(DilekceBtn);
                    }
                    row.Controls.Add(MahsupCell);
                    row.Controls.Add(DilekceCell);
                }

                IzinTable.Controls.Add(row);
            }
        }
        private void FillModalInfo()
        {
            int personelId = paramPersonelIdLbl.Value.ConvertToInt();
            Personel personel = new Personel();
            personel = personel.Select<Personel>(personelId);
            if (personel != null)
            {
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                FillOnayLbl(personel);
                FillIzinDonemTable(personel);
                AciklamaTxt.Text = DateTime.Today.ConvertToDatetimeEmptyIfNull() + " Tarihinde Mazeret Iznine mahsup edilmistir.";
                OnaylaModalBtn.Visible = true;
            }
        }
        private void FillIzinDonemTable(Personel personel)
        {
            DateTime today = DateTime.Today;
            IsBilgileri ib = new IsBilgileri();
            ib = ib.SelectByPersonelId(personel.Id);
            if (ib != null)
            {

                IzinDonem izinDonem = new IzinDonem();
                IzinDonem ucretliIzinDonemi = izinDonem.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_UCRETLI_INT, today);
                IzinDonem mazeretIzinDonemi = izinDonem.SelectByIzinTarihi(personel.Id, ProjeConstants.IZINTIPI_MAZERET_INT, today);

                IzinDonemTableHeaders();
                TableRow row = new TableRow();

                TableCell UcretliIzinTipiCell = new TableCell();
                UcretliIzinTipiCell.Text = ProjeConstants.IZINTIPI_UCRETLI;
                row.Controls.Add(UcretliIzinTipiCell);

                TableCell UcretliIzinDonemiCell = new TableCell();
                UcretliIzinDonemiCell.Text = ucretliIzinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + ucretliIzinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                row.Controls.Add(UcretliIzinDonemiCell);

                string ucretliIzinHakki = string.Empty;
                TableCell UcretliIzinHakkiCell = new TableCell();
                ucretliIzinHakki = ucretliIzinDonemi.IzinHakki;
                UcretliIzinHakkiCell.Text = ucretliIzinHakki.ReturnZeroIfNull().ToString() + " " + ucretliIzinDonemi.Birim;
                row.Controls.Add(UcretliIzinHakkiCell);

                TableCell UcretliKullanilanIzinCell = new TableCell();
                UcretliKullanilanIzinCell.Text = ucretliIzinDonemi.KullanilanIzin.ReturnZeroIfNull().ToString() + " " + ucretliIzinDonemi.Birim;
                row.Controls.Add(UcretliKullanilanIzinCell);

                TableCell UcretliKalanIzinCell = new TableCell();
                int ucretlikalanIzin = ucretliIzinDonemi.KalanIzin.ReturnZeroIfNull().ConvertToInt();
                if (ucretlikalanIzin <= 0)
                {
                    UcretliKalanIzinCell.ForeColor = System.Drawing.Color.Red;
                    UcretliKalanIzinCell.Font.Bold = true;
                }
                UcretliKalanIzinCell.Text = ucretliIzinDonemi.KalanIzin.ReturnZeroIfNull().ToString() + " " + ucretliIzinDonemi.Birim;
                row.Controls.Add(UcretliKalanIzinCell);
                IzinDonemleriTable.Controls.Add(row);

                TableRow row1 = new TableRow();
                TableCell MazeretIzinTipiCell = new TableCell();
                MazeretIzinTipiCell.Text = ProjeConstants.IZINTIPI_MAZERET;
                row1.Controls.Add(MazeretIzinTipiCell);

                TableCell MazeretIzinDonemiCell = new TableCell();
                MazeretIzinDonemiCell.Text = mazeretIzinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + mazeretIzinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                row1.Controls.Add(MazeretIzinDonemiCell);

                string mazeretIzinHakki = string.Empty;
                TableCell MazeretIzinHakkiCell = new TableCell();
                mazeretIzinHakki = mazeretIzinDonemi.IzinHakki;
                MazeretIzinHakkiCell.Text = mazeretIzinHakki.ConvertToTimeSpanReturnInHHmm().ToString();
                row1.Controls.Add(MazeretIzinHakkiCell);

                TableCell MazeretKullanilanIzinCell = new TableCell();
                MazeretKullanilanIzinCell.Text = mazeretIzinDonemi.KullanilanIzin.ConvertToTimeSpanReturnInHHmm().ToString();
                row1.Controls.Add(MazeretKullanilanIzinCell);

                TableCell MazeretKalanIzinCell = new TableCell();
                TimeSpan mazeretkalanIzin = ucretliIzinDonemi.KalanIzin.ConvertToTimeSpan();
                TimeSpan sifirTs = new TimeSpan(0, 0, 0);
                if (mazeretkalanIzin <= sifirTs)
                {
                    MazeretKalanIzinCell.ForeColor = System.Drawing.Color.Red;
                    MazeretKalanIzinCell.Font.Bold = true;
                }
                MazeretKalanIzinCell.Text = mazeretIzinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm().ToString();
                row1.Controls.Add(MazeretKalanIzinCell);
                IzinDonemleriTable.Controls.Add(row1);

            }
            else
            {
                MessageHelper.PublishMessage("Ise baslama tarihi belirlenemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        private void IzinDonemTableHeaders()
        {
            IzinDonemleriTable.Rows.Clear();
            TableHeaderRow th = new TableHeaderRow();
            TableHeaderCell izinTipiCell = new TableHeaderCell();
            izinTipiCell.Text = "Izin Tipi";
            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "Izin Dönemi";
            TableHeaderCell hakCell = new TableHeaderCell();
            hakCell.Text = "Izin Hakki";
            TableHeaderCell kullanilanCell = new TableHeaderCell();
            kullanilanCell.Text = "Kullanilan Izin";
            TableHeaderCell kalanCell = new TableHeaderCell();
            kalanCell.Text = "Kalan Izin";

            th.Controls.Add(izinTipiCell);
            th.Controls.Add(donemCell);
            th.Controls.Add(hakCell);
            th.Controls.Add(kullanilanCell);
            th.Controls.Add(kalanCell);

            IzinDonemleriTable.Controls.Add(th);
        }
        private void FillOnayLbl(Personel personel)
        {
            if (personel != null)
            {
                OnayLbl.Text = "Onaylamaniz halinde Yillik Ücretli Izinden 1 gün (9 saat) Mazeret iznine aktarilacaktir.";
            }

        }
        private void IzinTableHeaders(bool isForExcel)
        {
            IzinTable.Rows.Clear();

            TableHeaderRow th = new TableHeaderRow();
            th.HorizontalAlign = HorizontalAlign.Center;
            TableHeaderCell siraCell = new TableHeaderCell();
            siraCell.Text = "Sira";
            siraCell.RowSpan = 2;

            TableHeaderCell adiSoyadiCell = new TableHeaderCell();
            adiSoyadiCell.Text = "Adi Soyadi";
            adiSoyadiCell.RowSpan = 2;

            TableHeaderCell mazeretCell = new TableHeaderCell();
            mazeretCell.Text = "Mazeret Izni";
            mazeretCell.Font.Bold = true;
            mazeretCell.ColumnSpan = 3;

            TableHeaderCell izinDonemiCell = new TableHeaderCell();
            izinDonemiCell.Text = "Yillik Izin Dönemi";
            izinDonemiCell.ColumnSpan = 2;

            TableHeaderCell mahsupBtnCell = new TableHeaderCell();
            mahsupBtnCell.Text = "Mahsup";
            mahsupBtnCell.RowSpan = 2;

            TableHeaderCell dilekceBtnCell = new TableHeaderCell();
            dilekceBtnCell.Text = "Dilekçe";
            dilekceBtnCell.RowSpan = 2;

            th.Controls.Add(siraCell);
            th.Controls.Add(adiSoyadiCell);
            th.Controls.Add(mazeretCell);

            th.Controls.Add(izinDonemiCell);
            if (!isForExcel)
            {
                th.Controls.Add(mahsupBtnCell);
                th.Controls.Add(dilekceBtnCell);
            }
            IzinTable.Controls.Add(th);

            TableHeaderRow th1 = new TableHeaderRow();
            th1.HorizontalAlign = HorizontalAlign.Center;
            TableHeaderCell izinHakkiCell = new TableHeaderCell();
            izinHakkiCell.Text = "Izin Hakki";
            TableHeaderCell kullanilanIzinCell = new TableHeaderCell();
            kullanilanIzinCell.Text = "Kullanilan Izin";
            TableHeaderCell kalanIzinCell = new TableHeaderCell();
            kalanIzinCell.Text = "Kalan Izin";


            TableHeaderCell donemCell = new TableHeaderCell();
            donemCell.Text = "Dönem";
            TableHeaderCell kalanCell = new TableHeaderCell();
            kalanCell.Text = "Kalan Izin";

            th1.Controls.Add(izinHakkiCell);
            th1.Controls.Add(kullanilanIzinCell);
            th1.Controls.Add(kalanIzinCell);

            th1.Controls.Add(donemCell);
            th1.Controls.Add(kalanCell);

            IzinTable.Controls.Add(th1);
        }
        protected void ExportToExcel()
        {
            string filename = "MazereteMahsup" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            IzinTable.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            //yeni izin hareketi yarat
            //
            try
            {
                bool isSaved = BirGunMahsupEt();
                if (isSaved)
                {
                    MessageHelper.PublishMessage("Izin Kaydi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
                    RedirectToPage(ProjeConstants.PAGE_MAZERETEMAHSUP);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("Izin Talebi Kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }

        }
        protected void IzinBilgileriBtn_Click(object sender, EventArgs e)
        {
            FillModalInfo();
        }

        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            FillIzinTable(true);
            ExportToExcel();
        }
    }
}
