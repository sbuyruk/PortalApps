using Model.IKYS;
using Model.Ortak;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using System.Linq;
using System.Collections.Generic;

namespace IKYS_WebParts.HarcirahGirisWP
{
    [ToolboxItemAttribute(false)]
    public partial class HarcirahGirisWP : WebPart
    {
        public HarcirahGirisWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }

        private string HarcirahIdQS
        {
            get
            {
                if (ViewState["HarcirahId"] == null)
                {
                    if (Page.Request.QueryString["HarcirahId"] != null)
                    {
                        ViewState["HarcirahId"] = Page.Request.QueryString["HarcirahId"];
                    }
                    else
                    {
                        ViewState["HarcirahId"] = string.Empty;
                    }
                }

                return ViewState["HarcirahId"].ToString();
            }
            set
            {
                ViewState["HarcirahId"] = value;
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

        private string SelectedUlkeFilter
        {
            get
            {
                string value = (ViewState["SelectedUlkeFilter"] ?? string.Empty).ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return ProjeConstants.TURKIYE;
                }

                return value;
            }
            set
            {
                ViewState["SelectedUlkeFilter"] = value;
            }
        }

        private int SelectedSeriIdFilter
        {
            get
            {
                object value = ViewState["SelectedSeriIdFilter"];
                if (value == null)
                {
                    return ProjeConstants.BOS_INT;
                }

                return value.ToString().ConvertToInt();
            }
            set
            {
                ViewState["SelectedSeriIdFilter"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillDropDowns();
                }

                HarcirahTablosunuDoldur();
            }
            catch (Exception exception)
            {
                ExceptionHelper exHelper = new ExceptionHelper(exception);
                exHelper.PublishException();
            }
        }

        private void FillDropDowns()
        {
            FillUlkeDDL(UlkeFilterDDL);
            UtilityHelper.SetDDLValue(UlkeFilterDDL, SelectedUlkeFilter);

            if (string.IsNullOrWhiteSpace(UlkeFilterDDL.SelectedValue) ||
                UlkeFilterDDL.SelectedValue == ProjeConstants.BOS_INT.ToString())
            {
                AyrintiTable.Rows.Clear();
                return;
            }

            FillTarihSeriDDL(TarihSeriDDL, UlkeFilterDDL.SelectedValue);
            UtilityHelper.SetDDLValue(TarihSeriDDL, SelectedSeriIdFilter.ToString());
        }


        private void FillUlkeDDL(DropDownList ddl)
        {
            ddl.Items.Clear();

            Harcirah harcirahDao = new Harcirah();
            DataTable dt = harcirahDao.SelectAllReturnDataTable();
            if (dt == null || dt.Rows.Count == 0)
            {
                ddl.Items.Add(new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString()));
                return;
            }

            var ulkeler = dt.Rows.Cast<DataRow>()
                .Select(r => r["Ulke"].ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x);

            //ddl.Items.Add(new ListItem(ProjeConstants.BOS, ProjeConstants.BOS_INT.ToString()));

            foreach (string ulke in ulkeler)
            {
                ddl.Items.Add(new ListItem(ulke, ulke));
            }
        }


        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
            Page.Response.Redirect(newUrl);
        }

        protected void EkleBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(YeniBaslangicTarihiTxt.Text))
                {
                    MessageHelper.PublishMessage("Baslangiç tarihi zorunludur.", ProjeConstants.MESAJ_BILGI, 3000);
                    return;
                }

                string seciliUlke = UlkeFilterDDL.SelectedValue;

                if (string.IsNullOrWhiteSpace(seciliUlke) ||
                    seciliUlke == ProjeConstants.BOS_INT.ToString())
                {
                    MessageHelper.PublishMessage("Ülke seçilmelidir.", ProjeConstants.MESAJ_BILGI, 3000);
                    return;
                }

                DateTime baslangicTarihi = YeniBaslangicTarihiTxt.Text.ConvertToDatetime();

                Harcirah harcirahDao = new Harcirah();
                List<Harcirah> mevcutKayitlar = harcirahDao.SelectAll<Harcirah>();

                List<Harcirah> seciliUlkeKayitlari = mevcutKayitlar
                    .Where(x => string.Equals((x.Ulke ?? string.Empty).Trim(), seciliUlke.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                int oncekiSeriId = seciliUlkeKayitlari.Count == 0 ? ProjeConstants.BOS_INT : seciliUlkeKayitlari.Max(x => x.SeriId);
                int newSeriId = (oncekiSeriId > 0 ? oncekiSeriId + 1 : 1);

                if (oncekiSeriId > 0)
                {
                    DateTime oncekiBitis = baslangicTarihi.AddDays(-1);

                    List<Harcirah> oncekiSeriKayitlari = seciliUlkeKayitlari
                        .Where(x => x.SeriId == oncekiSeriId)
                        .ToList();

                    foreach (Harcirah eski in oncekiSeriKayitlari)
                    {
                        eski.BitisTarihi = oncekiBitis;
                        eski.Degistiren = CurrentUserName;
                        eski.Update();
                    }
                }

                var kadroUlkeKombinasyonlari = seciliUlkeKayitlari
                    .Where(x => x.KadroGrupId > 0)
                    .Select(x => new
                    {
                        x.KadroGrupId,
                        x.Sira,
                        KadroGrubu = (x.KadroGrubu ?? string.Empty).Trim(),
                        Ulke = (x.Ulke ?? string.Empty).Trim(),
                        ParaBirimi = GetParaBirimiByUlke((x.Ulke ?? string.Empty).Trim())
                    })
                    .GroupBy(x => new { x.KadroGrupId, x.Ulke }, new KadroUlkeComparer())
                    .Select(g => g.First())
                    .ToList();

                if (kadroUlkeKombinasyonlari.Count == 0)
                {
                    MessageHelper.PublishMessage("Seçili ülke için kadro grubu/ülke tanimi bulunamadi.", ProjeConstants.MESAJ_BILGI, 3000);
                    return;
                }

                int eklendi = 0;
                foreach (var k in kadroUlkeKombinasyonlari.OrderBy(x => x.KadroGrupId).ThenBy(x => x.Sira))
                {
                    bool zatenVar = seciliUlkeKayitlari.Any(x =>
                        x.KadroGrupId == k.KadroGrupId
                        && string.Equals((x.Ulke ?? string.Empty).Trim(), k.Ulke, StringComparison.OrdinalIgnoreCase)
                        && x.BaslangicTarihi == baslangicTarihi
                        && x.SeriId == newSeriId);

                    if (zatenVar)
                    {
                        continue;
                    }

                    Harcirah yeni = new Harcirah
                    {
                        SeriId = newSeriId,
                        Sira = k.Sira,
                        KadroGrupId = k.KadroGrupId,
                        KadroGrubu = k.KadroGrubu,
                        Ulke = k.Ulke,
                        ParaBirimi = k.ParaBirimi,
                        BaslangicTarihi = baslangicTarihi,
                        BitisTarihi = DateTime.MinValue,
                        Olusturan = CurrentUserName
                    };

                    int id = yeni.Save();
                    if (id > 0)
                    {
                        eklendi++;
                    }
                }

                MessageHelper.PublishMessage(eklendi + " kayit eklendi.", ProjeConstants.MESAJ_BASARILI, 3000);
                RedirectToSelf();
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }

        private void HarcirahTablosunuDoldur()
        {
            AyrintiTable.Rows.Clear();
            SelectedSeriIdFilter = TarihSeriDDL.SelectedItem.Value.ConvertToInt();
            // Header
            TableHeaderRow headerRow = new TableHeaderRow();

            headerRow.Cells.Add(new TableHeaderCell { Text = "Sira" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Kadro Grubu" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Ülke" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Para Birimi" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Miktar" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Baslangiç Tarihi" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Bitis Tarihi" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Açiklama" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Güncelle" });
            headerRow.Cells.Add(new TableHeaderCell { Text = "Sil" });

            AyrintiTable.Rows.Add(headerRow);

            Harcirah harcirahDao = new Harcirah();
            List<Harcirah> list = harcirahDao.SelectAll<Harcirah>();

            if (!string.IsNullOrWhiteSpace(SelectedUlkeFilter))
            {
                list = list.Where(x => string.Equals((x.Ulke ?? string.Empty).Trim(), SelectedUlkeFilter.Trim(), StringComparison.OrdinalIgnoreCase))
                           .ToList();
            }

            if (SelectedSeriIdFilter > 0)
            {
                list = list.Where(x => x.SeriId == SelectedSeriIdFilter).ToList();
            }

            int sira = 1;
            foreach (Harcirah item in list)
            {
                TableRow row = new TableRow();

                TableCell siraCell = new TableCell { Text = sira.ToString() };

                // Kadro Grubu (Text)
                TableCell kadroCell = new TableCell();
                TextBox kadroTxt = new TextBox
                {
                    CssClass = "form-control",
                    Text = item.KadroGrubu,
                    Enabled = false
                };
                kadroCell.Controls.Add(kadroTxt);

                // Ülke (Text)
                TableCell ulkeCell = new TableCell();
                TextBox ulkeTxt = new TextBox
                {
                    CssClass = "form-control",
                    Text = item.Ulke,
                    Enabled = false
                };
                ulkeCell.Controls.Add(ulkeTxt);

                // Para birimi
                TableCell paraCell = new TableCell();
                TextBox paraTxt = new TextBox { CssClass = "form-control", Text = item.ParaBirimi, Enabled = false };
                paraCell.Controls.Add(paraTxt);

                // Miktar
                TableCell miktarCell = new TableCell();
                TextBox miktarTxt = new TextBox { CssClass = "form-control input-money text-end", Text = item.Miktar.ToString() };
                miktarCell.Controls.Add(miktarTxt);

                // Baslangiç / Bitis
                TableCell basTarCell = new TableCell();
                TextBox basTarTxt = new TextBox { CssClass = "form-control", Text = item.BaslangicTarihi.ConvertToDatetimeEmptyIfNull(), Enabled = false };
                basTarCell.Controls.Add(basTarTxt);

                TableCell bitTarCell = new TableCell();
                TextBox bitTarTxt = new TextBox { CssClass = "form-control", Text = item.BitisTarihi.ConvertToDatetimeEmptyIfNull(), Enabled = false };
                bitTarCell.Controls.Add(bitTarTxt);

                // Açiklama
                TableCell aciklamaCell = new TableCell();
                TextBox aciklamaTxt = new TextBox { CssClass = "form-control", Text = item.Aciklama };
                aciklamaCell.Controls.Add(aciklamaTxt);

                // Güncelle
                TableCell guncelleCell = new TableCell();
                LinkButton guncelleBtn = new LinkButton
                {
                    Text = "Güncelle",
                    CssClass = "btn btn-outline-primary",
                    ID = "GuncelleBtn" + sira
                };

                guncelleBtn.Click += delegate
                {
                    try
                    {
                        item.ParaBirimi = paraTxt.Text;
                        item.Miktar = miktarTxt.Text.ConvertToDecimal();
                        item.BaslangicTarihi = basTarTxt.Text.ConvertToDatetime();
                        item.BitisTarihi = bitTarTxt.Text.ConvertToDatetime();
                        item.Aciklama = aciklamaTxt.Text;
                        item.Degistiren = CurrentUserName;

                        if (item.Update())
                        {
                            MessageHelper.PublishMessage("Güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
                            RedirectToSelf();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper exHelper = new ExceptionHelper(ex);
                        exHelper.PublishException();
                    }
                };
                guncelleCell.Controls.Add(guncelleBtn);

                // Sil
                TableCell silCell = new TableCell();
                LinkButton silBtn = new LinkButton
                {
                    Text = "Sil",
                    CssClass = "btn btn-outline-danger",
                    ID = "SilBtn" + sira
                };

                silBtn.Click += delegate
                {
                    try
                    {
                        item.Degistiren = CurrentUserName;

                        if (item.Delete())
                        {
                            MessageHelper.PublishMessage("Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
                            RedirectToSelf();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper exHelper = new ExceptionHelper(ex);
                        exHelper.PublishException();
                    }
                };
                silCell.Controls.Add(silBtn);

                row.Cells.Add(siraCell);
                row.Cells.Add(kadroCell);
                row.Cells.Add(ulkeCell);
                row.Cells.Add(paraCell);
                row.Cells.Add(miktarCell);
                row.Cells.Add(basTarCell);
                row.Cells.Add(bitTarCell);
                row.Cells.Add(aciklamaCell);
                row.Cells.Add(guncelleCell);
                row.Cells.Add(silCell);

                AyrintiTable.Rows.Add(row);
                sira++;
            }
        }

        private void RedirectToSelf()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HARCIRAH_GIRIS;
            Page.Response.Redirect(newUrl, true);
        }

        protected void UlkeFilterDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedUlkeFilter = UlkeFilterDDL.SelectedValue;

            if (string.IsNullOrWhiteSpace(SelectedUlkeFilter) ||
                SelectedUlkeFilter == ProjeConstants.BOS_INT.ToString())
            {
                TarihSeriDDL.Items.Clear();
                SelectedSeriIdFilter = ProjeConstants.BOS_INT;
                AyrintiTable.Rows.Clear();
                return;
            }

            FillTarihSeriDDL(TarihSeriDDL, SelectedUlkeFilter);
            SelectedSeriIdFilter = ProjeConstants.BOS_INT;
            UtilityHelper.SetDDLValue(TarihSeriDDL, SelectedSeriIdFilter.ToString());

            HarcirahTablosunuDoldur();
        }
        protected void TarihSeriDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedSeriIdFilter = TarihSeriDDL.SelectedValue.ConvertToInt();
            HarcirahTablosunuDoldur();
        }

        private class KadroUlkeComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                return x?.ToString() == y?.ToString();
            }

            public int GetHashCode(object obj)
            {
                return obj?.ToString().GetHashCode() ?? 0;
            }
        }

        private static string GetParaBirimiByUlke(string ulke)
        {
            if (string.IsNullOrWhiteSpace(ulke))
            {
                return string.Empty;
            }

            string u = ulke.Trim();

            if (string.Equals(u, ProjeConstants.TURKIYE, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "Türkiye", StringComparison.OrdinalIgnoreCase))
            {
                return "TL";
            }

            if (string.Equals(u, "Ingiltere", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "Ingiltere", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "United Kingdom", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "UK", StringComparison.OrdinalIgnoreCase))
            {
                return "Sterlin";
            }

            if (string.Equals(u, "Avrupa", StringComparison.OrdinalIgnoreCase))
            {
                return "Euro";
            }

            if (string.Equals(u, "Avrupa Harici", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "Avrupa Disi", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u, "Avrupa Disi", StringComparison.OrdinalIgnoreCase))
            {
                return "USD";
            }

            return (string.Equals(u, ProjeConstants.TURKIYE, StringComparison.OrdinalIgnoreCase) ? "TL" : "USD");
        }

        private void FillTarihSeriDDL(DropDownList ddl, string ulke)
        {
            ddl.Items.Clear();

            if (string.IsNullOrWhiteSpace(ulke) || ulke == ProjeConstants.BOS_INT.ToString())
            {
                return;
            }

            Harcirah harcirahDao = new Harcirah();
            List<Harcirah> list = harcirahDao.SelectAll<Harcirah>();

            list = list
                .Where(x => x.SeriId > 0)
                .Where(x => string.Equals((x.Ulke ?? string.Empty).Trim(), ulke.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            var seriler = list
                .GroupBy(x => new { x.Ulke, x.SeriId })
                .Select(g => new
                {
                    g.Key.SeriId,
                    BaslangicTarihi = g.Min(x => x.BaslangicTarihi),
                    BitisTarihi = g.Max(x => x.BitisTarihi)
                })
                .OrderByDescending(x => x.SeriId)
                .ToList();

            foreach (var s in seriler)
            {
                string text = s.BaslangicTarihi.ConvertToDatetimeEmptyIfNull() + "-" + s.BitisTarihi.ConvertToDatetimeEmptyIfNull();
                ddl.Items.Add(new ListItem(text, s.SeriId.ToString()));
            }
        }
    }
}
