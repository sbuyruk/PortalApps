using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.GorevOnayListesiKisiselWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevOnayListesiKisiselWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevOnayListesiKisiselWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            ChromeType = PartChromeType.None;
        }

        protected string TabloJson { get; private set; } = "[]";
        protected string TabloId => ClientID + "_GorevOnayListesiKisisel";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TabloJson = GetTableJson(GetDataTable());
            }
            catch (Exception ex)
            {
                ExceptionHelper exceptionHelper = new ExceptionHelper(ex);
                exceptionHelper.PublishException();
            }
        }

        private DataTable GetDataTable()
        {
            string currentUserName = UtilityHelper.GetCurrentUserLoginName();
            Personel personel = string.IsNullOrWhiteSpace(currentUserName)
                ? null : IKYSOrtak.PersonelGetir(currentUserName);
            if (personel == null || personel.Id <= 0)
            {
                MessageHelper.PublishMessage("Personel bulunamadı", ProjeConstants.MESAJ_HATA);
                return null;
            }

            GorevOnay gorevOnay = new GorevOnay();
            return gorevOnay.SelectAllByPersonelReturnDT(personel.Id);
        }

        private string GetTableJson(DataTable dataTable)
        {
            List<object> list = new List<object>();
            DateTime now = DateTime.Now;
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime? baslangicTarihi = row["BaslangicTarihi"] as DateTime?;
                    DateTime? bitisTarihi = row["BitisTarihi"] as DateTime?;
                    int amirOnayi;
                    bool hasAmirOnayi = int.TryParse(row["AmirOnayi"].ToString(), out amirOnayi)
                        && Enum.IsDefined(typeof(GorevOnay.AmirOnayDurumu), amirOnayi);
                    int gorevOnayId;
                    bool hasGorevOnayId = int.TryParse(row["GorevOnayId"].ToString(), out gorevOnayId) && gorevOnayId > 0;
                    bool duzenlenebilir = hasGorevOnayId && hasAmirOnayi
                        && (row["Odendi"] as bool?) == false
                        && (amirOnayi == (int)GorevOnay.AmirOnayDurumu.OnayBekliyor
                            || amirOnayi == (int)GorevOnay.AmirOnayDurumu.OnayGerekmez)
                        && bitisTarihi.HasValue && now - bitisTarihi.Value <= TimeSpan.FromDays(14);
                    string duzenle = string.Empty;
                    if (duzenlenebilir)
                    {
                        string url = ProjeConstants.PAGE_GOREVONAY_GIRIS + "?GorevOnayId="
                            + gorevOnayId.ToString(CultureInfo.InvariantCulture);
                        duzenle = "<a href=\"" + HttpUtility.HtmlAttributeEncode(url)
                            + "\" class=\"btn btn-outline-primary\">Düzenle</a>";
                    }

                    list.Add(new
                    {
                        AdiSoyadi = HttpUtility.HtmlEncode(row["AdiSoyadi"].ToString()),
                        GorevinSebebi = HttpUtility.HtmlEncode(row["GorevinSebebi"].ToString()),
                        BaslangicTarihi = baslangicTarihi.HasValue ? baslangicTarihi.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture) : string.Empty,
                        BitisTarihi = bitisTarihi.HasValue ? bitisTarihi.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture) : string.Empty,
                        BaslangicTarihiSira = baslangicTarihi.HasValue ? baslangicTarihi.Value.Ticks / TimeSpan.TicksPerMillisecond : 0,
                        BitisTarihiSira = bitisTarihi.HasValue ? bitisTarihi.Value.Ticks / TimeSpan.TicksPerMillisecond : 0,
                        GorevinYeri = HttpUtility.HtmlEncode(row["GorevinYeri"].ToString()),
                        AmirOnayi = hasAmirOnayi ? HttpUtility.HtmlEncode(UtilityHelper.GetEnumDisplayName((GorevOnay.AmirOnayDurumu)amirOnayi).ToString()) : string.Empty,
                        Duzenle = duzenle
                    });
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            return serializer.Serialize(list);
        }
    }
}
