using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace IKYS_WebParts.BekleyenIslemlerWP
{
    [ToolboxItemAttribute(false)]
    public partial class BekleyenIslemlerWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "BekleyenIslemlerWP"); } }
        private void ReportError(string operation, Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, operation, error);
            Messages.Error("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public BekleyenIslemlerWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected string SaltOkunurGorevlerHtml { get; private set; } = string.Empty;
        private Personel currentPersonel;
        private bool currentPersonelResolved;
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
        private string AmirBirimIdList
        {
            get
            {
                if (ViewState["AmirBirimIdList"] == null)
                {
                    ViewState["AmirBirimIdList"] = string.Empty;
                }
                return ViewState["AmirBirimIdList"].ToString();
            }
            set
            {
                ViewState["AmirBirimIdList"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                new GorevOnayService().TryRecordWebPartOpened(LoggingContext);
                if (!ProjeConstants.AMIRONAYIETKINMI)
                {
                    return;
                }

                if (!Page.IsPostBack)
                {
                    if (AuthQS.Equals("IKYS"))
                    {
                        TabloOlustur();
                    }
                    else
                    {
                        Personel personel = GetCurrentPersonel();
                        if (personel != null && personel.Id > 0)
                        {
                            List<BirimTanim> amirOlduguBirimler = new BirimTanimService().GetByAmirId(personel.Id, LoggingContext);
                            if (amirOlduguBirimler != null && amirOlduguBirimler.Count > 0)
                            {
                                AmirBirimIdList = string.Join(",", amirOlduguBirimler.Select(b => b.Id));
                                TabloOlustur();
                            }
                            else
                            {
                                MesajGoster();
                            }
                        }
                        else
                        {
                            MesajGoster();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        private void MesajGoster()
        {
            TabloDiv.Style["display"] = "none";
            MesajDiv.Style["display"] = "block";
        }
        private Personel GetCurrentPersonel()
        {
            if (!currentPersonelResolved)
            {
                currentPersonel = string.IsNullOrWhiteSpace(CurrentUserName)
                    ? null : IKYSOrtak.PersonelGetir(CurrentUserName, LoggingContext);
                currentPersonelResolved = true;
            }
            return currentPersonel;
        }

        protected override void OnPreRender(EventArgs e)
        {
            // Refresh after approval/rejection events, including on postbacks.
            TabloDiv.Visible = ProjeConstants.AMIRONAYIETKINMI
                && (AuthQS.Equals("IKYS") || !string.IsNullOrEmpty(AmirBirimIdList));
            MesajDiv.Style["display"] = TabloDiv.Visible ? "none" : "block";
            try
            {
                SaltOkunurGorevleriOlustur();
            }
            catch (Exception ex)
            {
                MesajDiv.Style["display"] = "none";
                ReportError("LoadReadOnlyTasks", ex);
            }
            base.OnPreRender(e);
        }

        private void SaltOkunurGorevleriOlustur()
        {
            bool ikysGorunumu = AuthQS.Equals("IKYS");
            int? personelId = null;
            if (!ikysGorunumu)
            {
                Personel personel = GetCurrentPersonel();
                if (personel == null || personel.Id <= 0) return;
                personelId = personel.Id;
            }

            GorevOnayService service = new GorevOnayService();
            DataTable bekleyenler = service.GetOnayBekleyenGorevler(personelId, LoggingContext);
            DataTable onaylananlar = service.GetSonOnaylananGorevler(personelId, LoggingContext);
            StringBuilder html = new StringBuilder();
            if (bekleyenler != null && bekleyenler.Rows.Count > 0)
            {
                html.Append("<section class='mt-4'><h4 class='section-title'>")
                    .Append(ikysGorunumu ? "Onay Bekleyen Görevler" : "Onay Bekleyen Görevlerim")
                    .Append("</h4>");
                AppendSaltOkunurTablo(html, bekleyenler);
                html.Append("</section>");
            }
            if (onaylananlar != null && onaylananlar.Rows.Count > 0)
            {
                html.Append("<details class='mt-4'><summary class='text-primary'>")
                    .Append(ikysGorunumu ? "Son 5 Onaylanmış Görev" : "Son 5 Onaylanmış Görevim")
                    .Append("</summary><p class='text-secondary mt-2'>Görev başlangıç tarihine göre en yeni kayıtlar gösterilmektedir.</p>");
                AppendSaltOkunurTablo(html, onaylananlar);
                html.Append("</details>");
            }
            SaltOkunurGorevlerHtml = html.ToString();
            if (html.Length > 0) MesajDiv.Style["display"] = "none";
        }

        private static void AppendSaltOkunurTablo(StringBuilder html, DataTable table)
        {
            string[] columns = { "AdiSoyadi", "BaslangicTarihi", "BitisTarihi", "GorevinSebebi",
                "GorevinYeri", "UlasimAraci", "Transfer", "Konaklama", "Aciklama" };
            string[] titles = { "Adı Soyadı", "Başlangıç Tarihi", "Bitiş Tarihi", "Görevin Sebebi",
                "Görevin Yeri", "Ulaşım Aracı", "Transfer", "Konaklama", "Açıklama" };
            html.Append("<div class='table-responsive mt-2'><table class='table table-bordered table-striped'><thead><tr>");
            foreach (string title in titles)
                html.Append("<th scope='col'>").Append(HttpUtility.HtmlEncode(title)).Append("</th>");
            html.Append("</tr></thead><tbody>");
            foreach (DataRow row in table.Rows)
            {
                html.Append("<tr>");
                foreach (string column in columns)
                {
                    object value = row[column];
                    string text = value is DateTime
                        ? ((DateTime)value).ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)
                        : Convert.ToString(value, CultureInfo.InvariantCulture);
                    html.Append("<td>").Append(HttpUtility.HtmlEncode(text)).Append("</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</tbody></table></div>");
        }
        private void TabloOlustur()
        {
            var jsonData = TabloJson();
            var jsString = CreateDataTable(jsonData);
            UtilityHelper.ScriptCalistir(jsString);
        }
        private string TabloJson()
        {
            string jSon = string.Empty;
            try
            {
                List<BekleyenIslemListItem> list = GetDataList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                jSon = serializer.Serialize(list);
            }
            catch (Exception exception)
            {
                ReportError("Unhandled", exception);
            }
            return jSon;
        }
        private List<BekleyenIslemListItem> GetDataList()
        {
            if (!ProjeConstants.AMIRONAYIETKINMI)
            {
                return new List<BekleyenIslemListItem>();
            }

            GorevOnayService gorevOnayService = new GorevOnayService();
            DataTable dataTable = AuthQS.Equals("IKYS")
                ? gorevOnayService.GetBekleyenAmirOnayi(LoggingContext)
                : gorevOnayService.GetBekleyenAmirOnayiByBirimIds(AmirBirimIdList, LoggingContext);

            List<BekleyenIslemListItem> list = new List<BekleyenIslemListItem>();

            if (dataTable == null) return list;

            foreach (DataRow row in dataTable.Rows)
            {
                int gorevOnayId = row["GorevOnayId"].ConvertToInt();

                BekleyenIslemListItem item = new BekleyenIslemListItem();
                item.GorevOnayId = gorevOnayId.ToString();
                item.AdiSoyadi = row["AdiSoyadi"].ToString();
                item.BaslangicTarihi = row["BaslangicTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.BitisTarihi = row["BitisTarihi"].ConvertToDatetime().ToString("dd.MM.yyyy");
                item.GorevinSebebi = row["GorevinSebebi"].ToString();
                item.GorevinYeri = row["GorevinYeri"].ToString();
                item.UlasimAraci = row["UlasimAraci"].ToString();
                item.Transfer = row["Transfer"].ToString();
                item.Konaklama = row["Konaklama"].ToString();
                item.Aciklama = row["Aciklama"].ToString();
                item.AmirOnayi = row["AmirOnayi"].ReturnZeroIfNull().ConvertToInt();
                item.OnayRedAciklama = row["OnayRedAciklama"].ReturnEmptyIfNull().ToString();

                if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.OnayBekliyor)
                    item.AmirOnayiSiraNo = 0;
                //else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Reddedildi)
                //    item.AmirOnayiSiraNo = 1;
                //else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Onaylandi)
                //    item.AmirOnayiSiraNo = 2;
                else
                    item.AmirOnayiSiraNo = 3;
                if (item.OdendiMi)
                {
                    item.Onayla = "<span class='text-success'>Ödendi</span>";
                    item.Reddet = string.Empty;
                }
                else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Onaylandi)
                {
                    item.Onayla = UtilityHelper.GetEnumDisplayName(GorevOnay.AmirOnayDurumu.Onaylandi);
                    item.Reddet =string.Empty;
                }
                else if (item.AmirOnayi == (int)GorevOnay.AmirOnayDurumu.Reddedildi)
                {
                    item.Onayla = UtilityHelper.GetEnumDisplayName(GorevOnay.AmirOnayDurumu.Reddedildi);
                    item.Reddet = System.Environment.NewLine + item.OnayRedAciklama;
                }
                else
                {
                    item.Onayla = "<a href='javascript:OpenOnayla(" + gorevOnayId + ");' class='btn btn-outline-primary'>Onayla</a>";
                    item.Reddet = "<a href='javascript:OpenReddet(" + gorevOnayId + ");' class='btn btn-outline-danger'>Reddet</a>";
                }
                item.Incele = "<a href='javascript:OpenIncele(" + gorevOnayId + ");' class='btn btn-outline-secondary'>İncele</a>";

                list.Add(item);
            }
            return list;
        }
        private class BekleyenIslemListItem
        {
            public string GorevOnayId { get; set; }
            public string AdiSoyadi { get; set; }
            public string BaslangicTarihi { get; set; }
            public string BitisTarihi { get; set; }
            public string GorevinSebebi { get; set; }
            public string GorevinYeri { get; set; }
            public string UlasimAraci { get; set; }
            public string Transfer { get; set; }
            public string Konaklama { get; set; }
            public string Aciklama { get; set; }
            public string Onayla { get; set; }
            public string Reddet { get; set; }
            public string Incele { get; set; }
            public int AmirOnayi { get; set; }
            public int AmirOnayiSiraNo { get; set; }
            public string OnayRedAciklama { get; set; }
            public bool OdendiMi { get; set; } = false;

        }
        private string CreateDataTable(string jsonData)
        {
            string tableString = @"
                if ( jQuery.fn.DataTable.isDataTable('#CustomDataTable') ) {
                    jQuery('#CustomDataTable').DataTable().destroy();
                }
                jQuery('#CustomDataTable tbody').empty();

                jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
                jQuery('#CustomDataTable').DataTable({
                    data: " + jsonData + @",
                    columns: [
                        { data: 'AdiSoyadi' },
                        { data: 'BaslangicTarihi' },
                        { data: 'BitisTarihi' },
                        { data: 'GorevinSebebi' },
                        { data: 'GorevinYeri' },
                        { data: 'UlasimAraci' },
                        { data: 'Transfer' },
                        { data: 'Konaklama' },
                        { data: 'Aciklama' },
                        { data: 'Onayla' },
                        { data: 'Reddet' },
                        { data: 'Incele' },
                        { data: 'AmirOnayi', visible: false },
                        { data: 'AmirOnayiSiraNo', visible: false },
                    ],
                    columnDefs: [
                        { type: 'turkish', targets: [0,3,4,5] }
                    ],
                    'order': [[13, 'asc'], [1, 'desc']],//once AmirOnayi oncelik sirasi, sonra tarih desc
                    'createdRow': function (row, data, dataIndex) {
                        if (data.AmirOnayi == 1) {
                            jQuery(row).addClass('table-success');
                        }
                        else if (data.AmirOnayi == 2) {
                            jQuery(row).addClass('table-danger');
                        }
                    },
                    'language': {
                        'url': '" + UtilityHelper.TurkishTxtURLGetir() + @"',
                        'decimal': ',',
                        'thousands': '.'
                    }
                });
            ";

            return tableString;
        }
        protected void ModalOnaylaBtn_Click(object sender, EventArgs e)
        {
            if (!ProjeConstants.AMIRONAYIETKINMI) return;

            try
            {
                GorevOnay gorevOnay = new GorevOnayService().GetById(paramGorevOnayIdLbl.Value.ConvertToInt(), LoggingContext);
                if (gorevOnay != null)
                {
                    Personel personel = new PersonelService().GetById(gorevOnay.PersonelId, LoggingContext);
                    OnayLbl.Text = personel.Adi + " " + personel.Soyadi + " için "
                        + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy")
                        + " tarihleri arasındaki görevi onaylamak istediğinize emin misiniz?";
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalOnay();");
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        protected void ModalReddetBtn_Click(object sender, EventArgs e)
        {
            if (!ProjeConstants.AMIRONAYIETKINMI) return;

            try
            {
                GorevOnay gorevOnay = new GorevOnayService().GetById(paramGorevOnayIdLbl.Value.ConvertToInt(), LoggingContext);
                if (gorevOnay != null)
                {
                    Personel personel = new PersonelService().GetById(gorevOnay.PersonelId, LoggingContext);
                    ReddetLbl.Text = personel.Adi + " " + personel.Soyadi + " için "
                        + gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy") + " - " + gorevOnay.BitisTarihi.ToString("dd.MM.yyyy")
                        + " tarihleri arasındaki görevi reddetmek istediğinize emin misiniz?";
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalReddet();");
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        protected void ModalInceleBtn_Click(object sender, EventArgs e)
        {
            if (!ProjeConstants.AMIRONAYIETKINMI) return;

            try
            {
                GorevOnay gorevOnay = new GorevOnayService().GetById(paramGorevOnayIdLbl.Value.ConvertToInt(), LoggingContext);
                if (gorevOnay != null)
                {
                    Personel personel = new PersonelService().GetById(gorevOnay.PersonelId, LoggingContext);

                    GorevInfoTable.Controls.Clear();

                    AddGorevInfoRow("Adı Soyadı", personel.Adi + " " + personel.Soyadi);
                    AddGorevInfoRow("Başlangıç Tarihi", gorevOnay.BaslangicTarihi.ToString("dd.MM.yyyy"));
                    AddGorevInfoRow("Bitiş Tarihi", gorevOnay.BitisTarihi.ToString("dd.MM.yyyy"));
                    AddGorevInfoRow("Süre", gorevOnay.Sure.ToString());
                    AddGorevInfoRow("Görevin Sebebi", gorevOnay.GorevinSebebi);
                    AddGorevInfoRow("Görevin Yeri", gorevOnay.GorevinYeri);
                    AddGorevInfoRow("Ulaşım Aracı", gorevOnay.UlasimAraci);
                    AddGorevInfoRow("Transfer", gorevOnay.Transfer);
                    AddGorevInfoRow("Konaklama", gorevOnay.Konaklama);
                    AddGorevInfoRow("Amir Onayı", UtilityHelper.GetEnumDisplayName((GorevOnay.AmirOnayDurumu)gorevOnay.AmirOnayi));
                    AddGorevInfoRow("Onay/Red Açıklama", gorevOnay.OnayRedAciklama);
                    AddGorevInfoRow("Açıklama", gorevOnay.Aciklama);
                }
                TabloOlustur();
                UtilityHelper.ScriptCalistir("OpenModalIncele();");
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }

        private void AddGorevInfoRow(string baslik, string deger)
        {
            HtmlTableRow row = new HtmlTableRow();

            HtmlTableCell baslikCell = new HtmlTableCell
            {
                InnerText = baslik
            };
            baslikCell.Attributes["style"] = "font-weight:bold; width:25%;";

            HtmlTableCell degerCell = new HtmlTableCell
            {
                InnerText = deger ?? string.Empty
            };

            row.Cells.Add(baslikCell);
            row.Cells.Add(degerCell);

            GorevInfoTable.Rows.Add(row);
        }
        protected void OnaylaBtn_Click(object sender, EventArgs e)
        {
            if (!ProjeConstants.AMIRONAYIETKINMI) return;

            try
            {
                GorevOnay gorevOnay = new GorevOnayService().GetById(paramGorevOnayIdLbl.Value.ConvertToInt(), LoggingContext);
                if (gorevOnay != null)
                {
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Onaylandi;
                    bool isSuccess = new GorevOnayService().Update(gorevOnay, LoggingContext);
                    if (isSuccess)
                    {
                        OnaylandiMailiGonder(gorevOnay);
                        Messages.Info("Görev onaylandı.");
                    }
                    else
                    {
                        Messages.Error("Onaylama işlemi başarısız oldu.");
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        protected void ReddetBtn_Click(object sender, EventArgs e)
        {
            if (!ProjeConstants.AMIRONAYIETKINMI) return;

            try
            {
                GorevOnay gorevOnay = new GorevOnayService().GetById(paramGorevOnayIdLbl.Value.ConvertToInt(), LoggingContext);
                if (gorevOnay != null)
                {
                    gorevOnay.OnayRedAciklama = ReddetAciklamaTxt.Text;
                    gorevOnay.AmirOnayi = (int)GorevOnay.AmirOnayDurumu.Reddedildi;
                    bool isSuccess = new GorevOnayService().Update(gorevOnay, LoggingContext);
                    if (isSuccess)
                    {
                        ReddedildiMailiGonder(gorevOnay);
                        Messages.Info("Görev reddedildi.");
                    }
                    else
                    {
                        Messages.Error("Reddetme işlemi başarısız oldu.");
                    }
                }
                TabloOlustur();
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        private void OnaylandiMailiGonder(GorevOnay gorevOnay)
        {
            try
            {
                Personel personel = new PersonelService().GetById(gorevOnay.PersonelId, LoggingContext);
                if (personel != null)
                {
                    List<string> emailList = new List<string>();
                    //Bu personelin Amirini bul
                    IsBilgileri isBilgileri = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
                    Personel amir = null;
                    if (isBilgileri != null)
                    {
                        BirimTanim birimTanim = new BirimTanimService().GetById(isBilgileri.BirimId, LoggingContext);
                        amir = new PersonelService().GetById(birimTanim.AmirId, LoggingContext);
                        if (birimTanim != null)
                        {
                            IletisimBilgileri amirIsBilgileri = new IletisimBilgileriService().GetByPersonelId(amir.Id, LoggingContext);
                            emailList.Add(amirIsBilgileri.InternetEPosta);
                        }
                        IletisimBilgileri ib = new IletisimBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
                        emailList.Add(ib.InternetEPosta);
                    }
                    //amire mail gönder
                    //personele  mail gönder
                    if (!IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIçi/YurtDisi", emailList, "Onay", LoggingContext))
                        Messages.Warning("Bilgiler kaydedildi ancak bildirim e-postası gönderilemedi. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));

                }
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        private void ReddedildiMailiGonder(GorevOnay gorevOnay)
        {
            try
            {
                Personel personel = new PersonelService().GetById(gorevOnay.PersonelId, LoggingContext);
                if (personel != null)
                {
                    List<string> emailList = new List<string>();
                    //Bu personelin Amirini bul
                    IsBilgileri isBilgileri = new IsBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
                    Personel amir = null;
                    if (isBilgileri != null)
                    {
                        BirimTanim birimTanim = new BirimTanimService().GetById(isBilgileri.BirimId, LoggingContext);
                        amir = new PersonelService().GetById(birimTanim.AmirId, LoggingContext);
                        if (birimTanim != null)
                        {
                            IletisimBilgileri amirIsBilgileri = new IletisimBilgileriService().GetByPersonelId(amir.Id, LoggingContext);
                            emailList.Add(amirIsBilgileri.InternetEPosta);
                        }
                        IletisimBilgileri ib = new IletisimBilgileriService().GetByPersonelId(personel.Id, LoggingContext);
                        emailList.Add(ib.InternetEPosta);
                    }
                    //amire mail gönder
                    //personele  mail gönder
                    if (!IKYSOrtak.GorevOnayEPostasiGonder(personel, gorevOnay.Id, "YurtIçi/YurtDisi", emailList, "Red", LoggingContext))
                        Messages.Warning("Bilgiler kaydedildi ancak bildirim e-postası gönderilemedi. Başvuru kodu: " + LoggingContext.OperationId.ToString("N"));

                }
            }
            catch (Exception ex)
            {
                ReportError("Unhandled", ex);
            }
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }
    }
}
