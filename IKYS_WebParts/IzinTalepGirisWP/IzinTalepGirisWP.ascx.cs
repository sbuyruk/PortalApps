using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.IzinTalepGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class IzinTalepGirisWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public IzinTalepGirisWP()
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
        private string IzinTalepIdQS
        {
            get
            {

                if (ViewState["IzinTalepId"] == null)
                {
                    if (Page.Request.QueryString["IzinTalepId"] != null)
                    {
                        ViewState["IzinTalepId"] = Page.Request.QueryString["IzinTalepId"];
                    }
                    else
                    {
                        ViewState["IzinTalepId"] = string.Empty;
                    }
                }
                return ViewState["IzinTalepId"].ToString();
            }

            set
            {
                ViewState["IzinTalepId"] = value;
            }
        }
        private string IzinTanimIdQS
        {
            get
            {

                if (ViewState["IzinTanimId"] == null)
                {
                    if (Page.Request.QueryString["IzinTanimId"] != null)
                    {
                        ViewState["IzinTanimId"] = Page.Request.QueryString["IzinTanimId"];
                    }
                    else
                    {
                        ViewState["IzinTanimId"] = IzinTanimDDL.SelectedItem.Value;
                    }
                }
                return ViewState["IzinTanimId"].ToString();
            }

            set
            {
                ViewState["IzinTanimId"] = value;
            }
        }
        private string IzinDonemIdQS
        {
            get
            {

                if (ViewState["IzinDonemId"] == null)
                {
                    if (Page.Request.QueryString["IzinDonemId"] != null)
                    {
                        ViewState["IzinDonemId"] = Page.Request.QueryString["IzinDonemId"];
                    }
                    else
                    {
                        ViewState["IzinDonemId"] = string.Empty;
                    }
                }
                return ViewState["IzinDonemId"].ToString();
            }

            set
            {
                ViewState["IzinDonemId"] = value;
            }
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
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IzinTalepTableDiv.Attributes["style"] = "display:none";
                if (!Page.IsPostBack)
                {
                    if (string.IsNullOrEmpty(DestinationAppQS) || (!DestinationAppQS.Equals("TD")))
                    {
                        OpenTalepGiris();
                    }
                    else if (DestinationAppQS.Equals("TD"))
                    {
                        OpenTalepDuzenle();
                    }
                    TabloyuDoldur();
                }
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (PersonelIdQS.ConvertToInt() > 0)
            {
                TabloyuDoldur();
            }
        }

        private void OpenTalepDuzenle()
        {
            BackBtn.Visible = true;
            SaveBtn.Visible = false;
            UpdateBtn.Visible = true;

            PersonelDiv.Attributes["style"] = "display:none";
            EPostaDiv.Attributes["style"] = "display:none";
            IzinTipiDDLDiv.Attributes["style"] = "display:none";
            IzinTipiLblDiv.Attributes["style"] = "display:block";
            Personel personel = PersonelGetir();
            if (!Page.IsPostBack)
            {

                if (personel != null)
                {
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                    FillVekilImzaDDL();
                    FillAmirImzaDDL();
                    FillOnayImzaDDL();
                    FillIzinBasSaat();
                    FillIzinBitSaat();

                    FillIzinTanim();
                    FillIzinTalepForm();
                }
            }
        }
        private void OpenTalepGiris()
        {
            BackBtn.Visible = false;
            //SaveBtn.Visible = true;
            UpdateBtn.Visible = false;
            IzinTipiDDLDiv.Attributes["style"] = "display:block";
            IzinTipiLblDiv.Attributes["style"] = "display:none";
            if (!Page.IsPostBack)
            {

                FillIzinTanim();
                FillVekilImzaDDL();
                FillAmirImzaDDL();
                FillOnayImzaDDL();

                FillIzinBasSaat();
                FillIzinBitSaat();

                Personel personel = PersonelGetir();

                if (AuthQS.Equals("IKYS"))
                {

                    //personelDDL doldur
                    //personelDiv display:block yap
                    //PersonelIdQS ten geleni PersonelDDL e ata
                    FillPersonelDDL();
                    PersonelDiv.Attributes["style"] = "display:block";
                    EPostaDiv.Attributes["style"] = "display:block";
                }
                else
                {
                    //personelDiv display:none yap
                    PersonelDiv.Attributes["style"] = "display:none";
                    EPostaDiv.Attributes["style"] = "display:none";
                    if (PersonelDDL.Items != null)
                        PersonelDDL.Items.Clear();
                }


                if (personel != null)
                {
                    SetDefaultVekilAmirOnay();
                    IzinBasTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                    IzinBitTarTxt.Value = DateTime.Today.ConvertToDatetimeEmptyIfNull();
                    KalanIzinKontrolIslemleri();
                    PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                }

            }
        }
        private void TabloyuDoldur()
        {
            Personel personel = PersonelGetir();
            if (personel != null)
            {
                IzinTalep izinTalep = new IzinTalep();
                DataTable dataTable = izinTalep.SelectIzinTalepleriReturnDT(personel.Id, IzinTanimIdQS.ConvertToInt(), false, true);
                int siraNo = 1;
                if (dataTable == null)
                {
                    IzinTalepTableDiv.Attributes["style"] = "display:block";
                    IzinTalepTable.Rows.Clear();
                    TableRow tr = new TableRow();
                    TableCell tc = new TableCell();
                    tc.Text = "İzin talebi bulunmamaktadır.";
                    tr.Controls.Add(tc);
                    IzinTalepTable.Controls.Add(tr);
                }
                else
                {
                    IzinTalepTableDiv.Attributes["style"] = "display:block";
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        if (siraNo > 5)
                        {
                            break;
                        }
                        string basTar = dataRow["BaslangicTarihi"].ConvertToDatetimeEmptyIfNull();
                        string bitTar = dataRow["BitisTarihi"].ConvertToDatetimeEmptyIfNull();
                        string birim = dataRow["Birim"].ToString();
                        string sure = dataRow["Sure"].ToString();
                        string sureDb = dataRow["Sure"].ToString();
                        int onayDurumuId = dataRow["OnayDurumuId"].ConvertToInt();
                        string onayDurumu = dataRow["OnayDurumu"].ReturnZeroIfNull().ToString();
                        string aciklama = dataRow["Aciklama"].ReturnZeroIfNull().ToString();
                        string izinTipi = dataRow["IzinTipi"].ReturnEmptyIfNull().ToString();
                        int izinTipiId = dataRow["IzinTipiId"].ConvertToInt();
                        int izinTalepId = dataRow["IzinTalepId"].ConvertToInt();
                        int izinDonemId = dataRow["IzinDonemId"].ConvertToInt();
                        if (izinTipiId == ProjeConstants.IZINTIPI_MAZERET_INT)
                        {
                            basTar = dataRow["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                            bitTar = dataRow["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy HH:mm");
                            //sure = sure.ConvertToTimeSpanReturnInHHmm();
                        }
                        else
                        {
                            sure = sure + " " + birim;
                        }
                        switch (siraNo)
                        {
                            case 1:
                                {
                                    BirinciSatir(siraNo, izinTipi, basTar, bitTar, sure, onayDurumuId, onayDurumu, aciklama,izinDonemId, izinTalepId);
                                    break;
                                }
                            case 2:
                                {
                                    IkinciSatir(siraNo, izinTipi, basTar, bitTar, sure, onayDurumuId, onayDurumu, aciklama, izinDonemId, izinTalepId);
                                    break;
                                }
                            case 3:
                                {
                                    UcuncuSatir(siraNo, izinTipi, basTar, bitTar, sure, onayDurumuId, onayDurumu, aciklama, izinDonemId, izinTalepId);
                                    break;
                                }
                            case 4:
                                {
                                    DorduncuSatir(siraNo, izinTipi, basTar, bitTar, sure, onayDurumuId, onayDurumu, aciklama, izinDonemId, izinTalepId);
                                    break;
                                }
                            case 5:
                                {
                                    BesinciSatir(siraNo, izinTipi, basTar, bitTar, sure, onayDurumuId, onayDurumu, aciklama, izinDonemId, izinTalepId);
                                    break;
                                }
                        }
                        siraNo++;
                    }
                }
            }
        }

        private void BirinciSatir(int siraNo, string izinTipi, string basTar, string bitTar, string sure, int onayDurumuId, string onayDurumu, string aciklama,int izinDonemId,int izinTalepId)
        {
            SiraR1Cell.Text = siraNo + "";
            IzinTipiR1Cell.Text = izinTipi;
            BasTarR1Cell.Text = basTar;
            BitTarR1Cell.Text = bitTar;
            SureR1Cell.Text = izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET)? sure.ConvertToTimeSpanReturnInHHmm():sure;
            OnayDurumuR1Cell.Text = onayDurumu;
            OnayDurumuR1Cell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();

            if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR)
                || onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
            {
                IzinTalepIdHdn1.Value = izinTalepId.ToString();
                YazdirLnk1.Visible = false;
                SilBtn1.Visible = true;
                SilBtn1.ToolTip = "Talebiniz İşlem görmeden önce silebilirsiniz.";
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
            {
                YazdirLnk1.Visible = true;
                SilBtn1.Visible = false;
                YazdirLnk1.Target = "_blank";
                YazdirLnk1.ToolTip = "Talebiniz yazdırmak için düğmeye basınız.";
                YazdirLnk1.Text = "Yazdır";
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                int index = currentUrl.IndexOf(rawUrl);
                string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                string sonIzin = KalacakIzinHesapla(izinDonemId, sure);

                var fullUrl = "";

                if (izinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                }
                else if (izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET))
                {
                    IzinDonem id = new IzinDonem();
                    id = id.Select<IzinDonem>(izinDonemId);
                    string kalanIzin = string.Empty;
                    if (id != null)
                    {
                        kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }

                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                }
                else //diger İzinler
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, sure, "", "");
                }
                YazdirLnk1.NavigateUrl = fullUrl;
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT) ||
                onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_REDDEDILDI))
            {
                YazdirLnk1.Visible = false;
                SilBtn1.Visible = false;
            }
        }
        private void IkinciSatir(int siraNo, string izinTipi, string basTar, string bitTar, string sure, int onayDurumuId, string onayDurumu, string aciklama, int izinDonemId, int izinTalepId)
        {
            SiraR2Cell.Text = siraNo + "";
            IzinTipiR2Cell.Text = izinTipi;
            BasTarR2Cell.Text = basTar;
            BitTarR2Cell.Text = bitTar;
            SureR2Cell.Text = izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET) ? sure.ConvertToTimeSpanReturnInHHmm() : sure;
            OnayDurumuR2Cell.Text = onayDurumu;
            OnayDurumuR2Cell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();

            if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR)
                || onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
            {
                IzinTalepIdHdn2.Value = izinTalepId.ToString();
                YazdirLnk2.Visible = false;
                SilBtn2.Visible = true;
                SilBtn2.ToolTip = "Talebiniz İşlem görmeden önce silebilirsiniz.";
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
            {
                YazdirLnk2.Visible = true;
                SilBtn2.Visible = false;
                YazdirLnk2.Target = "_blank";
                YazdirLnk2.ToolTip = "Talebiniz yazdırmak için düğmeye basınız.";
                YazdirLnk2.Text = "Yazdır";
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                int index = currentUrl.IndexOf(rawUrl);
                string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                string sonIzin = KalacakIzinHesapla(izinDonemId, sure);

                var fullUrl = "";

                if (izinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                }
                else if (izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET))
                {
                    IzinDonem id = new IzinDonem();
                    id = id.Select<IzinDonem>(izinDonemId);
                    string kalanIzin = string.Empty;
                    if (id != null)
                    {
                        kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }

                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                }
                else //diger İzinler
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, sure, "", "");
                }
                YazdirLnk2.NavigateUrl = fullUrl;
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT) ||
                onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_REDDEDILDI))
            {
                YazdirLnk2.Visible = false;
                SilBtn2.Visible = false;
            }
        }
        private void UcuncuSatir(int siraNo, string izinTipi, string basTar, string bitTar, string sure, int onayDurumuId, string onayDurumu, string aciklama, int izinDonemId, int izinTalepId)
        {
            SiraR3Cell.Text = siraNo + "";
            IzinTipiR3Cell.Text = izinTipi;
            BasTarR3Cell.Text = basTar;
            BitTarR3Cell.Text = bitTar;
            SureR3Cell.Text = izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET) ? sure.ConvertToTimeSpanReturnInHHmm() : sure;
            OnayDurumuR3Cell.Text = onayDurumu;
            OnayDurumuR3Cell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();

            if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR)
                || onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
            {
                IzinTalepIdHdn3.Value = izinTalepId.ToString();
                YazdirLnk3.Visible = false;
                SilBtn3.Visible = true;
                SilBtn3.ToolTip = "Talebiniz İşlem görmeden önce silebilirsiniz.";
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
            {
                YazdirLnk3.Visible = true;
                SilBtn3.Visible = false;
                YazdirLnk3.Target = "_blank";
                YazdirLnk3.ToolTip = "Talebiniz yazdırmak için düğmeye basınız.";
                YazdirLnk3.Text = "Yazdır";
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                int index = currentUrl.IndexOf(rawUrl);
                string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                string sonIzin = KalacakIzinHesapla(izinDonemId, sure);

                var fullUrl = "";

                if (izinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                }
                else if (izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET))
                {
                    IzinDonem id = new IzinDonem();
                    id = id.Select<IzinDonem>(izinDonemId);
                    string kalanIzin = string.Empty;
                    if (id != null)
                    {
                        kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }

                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                }
                else //diger İzinler
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, sure, "", "");
                }
                YazdirLnk3.NavigateUrl = fullUrl;
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT) ||
                onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_REDDEDILDI))
            {
                YazdirLnk3.Visible = false;
                SilBtn3.Visible = false;
            }
        }
        private void DorduncuSatir(int siraNo, string izinTipi, string basTar, string bitTar, string sure, int onayDurumuId, string onayDurumu, string aciklama, int izinDonemId, int izinTalepId)
        {
            SiraR4Cell.Text = siraNo + "";
            IzinTipiR4Cell.Text = izinTipi;
            BasTarR4Cell.Text = basTar;
            BitTarR4Cell.Text = bitTar;
            SureR4Cell.Text = izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET) ? sure.ConvertToTimeSpanReturnInHHmm() : sure;
            OnayDurumuR4Cell.Text = onayDurumu;
            OnayDurumuR4Cell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();

            if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR)
                || onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
            {
                IzinTalepIdHdn4.Value = izinTalepId.ToString();
                YazdirLnk4.Visible = false;
                SilBtn4.Visible = true;
                SilBtn4.ToolTip = "Talebiniz İşlem görmeden önce silebilirsiniz.";
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
            {
                YazdirLnk4.Visible = true;
                SilBtn4.Visible = false;
                YazdirLnk4.Target = "_blank";
                YazdirLnk4.ToolTip = "Talebiniz yazdırmak için düğmeye basınız.";
                YazdirLnk4.Text = "Yazdır";
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                int index = currentUrl.IndexOf(rawUrl);
                string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                string sonIzin = KalacakIzinHesapla(izinDonemId, sure);

                var fullUrl = "";

                if (izinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                }
                else if (izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET))
                {
                    IzinDonem id = new IzinDonem();
                    id = id.Select<IzinDonem>(izinDonemId);
                    string kalanIzin = string.Empty;
                    if (id != null)
                    {
                        kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }

                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                }
                else //diger İzinler
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, sure, "", "");
                }
                YazdirLnk4.NavigateUrl = fullUrl;
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT) ||
                onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_REDDEDILDI))
            {
                YazdirLnk4.Visible = false;
                SilBtn4.Visible = false;
            }
        }
        private void BesinciSatir(int siraNo, string izinTipi, string basTar, string bitTar, string sure, int onayDurumuId, string onayDurumu, string aciklama, int izinDonemId, int izinTalepId)
        {
            SiraR5Cell.Text = siraNo + "";
            IzinTipiR5Cell.Text = izinTipi;
            BasTarR5Cell.Text = basTar;
            BitTarR5Cell.Text = bitTar;
            SureR5Cell.Text = izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET) ? sure.ConvertToTimeSpanReturnInHHmm() : sure;
            OnayDurumuR5Cell.Text = onayDurumu;
            OnayDurumuR5Cell.ToolTip = aciklama.ReturnEmptyIfNull().ToString();

            if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR)
                || onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR))
            {
                IzinTalepIdHdn5.Value = izinTalepId.ToString();
                YazdirLnk5.Visible = false;
                SilBtn5.Visible = true;
                SilBtn5.ToolTip = "Talebiniz İşlem görmeden önce silebilirsiniz.";
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KONTROLEDILDI_INT))
            {
                YazdirLnk5.Visible = true;
                SilBtn5.Visible = false;
                YazdirLnk5.Target = "_blank";
                YazdirLnk5.ToolTip = "Talebiniz yazdırmak için düğmeye basınız.";
                YazdirLnk5.Text = "Yazdır";
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
                int index = currentUrl.IndexOf(rawUrl);
                string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);
                string sonIzin = KalacakIzinHesapla(izinDonemId, sure);

                var fullUrl = "";

                if (izinTipi.Equals(ProjeConstants.IZINTIPI_UCRETLI))
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&IzinDonemId={2}&SonIzin={3}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId, sonIzin);//string.Format("{0}?rv:RelativeReportUrl={1}&rp:IzinTalepId={2}&rp:IzinDonemId={3}", ProjeConstants.RAPOR_SUNUCU_SAYFASI, ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, izinDonemId);
                }
                else if (izinTipi.Equals(ProjeConstants.IZINTIPI_MAZERET))
                {
                    IzinDonem id = new IzinDonem();
                    id = id.Select<IzinDonem>(izinDonemId);
                    string kalanIzin = string.Empty;
                    if (id != null)
                    {
                        kalanIzin = id.KalanIzin.ConvertToTimeSpanReturnInHHmm();
                    }

                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_MAZERETIZINBELGESI_URL, izinTalepId, sure, kalanIzin, sonIzin);
                }
                else //diger İzinler
                {
                    fullUrl = string.Format("{0}?IzinTalepId={1}&SureStr={2}&KalanIzinStr={3}&SonIzin={4}", rootUrl + ProjeConstants.RAPOR_IZINBELGESI_URL, izinTalepId, sure, "", "");
                }
                YazdirLnk5.NavigateUrl = fullUrl;
            }
            else if (onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_KAYITLARAISLENDI_INT) ||
                onayDurumuId.Equals(ProjeConstants.PER_IZINTALEBI_REDDEDILDI))
            {
                YazdirLnk5.Visible = false;
                SilBtn5.Visible = false;
            }
        }
        private string KalacakIzinHesapla(int izinDonemId, string sure)
        {
            string sonuctaKalanIzin = string.Empty;
            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(izinDonemId);
            if (izinDonemi != null)
            {
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {

                    int oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToInt();
                    int sonuc = oncekiToplamIzin + sure.ConvertToInt();
                    int izinHakki = izinDonemi.IzinHakki.ConvertToInt();
                    int kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ToString() + " " + izinDonemi.Birim;
                }
                if (izinDonemi.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    TimeSpan oncekiToplamIzin = izinDonemi.KullanilanIzin.ConvertToTimeSpan();
                    TimeSpan sonuc = oncekiToplamIzin + sure.ConvertToTimeSpan();
                    TimeSpan kalanIzin = new TimeSpan(0, 0, 0); ;
                    TimeSpan izinHakki = izinDonemi.IzinHakki.ConvertToTimeSpan();
                    kalanIzin = izinHakki - sonuc;
                    sonuctaKalanIzin = kalanIzin.ConvertToTimeSpanReturnInHHmm();
                }
            }
            return sonuctaKalanIzin;
        }

        private void SetDefaultVekilAmirOnay()
        {
            SelectDDLValue(VekilImzaDDL, ProjeConstants.BOS_INT.ToString());
            SelectDDLValue(AmirImzaDDL, ProjeConstants.BOS_INT.ToString());
            SelectDDLValue(OnayImzaDDL, ProjeConstants.BOS_INT.ToString());

        }
        private void FillPersonelDDL()
        {
            PersonelDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();

            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                PersonelDDL.Items.Add(li);
            }

            string secilenPer = !string.IsNullOrEmpty(PersonelIdQS) ? PersonelIdQS : "0";
            ListItem perItem = new ListItem();
            if (!string.IsNullOrEmpty(secilenPer))
                perItem = PersonelDDL.Items.FindByValue(secilenPer);

            if (perItem != null)
            {
                PersonelDDL.SelectedValue = perItem.Value;
                PersonelIdQS = perItem.Value;
            }
        }
        private void FillVekilImzaDDL()
        {
            VekilImzaDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString());
            VekilImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                if (item.Id == ProjeConstants.GENELMUDUR_PERSONELID)
                {
                    continue;
                }
                else
                {
                    ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                    VekilImzaDDL.Items.Add(li);
                }
            }
        }
        private void FillAmirImzaDDL()
        {
            AmirImzaDDL.Items.Clear();
            Personel personel = new Personel();
            List<Personel> list = personel.SelectCalisanPersonel();
            ListItem bosLi = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString());
            AmirImzaDDL.Items.Add(bosLi);
            foreach (Personel item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString() + " " + item.Soyadi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                AmirImzaDDL.Items.Add(li);
            }
        }
        private void FillOnayImzaDDL()
        {
            OnayImzaDDL.Items.Clear();
            ListItem bosLi = new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString());
            OnayImzaDDL.Items.Add(bosLi);
            Personel personel = new Personel();
            DataTable dataTable = personel.SelectAmirReturnDataTable();
            foreach (DataRow dataRow in dataTable.Rows)
            {

                int personelId = dataRow["PersonelId"].ConvertToInt();
                string adiSoyadi = dataRow["Adi"].ToString() + " " + dataRow["Soyadi"].ToString();
                if (OnayImzaDDL.Items.FindByText(adiSoyadi) != null)
                {
                    continue;
                }
                else
                {
                    ListItem li = new ListItem(adiSoyadi, personelId.ToString());
                    OnayImzaDDL.Items.Add(li);
                }

            }
        }
        private void FillIzinTanim()
        {
            IzinTanimDDL.Items.Clear();
            IzinTanim izinTanim = new IzinTanim();
            List<IzinTanim> list = izinTanim.SelectAll<IzinTanim>();
            foreach (IzinTanim item in list)
            {
                ListItem li = new ListItem(item.Adi.ReturnEmptyIfNull().ToString(), item.Id.ReturnZeroIfNull().ToString());
                IzinTanimDDL.Items.Add(li);
            }
        }
        private void FillIzinBasSaat()
        {
            IzinBasSaatDDL.Items.Clear();
            TimeSpan bastarTS = ProjeConstants.MESAI_BASLAMA_SAATI;//new TimeSpan(7, 0, 0);
            TimeSpan aralikTS = ProjeConstants.MAZERETIZNI_SAAT_ARALIGI; //new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = ProjeConstants.MESAI_BITIS_SAATI;//new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            while (nextTS < bittarTS)
            {

                if ((nextTS >= oglenArasiBastarTS) && (nextTS < oglenArasiBittarTS))
                {
                    nextTS = nextTS + aralikTS;
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBasSaatDDL.Items.Add(li);
                    nextTS = nextTS + aralikTS;
                }
            }
        }
        private void FillIzinBitSaat()
        {
            IzinBitSaatDDL.Items.Clear();
            string bassaatStr = IzinBasSaatDDL.SelectedItem == null ? "07:05" : IzinBasSaatDDL.SelectedItem.Text;
            TimeSpan bastarTS = bassaatStr.ConvertToTimeSpan();
            TimeSpan aralikTS = new TimeSpan(0, 5, 0);
            TimeSpan bittarTS = new TimeSpan(17, 0, 0);

            TimeSpan oglenArasiBastarTS = new TimeSpan(12, 0, 0);
            TimeSpan oglenArasiBittarTS = new TimeSpan(13, 0, 0);

            TimeSpan nextTS = bastarTS;

            do
            {
                nextTS = nextTS + aralikTS;
                if ((nextTS > oglenArasiBastarTS) && (nextTS <= oglenArasiBittarTS))
                {
                    continue;
                }
                else
                {
                    string bastarStr = string.Format("{0:00}:{1:00}", nextTS.Hours, nextTS.Minutes);
                    ListItem li = new ListItem(bastarStr);
                    IzinBitSaatDDL.Items.Add(li);
                }

            } while (nextTS < bittarTS);
        }
        private void FillIzinTalepForm()
        {

            IzinTalep izinTalep = new IzinTalep();
            izinTalep = izinTalep.Select<IzinTalep>(IzinTalepIdQS.ConvertToInt());
            if (izinTalep != null)
            {
                IzinTalepIdLbl.Text = izinTalep.Id.ReturnEmptyIfNull().ToString();
                IzinTanim it = new IzinTanim();
                it = it.Select<IzinTanim>(izinTalep.IzinTipi);
                IzinTipiLbl.Text = it.Adi;
                SelectDDLValue(VekilImzaDDL, izinTalep.VekilImza.ToString());
                SelectDDLValue(AmirImzaDDL, izinTalep.AmirImza.ToString());
                SelectDDLValue(OnayImzaDDL, izinTalep.OnayImza.ToString());
                SelectDDLValue(IzinTanimDDL, izinTalep.IzinTipi.ToString());
                string bassaat = izinTalep.BaslangicTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBasSaatDDL, bassaat);
                string bitsaat = izinTalep.BitisTarihi.ToString("HH:mm");
                SelectDDLByText(IzinBitSaatDDL, bitsaat);
                AciklamaTxt.Text = izinTalep.Aciklama;
                IzinBasTarTxt.Value = izinTalep.BaslangicTarihi.ConvertToDatetimeEmptyIfNull();
                IzinBitTarTxt.Value = izinTalep.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                AdresTxt.Text = izinTalep.Adres.ReturnEmptyIfNull().ToString();
                SetLayoutByIzinTipi();
            }
            else
            {
                MessageHelper.PublishMessage("Izin talebi bulunamadı", ProjeConstants.MESAJ_BILGI);
            }
        }
        private void SetLayoutByIzinTipi()
        {
            IzinTanimIdQS = IzinTanimDDL.SelectedItem.Value;
            //izin tipi Mazeret ise
            if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT)) //(IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_MAZERET_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                AmirDiv.Attributes["style"] = "display:block";
                AciklamaDiv.Attributes["style"] = "display:none";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:none";
                IzinBasSaatDiv.Attributes["style"] = "display:block";
                IzinBitSaatDiv.Attributes["style"] = "display:block";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-warning";
                AdresLbl.InnerText = "Mazeret";
                //KalanIzinKontrolIslemleri();
                KullanilmayanLbl.Text = "Not: Kullanılmayan Mazeret izinleri müteakip yıla aktarılmaz.";
            }
            else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT)) // (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == (ProjeConstants.IZINTIPI_UCRETLI_INT))
            {
                VekilDiv.Attributes["style"] = "display:block";
                AmirDiv.Attributes["style"] = "display:block";
                OnayDiv.Attributes["style"] = "display:block";
                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:none";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                AciklamaDiv.Attributes["style"] = "display:block";
                AdresLbl.InnerText = "Adres";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-secondary";
                //KalanIzinKontrolIslemleri();
                KullanilmayanLbl.Text = "";
            }
            else if (IzinTanimIdQS.ConvertToInt() == (ProjeConstants.IZINTIPI_SUTIZNI_INT))
            {
                VekilDiv.Attributes["style"] = "display:none";
                AmirDiv.Attributes["style"] = "display:none";
                AciklamaDiv.Attributes["style"] = "display:block";
                OnayDiv.Attributes["style"] = "display:none";
                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:block";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-info";
                AciklamaTxt.Text = "Süt İzni (Günde 1 Buçuk saat)";
                KullanilmayanLbl.Text = "Not: Süt İzni girdiğiniz saatten başlayarak günlük 1 buçuk saat olarak uygulanır.";
                IletisimBilgileri ib = new IletisimBilgileri();
                ib = ib.SelectByPersonelId(PersonelIdQS.ConvertToInt());
                if (ib != null)
                {
                    Ilce ilce = new Ilce();
                    ilce = ilce.Select<Ilce>(ib.Ilcesi);
                    string ilcestr = ilce.IlceAdi;
                    AdresTxt.Text = ib.Adres + " " + ib.Semt + " " + ilcestr;
                }
            }
            else
            {
                VekilDiv.Attributes["style"] = "display:block";
                VekilDiv.Attributes["style"] = "display:block";
                OnayDiv.Attributes["style"] = "display:block";
                IzinBitTarDiv.Attributes["style"] = "display:block";
                IzinBasSaatDiv.Attributes["style"] = "display:none";
                IzinBitSaatDiv.Attributes["style"] = "display:none";
                IzinTalepDiv.Attributes["Class"] = "card-body alert-danger";
                AciklamaDiv.Attributes["style"] = "display:block";
                AdresLbl.InnerText = "Adres";
                KullanilmayanLbl.Text = "";
            }
        }
        private string SelectDDLValue(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByValue(value) != null)
            {
                ListItem li = ddlList.Items.FindByValue(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        private string SelectDDLByText(DropDownList ddlList, string value)
        {
            if (ddlList.Items.FindByText(value) != null)
            {
                ListItem li = ddlList.Items.FindByText(value);
                ddlList.SelectedValue = li.Value;
                return li.Value;
            }
            return string.Empty;
        }
        /// <summary>
        /// Yeni Oluşturulan İzin talebini IzinTalep_Table'a kaydeder.
        /// Girilen İzin başlangıç tarihine bakarak izin dönemi başlangıcını bulur,
        /// Bulduğu döneme ait IzinDonem_Table'da kayıt yoksa, ekler
        /// </summary>
        /// <returns></returns>
        private int YeniTalebiKaydet(int onayDurumu)
        {
            int izinTalepId = 0;
            try
            {

                Personel personel = PersonelGetir();
                IzinTalep izinTalep = new IzinTalep();
                izinTalep.PersonelId = personel.Id;
                izinTalep.IzinTipi = IzinTanimDDL.SelectedItem.Value.ConvertToInt();

                izinTalep.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                izinTalep.Adres = AdresTxt.Text;
                izinTalep.Aciklama = AciklamaTxt.Text;
                if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT)
                {
                    string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                    izinTalep.BaslangicTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BaslangicTarihi, basSaat);

                    izinTalep.BitisTarihi = IzinBitTarTxt.Value.ConvertToDatetime();
                    string bitSaat = "01:30";
                    izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BitisTarihi, basSaat);
                    izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BitisTarihi, bitSaat);
                    izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    izinTalep.Sure = IKYSOrtak.IzinSuresiHesapla(izinTalep.IzinTipi, izinTalep.BaslangicTarihi, izinTalep.BitisTarihi);
                    izinTalep.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                }
                else if (izinTalep.IzinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    TimeSpan izinBitisSaati = new TimeSpan(17, 0, 0);
                    DateTime bitisTar = (IzinBitTarTxt.Value.ConvertToDatetime() + izinBitisSaati);
                    izinTalep.BitisTarihi = bitisTar.ConvertToDatetime();
                    izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    izinTalep.Sure = IKYSOrtak.IzinSuresiHesapla(izinTalep.IzinTipi, izinTalep.BaslangicTarihi, izinTalep.BitisTarihi);
                    izinTalep.VekilImza = VekilImzaDDL.SelectedItem.Value.ConvertToInt();
                    izinTalep.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                    izinTalep.OnayImza = OnayImzaDDL.SelectedItem.Value.ConvertToInt();


                }
                else if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    DateTime izinTarihi = izinTalep.BaslangicTarihi;
                    string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                    izinTalep.BaslangicTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, basSaat);
                    string bitSaat = IzinBitSaatDDL.SelectedItem.Text;
                    izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, bitSaat);
                    izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                    izinTalep.Sure = IKYSOrtak.IzinSuresiHesapla(izinTalep.IzinTipi, izinTalep.BaslangicTarihi, izinTalep.BitisTarihi);
                    izinTalep.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                }

                izinTalep.Aktif = true;

                izinTalep.IzinDonemId = 0;
                //sadece Mazeret ve Ucretli izinler için Dönem hesapla
                if ((izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) ||
                   (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT))
                {
                    IzinDonem izinDonemi = new IzinDonem();
                    izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinTalep.IzinTipi, izinTalep.BaslangicTarihi);
                    if (izinDonemi == null)
                    {
                        izinDonemi = new IzinDonem();
                        izinDonemi = izinDonemi.IzinDonemiOlustur(personel, izinTalep.IzinTipi, izinTalep.BaslangicTarihi, CurrentUserName);
                    }
                    else
                    {
                        izinDonemi = izinDonemi.IzinDonemiGuncelle(personel, izinTalep.IzinTipi, izinTalep.BaslangicTarihi, CurrentUserName);
                    }
                    izinTalep.IzinDonemId = izinDonemi != null ? izinDonemi.Id : 0;
                }
                izinTalep.Olusturan = CurrentUserName;
                izinTalep.OnayDurumu = onayDurumu;
                izinTalep.EPostaGonder = EPostaGonderChk.Checked;
                izinTalepId = izinTalep.Save();

            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return izinTalepId;
        }
        private bool IzinTalebiGuncelle()
        {
            bool isSaved = false;
            try
            {
                Personel personel = PersonelGetir();
                IzinTalep izinTalep = new IzinTalep();
                izinTalep = izinTalep.Select<IzinTalep>(IzinTalepIdQS.ConvertToInt());
                if (izinTalep != null)
                {
                    izinTalep.BaslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
                    izinTalep.Adres = AdresTxt.Text;
                    izinTalep.Aciklama = AciklamaTxt.Text;

                    if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT)//süt izni, 1buçuk saat
                    {
                        string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                        izinTalep.BaslangicTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BaslangicTarihi, basSaat);

                        DateTime bitisTar = IzinBitTarTxt.Value.ConvertToDatetime();
                        string bitSaat = "01:30";
                        izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BitisTarihi, basSaat);
                        izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTalep.BitisTarihi, bitSaat);
                        izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                    }
                    else if (izinTalep.IzinTipi != ProjeConstants.IZINTIPI_MAZERET_INT) //Mazeret hariç diğr izinler
                    {
                        TimeSpan izinBitisSaati = new TimeSpan(0, 17, 0, 0);
                        DateTime bitisTar = (IzinBitTarTxt.Value.ConvertToDatetime() + izinBitisSaati);
                        izinTalep.BitisTarihi = bitisTar.ConvertToDatetime();
                        izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_GUN;
                        izinTalep.Sure = IKYSOrtak.IzinSuresiHesapla(izinTalep.IzinTipi, izinTalep.BaslangicTarihi, izinTalep.BitisTarihi);
                        izinTalep.VekilImza = VekilImzaDDL.SelectedItem.Value.ConvertToInt();
                        izinTalep.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();
                        izinTalep.OnayImza = OnayImzaDDL.SelectedItem.Value.ConvertToInt();

                    }
                    else if (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                    {
                        DateTime izinTarihi = izinTalep.BaslangicTarihi;
                        string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                        izinTalep.BaslangicTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, basSaat);
                        string bitSaat = IzinBitSaatDDL.SelectedItem.Text;
                        izinTalep.BitisTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, bitSaat);
                        izinTalep.Birim = ProjeConstants.IZIN_BIRIMI_SAAT;
                        izinTalep.Sure = IKYSOrtak.IzinSuresiHesapla(izinTalep.IzinTipi, izinTalep.BaslangicTarihi, izinTalep.BitisTarihi);
                        izinTalep.AmirImza = AmirImzaDDL.SelectedItem.Value.ConvertToInt();

                    }

                    izinTalep.Aktif = true;

                    izinTalep.IzinDonemId = 0;
                    //sadece Mazeret ve Ucretli izinler için Dönem hesapla
                    if ((izinTalep.IzinTipi == ProjeConstants.IZINTIPI_MAZERET_INT) ||
                       (izinTalep.IzinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT))
                    {
                        IzinDonem izinDonemi = new IzinDonem();
                        izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinTalep.IzinTipi, izinTalep.BaslangicTarihi);
                        if (izinDonemi == null)
                        {
                            izinDonemi = new IzinDonem();
                            izinDonemi = izinDonemi.IzinDonemiOlustur(personel, izinTalep.IzinTipi, izinTalep.BaslangicTarihi, CurrentUserName);
                        }
                        else
                        {
                            izinDonemi = izinDonemi.IzinDonemiGuncelle(personel, izinTalep.IzinTipi, izinTalep.BaslangicTarihi, CurrentUserName);
                        }
                        izinTalep.IzinDonemId = izinDonemi != null ? izinDonemi.Id : 0;
                    }
                    izinTalep.Degistiren = CurrentUserName;
                    izinTalep.EPostaGonder = EPostaGonderChk.Checked;
                    isSaved = izinTalep.Update();
                }
                else
                {
                    MessageHelper.PublishMessage("Izin talebi bulunamadı", ProjeConstants.MESAJ_BILGI);
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }

            return isSaved;
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
                PersonelIdQS = personel.Id.ToString();
            }

            return personel;
        }
        protected void IzinTanimDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            IzinTanimIdQS = IzinTanimDDL.SelectedItem.Value;
            SetDefaultVekilAmirOnay();
            KalanIzinKontrolIslemleri();
            SetLayoutByIzinTipi();
            TabloyuDoldur();

        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void BackBtn_Click(object sender, EventArgs e)
        {
            if (IzinTanimIdQS.ConvertToInt() == ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                RedirectToPage(ProjeConstants.PAGE_MAZERETIZINTALEP_LIST+ "?SecilenId=" + IzinTalepIdQS);
            }
            else
            {
                RedirectToPage(ProjeConstants.PAGE_IZINTALEP_LIST + "?SecilenId=" + IzinTalepIdQS);
            }
        }
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if ((IzinTanimDDL.SelectedItem.Value.ConvertToInt()==ProjeConstants.IZINTIPI_UCRETLI_INT)
                    || (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.IZINTIPI_BABALIK_INT)
                    || (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.IZINTIPI_DOGUM_INT)
                    || (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.IZINTIPI_EVLENME_INT)
                    || (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.IZINTIPI_OLUM_INT)
                    || (IzinTanimDDL.SelectedItem.Value.ConvertToInt() == ProjeConstants.IZINTIPI_UCRETSIZ_INT)
                 )
                {
                    if (string.IsNullOrEmpty(VekilImzaDDL.SelectedItem.Text) || string.IsNullOrEmpty(AmirImzaDDL.SelectedItem.Text) )
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("Vekil ve Amir Seçiniz.", ProjeConstants.MESAJ_HATA);
                        return;
                    }

                }
                Personel personel = new Personel();
                personel = PersonelGetir();
                if (personel != null)
                {
                    bool isValid = KalanIzinKontrolIslemleri();
                    if (isValid)
                    {
                        bool devamEdenIzinTalebiVarMi = IslemiDevamEdenIzinTalebiVarMi(personel.Id);// SB 13/09/2019 devam eden izin talebi kontrolu eklendi
                                                                                                    //bool cakismaVarMi = BuTarihteCakisanIzinTalebiVarMi(personel.Id);//  SB 13/09/2019 devam eden izin talebi kontrolu eklendiğinden çalışma kontrolüne gerek kalmadı
                        if (devamEdenIzinTalebiVarMi)//if (cakismaVarMi)
                        {
                            //MessageHelper.PublishMessage("Bu tarihle çakışan bir izin talebiniz zaten var."+System.Environment.NewLine+
                            //    "Kişisel sayfanızdan İzin taleplerinizi görebilirsiniz.", ProjeConstants.MESAJ_HATA,15000);
                            MessageHelper.PublishMessage("İşlemi devam eden bir izin talebiniz zaten var." + System.Environment.NewLine +
                               "Yeni bir izin talep etmeden önce var olan izin talebinizin sonuçlanması gerekmektedir.", ProjeConstants.MESAJ_HATA, 15000);
                            SaveBtn.Visible = false;
                            UpdateBtn.Visible = false;
                            UcretliIzinDilekceBtn.Visible = false;

                        }
                        else
                        {
                            //Talebi kaydet
                            //İzinTalepTablosunu Doldur
                            int izinTalepId = YeniTalebiKaydet(ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR);
                            if (izinTalepId > 0)
                            {
                                if (EPostaGonderChk.Checked)
                                {
                                    IKYSOrtak.IzinTalepOlusturmaEPostasiGonder(personel, izinTalepId);
                                }
                                TabloyuDoldur();
                                SaveBtn.Visible = false;
                                MessageHelper.PublishMessage("İzin Talebi Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                            }
                        }

                    }
                    else
                    {
                        MessageHelper.PublishMessage("Lütfen Kalan İzin sürenizi Kontrol Ediniz.", ProjeConstants.MESAJ_HATA);
                    }
                }
                else
                {
                    MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                }

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("İzin Talebi kaydedilemedi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }
        }
        private bool IslemiDevamEdenIzinTalebiVarMi(int personelId)
        {
            bool izinTalebiVarMi = false;
            int izinTipi = IzinTanimDDL.SelectedItem.Value.ConvertToInt();

            IzinTalep izinTalep = new IzinTalep();
            izinTalep = izinTalep.SelectIslemiDevamEdenIzinTalebiVarMı(personelId, izinTipi);
            if (izinTalep != null)
            {
                izinTalebiVarMi = true;
            }
            return izinTalebiVarMi;
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //Talebi kaydet
                //İzinTalepTablosunu Doldur
                bool isSaved = IzinTalebiGuncelle();
                if (isSaved)
                    MessageHelper.PublishMessage("İzin Talebi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);

            }
            catch (Exception exception)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
                Exception exceptionInfo = new Exception("İzin Talebi Güncellenemdi");
                exceptionHelper.Exceptions.Add(exceptionInfo);
                exceptionHelper.PublishException();
            }

        }
        private bool KalanIzinKontrolIslemleri()
        {
            SaveBtn.Visible = false;
            ClearIzinBilgileri();
            Personel personel = new Personel();
            personel = PersonelGetir();
            int izinTipi = IzinTanimDDL.SelectedItem.Value.ConvertToInt();
            DateTime baslangicTarihi = IzinBasTarTxt.Value.ConvertToDatetime();
            if (personel == null)
            {
                return false;
            }
            bool devamEdenIzinTalebiVarMi = IslemiDevamEdenIzinTalebiVarMi(personel.Id);
            if (devamEdenIzinTalebiVarMi)//if (cakismaVarMi)
            {
                //MessageHelper.PublishMessage("Bu tarihle çakışan bir izin talebiniz zaten var."+System.Environment.NewLine+
                //    "Kişisel sayfanızdan İzin taleplerinizi görebilirsiniz.", ProjeConstants.MESAJ_HATA,15000);
                MessageHelper.PublishMessage("İşlemi devam eden bir izin talebiniz zaten var." + System.Environment.NewLine +
                   "Yeni bir izin talep etmeden önce var olan izin talebinizin sonuçlanması gerekmektedir.", ProjeConstants.MESAJ_HATA, 5000);
                return false;
            }
            if (!ValidateInputs(izinTipi))
            {
                return false;
            }
            #region sut izni
            if (izinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT)
            {
                IzinSuresiLbl.Text = string.Empty;
                KullanilanIzinLbl.Text = string.Empty;
                KalanIzinLbl.Text = string.Empty;
                UyariLbl.Text = string.Empty;
                UcretliIzinDilekceBtn.Visible = false;

                Aile aile = new Aile();

                aile = aile.SelectEnGencCocukByPersonelId(personel.Id);

                if (aile != null)
                {
                    DateTime dogumTarihi = aile.DogumTar;
                    DateTime sonIzinHakkiTarihi = dogumTarihi.AddYears(1);
                    DateTime izinBitisTarihi = string.IsNullOrEmpty(IzinBitTarTxt.Value) ? DateTime.Today : IzinBitTarTxt.Value.ConvertToDatetime();
                    if (izinBitisTarihi <= sonIzinHakkiTarihi)
                    {
                        SaveBtn.Visible = true;
                        return true;
                    }
                    else
                    {
                        UyariLbl.Text = "Yalnızca bir yaşını doldurmamış çocuğu olan personel için süt izni girilebilir. ";
                        return false;
                    }
                }
                else
                {
                    UyariLbl.Text = "Yalnızca bir yaşını doldurmamış çocuğu olan personel için süt izni girilebilir. ";
                    return false;
                }

            }
            #endregion
            #region diger izinler
            else if (izinTipi != ProjeConstants.IZINTIPI_UCRETLI_INT && izinTipi != ProjeConstants.IZINTIPI_MAZERET_INT) // bu iki izin tipi dışındaki izinler için kontrol yapmasın
            {
                IzinSuresiLbl.Text = string.Empty;
                KullanilanIzinLbl.Text = string.Empty;
                KalanIzinLbl.Text = string.Empty;
                UyariLbl.Text = string.Empty;
                UcretliIzinDilekceBtn.Visible = false;
                SaveBtn.Visible = true;
                //SetLayoutByIzinTipi();
                return true;
            }
            #endregion
            #region ucretli ve mazeret  --> !DİKKAT
            else
            {
                bool isValid = false;
                MazereteMahsupDilekceBtn.Visible = false;
                UcretliIzinDilekceBtn.Visible = false;
                //izinDonemini bul
                IzinDonem izinDonemi = new IzinDonem();

                DateTime tarih = DateTime.Now;

                string bastarStr = baslangicTarihi.ConvertToDatetimeEmptyIfNull();
                DateTime kontrolEdilecekTarih = string.IsNullOrEmpty(bastarStr) ? tarih : baslangicTarihi;
                izinDonemi = izinDonemi.SelectByIzinTarihi(personel.Id, izinTipi, kontrolEdilecekTarih);
                if (izinDonemi == null)
                {
                    izinDonemi = new IzinDonem();
                    izinDonemi = izinDonemi.IzinDonemiOlustur(personel, izinTipi, kontrolEdilecekTarih, CurrentUserName);
                }

                if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
                {
                    GecmisDonemlerdenKalanIznLbl.Text=string.Empty;
                    KullanilanIzinLbl.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                        + " İzin döneminde, " + izinDonemi.IzinHakki.ConvertToTimeSpanReturnInHHmm() + " " + ProjeConstants.IZINTIPI_MAZERET + " izninizden kullandığınız izin süresi "
                        + izinDonemi.KullanilanIzin.ConvertToTimeSpanReturnInHHmm()
                        + System.Environment.NewLine;
                    TimeSpan kalanIzinTs = izinDonemi.KalanIzin.ConvertToTimeSpan();

                    DateTime izinTarihi = baslangicTarihi;
                    string basSaat = IzinBasSaatDDL.SelectedItem.Text;
                    baslangicTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, basSaat);
                    string bitSaat = IzinBitSaatDDL.SelectedItem.Text;
                    DateTime bitisTarihi = UtilityHelper.TariheSaatEkle(izinTarihi, bitSaat);

                    string sureStr = IKYSOrtak.IzinSuresiHesapla(izinTipi, baslangicTarihi, bitisTarihi);
                    IzinSuresiLbl.Text = "Kullanmak istediğiniz izin süresi: " + sureStr.ConvertToTimeSpanReturnInHHmm() ;
                    TimeSpan sureTs = sureStr.ConvertToTimeSpan();
                    TimeSpan sonuctaKalanIzinTs = kalanIzinTs - sureTs;
                    TimeSpan sifirTs = new TimeSpan(0, 0, 0);

                    if (kalanIzinTs <= sifirTs)
                    {
                        isValid = false;
                        SaveBtn.Visible = false;
                        KalanIzinLbl.Text = " Kullanabileceğiniz " + ProjeConstants.IZINTIPI_MAZERET + " izniniz bulunmamaktadır.";
                        UyariLbl.Text = " Mazeret izni kullanabilmeniz için 'Yıllık Ücretli İzninizden Mazeret İznine Mahsup' dilekçenizi onaylatarak Personel Kısmına teslim etmeniz gerekmektedir.";
                        IzinDonemIdQS = izinDonemi.Id.ToString();
                        MazereteMahsupDilekceBtn.Visible = true;
                    }
                    else if (sonuctaKalanIzinTs < sifirTs)
                    {
                        isValid = false;
                        SaveBtn.Visible = true;
                        KalanIzinLbl.Text = " Kullanabileceğiniz en fazla " + izinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm() + ProjeConstants.IZINTIPI_MAZERET + " izniniz bulunmaktadır.";
                        UyariLbl.Text = "Daha fazla izin talebinde bulunmak istiyorsanız lütfen 'Yıllık Ücretli İzninizden Mazeret İznine Mahsup' dilekçenizi onaylatarak Personel Kısmına teslim etmeniz gerekmektedir.";
                        int selectedSaat = IzinBasSaatDDL.SelectedItem.Value.ConvertToInt();
                        FillIzinBitSaat();
                        IzinDonemIdQS = izinDonemi.Id.ToString();
                        MazereteMahsupDilekceBtn.Visible = true;
                    }
                    else
                    {
                        isValid = true;
                        SaveBtn.Visible = true;
                        KalanIzinLbl.Text = " Kullanabileceğiniz, " + izinDonemi.KalanIzin.ConvertToTimeSpanReturnInHHmm() + " " + ProjeConstants.IZINTIPI_MAZERET + " izniniz bulunmaktadır.";
                    }
                }
                else if (izinTipi == ProjeConstants.IZINTIPI_UCRETLI_INT)
                {
                    KullanilanIzinLbl.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull() 
                        + " İzin döneminde, " + izinDonemi.IzinHakki.ConvertToInt() + " " + izinDonemi.Birim + " " + ProjeConstants.IZINTIPI_UCRETLI + " izninizden kullandığınız izin süresi : "
                        + izinDonemi.KullanilanIzin.ConvertToInt() + " " + izinDonemi.Birim + "dür. " + System.Environment.NewLine;

                    int kalanIzinInt = izinDonemi.KalanIzin.ConvertToInt();
                    int kalanIzinToplami = KalanIzinToplamıGetir(personel);
                    int gecmisDonemlerdenKalanIzin = kalanIzinToplami - kalanIzinInt;

                    if (kalanIzinInt < 0)
                    {
                        if (gecmisDonemlerdenKalanIzin > 0)
                        {
                           
                            
                            if (gecmisDonemlerdenKalanIzin >= Math.Abs(kalanIzinInt))
                            {
                                KullanilanIzinLbl.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                                   + " İzin dönemine ait izninizin tamamını kullandınız. İlave olarak geçmiş dönemleden kalan izin hakkınızdan karşılanan " + Math.Abs(kalanIzinInt) + " gün ile birlikte, bu izin döneminde kullandığınız izin toplamı "
                                   + izinDonemi.KullanilanIzin.ConvertToInt() + " " + izinDonemi.Birim + "dür. " + System.Environment.NewLine;

                                GecmisDonemlerdenKalanIznLbl.Text = " Geçmiş dönemlerden kalan <strong>" + gecmisDonemlerdenKalanIzin + "</strong> gün izninizin <strong>" + Math.Abs(kalanIzinInt) + "</strong> gününü kullandınız.";
                            }
                            else
                            {
                                KullanilanIzinLbl.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                                   + " İzin dönemine ait izninizin tamamını kullandınız. İlave olarak geçmiş dönemleden kalan izin hakkınızdan karşılanan " + gecmisDonemlerdenKalanIzin + " gün ve fazladan kullandığınız "
                                   + (Math.Abs(kalanIzinInt)- gecmisDonemlerdenKalanIzin) + "gün ile birlikte, bu izin döneminde kullandığınız izin toplamı "
                                   + izinDonemi.KullanilanIzin.ConvertToInt() + " gündür. " + System.Environment.NewLine;
                                
                                GecmisDonemlerdenKalanIznLbl.Text = " Geçmiş dönemlerden kalan <strong>" + gecmisDonemlerdenKalanIzin + "</strong> gün izninizi kullandınız.";
                            }
                        }
                        else
                        {
                            KullanilanIzinLbl.Text = izinDonemi.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + izinDonemi.BitisTarihi.ConvertToDatetimeEmptyIfNull()
                                  + " İzin dönemine ait izninizin tamamını kullandınız. İlave olarak fazladan kullandığınız "
                                  + Math.Abs(kalanIzinInt)  + "gün ile birlikte, bu izin döneminde kullandığınız izin toplamı "
                                  + izinDonemi.KullanilanIzin.ConvertToInt() + " gündür. " + System.Environment.NewLine;
                            
                            GecmisDonemlerdenKalanIznLbl.Text = " Geçmiş Dönemlerden kalan kullanılmamış izniniz bulunmamaktadır.";
                        } 
                    }
                    else
                    {
                        if (gecmisDonemlerdenKalanIzin > 0)
                        {
                            
                                GecmisDonemlerdenKalanIznLbl.Text = " Geçmiş dönemlerden kalan <strong>" + gecmisDonemlerdenKalanIzin + "</strong> gün izniniz bulunmaktadır.";
                         }
                        else
                            GecmisDonemlerdenKalanIznLbl.Text = " Geçmiş Dönemlerden kalan kullanılmamış izniniz bulunmamaktadır.";
                    }
                    
                     

                    DateTime bitisTarihi = IzinBitTarTxt.Value.ConvertToDatetime();
                    string sureStr = IKYSOrtak.IzinSuresiHesapla(izinTipi, baslangicTarihi, bitisTarihi);
                    IzinSuresiLbl.Text = "Kullanmak istediğiniz izin süresi: " + sureStr + " "+ izinDonemi.Birim;
                    int sureInt = sureStr.ConvertToInt();
                    int sonuctaKalanIzinInt = kalanIzinToplami - sureInt;
                    if (kalanIzinToplami <= 0)
                    {
                        isValid = false;
                        SaveBtn.Visible = false;
                        KalanIzinLbl.Text = " Kullanabileceğiniz, " + ProjeConstants.IZINTIPI_UCRETLI + " izniniz bulunmamaktadır.";
                        UyariLbl.Text = " İzin talebinde bulunmadan önce Gelecek Dönem Yıllık Ücretli İzninizden mahsup edilmesi için dilekçenizi onaylatarak Personel Kısmına teslim etmeniz gerekmektedir.";
                        IzinDonemIdQS = izinDonemi.Id.ToString();
                        UcretliIzinDilekceBtn.Visible = true;
                    }
                    else if (sonuctaKalanIzinInt < 0)
                    {
                        //son izin tarihini geçti
                        //kalan izine göre yeni bitis tarihi girip kaydetsin
                        isValid = false;
                        SaveBtn.Visible = false;
                        KalanIzinLbl.Text = " Kullanabileceğiniz, en fazla <strong>" + kalanIzinToplami + "</strong> " + izinDonemi.Birim + " " + ProjeConstants.IZINTIPI_UCRETLI + " izniniz bulunmaktadır.";
                        UyariLbl.Text = " Lütfen izin bitiş tarihini buna göre seçerek tekrar talebinizi gönderiniz.";
                        IzinBitTarTxt.Value = baslangicTarihi.AddDays(kalanIzinInt - 1).ConvertToDatetimeEmptyIfNull();
                    }
                    else
                    {
                        isValid = true;
                        SaveBtn.Visible = true;
                        KalanIzinLbl.Text = " Kullanabileceğiniz toplam " + kalanIzinToplami + " " + izinDonemi.Birim + " " + ProjeConstants.IZINTIPI_UCRETLI + " izniniz bulunmaktadır.";
                    }
                }
                else// diğer izinler
                {
                    isValid = true;
                    SaveBtn.Visible = true;
                    IzinSuresiLbl.Text = string.Empty;
                    KullanilanIzinLbl.Text = string.Empty;
                    KalanIzinLbl.Text = string.Empty;
                }
                if (isValid)
                {
                    UyariLbl.Text = string.Empty;
                    KalanIzinLbl.ForeColor = System.Drawing.Color.Green;
                    KalanIzinLbl.Font.Bold = true;
                }
                else
                {
                    KalanIzinLbl.ForeColor = System.Drawing.Color.Red;
                    KalanIzinLbl.Font.Bold = true;
                }
                return isValid;
            }
            #endregion


        }

        private void ClearIzinBilgileri()
        {
            IzinSuresiLbl.Text = string.Empty;
            KullanilanIzinLbl.Text = string.Empty;
            GecmisDonemlerdenKalanIznLbl.Text = string.Empty;
            KalanIzinLbl.Text = string.Empty;
            UyariLbl.Text = string.Empty;
            KullanilmayanLbl.Text=string.Empty;
            IzinSuresiLbl.Text = string.Empty;
            KullanilanIzinLbl.Text = string.Empty;
            GecmisDonemlerdenKalanIznLbl.Text = string.Empty;
            KalanIzinLbl.Text = string.Empty;
            UyariLbl.Text = string.Empty;
            KullanilmayanLbl.Text = string.Empty;
        }

        private int KalanIzinToplamıGetir(Personel personel)
        {
           
            IzinDonem izinDonemDao=new IzinDonem();
            int kalanIzinToplami = 0;

            DataTable dataTable = izinDonemDao.SelectSUMKalanIzinByPersonelId(personel.Id,false);
            if (dataTable != null)
            {
                DataRow dataRow = dataTable.Rows[0];

                kalanIzinToplami = dataRow["KalanIzinToplami"].ConvertToInt();// - izinDonemi.KalanIzin.ConvertToInt();
            }
            
            return kalanIzinToplami;
        }

        private bool ValidateInputs(int izinTipi)
        {
            bool isValidated = true;
            if (string.IsNullOrEmpty(IzinBasTarTxt.Value))
            {
                isValidated = false;
                MessageHelper.PublishMessage("Lütfen izin başlangıç tarihini giriniz.", ProjeConstants.MESAJ_HATA, 2000);
            }
            else if (izinTipi != ProjeConstants.IZINTIPI_MAZERET_INT)
            {
                if (string.IsNullOrEmpty(IzinBitTarTxt.Value))
                {
                    isValidated = false;
                    MessageHelper.PublishMessage("Lütfen izin bitiş tarihini giriniz.", ProjeConstants.MESAJ_HATA, 2000);

                }

            }
            else if (izinTipi == ProjeConstants.IZINTIPI_MAZERET_INT)
            {

                if (string.IsNullOrEmpty(IzinBasSaatDDL.SelectedValue)
                    || string.IsNullOrEmpty(IzinBitSaatDDL.SelectedValue)
                    )
                {
                    isValidated = false;
                    MessageHelper.PublishMessage("Lütfen izin başlama ve bitiş saatlerini giriniz.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            else if (izinTipi == ProjeConstants.IZINTIPI_SUTIZNI_INT)
            {

                if (string.IsNullOrEmpty(IzinBasSaatDDL.SelectedValue))
                {
                    isValidated = false;
                    MessageHelper.PublishMessage("Lütfen izin başlama saatini giriniz.", ProjeConstants.MESAJ_HATA, 2000);
                }
            }
            return isValidated;
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
        protected void IzinBasSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillIzinBitSaat();
        }
        protected void PersonelDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            Personel personel = new Personel();
            personel = personel.Select<Personel>(PersonelDDL.SelectedItem.Value.ConvertToInt());
            if (personel != null)
            {
                PersonelIdQS = personel.Id.ToString();
               
                PersonelAdiLbl.Text = personel.Adi + " " + personel.Soyadi;
                SetLayoutByIzinTipi();
                TabloyuDoldur();
                SetDefaultVekilAmirOnay();
                KalanIzinKontrolIslemleri();
            }
            else
            {
                MessageHelper.PublishMessage("Personel Bulunamadı!", ProjeConstants.MESAJ_HATA);
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_PERSONEL_LIST;
                Page.Response.Redirect(newUrl);
            }

        }
        protected void MazereteMahsupDilekceBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);

            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(IzinDonemIdQS.ConvertToInt());
            if (izinDonemi != null)
            {

                //TimeSpan kalanIzinTs = izinDonemi.KalanIzin.ConvertToTimeSpan();
                //double kalanIzinSaat = Math.Round(Math.Abs(kalanIzinTs.TotalHours) + 0.5);
                //int mahsupGun = (int)Math.Round((kalanIzinSaat / ProjeConstants.BIRGUN_MAZERET_SAAT_INT + 0.5));
                var fullUrl = string.Format("{0}?IzinDonemId={1}&MahsupGun={2}", rootUrl + ProjeConstants.RAPOR_MAZERETEMAHSUPDILEKCE, IzinDonemIdQS, ProjeConstants.BIRGUN_MAZERETEMAHSUP);
                ResponseHelper.Redirect(fullUrl, "_blank", "");
            }
            else
            {
                MessageHelper.PublishMessage("İzin Dönemi Bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void UcretliIzinDilekceBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);

            IzinDonem izinDonemi = new IzinDonem();
            izinDonemi = izinDonemi.Select<IzinDonem>(IzinDonemIdQS.ConvertToInt());
            if (izinDonemi != null)
            {
                if (ValidateInputs(ProjeConstants.IZINTIPI_UCRETLI_INT))
                {
                    int izinTalepId = YeniTalebiKaydet(ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR);

                    DateTime izinBasTar = IzinBasTarTxt.Value.ConvertToDatetime();
                    DateTime izinBitTar = IzinBitTarTxt.Value.ConvertToDatetime();

                    int yil = izinDonemi.BaslangicTarihi.Year + 1;
                    int ay = izinDonemi.BaslangicTarihi.Month;
                    int gun = izinDonemi.BaslangicTarihi.Day;
                    DateTime gelecekIzinDonemiBasi = new DateTime(yil, ay, gun);
                    string sure = IKYSOrtak.IzinSuresiHesapla(ProjeConstants.IZINTIPI_UCRETLI_INT, izinBasTar, izinBitTar);
                    var fullUrl = string.Format("{0}?PersonelId={1}&GelecekDonem={2}&IzinBasTar={3}&IzinBitTar={4}&Sure={5}&AmirId={6}&OnaylayanId={7}",
                        rootUrl + ProjeConstants.RAPOR_UCRETLIMAHSUPDILEKCE, PersonelIdQS, gelecekIzinDonemiBasi.ConvertToDatetimeEmptyIfNull(),
                        izinBasTar.ConvertToDatetimeEmptyIfNull(), izinBitTar.ConvertToDatetimeEmptyIfNull(), sure, AmirImzaDDL.SelectedItem.Value, OnayImzaDDL.SelectedItem.Value);
                    ResponseHelper.Redirect(fullUrl, "_blank", "");

                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin Dönemi Bulunamadı.", ProjeConstants.MESAJ_HATA);
            }
        }

        protected void TarihKontrolBtn_Click(object sender, EventArgs e)
        {
            if (IzinBitTarTxt.Value.ConvertToDatetime() < IzinBasTarTxt.Value.ConvertToDatetime())
            {
                IzinBitTarTxt.Value = IzinBasTarTxt.Value;
            }
            KalanIzinKontrolIslemleri();
        }
        protected void IzinBitSaatDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            KalanIzinKontrolIslemleri();
        }
        protected void SilBtn1_Click(object sender, EventArgs e)
        {
            IzinTalep silinecekIzinTalebi = new IzinTalep();
            silinecekIzinTalebi = silinecekIzinTalebi.Select<IzinTalep>(IzinTalepIdHdn1.Value.ConvertToInt());
            if (silinecekIzinTalebi != null)
            {
                if (silinecekIzinTalebi.OnayDurumu==ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR || 
                    silinecekIzinTalebi.OnayDurumu==ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR)
                {
                    bool isdeleted = silinecekIzinTalebi.Delete();
                    if (isdeleted)
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                        MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    MessageHelper.PublishMessage("Talebinizin onay durumu değiştiğinden silinemedi.", ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
            }

        }
        protected void SilBtn2_Click(object sender, EventArgs e)
        {
            IzinTalep silinecekIzinTalebi = new IzinTalep();
            silinecekIzinTalebi = silinecekIzinTalebi.Select<IzinTalep>(IzinTalepIdHdn2.Value.ConvertToInt());
            if (silinecekIzinTalebi != null)
            {
                if (silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR ||
                    silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR)
                {
                    bool isdeleted = silinecekIzinTalebi.Delete();
                    if (isdeleted)
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                        MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    MessageHelper.PublishMessage("Talebinizin onay durumu değiştiğinden silinemedi.", ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
            }

        }
        protected void SilBtn3_Click(object sender, EventArgs e)
        {
            IzinTalep silinecekIzinTalebi = new IzinTalep();
            silinecekIzinTalebi = silinecekIzinTalebi.Select<IzinTalep>(IzinTalepIdHdn3.Value.ConvertToInt());
            if (silinecekIzinTalebi != null)
            {
                if (silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR ||
                    silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR)
                {
                    bool isdeleted = silinecekIzinTalebi.Delete();
                    if (isdeleted)
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                        MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    MessageHelper.PublishMessage("Talebinizin onay durumu değiştiğinden silinemedi.", ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
            }

        }
        protected void SilBtn4_Click(object sender, EventArgs e)
        {
            IzinTalep silinecekIzinTalebi = new IzinTalep();
            silinecekIzinTalebi = silinecekIzinTalebi.Select<IzinTalep>(IzinTalepIdHdn4.Value.ConvertToInt());
            if (silinecekIzinTalebi != null)
            {
                if (silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR ||
                    silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR)
                {
                    bool isdeleted = silinecekIzinTalebi.Delete();
                    if (isdeleted)
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                        MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    MessageHelper.PublishMessage("Talebinizin onay durumu değiştiğinden silinemedi.", ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
            }

        }
        protected void SilBtn5_Click(object sender, EventArgs e)
        {
            IzinTalep silinecekIzinTalebi = new IzinTalep();
            silinecekIzinTalebi = silinecekIzinTalebi.Select<IzinTalep>(IzinTalepIdHdn5.Value.ConvertToInt());
            if (silinecekIzinTalebi != null)
            {
                if (silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_ISLEMBEKLIYOR ||
                    silinecekIzinTalebi.OnayDurumu == ProjeConstants.PER_IZINTALEBI_DILEKCEBEKLIYOR)
                {
                    bool isdeleted = silinecekIzinTalebi.Delete();
                    if (isdeleted)
                    {
                        TabloyuDoldur();
                        MessageHelper.PublishMessage("İzin talebi silindi", ProjeConstants.MESAJ_BASARILI, 2000);
                    }
                    else
                        MessageHelper.PublishMessage("İzin talebi silinemedi ", ProjeConstants.MESAJ_HATA);
                }
                else
                {
                    MessageHelper.PublishMessage("Talebinizin onay durumu değiştiğinden silinemedi.", ProjeConstants.MESAJ_BILGI);
                }
            }
            else
            {
                MessageHelper.PublishMessage("İzin talebi bulunamadı, talebiniz zaten silinmiş olabilir. Lütfen kontolediniz.", ProjeConstants.MESAJ_BILGI);
            }

        }
    }
}
