using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;


namespace TBYS_WebParts.YasalFaizWP
{
    [ToolboxItemAttribute(false)]
    public partial class YasalFaizWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YasalFaizWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
            if (!Page.IsPostBack)
            {
                YilDDLDoldur();
                SetDDLValues();
                if (!BuYilinTablosuVarMi())
                {
                    BuYilinTablosunuOlustur();
                }
                //FaizOraniTxt.Text= ProjeConstants.YASAL_FAIZ_ORANI.ToString();
                FaizOraniTablosunuDoldur();
            }
        }
        private void BuYilinTablosunuOlustur()
        {
            try
            {
                IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
                int yil = YilDDL.SelectedItem.Value.ConvertToInt();
                if (yil > 0)
                {
                    YasalFaiz yasalFaizDbo = new YasalFaiz();
                    decimal sonFaizOrani = yasalFaizDbo.SelectSonFaizOrani();
                    decimal sonTufe = yasalFaizDbo.SelectSonTufe();
                    decimal sonUfe = yasalFaizDbo.SelectSonUfe();
                    for (int i = 1; i < 13; i++)
                    {
                        YasalFaiz yasalFaiz = new YasalFaiz();
                        yasalFaiz.Ay = i;
                        DateTime tarih = new DateTime(yil, yasalFaiz.Ay, 1);
                        yasalFaiz.AyAdi = tarih.ToString("MMMM", culturInfo);
                        yasalFaiz.Yil = yil;
                        yasalFaiz.FaizOrani = sonFaizOrani;// FaizOraniTxt.Text.ConvertToDecimal();
                        yasalFaiz.Tufe = sonTufe;// TufeTxt.Text.ConvertToDecimal();
                        yasalFaiz.Ufe = sonUfe;// UfeTxt.Text.ConvertToDecimal();
                        yasalFaiz.Aciklama = "";// yil + " yılı " + yasalFaiz.AyAdi + " faiz oranı";
                        yasalFaiz.Save();
                    }

                }
            }
            catch (Exception)
            {

                MessageHelper.PublishMessage("Faiz oranları oluşturulamadı", ProjeConstants.MESAJ_HATA);
            }
        }
        private bool BuYilinTablosuVarMi()
        {
            bool buYilinTablosuVarMi = false;
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            YasalFaiz yasalFaizDao = new YasalFaiz();
            List<YasalFaiz> list = yasalFaizDao.SelectByYil(yil);
            if (list.Count > 0)
            {
                buYilinTablosuVarMi = true;
            }
            return buYilinTablosuVarMi;
        }
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            for (int i = 1987; i <= year + 4; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        private void SetDDLValues()
        {
            try
            {
                //yil
                string yil = !string.IsNullOrEmpty(SecilenYilQS) ? SecilenYilQS : DateTime.Today.Year.ReturnEmptyIfNull().ToString();
                ListItem YilItem = new ListItem();
                if (!string.IsNullOrEmpty(yil))
                    YilItem = YilDDL.Items.FindByValue(yil);

                if (YilItem != null)
                {
                    YilDDL.SelectedValue = YilItem.Value;
                    SecilenYilQS = YilItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }
        }
        private void FaizOraniTablosunuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int buyil = YilDDL.SelectedItem.Value.ConvertToInt();
            YasalFaiz yasalFaizDao = new YasalFaiz();
            List<YasalFaiz> list = yasalFaizDao.SelectByYil(buyil);
            if (list.Count > 0)
            {
                int sira = 1;
                YasalFaiz yasalFaiz = list[0];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira1Cell.Text = sira++.ToString();
                    Ay1Cell.Text = ayAdi;
                    Yil1Cell.Text = yil;
                    FaizOrani1Txt.Text = faizOrani;
                    Tufe1Txt.Text = tufe;
                    Ufe1Txt.Text = ufe;
                    Aciklama1Txt.Text = aciklama;
                }
                yasalFaiz = list[1];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira2Cell.Text = sira++.ToString();
                    Ay2Cell.Text = ayAdi;
                    Yil2Cell.Text = yil;
                    FaizOrani2Txt.Text = faizOrani;
                    Tufe2Txt.Text = tufe;
                    Ufe2Txt.Text = ufe;
                    Aciklama2Txt.Text = aciklama;
                }
                yasalFaiz = list[2];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira3Cell.Text = sira++.ToString();
                    Ay3Cell.Text = ayAdi;
                    Yil3Cell.Text = yil;
                    FaizOrani3Txt.Text = faizOrani;
                    Tufe3Txt.Text = tufe;
                    Ufe3Txt.Text = ufe;
                    Aciklama3Txt.Text = aciklama;
                }
                yasalFaiz = list[3];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira4Cell.Text = sira++.ToString();
                    Ay4Cell.Text = ayAdi;
                    Yil4Cell.Text = yil;
                    FaizOrani4Txt.Text = faizOrani;
                    Tufe4Txt.Text = tufe;
                    Ufe4Txt.Text = ufe;
                    Aciklama4Txt.Text = aciklama;
                }
                yasalFaiz = list[4];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira5Cell.Text = sira++.ToString();
                    Ay5Cell.Text = ayAdi;
                    Yil5Cell.Text = yil;
                    FaizOrani5Txt.Text = faizOrani;
                    Tufe5Txt.Text = tufe;
                    Ufe5Txt.Text = ufe;
                    Aciklama5Txt.Text = aciklama;
                }
                yasalFaiz = list[5];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira6Cell.Text = sira++.ToString();
                    Ay6Cell.Text = ayAdi;
                    Yil6Cell.Text = yil;
                    FaizOrani6Txt.Text = faizOrani;
                    Tufe6Txt.Text = tufe;
                    Ufe6Txt.Text = ufe;
                    Aciklama6Txt.Text = aciklama;
                }
                yasalFaiz = list[6];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira7Cell.Text = sira++.ToString();
                    Ay7Cell.Text = ayAdi;
                    Yil7Cell.Text = yil;
                    FaizOrani7Txt.Text = faizOrani;
                    Tufe7Txt.Text = tufe;
                    Ufe7Txt.Text = ufe;
                    Aciklama7Txt.Text = aciklama;
                }
                yasalFaiz = list[7];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira8Cell.Text = sira++.ToString();
                    Ay8Cell.Text = ayAdi;
                    Yil8Cell.Text = yil;
                    FaizOrani8Txt.Text = faizOrani;
                    Tufe8Txt.Text = tufe;
                    Ufe8Txt.Text = ufe;
                    Aciklama8Txt.Text = aciklama;
                }
                yasalFaiz = list[8];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira9Cell.Text = sira++.ToString();
                    Ay9Cell.Text = ayAdi;
                    Yil9Cell.Text = yil;
                    FaizOrani9Txt.Text = faizOrani;
                    Tufe9Txt.Text = tufe;
                    Ufe9Txt.Text = ufe;
                    Aciklama9Txt.Text = aciklama;
                }
                yasalFaiz = list[9];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira10Cell.Text = sira++.ToString();
                    Ay10Cell.Text = ayAdi;
                    Yil10Cell.Text = yil;
                    FaizOrani10Txt.Text = faizOrani;
                    Tufe10Txt.Text = tufe;
                    Ufe10Txt.Text = ufe;
                    Aciklama10Txt.Text = aciklama;
                }
                yasalFaiz = list[10];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira11Cell.Text = sira++.ToString();
                    Ay11Cell.Text = ayAdi;
                    Yil11Cell.Text = yil;
                    FaizOrani11Txt.Text = faizOrani;
                    Tufe11Txt.Text = tufe;
                    Ufe11Txt.Text = ufe;
                    Aciklama11Txt.Text = aciklama;
                }
                yasalFaiz = list[11];
                if (yasalFaiz != null)
                {
                    string yil = yasalFaiz.Yil.ToString();
                    int ay = yasalFaiz.Ay;
                    string ayAdi = yasalFaiz.AyAdi;
                    string faizOrani = yasalFaiz.FaizOrani.ToString();
                    string tufe = yasalFaiz.Tufe.ToString();
                    string ufe = yasalFaiz.Ufe.ToString();
                    string aciklama = yasalFaiz.Aciklama;

                    Sira12Cell.Text = sira++.ToString();
                    Ay12Cell.Text = ayAdi;
                    Yil12Cell.Text = yil;
                    FaizOrani12Txt.Text = faizOrani;
                    Tufe12Txt.Text = tufe;
                    Ufe12Txt.Text = ufe;
                    Aciklama12Txt.Text = aciklama;
                }
            }
        }
        //private void FaizOraniTablosunuDoldur(decimal forani)
        //{
        //    IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        //    int buyil = YilDDL.SelectedItem.Value.ConvertToInt();
        //    YasalFaiz yasalFaizDao = new YasalFaiz();
        //    List<YasalFaiz> list = yasalFaizDao.selectByYil(buyil);
        //    int sira = 1;
        //    foreach (YasalFaiz yasalFaiz in list)
        //    {

        //        int yil = yasalFaiz.Yil;
        //        int ay = yasalFaiz.Ay;
        //        string ayAdi = yasalFaiz.AyAdi;
        //        decimal faizOrani = forani < 0 ? yasalFaiz.FaizOrani:forani;
        //        string aciklama = yasalFaiz.Aciklama;
        //        TableRow row = new TableRow();

        //        TableCell AyCell = new TableCell();
        //        AyCell.Text = ay.ToString();

        //        TableCell AyAdiCell = new TableCell();
        //        AyAdiCell.Text = ayAdi.ToString();

        //        TableCell YilCell = new TableCell();
        //        YilCell.Text = yil.ToString();

        //        TableCell FaizOraniCell = new TableCell();
        //        TextBox faizOraniTxt = new TextBox();
        //        faizOraniTxt.Text= faizOrani.ToString();
        //        faizOraniTxt.CssClass = "form-control input-money text-right";
        //        FaizOraniCell.Controls.Add(faizOraniTxt); 

        //        TableCell AciklamaCell = new TableCell();
        //        AciklamaCell.Text = faizOrani.ToString();

        //        TableCell GüncelleCell = new TableCell();
        //        {
        //            Button GuncelleBtn = new Button();
        //            GuncelleBtn.ID = "SaveBtn" + sira++;
        //            GuncelleBtn.Text = "Kaydet";
        //            GuncelleBtn.CssClass = "form-control btn btn-outline-success";
        //            GuncelleBtn.Click += delegate
        //            {
        //                try
        //                {
        //                    for (int i = 1; i < 13; i++)
        //                    {
        //                        yasalFaiz.FaizOrani = faizOrani.ConvertToDecimal();
        //                        yasalFaiz.Degistiren = CurrentUserName;
        //                        if (yasalFaiz.Update())
        //                        {
        //                            MessageHelper.PublishMessage("Kayıt Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
        //                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
        //                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
        //                            newUrl += "/" + ProjeConstants.PAGE_YASALFAIZ;
        //                            Page.Response.Redirect(newUrl, true);
        //                        }
        //                    }
        //                }
        //                catch (Exception)
        //                {

        //                    MessageHelper.PublishMessage("Faiz Oranları Kaydedilemedi", ProjeConstants.MESAJ_HATA);
        //                }


        //            };
        //            GüncelleCell.Controls.Add(GuncelleBtn);

        //            row.Controls.Add(AyCell);
        //            row.Controls.Add(AyAdiCell);
        //            row.Controls.Add(YilCell);
        //            row.Controls.Add(FaizOraniCell);
        //            row.Controls.Add(GüncelleCell);
        //            AyrintiTable.Controls.Add(row);
        //        }
        //    }
        //}
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
            RedirectToPage(ProjeConstants.PAGE_HOME);
        }
        protected void OdemePlaniListBtn_Click(object sender, EventArgs e)
        {
            RedirectToPage(ProjeConstants.PAGE_ODEMEPLANI_LIST);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value;
            RedirectToPage(ProjeConstants.PAGE_YASALFAIZ + "?SecilenYil=" + SecilenYilQS);
        }

        protected void HepsiniKaydetBtn_Click(object sender, EventArgs e)
        {

            try
            {

                for (int i = 1; i < 13; i++)
                {
                    string ay = AyrintiTable.Rows[i].Cells[0].Text;
                    string yil = AyrintiTable.Rows[i].Cells[2].Text;
                    string faizOraniTxtStr = "FaizOrani" + i + "Txt";
                    string tufeTxtStr = "Tufe" + i + "Txt";
                    string ufeTxtStr = "Ufe" + i + "Txt";
                    string faizOranix = ((TextBox)this.FindControl(faizOraniTxtStr)).Text;
                    string tufex = ((TextBox)this.FindControl(tufeTxtStr)).Text;
                    string ufex = ((TextBox)this.FindControl(ufeTxtStr)).Text;
                    string aciklamaTxtStr = "Aciklama" + i + "Txt";
                    string aciklama = ((TextBox)this.FindControl(aciklamaTxtStr)).Text;
                    YasalFaiz yasalFaiz = new YasalFaiz();
                    yasalFaiz = yasalFaiz.SelectByYilAy(yil.ConvertToInt(), ay.ConvertToInt());
                    yasalFaiz.FaizOrani = faizOranix.ConvertToDecimal();
                    yasalFaiz.Tufe = tufex.ConvertToDecimal();
                    yasalFaiz.Ufe = ufex.ConvertToDecimal();
                    yasalFaiz.Degistiren = CurrentUserName;
                    yasalFaiz.Aciklama = aciklama;
                    yasalFaiz.Update();
                }
            }
            catch (Exception)
            {

                MessageHelper.PublishMessage("Faiz Oranları Kaydedilemedi", ProjeConstants.MESAJ_HATA);
            }
        }
        protected void Kaydet1Btn_Click(object sender, EventArgs e)
        {
            int button = 0;

            YasalFaiz yasalFaizDbo = new YasalFaiz();
            decimal sonFaizOrani = yasalFaizDbo.SelectSonFaizOrani();
            decimal sonTufe = yasalFaizDbo.SelectSonTufe();
            decimal sonUfe = yasalFaizDbo.SelectSonUfe();

            string faizOrani = sonFaizOrani.ToString();
            string tufe = sonTufe.ToString();
            string ufe = sonUfe.ToString();

            LinkButton senderBtn = sender as LinkButton;
            switch (senderBtn.ID.ToString())
            {
                case "Kaydet1Btn":
                    button = 1;
                    faizOrani = FaizOrani1Txt.Text;
                    tufe = Tufe1Txt.Text;
                    ufe = Ufe1Txt.Text;
                    break;
                case "Kaydet2Btn":
                    button = 2;
                    faizOrani = FaizOrani2Txt.Text;
                    tufe = Tufe2Txt.Text;
                    ufe = Ufe2Txt.Text;
                    break;
                case "Kaydet3Btn":
                    button = 3;
                    faizOrani = FaizOrani3Txt.Text;
                    tufe = Tufe3Txt.Text;
                    ufe = Ufe3Txt.Text;
                    break;
                case "Kaydet4Btn":
                    button = 4;
                    faizOrani = FaizOrani4Txt.Text;
                    tufe = Tufe4Txt.Text;
                    ufe = Ufe4Txt.Text;
                    break;
                case "Kaydet5Btn":
                    button = 5;
                    faizOrani = FaizOrani5Txt.Text;
                    tufe = Tufe5Txt.Text;
                    ufe = Ufe5Txt.Text;
                    break;
                case "Kaydet6Btn":
                    button = 6;
                    faizOrani = FaizOrani6Txt.Text;
                    tufe = Tufe6Txt.Text;
                    ufe = Ufe6Txt.Text;
                    break;
                case "Kaydet7Btn":
                    button = 7;
                    faizOrani = FaizOrani7Txt.Text;
                    tufe = Tufe7Txt.Text;
                    ufe = Ufe7Txt.Text;
                    break;
                case "Kaydet8Btn":
                    button = 8;
                    faizOrani = FaizOrani8Txt.Text;
                    tufe = Tufe8Txt.Text;
                    ufe = Ufe8Txt.Text;
                    break;
                case "Kaydet9Btn":
                    button = 9;
                    faizOrani = FaizOrani9Txt.Text;
                    tufe = Tufe9Txt.Text;
                    ufe = Ufe9Txt.Text;
                    break;
                case "Kaydet10Btn":
                    button = 10;
                    faizOrani = FaizOrani10Txt.Text;
                    tufe = Tufe10Txt.Text;
                    ufe = Ufe10Txt.Text;
                    break;
                case "Kaydet11Btn":
                    button = 11;
                    faizOrani = FaizOrani11Txt.Text;
                    tufe = Tufe11Txt.Text;
                    ufe = Ufe11Txt.Text;
                    break;
                case "Kaydet12Btn":
                    button = 12;
                    faizOrani = FaizOrani12Txt.Text;
                    tufe = Tufe12Txt.Text;
                    ufe = Ufe12Txt.Text;
                    break;
            }

            try
            {
                bool kaydedildi = false;
                if (button == 0)
                {
                    for (int i = 1; i < 13; i++)
                    {
                        string ay = AyrintiTable.Rows[i].Cells[0].Text;
                        string yil = AyrintiTable.Rows[i].Cells[2].Text;
                        //string faizOrani = AyrintiTable.Rows[i].Cells[3].Text;
                        string faizOraniTxtStr = "FaizOrani" + i + "Txt";
                        string tufeTxtStr = "Tufe" + i + "Txt";
                        string ufeTxtStr = "Ufe" + i + "Txt";
                        string faizOranix = ((TextBox)this.FindControl(faizOraniTxtStr)).Text;
                        string tufex = ((TextBox)this.FindControl(faizOraniTxtStr)).Text;
                        string ufex = ((TextBox)this.FindControl(faizOraniTxtStr)).Text;


                        string aciklamaTxtStr = "Aciklama" + i + "Txt";
                        string aciklama = ((TextBox)this.FindControl(aciklamaTxtStr)).Text;
                        YasalFaiz yasalFaiz = new YasalFaiz();
                        yasalFaiz = yasalFaiz.SelectByYilAy(yil.ConvertToInt(), ay.ConvertToInt());
                        yasalFaiz.FaizOrani = faizOranix.ConvertToDecimal();
                        yasalFaiz.Tufe = tufex.ConvertToDecimal();
                        yasalFaiz.Ufe = ufex.ConvertToDecimal();
                        yasalFaiz.Degistiren = CurrentUserName;
                        yasalFaiz.Aciklama = aciklama;
                        yasalFaiz.Update();
                    }
                }
                else
                {
                    string ay = AyrintiTable.Rows[button].Cells[0].Text;
                    string yil = AyrintiTable.Rows[button].Cells[2].Text;
                    //string faizOrani = AyrintiTable.Rows[i].Cells[3].Text;
                    string faizOraniTxtStr = "FaizOrani" + button + "Txt";
                    string tufeTxtStr = "Tufe" + button + "Txt";
                    string ufeTxtStr = "Ufe" + button + "Txt";

                    string aciklamaTxtStr = "Aciklama" + button + "Txt";
                    string aciklama = ((TextBox)this.FindControl(aciklamaTxtStr)).Text;

                    //string faizOrani = ((TextBox)this.FindControl(tboxname)).Text;
                    YasalFaiz yasalFaiz = new YasalFaiz();
                    yasalFaiz = yasalFaiz.SelectByYilAy(yil.ConvertToInt(), ay.ConvertToInt());
                    yasalFaiz.FaizOrani = faizOrani.ConvertToDecimal();
                    yasalFaiz.Tufe = tufe.ConvertToDecimal();
                    yasalFaiz.Ufe = ufe.ConvertToDecimal();
                    yasalFaiz.Aciklama = aciklama;
                    yasalFaiz.Degistiren = CurrentUserName;
                    kaydedildi = yasalFaiz.Update();
                }
                if (kaydedildi)
                {
                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
                }

            }
            catch (Exception)
            {

                MessageHelper.PublishMessage("Faiz Oranları Kaydedilemedi", ProjeConstants.MESAJ_HATA);
            }
        }
    }
}
