using DocumentFormat.OpenXml.Spreadsheet;
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

namespace NBYS_WebParts.SMSAylikBagisCizelgesi
{
    [ToolboxItemAttribute(false)]
    public partial class SMSAylikBagisCizelgesi : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SMSAylikBagisCizelgesi()
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
            int yil = SecilenYilQS.ConvertToInt();
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
                    int ay = i;
                    smsAylikBagis.SMSTutari = SMSTutariBul(ay,yil);
                    smsAylikBagis.TurkcellSMSAdedi = 0;
                    smsAylikBagis.VodafoneSMSAdedi = 0;
                    smsAylikBagis.TurkTelekomSMSAdedi = 0;
                    smsAylikBagis.Yil = SecilenYilQS.ConvertToInt();
                    smsAylikBagis.Aciklama = "";
                    smsAylikBagis.Save();
                }
            }
        }

        private decimal SMSTutariBul(int ay, int yil)
        {
            decimal smsBedeli;
            DateTime smstarihi= new DateTime(yil,ay,1);
            if (smstarihi > ProjeConstants.SMS_2024)
            {
                smsBedeli = ProjeConstants.SMS_TUTAR_2024;
            }
            else if (smstarihi > ProjeConstants.SMS_2023)
            {
                smsBedeli = ProjeConstants.SMS_TUTAR_2023;
            }
            else
            {
                smsBedeli = ProjeConstants.SMS_TUTAR_2022;
            }
            return smsBedeli;
        }

        private void CizelgeyiDoldur(int yil)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            SMSAylikBagis smsAylikBagisDao = new SMSAylikBagis();
            List<SMSAylikBagis> list = smsAylikBagisDao.SelectByYilReturnList(yil);
            
            int ToplamTurkcellSMSAdedi = 0;
            decimal ToplamTurkcellSMSTutari = 0;
            int ToplamVodafoneSMSAdedi = 0;
            decimal ToplamVodafoneSMSTutari = 0;
            int ToplamTurkTelekomSMSAdedi = 0;
            decimal ToplamTurkTelekomSMSTutari = 0;
            int ToplamSMSAdedi = 0;
            decimal ToplamSMSTutari = 0;

            foreach (SMSAylikBagis item in list)
            {
                TableRow row = new TableRow();

                TableCell AyCell = new TableCell();
                AyCell.Text = new DateTime(SecilenYilQS.ConvertToInt(), item.Ay, 1).ToString("MMMM", culturInfo);

                ToplamTurkcellSMSAdedi += item.TurkcellSMSAdedi;
                TableCell TurkcellSMSAdediCell = new TableCell();
                TurkcellSMSAdediCell.Text = item.TurkcellSMSAdedi.ToString();
                TurkcellSMSAdediCell.CssClass = "text-end";

                ToplamTurkcellSMSTutari += item.TurkcellSMSAdedi * item.SMSTutari;
                TableCell TurkcellSMSTutariCell = new TableCell();
                TurkcellSMSTutariCell.Text = (item.TurkcellSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkcellSMSTutariCell.CssClass = "text-end";

                ToplamVodafoneSMSAdedi += item.VodafoneSMSAdedi;
                TableCell VodafoneSMSAdediCell = new TableCell();
                VodafoneSMSAdediCell.Text = item.VodafoneSMSAdedi.ToString();
                VodafoneSMSAdediCell.CssClass = "text-end";

                ToplamVodafoneSMSTutari += item.VodafoneSMSAdedi * item.SMSTutari;
                TableCell VodafoneSMSTutariCell = new TableCell();
                VodafoneSMSTutariCell.Text = (item.VodafoneSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                VodafoneSMSTutariCell.CssClass = "text-end";

                ToplamTurkTelekomSMSAdedi += item.TurkTelekomSMSAdedi;
                TableCell TurkTelekomSMSAdediCell = new TableCell();
                TurkTelekomSMSAdediCell.Text = item.TurkTelekomSMSAdedi.ToString();
                TurkTelekomSMSAdediCell.CssClass = "text-end";

                ToplamTurkTelekomSMSTutari += item.TurkTelekomSMSAdedi * item.SMSTutari;
                TableCell TurkTelekomSMSTutariCell = new TableCell();
                TurkTelekomSMSTutariCell.Text = (item.TurkTelekomSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkTelekomSMSTutariCell.CssClass = "text-end";

                int ayToplamSMSAdedi = item.TurkcellSMSAdedi + item.VodafoneSMSAdedi + item.TurkTelekomSMSAdedi;
                ToplamSMSAdedi += ayToplamSMSAdedi;
                TableCell AyToplamSMSAdediCell = new TableCell();
                AyToplamSMSAdediCell.Text = ayToplamSMSAdedi.ToString();
                AyToplamSMSAdediCell.CssClass = "text-end";

                ToplamSMSTutari += (ayToplamSMSAdedi * item.SMSTutari);
                TableCell AyToplamSMSTutariCell = new TableCell();
                AyToplamSMSTutariCell.Text = (ayToplamSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                AyToplamSMSTutariCell.CssClass = "text-end";

                TableCell DuzenleCell = new TableCell();

                LinkButton duzenleBtn = new LinkButton();
                duzenleBtn.CausesValidation = false;
                duzenleBtn.Text = "Düzenle";
                duzenleBtn.CssClass = "btn btn-outline-primary";
                duzenleBtn.Click += delegate
                {
                    HiddenSMSAylikBagisId.Value = item.Id.ToString();
                    PopupMesajLbl.Text = new DateTime(item.Yil, item.Ay, 1).ToString("MMMM", culturInfo) + " " + item.Yil + " SMS Bagislari";
                    SMSAdediniDegistirDiv.Attributes["style"] = "display: block";
                    TurkcellSMSTxt.Text = item.TurkcellSMSAdedi.ToString();
                    VodafoneSMSTxt.Text = item.VodafoneSMSAdedi.ToString();
                    TurkTelekomSMSTxt.Text = item.TurkTelekomSMSAdedi.ToString();
                    var openPopup = "OpenModalOnay();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
                };
                DuzenleCell.Controls.Add(duzenleBtn);

                row.Controls.Add(AyCell);
                row.Controls.Add(TurkcellSMSAdediCell);
                row.Controls.Add(TurkcellSMSTutariCell);
                row.Controls.Add(VodafoneSMSAdediCell);
                row.Controls.Add(VodafoneSMSTutariCell);
                row.Controls.Add(TurkTelekomSMSAdediCell);
                row.Controls.Add(TurkTelekomSMSTutariCell);
                row.Controls.Add(AyToplamSMSAdediCell);
                row.Controls.Add(AyToplamSMSTutariCell);
                //bool isYetkili = KullaniciGrubundanKontrolEt();
                row.Controls.Add(DuzenleCell);
                AyrintiTable.Controls.Add(row);
            }

            TableRow toplamRow = new TableRow();
            TableCell AyBaslikCell = new TableCell();
            AyBaslikCell.Text = "Toplam";
            AyBaslikCell.CssClass = "text-end fw-bold";
            
            TableCell ToplamTurkcellSMSAdediCell = new TableCell();
            ToplamTurkcellSMSAdediCell.Text = ToplamTurkcellSMSAdedi.ToString();
            ToplamTurkcellSMSAdediCell.CssClass = "text-end fw-bold";
            
            TableCell ToplamTurkcellSMSTutariCell = new TableCell();
            ToplamTurkcellSMSTutariCell.Text = (ToplamTurkcellSMSTutari).ToString("N", culturInfo);
            ToplamTurkcellSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamVodafoneSMSAdediCell = new TableCell();
            ToplamVodafoneSMSAdediCell.Text = ToplamVodafoneSMSAdedi.ToString();
            ToplamVodafoneSMSAdediCell.CssClass = "text-end fw-bold";


            TableCell ToplamVodafoneSMSTutariCell = new TableCell();
            ToplamVodafoneSMSTutariCell.Text = ToplamVodafoneSMSTutari.ToString("N", culturInfo);
            ToplamVodafoneSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkTelekomSMSAdediCell = new TableCell();
            ToplamTurkTelekomSMSAdediCell.Text = ToplamTurkTelekomSMSAdedi.ToString();
            ToplamTurkTelekomSMSAdediCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkTelekomSMSTutariCell = new TableCell();
            ToplamTurkTelekomSMSTutariCell.Text = ToplamTurkTelekomSMSTutari.ToString("N", culturInfo);
            ToplamTurkTelekomSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamSMSAdediCell = new TableCell();
            ToplamSMSAdediCell.Text = ToplamSMSAdedi.ToString();
            ToplamSMSAdediCell.CssClass = "text-end fw-bold";


            TableCell ToplamSMSTutariCell = new TableCell();
            ToplamSMSTutariCell.Text = ToplamSMSTutari.ToString("N", culturInfo);
            ToplamSMSTutariCell.CssClass = "text-end fw-bold";

            toplamRow.Controls.Add(AyBaslikCell);
            toplamRow.Controls.Add(ToplamTurkcellSMSAdediCell);
            toplamRow.Controls.Add(ToplamTurkcellSMSTutariCell);
            toplamRow.Controls.Add(ToplamVodafoneSMSAdediCell);
            toplamRow.Controls.Add(ToplamVodafoneSMSTutariCell);
            toplamRow.Controls.Add(ToplamTurkTelekomSMSAdediCell);
            toplamRow.Controls.Add(ToplamTurkTelekomSMSTutariCell);
            toplamRow.Controls.Add(ToplamSMSAdediCell);
            toplamRow.Controls.Add(ToplamSMSTutariCell);
            AyrintiTable.Controls.Add(toplamRow);
        }

        //private bool KullaniciGrubundanKontrolEt(SPUser kullanici)
        //{
        //    bool yetkiliMi = false;
        //    SPSite site = new SPSite(SPContext.Current.Web.Url);
        //    SPWeb web = site.OpenWeb();
        //    SPListItem item;

        //    SPFieldUserValue usersField = new SPFieldUserValue(mainWeb, item["Users"].ToString());

        //    bool isUser = SPUtility.IsLoginValid(site, usersField.User.LoginName);
        //    List<SPUser> users = new List<SPUser>();

        //    if (isUser)
        //    {
        //        // add a single user to the list
        //        users.Add(usersField.User);
        //    }
        //    else
        //    {
        //        SPGroup group = web.Groups.GetByID(usersField.LookupId);

        //        foreach (SPUser user in group.Users)
        //        {

        //        }
        //    }
        //    return yetkiliMi;
        //}

        /// <summary>
        /// LinkButton Düzenle excele aktarirken hata verdiginden 
        /// CizelgeyiDoldur() metodu ile ayni islemi Linkbutton olmadan yapiyor
        /// </summary>
        /// <param name="yil"></param>
        private void ExcelIcinCizelgeyiDoldur(int yil)
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            SMSAylikBagis smsAylikBagisDao = new SMSAylikBagis();
            List<SMSAylikBagis> list = smsAylikBagisDao.SelectByYilReturnList(yil);

            int ToplamTurkcellSMSAdedi = 0;
            decimal ToplamTurkcellSMSTutari = 0;
            int ToplamVodafoneSMSAdedi = 0;
            decimal ToplamVodafoneSMSTutari = 0;
            int ToplamTurkTelekomSMSAdedi = 0;
            decimal ToplamTurkTelekomSMSTutari = 0;
            int ToplamSMSAdedi = 0;
            decimal ToplamSMSTutari = 0;

            foreach (SMSAylikBagis item in list)
            {
                TableRow row = new TableRow();

                TableCell AyCell = new TableCell();
                AyCell.Text = new DateTime(SecilenYilQS.ConvertToInt(), item.Ay, 1).ToString("MMMM", culturInfo);

                ToplamTurkcellSMSAdedi += item.TurkcellSMSAdedi;
                TableCell TurkcellSMSAdediCell = new TableCell();
                TurkcellSMSAdediCell.Text = item.TurkcellSMSAdedi.ToString();
                TurkcellSMSAdediCell.CssClass = "text-end";

                ToplamTurkcellSMSTutari += item.TurkcellSMSAdedi * item.SMSTutari;
                TableCell TurkcellSMSTutariCell = new TableCell();
                TurkcellSMSTutariCell.Text = (item.TurkcellSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkcellSMSTutariCell.CssClass = "text-end";

                ToplamVodafoneSMSAdedi += item.VodafoneSMSAdedi;
                TableCell VodafoneSMSAdediCell = new TableCell();
                VodafoneSMSAdediCell.Text = item.VodafoneSMSAdedi.ToString();
                VodafoneSMSAdediCell.CssClass = "text-end";

                ToplamVodafoneSMSTutari += item.VodafoneSMSAdedi * item.SMSTutari;
                TableCell VodafoneSMSTutariCell = new TableCell();
                VodafoneSMSTutariCell.Text = (item.VodafoneSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                VodafoneSMSTutariCell.CssClass = "text-end";

                ToplamTurkTelekomSMSAdedi += item.TurkcellSMSAdedi;
                TableCell TurkTelekomSMSAdediCell = new TableCell();
                TurkTelekomSMSAdediCell.Text = item.TurkTelekomSMSAdedi.ToString();
                TurkTelekomSMSAdediCell.CssClass = "text-end";

                ToplamTurkTelekomSMSTutari += item.TurkcellSMSAdedi * item.SMSTutari;
                TableCell TurkTelekomSMSTutariCell = new TableCell();
                TurkTelekomSMSTutariCell.Text = (item.TurkTelekomSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                TurkTelekomSMSTutariCell.CssClass = "text-end";

                int ayToplamSMSAdedi = item.TurkcellSMSAdedi + item.VodafoneSMSAdedi + item.TurkTelekomSMSAdedi;
                ToplamSMSAdedi += ayToplamSMSAdedi;
                TableCell AyToplamSMSAdediCell = new TableCell();
                AyToplamSMSAdediCell.Text = ayToplamSMSAdedi.ToString();
                AyToplamSMSAdediCell.CssClass = "text-end";

                ToplamSMSTutari += (ayToplamSMSAdedi * item.SMSTutari);
                TableCell AyToplamSMSTutariCell = new TableCell();
                AyToplamSMSTutariCell.Text = (ayToplamSMSAdedi * item.SMSTutari).ToString("N", culturInfo);
                AyToplamSMSTutariCell.CssClass = "text-end";

                TableCell DuzenleCell = new TableCell();

                LinkButton duzenleBtn = new LinkButton();
                duzenleBtn.CausesValidation = false;
                duzenleBtn.Text = "Düzenle";
                duzenleBtn.CssClass = "btn btn-outline-primary";
                duzenleBtn.Click += delegate
                {
                    HiddenSMSAylikBagisId.Value = item.Id.ToString();
                    PopupMesajLbl.Text = new DateTime(item.Yil, item.Ay, 1).ToString("MMMM", culturInfo) + " " + item.Yil + " SMS Bagislari";
                    SMSAdediniDegistirDiv.Attributes["style"] = "display: block";
                    TurkcellSMSTxt.Text = item.TurkcellSMSAdedi.ToString();
                    VodafoneSMSTxt.Text = item.VodafoneSMSAdedi.ToString();
                    TurkTelekomSMSTxt.Text = item.TurkTelekomSMSAdedi.ToString();
                    var openPopup = "OpenModalOnay();";
                    System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), openPopup, true);
                };
                DuzenleCell.Controls.Add(duzenleBtn);

                row.Controls.Add(AyCell);
                row.Controls.Add(TurkcellSMSAdediCell);
                row.Controls.Add(TurkcellSMSTutariCell);
                row.Controls.Add(VodafoneSMSAdediCell);
                row.Controls.Add(VodafoneSMSTutariCell);
                row.Controls.Add(TurkTelekomSMSAdediCell);
                row.Controls.Add(TurkTelekomSMSTutariCell);
                row.Controls.Add(AyToplamSMSAdediCell);
                row.Controls.Add(AyToplamSMSTutariCell);
                //bool isYetkili = KullaniciGrubundanKontrolEt();
                row.Controls.Add(DuzenleCell);
                AyrintiTable.Controls.Add(row);
            }

            TableRow toplamRow = new TableRow();
            TableCell AyBaslikCell = new TableCell();
            AyBaslikCell.Text = "Toplam";
            AyBaslikCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkcellSMSAdediCell = new TableCell();
            ToplamTurkcellSMSAdediCell.Text = ToplamTurkcellSMSAdedi.ToString();
            ToplamTurkcellSMSAdediCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkcellSMSTutariCell = new TableCell();
            ToplamTurkcellSMSTutariCell.Text = (ToplamTurkcellSMSTutari).ToString("N", culturInfo);
            ToplamTurkcellSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamVodafoneSMSAdediCell = new TableCell();
            ToplamVodafoneSMSAdediCell.Text = ToplamVodafoneSMSAdedi.ToString();
            ToplamVodafoneSMSAdediCell.CssClass = "text-end fw-bold";


            TableCell ToplamVodafoneSMSTutariCell = new TableCell();
            ToplamVodafoneSMSTutariCell.Text = ToplamVodafoneSMSTutari.ToString("N", culturInfo);
            ToplamVodafoneSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkTelekomSMSAdediCell = new TableCell();
            ToplamTurkTelekomSMSAdediCell.Text = ToplamTurkTelekomSMSAdedi.ToString();
            ToplamTurkTelekomSMSAdediCell.CssClass = "text-end fw-bold";

            TableCell ToplamTurkTelekomSMSTutariCell = new TableCell();
            ToplamTurkTelekomSMSTutariCell.Text = ToplamTurkTelekomSMSTutari.ToString("N", culturInfo);
            ToplamTurkTelekomSMSTutariCell.CssClass = "text-end fw-bold";

            TableCell ToplamSMSAdediCell = new TableCell();
            ToplamSMSAdediCell.Text = ToplamSMSAdedi.ToString();
            ToplamSMSAdediCell.CssClass = "text-end fw-bold";


            TableCell ToplamSMSTutariCell = new TableCell();
            ToplamSMSTutariCell.Text = ToplamSMSTutari.ToString("N", culturInfo);
            ToplamSMSTutariCell.CssClass = "text-end fw-bold";

            toplamRow.Controls.Add(AyBaslikCell);
            toplamRow.Controls.Add(ToplamTurkcellSMSAdediCell);
            toplamRow.Controls.Add(ToplamTurkcellSMSTutariCell);
            toplamRow.Controls.Add(ToplamVodafoneSMSAdediCell);
            toplamRow.Controls.Add(ToplamVodafoneSMSTutariCell);
            toplamRow.Controls.Add(ToplamTurkTelekomSMSAdediCell);
            toplamRow.Controls.Add(ToplamTurkTelekomSMSTutariCell);
            toplamRow.Controls.Add(ToplamSMSAdediCell);
            toplamRow.Controls.Add(ToplamSMSTutariCell);
            ExcelTable.Controls.Add(toplamRow);
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
            int ilkYear = 2019;
            for (int i = ilkYear; i <= year; i++)
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
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            SMSAylikBagis smsAylikBagis = new SMSAylikBagis();
            int smsAylikBagisId = HiddenSMSAylikBagisId.Value.ConvertToInt();
            smsAylikBagis = smsAylikBagis.Select<SMSAylikBagis>(smsAylikBagisId);
            if (smsAylikBagis != null)
            {
                smsAylikBagis.TurkcellSMSAdedi = TurkcellSMSTxt.Text.ConvertToInt();
                smsAylikBagis.VodafoneSMSAdedi = VodafoneSMSTxt.Text.ConvertToInt();
                smsAylikBagis.TurkTelekomSMSAdedi = TurkTelekomSMSTxt.Text.ConvertToInt();
                smsAylikBagis.Degistiren = CurrentUserName;
                smsAylikBagis.Update();
                RedirectToPage(ProjeConstants.PAGE_SMSAYLIKCIZELGE + "?SecilenYil=" + SecilenYilQS);
            }
        }
    }
}
