using Model.Ortak;
using Model.TBYS;
using System;
using Utility.Logging;
using TBYS_WebParts.Logging;
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
        private PortalLogContext LoggingContext
        {
            get
            {
                return PortalLogContext.FromCurrentRequest("TBYS", "YasalFaizWP");
            }
        }

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public YasalFaizWP()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack) TbysWebPartLog.RecordOpened(LoggingContext);
            try
            {
                base.OnLoad(e);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception error)
            {
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.YasalFaizWP.Page_Load", error);
            }
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
                int yil = YilDDL.SelectedItem.Value.ConvertToInt();
                if (yil > 0)
                    new Model.Services.TBYS.YasalFaizService(LoggingContext).CreateYear(yil);
            }
            catch (Exception tbysError)
            {
                if (tbysError is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.YasalFaizWP.BuYilinTablosunuOlustur", tbysError);

                TbysWebPartLog.ReportError(LoggingContext, "TBYS.YasalFaizWP.BuYilinTablosunuOlustur", tbysError);
            }
        }
        private bool BuYilinTablosuVarMi()
        {
            bool buYilinTablosuVarMi = false;
            int yil = YilDDL.SelectedItem.Value.ConvertToInt();
            YasalFaiz yasalFaizDao = new YasalFaiz();
            List<YasalFaiz> list = new Model.Services.TBYS.YasalFaizService(LoggingContext).GetByYear(yil);
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
            catch (Exception tbysError)
            {
                if (tbysError is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.YasalFaizWP.SetDDLValues", tbysError);

                //TODO
            }
        }
        private void FaizOraniTablosunuDoldur()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            int buyil = YilDDL.SelectedItem.Value.ConvertToInt();
            YasalFaiz yasalFaizDao = new YasalFaiz();
            List<YasalFaiz> list = new Model.Services.TBYS.YasalFaizService(LoggingContext).GetByYear(buyil);
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
        //        faizOraniTxt.CssClass = "form-control input-money text-end";
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
        //                        if (new Model.Services.TBYS.YasalFaizService().Update(yasalFaiz))
        //                        {
        //                            MessageHelper.PublishMessage("Kayit Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
        //                            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
        //                            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
        //                            newUrl += "/" + ProjeConstants.PAGE_YASALFAIZ;
        //                            Page.Response.Redirect(newUrl, true);
        //                        }
        //                    }
        //                }
        //                catch (Exception)
        //                {

        //                    MessageHelper.PublishMessage("Faiz Oranlari Kaydedilemedi", ProjeConstants.MESAJ_HATA);
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
                if (ex is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.WriteError(LoggingContext, "TBYS.YasalFaizWP.RedirectToPage", ex);
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.YasalFaizWP.RedirectToPage", ex);
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
                List<YasalFaiz> edits = new List<YasalFaiz>();
                for (int month = 1; month <= 12; month++) edits.Add(ReadMonthValues(month));
                new Model.Services.TBYS.YasalFaizService(LoggingContext).UpdateMonths(edits);
                MessageHelper.PublishMessage("Kayıtlar güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception error)
            {
                if (error is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.YasalFaizWP.HepsiniKaydetBtn_Click", error);
            }
        }

        protected void Kaydet1Btn_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton button = sender as LinkButton;
                int month;
                if (button == null || !button.ID.StartsWith("Kaydet", StringComparison.Ordinal)
                    || !button.ID.EndsWith("Btn", StringComparison.Ordinal) || button.ID.Length < 10
                    || !int.TryParse(button.ID.Substring(6, button.ID.Length - 9), out month) || month < 1 || month > 12)
                    throw new InvalidOperationException("Güncellenecek ay belirlenemedi.");
                new Model.Services.TBYS.YasalFaizService(LoggingContext)
                    .UpdateMonths(new List<YasalFaiz> { ReadMonthValues(month) });
                MessageHelper.PublishMessage("Kayıt güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception error)
            {
                if (error is System.Threading.ThreadAbortException) throw;
                TbysWebPartLog.ReportError(LoggingContext, "TBYS.YasalFaizWP.Kaydet1Btn_Click", error);
            }
        }

        private YasalFaiz ReadMonthValues(int month)
        {
            return new YasalFaiz
            {
                Yil = YilDDL.SelectedItem.Value.ConvertToInt(),
                Ay = month,
                FaizOrani = ((TextBox)FindControl("FaizOrani" + month + "Txt")).Text.ConvertToDecimal(),
                Tufe = ((TextBox)FindControl("Tufe" + month + "Txt")).Text.ConvertToDecimal(),
                Ufe = ((TextBox)FindControl("Ufe" + month + "Txt")).Text.ConvertToDecimal(),
                Aciklama = ((TextBox)FindControl("Aciklama" + month + "Txt")).Text
            };
        }
    }
}
