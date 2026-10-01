using Model.NBYS;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.NakitBagisciKarti
{
    /// <summary>
    /// Nakit bağışçı özetini Bootstrap modal içinde gösteren yeniden kullanılabilir kontrol.
    /// Sayfada bu kontrol bulunduktan sonra NakitBagisciKartiAc(bagisciId) çağrısı yeterlidir.
    /// </summary>
    [ToolboxData("<{0}:NakitBagisciKarti runat=\"server\"></{0}:NakitBagisciKarti>")]
    public class NakitBagisciKarti : CompositeControl
    {
        private readonly CultureInfo culture = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        private HiddenField bagisciIdField;
        private LinkButton loadButton;
        private Literal content;
        private UpdatePanel updatePanel;

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            bagisciIdField = new HiddenField { ID = "BagisciId" };
            loadButton = new LinkButton
            {
                ID = "KartYukle",
                CausesValidation = false,
                CssClass = "d-none",
                Text = string.Empty
            };
            loadButton.Click += LoadButton_Click;

            content = new Literal { ID = "KartIcerik", Text = EmptyContent() };
            updatePanel = new UpdatePanel
            {
                ID = "KartPanel",
                UpdateMode = UpdatePanelUpdateMode.Conditional,
                ChildrenAsTriggers = true
            };
            // Postback controls must remain inside the SharePoint server form.
            Controls.Add(bagisciIdField);
            Controls.Add(loadButton);
            updatePanel.ContentTemplateContainer.Controls.Add(content);
            Controls.Add(updatePanel);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            EnsureChildControls();
            RegisterStyles();
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(loadButton);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            int bagisciId;
            if (!int.TryParse(bagisciIdField.Value, out bagisciId) || bagisciId < 1)
            {
                content.Text = ErrorContent("Geçerli bir bağışçı seçilemedi.");
                updatePanel.Update();
                ShowModal();
                return;
            }

            try
            {
                NakitBagisci bagisci = new NakitBagisciService().GetById(bagisciId);
                if (bagisci == null)
                {
                    content.Text = ErrorContent("Bağışçı kaydı bulunamadı.");
                }
                else
                {
                    content.Text = BuildContent(bagisci);
                }
            }
            catch
            {
                content.Text = ErrorContent("Bağışçı kartı yüklenirken bir hata oluştu.");
            }

            updatePanel.Update();
            ShowModal();
        }

        private string BuildContent(NakitBagisci bagisci)
        {
            List<NakitBagisHareket> donations = new NakitBagisHareketService()
                .GetByBagisciId(bagisci.Id)
                .OrderByDescending(x => x.BagisTarihi)
                .ToList();
            List<Armagan> gifts = new ArmaganService()
                .GetByBagisciId(bagisci.Id)
                .OrderByDescending(x => x.Tarih)
                .ToList();
            DuzenliNakitBagisci regular = new DuzenliNakitBagisciService()
                .GetActiveByBagisciId(bagisci.Id);

            Dictionary<int, string> banks = new BankaTanimService().GetAll()
                .ToDictionary(x => x.Id, x => x.Banka ?? string.Empty);
            Dictionary<int, string> giftNames = new ArmaganTanimService().GetAll()
                .ToDictionary(x => x.Id, x => x.Armagan ?? string.Empty);

            decimal total = donations.Sum(x => x.BagisMiktari);
            decimal refunds = donations.Where(x => x.IadeEdildiMi).Sum(x => x.IadeMiktari);
            decimal net = total - refunds;
            string province = LocationName<Il>(bagisci.Ili, x => x.IlAdi);
            string district = LocationName<Ilce>(bagisci.Ilcesi, x => x.IlceAdi);
            string fullName = ((bagisci.Adi ?? string.Empty) + " " + (bagisci.Soyadi ?? string.Empty)).Trim();
            string initials = Initials(bagisci.Adi, bagisci.Soyadi);

            StringBuilder html = new StringBuilder();
            html.Append("<div class='modal-header nbk-header bg-primary text-white'>")
                .Append("<div class='nbk-avatar'>").Append(E(initials)).Append("</div>")
                .Append("<div class='flex-grow-1'><h4 class='modal-title mb-1'>").Append(E(fullName)).Append("</h4>")
                .Append("<div class='nbk-meta'>").Append(E(JoinLocation(province, district)))
                .Append(" &middot; Bağışçı No: ").Append(bagisci.Id).Append("</div></div>");
            if (regular != null)
                html.Append("<span class='badge nbk-badge'>Düzenli bağışçı</span>");
            html.Append("<button type='button' class='btn-close btn-close-white' data-bs-dismiss='modal' aria-label='Kapat'></button></div>");

            html.Append("<div class='modal-body nbk-body'>")
                .Append("<div class='nbk-metrics'>")
                .Append(Metric("Net bağış", Money(net), string.Empty))
                .Append(Metric("Bağış", donations.Count + " işlem", string.Empty))
                .Append(Metric("Armağan", gifts.Count + " kayıt", string.Empty))
                .Append(Metric("İade", Money(refunds), refunds > 0 ? "nbk-refund" : string.Empty))
                .Append("</div>")
                .Append("<div class='nbk-contact'>")
                .Append(Contact("Telefon", FirstValue(bagisci.Telefon1, bagisci.Telefon2, "Kayıtlı değil")))
                .Append(Contact("E-posta", FirstValue(bagisci.Eposta, "Kayıtlı değil")))
                .Append(Contact("Adres", FirstValue(bagisci.Adres, "Kayıtlı değil")))
                .Append("</div>")
                .Append(Tabs(bagisci, donations, gifts, regular, banks, giftNames, province, district))
                .Append("</div>")
                .Append("<div class='modal-footer'><span class='me-auto text-muted small'>Son güncelleme: ")
                .Append(Date(bagisci.DegistirmeTarihi == DateTime.MinValue ? bagisci.OlusturmaTarihi : bagisci.DegistirmeTarihi))
                .Append("</span><button type='button' class='btn btn-outline-secondary' data-bs-dismiss='modal'>Kapat</button>")
                .Append("<a class='btn btn-primary' href='").Append(E(ProjeConstants.PAGE_NAKITBAGISCI_EDIT))
                .Append("?NakitBagisciId=").Append(bagisci.Id).Append("'>Bilgileri düzenle</a></div>");
            return html.ToString();
        }

        private string Tabs(NakitBagisci bagisci, IList<NakitBagisHareket> donations,
            IList<Armagan> gifts, DuzenliNakitBagisci regular, IDictionary<int, string> banks,
            IDictionary<int, string> giftNames, string province, string district)
        {
            string suffix = ClientID;
            StringBuilder html = new StringBuilder();
            html.Append("<ul class='nav nav-tabs nbk-tabs' role='tablist'>")
                .Append(TabLink("Özet", "ozet" + suffix, true, null))
                .Append(TabLink("Bağışlar", "bagis" + suffix, false, donations.Count))
                .Append(TabLink("Armağanlar", "armagan" + suffix, false, gifts.Count))
                .Append(TabLink("İadeler", "iade" + suffix, false, donations.Count(x => x.IadeEdildiMi)))
                .Append("</ul><div class='tab-content pt-3'>")
                .Append("<div class='tab-pane show active' id='ozet").Append(suffix).Append("' role='tabpanel'>")
                .Append("<div class='row g-3'><div class='col-md-6'><div class='nbk-info'><h6>Bağışçı bilgileri</h6>")
                .Append(Info("Tür", bagisci.TuzelKisi ? "Tüzel kişi" : "Gerçek kişi"))
                .Append(Info("Meslek", FirstValue(bagisci.Meslek, "Kayıtlı değil")))
                .Append(Info("Konum", JoinLocation(province, district)))
                .Append(Info("Düzenli bağış", regular == null ? "Bulunmuyor" : Money(regular.Tutar) + " / dönem"))
                .Append("</div></div><div class='col-md-6'><div class='nbk-info'><h6>İletişim tercihleri</h6>")
                .Append(Info("Belge gönderimi", bagisci.BelgeIstemiyor ? "İstemiyor" : "Uygun"))
                .Append(Info("Dergi gönderimi", bagisci.DergiGonderilmesin ? "Gönderilmesin" : "Uygun"))
                .Append(Info("Ulaşılabilirlik", bagisci.Ulasilamiyor ? "Ulaşılamıyor" : "Ulaşılabiliyor"))
                .Append(Info("Durum", bagisci.Sag ? "Sağ" : "Vefat"))
                .Append("</div></div></div>");
            if (!string.IsNullOrWhiteSpace(bagisci.Aciklama))
                html.Append("<div class='alert nbk-note mt-3 mb-0'><strong>Not:</strong> ").Append(E(bagisci.Aciklama)).Append("</div>");
            html.Append("</div>")
                .Append("<div class='tab-pane' id='bagis").Append(suffix).Append("' role='tabpanel'>")
                .Append(DonationTable(donations.Take(5), banks)).Append("</div>")
                .Append("<div class='tab-pane' id='armagan").Append(suffix).Append("' role='tabpanel'>")
                .Append(GiftTable(gifts.Take(5), giftNames)).Append("</div>")
                .Append("<div class='tab-pane' id='iade").Append(suffix).Append("' role='tabpanel'>")
                .Append(RefundTable(donations.Where(x => x.IadeEdildiMi).Take(5), banks)).Append("</div></div>");
            return html.ToString();
        }

        private string DonationTable(IEnumerable<NakitBagisHareket> rows, IDictionary<int, string> banks)
        {
            List<NakitBagisHareket> list = rows.ToList();
            if (list.Count == 0) return Empty("Bağış kaydı bulunmuyor.");
            StringBuilder html = TableStart("Tarih", "Bağış tipi", "Banka", "Döviz", "Tutar", "Durum");
            foreach (NakitBagisHareket row in list)
            {
                html.Append("<tr><td>").Append(Date(row.BagisTarihi)).Append("</td><td>")
                    .Append(E(FirstValue(row.BagisTipi, "Bağış"))).Append("</td><td>")
                    .Append(E(Lookup(banks, row.BankaId))).Append("</td><td>").Append(E(row.DovizCinsi))
                    .Append("</td><td class='text-end fw-bold'>").Append(Money(row.BagisMiktari, row.DovizCinsi))
                    .Append("</td><td>").Append(row.IadeEdildiMi ? Badge("İade edildi", "danger") : Badge("Tamamlandı", "success"))
                    .Append("</td></tr>");
            }
            return html.Append("</tbody></table></div>").ToString();
        }

        private string GiftTable(IEnumerable<Armagan> rows, IDictionary<int, string> giftNames)
        {
            List<Armagan> list = rows.ToList();
            if (list.Count == 0) return Empty("Armağan kaydı bulunmuyor.");
            StringBuilder html = TableStart("Tarih", "Armağan", "Belgede yazan isim", "İlişkili bağış", "Durum");
            foreach (Armagan row in list)
            {
                html.Append("<tr><td>").Append(Date(row.Tarih)).Append("</td><td>")
                    .Append(E(Lookup(giftNames, row.ArmaganTanimId))).Append("</td><td>")
                    .Append(E(FirstValue(row.BelgedeYazanIsim, "—"))).Append("</td><td class='text-end fw-bold'>")
                    .Append(Money(row.BagisMiktari, row.DovizCinsi)).Append("</td><td>")
                    .Append(Badge(FirstValue(row.Durum, "Belirsiz"), "secondary")).Append("</td></tr>");
            }
            return html.Append("</tbody></table></div>").ToString();
        }

        private string RefundTable(IEnumerable<NakitBagisHareket> rows, IDictionary<int, string> banks)
        {
            List<NakitBagisHareket> list = rows.ToList();
            if (list.Count == 0) return Empty("İade işlemi bulunmuyor.");
            StringBuilder html = TableStart("İade tarihi", "Bağış tarihi", "Banka", "Sebep", "İade eden", "Tutar");
            foreach (NakitBagisHareket row in list)
            {
                html.Append("<tr><td>").Append(Date(row.IadeTarihi)).Append("</td><td>")
                    .Append(Date(row.BagisTarihi)).Append("</td><td>").Append(E(Lookup(banks, row.BankaId)))
                    .Append("</td><td>").Append(E(FirstValue(row.IadeSebebi, "—"))).Append("</td><td>")
                    .Append(E(FirstValue(row.IadeEden, "—"))).Append("</td><td class='text-end fw-bold text-danger'>")
                    .Append(Money(row.IadeMiktari, row.DovizCinsi)).Append("</td></tr>");
            }
            return html.Append("</tbody></table></div>").ToString();
        }

        private void RegisterStyles()
        {
            string css = @"<style>
.nbk-modal{z-index:1055 !important;opacity:1 !important;filter:alpha(opacity=100) !important}.nbk-modal.show{display:block !important;opacity:1 !important;transform:none !important}.nbk-modal .modal-dialog{max-width:960px;opacity:1 !important;filter:alpha(opacity=100) !important;transform:none !important}.nbk-header{background:linear-gradient(112deg,#132836,#1b4f57);color:#fff}.nbk-avatar{width:50px;height:50px;border-radius:13px;background:#efc56d;color:#1a303b;display:grid;place-items:center;font-weight:800;margin-right:14px}.nbk-meta{color:#c2d1d5;font-size:.82rem}.nbk-badge{background:#efc56d;color:#20323a;margin-right:12px}.nbk-body{background:#fff}.nbk-metrics{display:grid;grid-template-columns:repeat(4,1fr);gap:9px;margin-bottom:13px}.nbk-metric{border:1px solid #dce3e6;border-radius:11px;padding:10px 12px}.nbk-metric small{display:block;color:#67757d;font-size:.7rem;font-weight:700;text-transform:uppercase}.nbk-metric strong{display:block;font-size:1.05rem;margin-top:3px}.nbk-refund strong{color:#ad4c47}.nbk-contact{display:grid;grid-template-columns:1fr 1.25fr 1.75fr;border:1px solid #dce3e6;border-radius:11px;background:#f9fbfb;margin-bottom:13px}.nbk-contact-item{padding:9px 12px;border-right:1px solid #dce3e6;min-width:0}.nbk-contact-item:last-child{border:0}.nbk-contact-item small{display:block;color:#67757d;font-weight:700;text-transform:uppercase}.nbk-contact-item span{display:block;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}.nbk-tabs .nav-link{font-weight:700;color:#67757d}.nbk-tabs .nav-link.active{color:#132836}.nbk-info{border:1px solid #dce3e6;border-radius:11px;padding:13px 14px;height:100%}.nbk-info h6{text-transform:uppercase;color:#53636b}.nbk-info-row{display:flex;justify-content:space-between;gap:12px;padding:4px 0;font-size:.86rem}.nbk-info-row span{color:#67757d}.nbk-note{background:#fff4dd;border-left:3px solid #d9a23d}.nbk-table{font-size:.84rem}.nbk-empty{padding:30px;text-align:center;color:#67757d}@media(max-width:700px){.nbk-modal .modal-dialog{margin:0;max-width:none;height:100%}.nbk-modal .modal-content{min-height:100%;border-radius:0}.nbk-metrics{grid-template-columns:1fr 1fr}.nbk-contact{grid-template-columns:1fr}.nbk-contact-item{border-right:0;border-bottom:1px solid #dce3e6}}
.nbk-modal{font-size:14px;line-height:1.5}
.nbk-modal .nbk-header{background-image:none;background-color:var(--bs-primary,#0d6efd);color:var(--bs-white,#fff)}
.nbk-modal .modal-title{font-size:20px;color:inherit}
.nbk-modal .nbk-avatar,.nbk-modal .nbk-badge{background:var(--bs-light,#f8f9fa);color:var(--bs-dark,#212529)}
.nbk-modal .nbk-meta{font-size:14px;color:inherit}
.nbk-modal .nbk-metric small,.nbk-modal .nbk-contact-item small{font-size:13px}
.nbk-modal .nbk-metric strong{font-size:18px}
.nbk-modal .nbk-contact-item span,.nbk-modal .nbk-tabs .nav-link,.nbk-modal .nbk-info-row,.nbk-modal .nbk-table,.nbk-modal .modal-footer .small{font-size:14px}
.nbk-modal .nbk-info h6{font-size:16px}
.nbk-modal .nbk-tabs .badge,.nbk-modal .nbk-table .badge{font-size:12px}
.nbk-modal .tab-content>.tab-pane{display:none}
.nbk-modal .tab-content>.tab-pane.active{display:block;opacity:1;visibility:visible}
</style>";
            if (Page.Header != null && Page.Header.FindControl("NakitBagisciKartiCss") == null)
            {
                Literal cssLiteral = new Literal { ID = "NakitBagisciKartiCss", Text = css };
                Page.Header.Controls.Add(cssLiteral);
            }
        }

        private void ShowModal()
        {
            string script = "var modalElement=document.getElementById('" + ClientID + "_Modal');if(modalElement){NakitBagisciKartiHazirla(modalElement);bootstrap.Modal.getOrCreateInstance(modalElement).show();}";
            ScriptManager.RegisterStartupScript(updatePanel, GetType(), ClientID + "_show", script, true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            bagisciIdField.RenderControl(writer);
            loadButton.RenderControl(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal fade nbk-modal");
            writer.AddAttribute("data-nbk-id-field", bagisciIdField.ClientID);
            writer.AddAttribute("data-nbk-load-button", loadButton.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_Modal");
            writer.AddAttribute("tabindex", "-1");
            writer.AddAttribute("aria-hidden", "true");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-dialog modal-dialog-centered modal-dialog-scrollable");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "modal-content");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            updatePanel.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            RenderClientScript(writer);
        }

        private void RenderClientScript(HtmlTextWriter writer)
        {
            string script = string.Format(@"<script type=""text/javascript"">
window.NakitBagisciKartiHazirla = window.NakitBagisciKartiHazirla || function (modalElement) {{
    if (modalElement && modalElement.parentNode !== document.body) document.body.appendChild(modalElement);
    return modalElement;
}};
window.NakitBagisciKartiAc = function (bagisciId) {{
    var idField = document.getElementById('{0}');
    var loadButton = document.getElementById('{1}');
    var modalElement = document.getElementById('{2}');
    if (!idField || !loadButton || !modalElement || !bagisciId) return false;
    idField.value = bagisciId;
    NakitBagisciKartiHazirla(modalElement);
    bootstrap.Modal.getOrCreateInstance(modalElement).show();
    loadButton.click();
    return false;
}};
</script>", bagisciIdField.ClientID, loadButton.ClientID, ClientID + "_Modal");
            writer.Write(script);
        }

        private string EmptyContent()
        {
            return "<div class='modal-header nbk-header bg-primary text-white'><h4 class='modal-title'>Nakit Bağışçı Kartı</h4>" +
                "<button type='button' class='btn-close btn-close-white' data-bs-dismiss='modal' aria-label='Kapat'></button></div>" +
                "<div class='modal-body nbk-empty'>Bağışçı bilgileri yükleniyor...</div>";
        }

        private string ErrorContent(string message)
        {
            return "<div class='modal-header nbk-header bg-primary text-white'><h4 class='modal-title'>Nakit Bağışçı Kartı</h4>" +
                "<button type='button' class='btn-close btn-close-white' data-bs-dismiss='modal' aria-label='Kapat'></button></div>" +
                "<div class='modal-body'><div class='alert alert-danger mb-0'>" + E(message) + "</div></div>";
        }

        private static string Metric(string label, string value, string css)
        {
            return "<div class='nbk-metric " + css + "'><small>" + E(label) + "</small><strong>" + E(value) + "</strong></div>";
        }

        private static string Contact(string label, string value)
        {
            return "<div class='nbk-contact-item'><small>" + E(label) + "</small><span title='" + E(value) + "'>" + E(value) + "</span></div>";
        }

        private static string Info(string label, string value)
        {
            return "<div class='nbk-info-row'><span>" + E(label) + "</span><strong>" + E(value) + "</strong></div>";
        }

        private static string TabLink(string text, string target, bool active, int? count)
        {
            string badge = count.HasValue ? " <span class='badge bg-light text-dark'>" + count.Value + "</span>" : string.Empty;
            return "<li class='nav-item' role='presentation'><button class='nav-link " + (active ? "active" : string.Empty) +
                "' data-bs-toggle='tab' data-bs-target='#" + target + "' type='button' role='tab'>" + E(text) + badge + "</button></li>";
        }

        private static StringBuilder TableStart(params string[] headers)
        {
            StringBuilder html = new StringBuilder("<div class='table-responsive'><table class='table table-sm table-hover align-middle nbk-table'><thead><tr>");
            foreach (string header in headers) html.Append("<th>").Append(E(header)).Append("</th>");
            return html.Append("</tr></thead><tbody>");
        }

        private static string Badge(string text, string type)
        {
            return "<span class='badge bg-" + type + "'>" + E(text) + "</span>";
        }

        private static string Empty(string text)
        {
            return "<div class='nbk-empty'>" + E(text) + "</div>";
        }

        private string Money(decimal value, string currency = null)
        {
            string code = string.IsNullOrWhiteSpace(currency) || currency == ProjeConstants.DOVIZ_TL ? "₺" : currency + " ";
            return code + value.ToString("N2", culture);
        }

        private static string Date(DateTime value)
        {
            return value == DateTime.MinValue ? "—" : value.ToString("dd.MM.yyyy");
        }

        private static string FirstValue(params string[] values)
        {
            return values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
        }

        private static string Lookup(IDictionary<int, string> values, int id)
        {
            string value;
            return id > 0 && values.TryGetValue(id, out value) && !string.IsNullOrWhiteSpace(value) ? value : "—";
        }

        private static string JoinLocation(string province, string district)
        {
            if (string.IsNullOrWhiteSpace(province)) return FirstValue(district, "Konum bilgisi yok");
            return string.IsNullOrWhiteSpace(district) ? province : province + " / " + district;
        }

        private static string Initials(string firstName, string lastName)
        {
            string first = string.IsNullOrWhiteSpace(firstName) ? string.Empty : firstName.Trim().Substring(0, 1);
            string last = string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim().Substring(0, 1);
            return (first + last).ToUpper(new CultureInfo("tr-TR"));
        }

        private static string LocationName<T>(int id, Func<T, string> selector) where T : ParentClass, new()
        {
            if (id < 1) return string.Empty;
            T value = new T().Select<T>(id);
            return value == null ? string.Empty : selector(value);
        }

        private static string E(object value)
        {
            return HttpUtility.HtmlEncode(value == null ? string.Empty : value.ToString());
        }
    }
}
