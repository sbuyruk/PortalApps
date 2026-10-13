<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankaDosyasiYuklemeWP.ascx.cs" Inherits="NBYS_WebParts.BankaDosyasiYuklemeWP.BankaDosyasiYuklemeWP" %>

<style type="text/css">
    .bdy, .bdy * { box-sizing: border-box; }
    .bdy { --bdy-ink: #15263e; --bdy-muted: #64748b; --bdy-line: #e1e8f0; --bdy-blue: #1768d2; --bdy-surface: #fff; max-width: 1440px; margin: 0 auto; padding: 24px 32px 0; background: #f5f7fb; color: var(--bdy-ink); font-family: "Segoe UI", Arial, sans-serif; font-size: 15px; }
    .bdy a { text-decoration: none; }
    .bdy-header { position: relative; margin-bottom: 24px; padding-right: 50px; }
    .bdy-brand { display: inline-flex; align-items: center; gap: 10px; margin-bottom: 24px; color: #29405c; font-weight: 700; }
    .bdy-brand-mark { display: inline-grid; width: 34px; height: 34px; place-items: center; border-radius: 10px; background: var(--bdy-blue); color: #fff; font-size: 19px; }
    .bdy-eyebrow { display: block; margin-bottom: 9px; color: var(--bdy-blue); font-size: 12px; font-weight: 800; letter-spacing: .12em; text-transform: uppercase; }
    .bdy-title { margin: 0; color: var(--bdy-ink); font-size: clamp(27px, 3vw, 36px); font-weight: 700; letter-spacing: -.035em; line-height: 1.15; }
    .bdy-subtitle { margin: 9px 0 0; color: var(--bdy-muted); font-size: 15px; line-height: 1.5; }
    .bdy-close { position: absolute; top: 0; right: 0; display: grid; width: 36px; height: 36px; place-items: center; border: 1px solid var(--bdy-line); border-radius: 9px; background: #fff; color: #60748b; font-size: 22px; line-height: 1; }
    .bdy-close:hover { background: #edf5ff; color: #0d4fa8; }
    .bdy-overview { display: grid; grid-template-columns: minmax(260px, 1.4fr) minmax(130px, 1fr); gap: 12px; margin-bottom: 24px; }
    .bdy-enhanced .bdy-overview { grid-template-columns: minmax(260px, 1.4fr) repeat(3, minmax(130px, 1fr)); }
    .bdy-overview-card { min-height: 105px; padding: 18px 20px; border: 1px solid var(--bdy-line); border-radius: 15px; background: var(--bdy-surface); box-shadow: 0 2px 7px #203a5a08; }
    .bdy-overview-label { display: block; margin-bottom: 10px; color: var(--bdy-muted); font-size: 13px; font-weight: 700; }
    .bdy-overview-value { display: flex; align-items: baseline; gap: 7px; }
    .bdy-overview-value strong { color: var(--bdy-ink); font-size: 29px; line-height: 1; }
    .bdy-overview-value span { color: var(--bdy-muted); font-size: 13px; }
    .bdy-live-card { display: none; }
    .bdy-enhanced .bdy-live-card { display: block; }
    .bdy-selected-value strong { color: var(--bdy-blue); }
    .bdy-done-value strong { color: #12805c; }
    .bdy-date-input { display: block; width: 220px; max-width: 100%; min-height: 40px; padding: 9px 12px; border: 1px solid #cbd7e5; border-radius: 8px; background: #fff; color: var(--bdy-ink); font-size: 15px; }
    .bdy-date-note { display: block; margin-top: 8px; color: var(--bdy-muted); font-size: 12px; line-height: 1.3; }
    .bdy-toolbar { display: none; align-items: center; justify-content: space-between; flex-wrap: wrap; gap: 12px; margin: 0 0 18px; }
    .bdy-enhanced .bdy-toolbar { display: flex; }
    .bdy-filters { display: flex; flex-wrap: wrap; gap: 5px; padding: 4px; border-radius: 10px; background: #e9eef5; }
    .bdy-filter { padding: 8px 13px; border: 0; border-radius: 7px; background: transparent; color: #52647b; font: 700 14px "Segoe UI", Arial, sans-serif; cursor: pointer; }
    .bdy-filter[aria-pressed="true"] { background: #fff; color: #0d4fa8; box-shadow: 0 1px 5px #23405d1b; }
    .bdy-search { width: min(100%, 310px); min-height: 39px; padding: 9px 12px; border: 1px solid #cbd7e5; border-radius: 9px; background: #fff; color: var(--bdy-ink); font-size: 14px; }
    .bdy-section { margin-bottom: 25px; }
    .bdy-section-heading { display: flex; justify-content: space-between; align-items: baseline; gap: 12px; margin-bottom: 11px; }
    .bdy-section-heading h2 { margin: 0; color: var(--bdy-ink); font-size: 18px; font-weight: 700; }
    .bdy-section-count { color: var(--bdy-muted); font-size: 13px; }
    .bdy-rows { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 9px 12px; }
    .bdy-row { display: grid; grid-template-columns: minmax(140px, 1fr) minmax(190px, 1.1fr); align-items: center; gap: 12px; min-height: 86px; padding: 14px 16px; border: 1px solid var(--bdy-line); border-radius: 11px; background: #fff; box-shadow: 0 1px 4px #243e5c07; }
    .bdy-enhanced .bdy-row { grid-template-columns: minmax(125px, 1.1fr) minmax(95px, .7fr) minmax(150px, 1fr); }
    .bdy-row[hidden] { display: none; }
    .bdy-row:hover { border-color: #b6cbe5; box-shadow: 0 3px 10px #243e5c0d; }
    .bdy-bank-line { display: flex; align-items: center; flex-wrap: wrap; gap: 8px; }
    .bdy-bank-name { color: var(--bdy-ink); font-size: 16px; font-weight: 700; line-height: 1.3; }
    .bdy-server-status { font-size: 17px; font-weight: 800; line-height: 1; }
    .bdy-kind { display: block; margin-top: 4px; color: var(--bdy-muted); font-size: 13px; }
    .bdy-state-label { display: none; width: max-content; max-width: 100%; align-items: center; gap: 6px; padding: 6px 9px; border-radius: 20px; background: #f1f4f8; color: #64758b; font-size: 12px; font-weight: 700; white-space: nowrap; }
    .bdy-enhanced .bdy-state-label { display: inline-flex; }
    .bdy-enhanced .bdy-server-status { display: none; }
    .bdy-state-label::before { content: ""; width: 6px; height: 6px; flex: none; border-radius: 50%; background: #95a2b2; }
    .bdy-row[data-state="selected"] .bdy-state-label { background: #e9f2ff; color: #155db9; }
    .bdy-row[data-state="selected"] .bdy-state-label::before { background: #2581e4; }
    .bdy-row[data-state="done"] .bdy-state-label { background: #e7f6ef; color: #0f7652; }
    .bdy-row[data-state="done"] .bdy-state-label::before { background: #18a471; }
    .bdy-row[data-state="error"] .bdy-state-label { background: #fff0f0; color: #aa333c; }
    .bdy-row[data-state="error"] .bdy-state-label::before { background: #d54851; }
    .bdy-file-side { min-width: 0; }
    .bdy-file-action { display: block; min-width: 0; }
    .bdy-file-action-text, .bdy-filename { display: none; }
    .bdy-file-input { display: block; width: 100%; min-width: 0; color: #486079; font-size: 13px; }
    .bdy-file-input::file-selector-button { margin-right: 11px; padding: 9px 12px; border: 1px solid #c8d9ee; border-radius: 8px; background: #f8fbff; color: #1256ac; font: 700 13px "Segoe UI", Arial, sans-serif; cursor: pointer; }
    .bdy-file-input:hover::file-selector-button { border-color: #8eb8ed; background: #edf5ff; }
    .bdy-file-input:disabled { opacity: .6; }
    .bdy-enhanced .bdy-file-side { display: flex; flex-direction: column; gap: 5px; }
    .bdy-enhanced .bdy-file-action { position: relative; display: flex; min-height: 40px; align-items: center; justify-content: center; overflow: hidden; border: 1px solid #c8d9ee; border-radius: 8px; background: #f8fbff; color: #1256ac; cursor: pointer; }
    .bdy-enhanced .bdy-file-action:hover { border-color: #8eb8ed; background: #edf5ff; }
    .bdy-enhanced .bdy-file-action-text { display: inline; font-size: 13px; font-weight: 700; }
    .bdy-enhanced .bdy-file-input { position: absolute; inset: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer; }
    .bdy-enhanced .bdy-file-input:focus-visible { opacity: 1; background: #fff; }
    .bdy-enhanced .bdy-filename { display: block; overflow: hidden; color: #64748b; font-size: 13px; text-align: center; text-overflow: ellipsis; white-space: nowrap; }
    .bdy-row[data-state="done"] .bdy-file-action { opacity: .6; cursor: not-allowed; }
    .bdy-row[data-state="selected"] .bdy-filename { color: #36516e; }
    .bdy-empty { display: none; padding: 30px 15px; border: 1px dashed #c4d1e1; border-radius: 12px; background: #fff; color: var(--bdy-muted); text-align: center; }
    .bdy-enhanced .bdy-empty:not([hidden]) { display: block; }
    .bdy-empty strong { display: block; margin-bottom: 5px; color: var(--bdy-ink); }
    .bdy-footer { position: sticky; bottom: 0; z-index: 2; display: flex; align-items: center; justify-content: space-between; gap: 20px; margin: 12px -32px 0; padding: 15px 32px; border-top: 1px solid #dce5ef; background: #fffffff2; box-shadow: 0 -8px 28px #142d4910; }
    .bdy-footer-copy strong { display: block; font-size: 14px; }
    .bdy-footer-copy span { display: block; margin-top: 3px; color: var(--bdy-muted); font-size: 13px; }
    .bdy-actions { display: flex; align-items: center; gap: 9px; }
    .bdy-action { display: inline-flex; min-height: 41px; align-items: center; justify-content: center; padding: 0 18px; border-radius: 8px; font-size: 14px; font-weight: 700; white-space: nowrap; }
    .bdy-action-primary { border: 1px solid var(--bdy-blue); background: var(--bdy-blue); color: #fff !important; }
    .bdy-action-primary:hover { background: #0d4fa8; color: #fff !important; }
    .bdy-action-secondary { border: 1px solid #c8d4e2; background: #fff; color: #34516f !important; }
    .bdy-action-secondary:hover { background: #f4f8fc; }
    .bdy :focus-visible { outline: 3px solid #8ab8ff; outline-offset: 2px; }
    @media (max-width: 1150px) {
        .bdy-enhanced .bdy-row { grid-template-columns: minmax(115px, 1fr) minmax(95px, .65fr); }
        .bdy-enhanced .bdy-file-side { grid-column: 1 / -1; }
        .bdy-enhanced .bdy-file-side { flex-direction: row; align-items: center; }
        .bdy-enhanced .bdy-file-action { min-width: 130px; }
        .bdy-enhanced .bdy-filename { min-width: 0; text-align: left; }
    }
    @media (max-width: 850px) {
        .bdy { padding: 20px 20px 0; }
        .bdy-overview { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        .bdy-enhanced .bdy-overview { grid-template-columns: repeat(2, minmax(0, 1fr)); }
        .bdy-overview-card:first-child { grid-column: 1 / -1; }
        .bdy-rows { grid-template-columns: 1fr; }
        .bdy-footer { margin-right: -20px; margin-left: -20px; padding-right: 20px; padding-left: 20px; }
    }
    @media (max-width: 520px) {
        .bdy { padding: 16px 14px 0; }
        .bdy-brand { margin-bottom: 20px; }
        .bdy-subtitle { font-size: 14px; }
        .bdy-overview { gap: 8px; }
        .bdy-overview-card { min-height: 86px; padding: 13px; }
        .bdy-overview-value strong { font-size: 24px; }
        .bdy-toolbar, .bdy-search { width: 100%; }
        .bdy-filters { width: 100%; justify-content: space-between; }
        .bdy-filter { padding: 8px 9px; font-size: 12px; }
        .bdy-row, .bdy-enhanced .bdy-row { grid-template-columns: 1fr; gap: 10px; padding: 13px; }
        .bdy-enhanced .bdy-file-side { grid-column: auto; }
        .bdy-footer { display: block; margin-right: -14px; margin-left: -14px; padding: 12px 14px; }
        .bdy-footer-copy { margin-bottom: 10px; }
        .bdy-actions { width: 100%; }
        .bdy-action { flex: 1; padding: 0 9px; font-size: 12px; }
    }
</style>

<div class="bdy">
    <header class="bdy-header">
        <div class="bdy-brand"><span class="bdy-brand-mark" aria-hidden="true">▤</span><span>Bağış Yönetimi</span></div>
        <asp:LinkButton ID="CloseBtn" runat="server" CssClass="bdy-close" OnClick="CloseBtn_Click" ToolTip="Kapat" aria-label="Kapat">&times;</asp:LinkButton>
        <span class="bdy-eyebrow">Banka işlemleri</span>
        <h1 class="bdy-title">Banka dosyası yükleme</h1>
        <p class="bdy-subtitle">İşlem tarihini belirleyin, bankaya ait dosyaları seçin ve tek seferde yükleyin.</p>
    </header>

    <div class="bdy-overview" aria-label="Dosya türleri özeti">
        <div class="bdy-overview-card">
            <span class="bdy-overview-label">İşlem tarihi</span>
            <asp:TextBox ID="IslemTarihiTxt" runat="server" CssClass="bdy-date-input DateTimePickerV1" AutoPostBack="true" OnTextChanged="islemTarihiTxt_TextChanged" ToolTip="İşlem tarihi"></asp:TextBox>
            <span class="bdy-date-note">Tarihi değiştirince seçili dosyalar temizlenir.</span>
        </div>
        <div class="bdy-overview-card">
            <span class="bdy-overview-label">Dosya türü</span>
            <div class="bdy-overview-value"><strong>21</strong><span>alan</span></div>
        </div>
        <div class="bdy-overview-card bdy-live-card">
            <span class="bdy-overview-label">Seçilen</span>
            <div class="bdy-overview-value bdy-selected-value"><strong class="bdy-selected-count">0</strong><span>dosya</span></div>
        </div>
        <div class="bdy-overview-card bdy-live-card">
            <span class="bdy-overview-label">Yüklenen</span>
            <div class="bdy-overview-value bdy-done-value"><strong class="bdy-done-count">0</strong><span>dosya</span></div>
        </div>
    </div>

    <div class="bdy-toolbar">
        <div class="bdy-filters" role="group" aria-label="Dosya durumuna göre filtrele">
            <button type="button" class="bdy-filter" data-filter="all" aria-pressed="true">Tümü</button>
            <button type="button" class="bdy-filter" data-filter="waiting" aria-pressed="false">Bekleyen</button>
            <button type="button" class="bdy-filter" data-filter="selected" aria-pressed="false">Seçilen</button>
            <button type="button" class="bdy-filter" data-filter="done" aria-pressed="false">Yüklenen</button>
        </div>
        <input class="bdy-search" type="search" placeholder="Banka veya dosya türü ara" aria-label="Banka veya dosya türü ara" />
    </div>

    <section class="bdy-section" aria-labelledby="bdy-daily-title">
        <div class="bdy-section-heading"><h2 id="bdy-daily-title">Günlük dosyalar</h2><span class="bdy-section-count">8 dosya türü</span></div>
        <div class="bdy-rows">
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="AkbankLbl" runat="server" CssClass="bdy-bank-name" Text="Akbank"></asp:Label>
                        <asp:Label ID="AkbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="AkbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Akbank günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="FinansbankLbl" runat="server" CssClass="bdy-bank-name" Text="Finansbank"></asp:Label>
                        <asp:Label ID="FinansbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="FinansbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Finansbank günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="GarantiLbl" runat="server" CssClass="bdy-bank-name" Text="Garanti Bankası"></asp:Label>
                        <asp:Label ID="GarantiOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="GarantiFU" runat="server" CssClass="bdy-file-input" ToolTip="Garanti Bankası günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="HalkbankLbl" runat="server" CssClass="bdy-bank-name" Text="Halkbank"></asp:Label>
                        <asp:Label ID="HalkbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="HalkbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Halkbank günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="IsbankLbl" runat="server" CssClass="bdy-bank-name" Text="İş Bankası"></asp:Label>
                        <asp:Label ID="IsbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="IsbankFU" runat="server" CssClass="bdy-file-input" ToolTip="İş Bankası günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="TebLbl" runat="server" CssClass="bdy-bank-name" Text="TEB"></asp:Label>
                        <asp:Label ID="TebOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="TebFU" runat="server" CssClass="bdy-file-input" ToolTip="TEB günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="VakifbankGunlukLbl" runat="server" CssClass="bdy-bank-name" Text="Vakıfbank"></asp:Label>
                        <asp:Label ID="VakifbankGunlukOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="VakifbankGunlukFU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıfbank günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Bankası"></asp:Label>
                        <asp:Label ID="ZiraatOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="ZiraatFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Bankası günlük dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
        </div>
    </section>

    <section class="bdy-section" aria-labelledby="bdy-periodic-title">
        <div class="bdy-section-heading"><h2 id="bdy-periodic-title">Dönemlik dosyalar</h2><span class="bdy-section-count">13 dosya türü</span></div>
        <div class="bdy-rows">
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="AkbankEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Akbank"></asp:Label>
                        <asp:Label ID="AkbankEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="AkbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Akbank ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="EDevletLbl" runat="server" CssClass="bdy-bank-name" Text="E-Devlet ile Bağış"></asp:Label>
                        <asp:Label ID="EDevletOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="EDevletFU" runat="server" CssClass="bdy-file-input" ToolTip="E-Devlet ile Bağış dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="FinansbankEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Finansbank"></asp:Label>
                        <asp:Label ID="FinansbankEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="FinansbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Finansbank ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="GarantiEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Garanti Bankası"></asp:Label>
                        <asp:Label ID="GarantiEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="GarantiEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Garanti Bankası ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="Halkbank2Lbl" runat="server" CssClass="bdy-bank-name" Text="Halkbank 2"></asp:Label>
                        <asp:Label ID="Halkbank2OkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="Halkbank2FU" runat="server" CssClass="bdy-file-input" ToolTip="Halkbank 2 dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="IsbankEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="İş Bankası"></asp:Label>
                        <asp:Label ID="IsbankEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="IsbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="İş Bankası ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="SMSVakifLbl" runat="server" CssClass="bdy-bank-name" Text="SMS Vakıfbank"></asp:Label>
                        <asp:Label ID="SMSVakifOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="SMSVakifFU" runat="server" CssClass="bdy-file-input" ToolTip="SMS Vakıfbank dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="Vakifbank2Lbl" runat="server" CssClass="bdy-bank-name" Text="Vakıfbank 2"></asp:Label>
                        <asp:Label ID="Vakifbank2OkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="Vakifbank2FU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıfbank 2 dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="VakifKatilimLbl" runat="server" CssClass="bdy-bank-name" Text="Vakıf Katılım"></asp:Label>
                        <asp:Label ID="VakifKatilimOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="VakifKatilimFU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıf Katılım dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="YKBEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Yapı Kredi Bankası"></asp:Label>
                        <asp:Label ID="YKBEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="YKBEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Yapı Kredi Bankası ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Bankası"></asp:Label>
                        <asp:Label ID="ZiraatEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="ZiraatEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Bankası ekstre seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatKatilimLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Katılım"></asp:Label>
                        <asp:Label ID="ZiraatKatilimOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="ZiraatKatilimFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Katılım dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="AlbarakaLbl" runat="server" CssClass="bdy-bank-name" Text="Albaraka"></asp:Label>
                        <asp:Label ID="AlbarakaOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <span class="bdy-state-label" aria-live="polite">Dosya bekleniyor</span>
                <div class="bdy-file-side">
                    <label class="bdy-file-action">
                        <span class="bdy-file-action-text">Dosya seç</span>
                        <asp:FileUpload ID="AlbarakaFU" runat="server" CssClass="bdy-file-input" ToolTip="Albaraka dönemlik dosya seçiniz" />
                    </label>
                    <span class="bdy-filename" aria-live="polite">Dosya seçilmedi</span>
                </div>
            </div>
        </div>
    </section>

    <div class="bdy-empty"><strong>Eşleşen dosya alanı bulunamadı.</strong>Arama veya filtre seçimini değiştirin.</div>

    <footer class="bdy-footer">
        <div class="bdy-footer-copy"><strong class="bdy-footer-title">Yüklenecek dosya seçilmedi</strong><span>Dosyaları ilgili banka satırından seçebilirsiniz.</span></div>
        <div class="bdy-actions">
            <asp:LinkButton ID="NextBtn" runat="server" CssClass="bdy-action bdy-action-secondary" Text="Ekstre listesine git →" OnClick="NextBtn_Click" />
            <asp:LinkButton ID="KaydetBtn" runat="server" CssClass="bdy-action bdy-action-primary" Text="Seçilen dosyaları yükle" OnClick="KaydetBtn_Click" />
        </div>
    </footer>
</div>

<script type="text/javascript">
    (function () {
        var script = document.currentScript;
        var root = script && script.previousElementSibling;
        if (!root || !root.classList.contains('bdy')) return;

        var rows = Array.prototype.slice.call(root.querySelectorAll('.bdy-row'));
        var sections = Array.prototype.slice.call(root.querySelectorAll('.bdy-section'));
        var filters = Array.prototype.slice.call(root.querySelectorAll('.bdy-filter'));
        var search = root.querySelector('.bdy-search');
        var selectedCount = root.querySelector('.bdy-selected-count');
        var doneCount = root.querySelector('.bdy-done-count');
        var footerTitle = root.querySelector('.bdy-footer-title');
        var empty = root.querySelector('.bdy-empty');
        var activeFilter = 'all';

        function stateOf(row) {
            var input = row.querySelector('input[type="file"]');
            var serverStatus = row.querySelector('.bdy-server-status').textContent.trim();
            if (serverStatus === 'X') return 'error';
            if (input.disabled || serverStatus.indexOf('√') !== -1) return 'done';
            if (input.files && input.files.length) return 'selected';
            return 'waiting';
        }

        function update() {
            var selected = 0;
            var done = 0;
            var visible = 0;
            var term = search.value.trim().toLocaleLowerCase('tr-TR');

            rows.forEach(function (row) {
                var input = row.querySelector('input[type="file"]');
                var state = stateOf(row);
                var file = input.files && input.files[0];
                var name = row.querySelector('.bdy-bank-name').textContent;
                var kind = row.querySelector('.bdy-kind').textContent;
                row.setAttribute('data-state', state);
                row.querySelector('.bdy-state-label').textContent = {
                    waiting: 'Dosya bekleniyor', selected: 'Seçildi', done: 'Yüklendi', error: 'Hata'
                }[state];
                row.querySelector('.bdy-file-action-text').textContent =
                    state === 'selected' ? 'Değiştir' : state === 'done' ? 'Yüklendi' : 'Dosya seç';
                row.querySelector('.bdy-filename').textContent = file ? file.name :
                    state === 'done' ? 'Bu tarih için yüklendi' : state === 'error' ? 'Yeniden dosya seçebilirsiniz' : 'Dosya seçilmedi';

                if (state === 'selected') selected++;
                if (state === 'done') done++;
                var matches = (!term || (name + ' ' + kind).toLocaleLowerCase('tr-TR').indexOf(term) !== -1) &&
                    (activeFilter === 'all' || state === activeFilter);
                row.hidden = !matches;
                if (matches) visible++;
            });

            selectedCount.textContent = selected;
            doneCount.textContent = done;
            footerTitle.textContent = selected ? selected + ' dosya yüklemeye hazır' :
                done ? done + ' dosya yüklendi' : 'Yüklenecek dosya seçilmedi';
            sections.forEach(function (section) {
                var shown = section.querySelectorAll('.bdy-row:not([hidden])').length;
                section.hidden = shown === 0;
                section.querySelector('.bdy-section-count').textContent = shown + ' dosya türü';
            });
            empty.hidden = visible !== 0;
        }

        function setFilter(value) {
            activeFilter = value;
            filters.forEach(function (button) {
                button.setAttribute('aria-pressed', String(button.getAttribute('data-filter') === value));
            });
            update();
        }

        rows.forEach(function (row) {
            row.querySelector('input[type="file"]').addEventListener('change', function () {
                setFilter('all');
            });
        });
        filters.forEach(function (button) {
            button.addEventListener('click', function () { setFilter(button.getAttribute('data-filter')); });
        });
        search.addEventListener('input', update);
        search.addEventListener('keydown', function (event) {
            if (event.key === 'Enter') event.preventDefault();
        });
        update();
        root.classList.add('bdy-enhanced');
    }());
</script>
