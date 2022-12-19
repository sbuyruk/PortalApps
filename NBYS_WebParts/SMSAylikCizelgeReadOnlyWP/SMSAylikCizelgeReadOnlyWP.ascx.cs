using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.SMSAylikCizelgeReadOnlyWP
{
    [ToolboxItemAttribute(false)]
    public partial class SMSAylikCizelgeReadOnlyWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SMSAylikCizelgeReadOnlyWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
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
                        ViewState["SecilenYil"] = DateTime.Today.Year;
                    }
                }
                return ViewState["SecilenYil"].ToString();
            }

            set
            {
                ViewState["SecilenYil"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!Page.IsPostBack)
                {
                    YilDDLDoldur();
                    SetDDLValues();
                }

                SecilenYilVerileriniOlustur();
                CizelgeyiDoldur(SecilenYilQS.ConvertToInt());
            }
            catch (Exception ex)
            {
                ExceptionHelper eh = new ExceptionHelper(ex);
                eh.PublishException();
            }
        }

        private void SecilenYilVerileriniOlustur()
        {
            SMSAylikBagis sms = new SMSAylikBagis();
            List<SMSAylikBagis> list = sms.SelectByYilReturnList(SecilenYilQS.ConvertToInt());
            if (list.Count < 1)
            {
                for (int i = 1; i < 13; i++)
                {
                    SMSAylikBagis smsAylikBagis = new SMSAylikBagis();
                    smsAylikBagis.Ay = i;
                    smsAylikBagis.DovizCinsi = "TL";
                    smsAylikBagis.IslemTarihi = DateTime.Now;
                    smsAylikBagis.Olusturan = CurrentUserName;
                    smsAylikBagis.SMSTutari = 10;
                    smsAylikBagis.TurkcellSMSAdedi = 0;
                    smsAylikBagis.VodafoneSMSAdedi = 0;
                    smsAylikBagis.TurkTelekomSMSAdedi = 0;
                    smsAylikBagis.Yil = SecilenYilQS.ConvertToInt();
                    smsAylikBagis.Aciklama = "";
                    smsAylikBagis.Save();
                }
            }
        }

        private void CizelgeyiDoldur(int yil)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            SMSAylikBagis smsAylikBagisDao = new SMSAylikBagis();
            List<SMSAylikBagis> list = smsAylikBagisDao.SelectByYilReturnList(yil);

            foreach (SMSAylikBagis item in list)
            {
                TableRow row = new TableRow();

                TableCell AyCell = new TableCell();
                AyCell.Text = new DateTime(SecilenYilQS.ConvertToInt(), item.Ay, 1).ToString("MMMM", culturInfo);

                TableCell TurkcellSMSAdediCell = new TableCell();
                TurkcellSMSAdediCell.Text = item.TurkcellSMSAdedi.ToString();
                TurkcellSMSAdediCell.CssClass = "text-right";
                TableCell TurkcellSMSTutariCell = new TableCell();
                TurkcellSMSTutariCell.Text = (item.TurkcellSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkcellSMSTutariCell.CssClass = "text-right";

                TableCell VodafoneSMSAdediCell = new TableCell();
                VodafoneSMSAdediCell.Text = item.VodafoneSMSAdedi.ToString();
                VodafoneSMSAdediCell.CssClass = "text-right";
                TableCell VodafoneSMSTutariCell = new TableCell();
                VodafoneSMSTutariCell.Text = (item.VodafoneSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                VodafoneSMSTutariCell.CssClass = "text-right";

                TableCell TurkTelekomSMSAdediCell = new TableCell();
                TurkTelekomSMSAdediCell.Text = item.TurkTelekomSMSAdedi.ToString();
                TurkTelekomSMSAdediCell.CssClass = "text-right";
                TableCell TurkTelekomSMSTutariCell = new TableCell();
                TurkTelekomSMSTutariCell.Text = (item.TurkTelekomSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkTelekomSMSTutariCell.CssClass = "text-right";

                int toplamSMSAdedi = item.TurkcellSMSAdedi + item.VodafoneSMSAdedi + item.TurkTelekomSMSAdedi;
                TableCell ToplamSMSAdediCell = new TableCell();
                ToplamSMSAdediCell.Text = toplamSMSAdedi.ToString();
                ToplamSMSAdediCell.CssClass = "text-right";
                TableCell ToplamSMSTutariCell = new TableCell();
                ToplamSMSTutariCell.Text = (toplamSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                ToplamSMSTutariCell.CssClass = "text-right";


                row.Controls.Add(AyCell);
                row.Controls.Add(TurkcellSMSAdediCell);
                row.Controls.Add(TurkcellSMSTutariCell);
                row.Controls.Add(VodafoneSMSAdediCell);
                row.Controls.Add(VodafoneSMSTutariCell);
                row.Controls.Add(TurkTelekomSMSAdediCell);
                row.Controls.Add(TurkTelekomSMSTutariCell);
                row.Controls.Add(ToplamSMSAdediCell);
                row.Controls.Add(ToplamSMSTutariCell);
                AyrintiTable.Controls.Add(row);
            }
        }


        /// <summary>
        /// LinkButton Düzenle excele aktarırken hata verdiğinden 
        /// CizelgeyiDoldur() metodu ile aynı işlemi Linkbutton olmadan yapıyor
        /// </summary>
        /// <param name="yil"></param>
        private void ExcelIcinCizelgeyiDoldur(int yil)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            SMSAylikBagis smsAylikBagisDao = new SMSAylikBagis();
            List<SMSAylikBagis> list = smsAylikBagisDao.SelectByYilReturnList(yil);

            foreach (SMSAylikBagis item in list)
            {
                TableRow row = new TableRow();

                TableCell AyCell = new TableCell();
                AyCell.Text = new DateTime(SecilenYilQS.ConvertToInt(), item.Ay, 1).ToString("MMMM", culturInfo);

                TableCell TurkcellSMSAdediCell = new TableCell();
                TurkcellSMSAdediCell.Text = item.TurkcellSMSAdedi.ToString();
                TurkcellSMSAdediCell.CssClass = "text-right";
                TableCell TurkcellSMSTutariCell = new TableCell();
                TurkcellSMSTutariCell.Text = (item.TurkcellSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkcellSMSTutariCell.CssClass = "text-right";

                TableCell VodafoneSMSAdediCell = new TableCell();
                VodafoneSMSAdediCell.Text = item.VodafoneSMSAdedi.ToString();
                VodafoneSMSAdediCell.CssClass = "text-right";
                TableCell VodafoneSMSTutariCell = new TableCell();
                VodafoneSMSTutariCell.Text = (item.VodafoneSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                VodafoneSMSTutariCell.CssClass = "text-right";

                TableCell TurkTelekomSMSAdediCell = new TableCell();
                TurkTelekomSMSAdediCell.Text = item.TurkTelekomSMSAdedi.ToString();
                TurkTelekomSMSAdediCell.CssClass = "text-right";
                TableCell TurkTelekomSMSTutariCell = new TableCell();
                TurkTelekomSMSTutariCell.Text = (item.TurkTelekomSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkTelekomSMSTutariCell.CssClass = "text-right";

                int toplamSMSAdedi = item.TurkcellSMSAdedi + item.VodafoneSMSAdedi + item.TurkTelekomSMSAdedi;
                TableCell ToplamSMSAdediCell = new TableCell();
                ToplamSMSAdediCell.Text = toplamSMSAdedi.ToString();
                ToplamSMSAdediCell.CssClass = "text-right";
                TableCell ToplamSMSTutariCell = new TableCell();
                ToplamSMSTutariCell.Text = (toplamSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                ToplamSMSTutariCell.CssClass = "text-right";

                row.Controls.Add(AyCell);
                row.Controls.Add(TurkcellSMSAdediCell);
                row.Controls.Add(TurkcellSMSTutariCell);
                row.Controls.Add(VodafoneSMSAdediCell);
                row.Controls.Add(VodafoneSMSTutariCell);
                row.Controls.Add(TurkTelekomSMSAdediCell);
                row.Controls.Add(TurkTelekomSMSTutariCell);
                row.Controls.Add(ToplamSMSAdediCell);
                row.Controls.Add(ToplamSMSTutariCell);

                ExcelTable.Controls.Add(row);
            }
        }
        private void SetDDLValues()
        {
            try
            {
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
        private void YilDDLDoldur()
        {
            var year = DateTime.Now.Year;
            int ikiYilOnce = year - 1;
            for (int i = ikiYilOnce; i <= year; i++)
            {
                YilDDL.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
        protected void ExportToExcel()
        {
            ExcelTableDiv.Attributes["style"] = "display: block";
            ExcelIcinCizelgeyiDoldur(SecilenYilQS.ConvertToInt());
            string filename = "SMSAylikCizelge" + DateTime.Now.Day.ToString()
                + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
                + new Random().Next() + ".xls";
            Page.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1254");
            Page.Response.Charset = "windows-1254";//ISO-8859-9
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

            //Get the HTML for the control.             
            ExcelTableDiv.RenderControl(hw);
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Page.Response.ContentType = "application/ms-excel";
            Page.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            string s = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            Page.Response.Write(s + tw.ToString());
            Page.Response.End();
            ExcelTableDiv.Attributes["style"] = "display: none";
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
        protected void YilDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SecilenYilQS = YilDDL.SelectedItem.Value.ToString();
            RedirectToPage(ProjeConstants.PAGE_SMSAYLIKCIZELGE + "?SecilenYil=" + SecilenYilQS);
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
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

    }
}