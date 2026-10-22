using Portal_WebParts.Logging;
using Utility.Logging;
using Model.Services.Portal;
using Model.IKYS;
using Model.Services.IKYS;
using Model.MTS;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Portal_WebParts.ToplantiKatilimTutanagiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ToplantiKatilimTutanagiWP : WebPart
    {
        private PortalLogContext logContext;
        private PortalLogContext LogContext { get { return logContext ?? (logContext = PortalLogContext.FromCurrentRequest("Portal", "ToplantiKatilimTutanagiWP")); } }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ToplantiKatilimTutanagiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string ToplantiIdQS
        {
            get
            {

                if (ViewState["ToplantiId"] == null)
                {
                    if (Page.Request.QueryString["ToplantiId"] != null)
                    {
                        ViewState["ToplantiId"] = Page.Request.QueryString["ToplantiId"];
                    }
                    else
                    {
                        ViewState["ToplantiId"] = string.Empty;
                    }
                }
                return ViewState["ToplantiId"].ToString();
            }

            set
            {
                ViewState["ToplantiId"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "Page_Load", () =>
            {
                if (!Page.IsPostBack) PortalWebPartLog.RecordOpened(LogContext);
                var manager = System.Web.UI.ScriptManager.GetCurrent(Page);
                if (manager != null) manager.RegisterPostBackControl(ExcelBtn);
                KatilimciListesiniDoldur();
            });
        }

        private void KatilimciListesiniDoldur()
        {
            Toplanti toplanti = new Toplanti();
            toplanti = new Model.Services.Portal.ToplantiService(LogContext).GetById(ToplantiIdQS.ConvertToInt());
            if (toplanti == null) throw new PortalInputException("Toplantı bulunamadı. Tutanak için toplantı listesinden yeniden seçim yapın.");
            if (toplanti != null)
            {
                BaslikCell.Text = "TOPLANTI KATILIM TUTANAGI <br style='mso-data-placement:same-cell;' />(" + System.Web.HttpUtility.HtmlEncode(toplanti.ToplantiKonusu) + ") <br style='mso-data-placement:same-cell;' />(" + toplanti.BaslangicTarihi.ToString("dd.MM.yyyy") + ")";
                ToplantiKatilim toplantiKatilim = new ToplantiKatilim();
                List<ToplantiKatilim> katilimciListesi = new Model.Services.Portal.ToplantiKatilimService(LogContext).GetByMeeting(toplanti.Id);
                int siraNo = 0;
                foreach (var item in katilimciListesi)
                {
                    if (item.Bilgi) continue;
                    TableRow tableRow = new TableRow();
                    TableCell siraNoCell = new TableCell();
                    TableCell adiSoyadiCell = new TableCell();
                    TableCell unvanCell = new TableCell();
                    TableCell kurumCell = new TableCell();
                    TableCell telefonCell = new TableCell();
                    TableCell epostaCell = new TableCell();
                    TableCell imzaCell = new TableCell();

                    siraNoCell.BorderStyle = BorderStyle.Solid;
                    adiSoyadiCell.BorderStyle = BorderStyle.Solid;
                    unvanCell.BorderStyle = BorderStyle.Solid;
                    kurumCell.BorderStyle = BorderStyle.Solid;
                    telefonCell.BorderStyle = BorderStyle.Solid;
                    epostaCell.BorderStyle = BorderStyle.Solid;
                    imzaCell.BorderStyle = BorderStyle.Solid;

                    siraNoCell.Style.Add("width", "40px");
                    adiSoyadiCell.Style.Add("width", "15%");
                    unvanCell.Style.Add("width", "15%");
                    kurumCell.Style.Add("width", "20%");
                    telefonCell.Style.Add("width", "10%");
                    epostaCell.Style.Add("width", "20%");
                    imzaCell.Style.Add("width", "100px");

                    siraNoCell.Text = (++siraNo).ToString();
                    Personel personel = new Personel();
                    DataTable dataTable = new PersonelService().GetPersonelReturnDataTable(item.KatilimciId, LogContext);
                    if (dataTable != null)
                    {
                        adiSoyadiCell.Text = dataTable.Rows[0]["Adi"].ReturnEmptyIfNull().ToString() + " " + dataTable.Rows[0]["Soyadi"].ReturnEmptyIfNull().ToString();
                        unvanCell.Text = dataTable.Rows[0]["Gorev"].ReturnEmptyIfNull().ToString();
                        kurumCell.Text = dataTable.Rows[0]["BirimSube"].ReturnEmptyIfNull().ToString();
                        telefonCell.Text = dataTable.Rows[0]["CepTelefonu"].ReturnEmptyIfNull().ToString();
                        epostaCell.Text = dataTable.Rows[0]["InternetEPosta"].ReturnEmptyIfNull().ToString();
                    }

                    tableRow.Controls.Add(siraNoCell);
                    tableRow.Controls.Add(adiSoyadiCell);
                    tableRow.Controls.Add(unvanCell);
                    tableRow.Controls.Add(kurumCell);
                    tableRow.Controls.Add(telefonCell);
                    tableRow.Controls.Add(epostaCell);
                    tableRow.Controls.Add(imzaCell);
                    KatilimciBilgileriTable.Rows.Add(tableRow);
                }
                int sayfasonu = siraNo + (10 - siraNo % 10);
                for (int i = siraNo; i < sayfasonu; i++)
                {
                    TableRow tableRow = new TableRow();
                    TableCell siraNoCell = new TableCell();
                    TableCell adiSoyadiCell = new TableCell();
                    TableCell unvanCell = new TableCell();
                    TableCell kurumCell = new TableCell();
                    TableCell telefonCell = new TableCell();
                    TableCell epostaCell = new TableCell();
                    TableCell imzaCell = new TableCell();

                    siraNoCell.BorderStyle = BorderStyle.Solid;
                    adiSoyadiCell.BorderStyle = BorderStyle.Solid;
                    unvanCell.BorderStyle = BorderStyle.Solid;
                    kurumCell.BorderStyle = BorderStyle.Solid;
                    telefonCell.BorderStyle = BorderStyle.Solid;
                    epostaCell.BorderStyle = BorderStyle.Solid;
                    imzaCell.BorderStyle = BorderStyle.Solid;

                    siraNoCell.BorderWidth = Unit.Parse("1px");

                    siraNoCell.Text = (++siraNo).ToString();

                    tableRow.Controls.Add(siraNoCell);
                    tableRow.Controls.Add(adiSoyadiCell);
                    tableRow.Controls.Add(unvanCell);
                    tableRow.Controls.Add(kurumCell);
                    tableRow.Controls.Add(telefonCell);
                    tableRow.Controls.Add(epostaCell);
                    tableRow.Controls.Add(imzaCell);
                    KatilimciBilgileriTable.Rows.Add(tableRow);
                }
            }
        }

        protected void ExportToExcel()
        {
            string html;
            using (var text = new System.IO.StringWriter())
            using (var writer = new System.Web.UI.HtmlTextWriter(text))
            {
                MainCardDiv.RenderControl(writer);
                html = "<!DOCTYPE html><html><head><meta charset='utf-8'/></head><body>" + text + "</body></html>";
            }
            // Render fully before changing the response into a download.
            Page.Response.Clear();
            Page.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Page.Response.Charset = "utf-8";
            Page.Response.ContentType = "application/vnd.ms-excel";
            Page.Response.AddHeader("Content-Disposition", "attachment; filename=ToplantiKatilimTutanagi-" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Page.Response.Write(html);
            Page.Response.End();
        }
        protected void ExcelBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "ExcelBtn_Click", () =>
            {
                ExportToExcel();
            });
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            PortalWebPartLog.ExecuteAction(LogContext, "CloseBtn_Click", () =>
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            });
        }
    }
}
