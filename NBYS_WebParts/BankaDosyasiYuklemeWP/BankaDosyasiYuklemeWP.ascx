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
    .bdy-overview { display: grid; grid-template-columns: minmax(260px, 1.4fr) repeat(2, minmax(150px, 1fr)); gap: 12px; margin-bottom: 24px; }
    .bdy-overview-card { min-height: 105px; padding: 18px 20px; border: 1px solid var(--bdy-line); border-radius: 15px; background: var(--bdy-surface); box-shadow: 0 2px 7px #203a5a08; }
    .bdy-overview-label { display: block; margin-bottom: 10px; color: var(--bdy-muted); font-size: 13px; font-weight: 700; }
    .bdy-overview-value { display: flex; align-items: baseline; gap: 7px; }
    .bdy-overview-value strong { color: var(--bdy-ink); font-size: 29px; line-height: 1; }
    .bdy-overview-value span { color: var(--bdy-muted); font-size: 13px; }
    .bdy-date-input { display: block; width: 220px; max-width: 100%; min-height: 40px; padding: 9px 12px; border: 1px solid #cbd7e5; border-radius: 8px; background: #fff; color: var(--bdy-ink); font-size: 15px; }
    .bdy-section { margin-bottom: 25px; }
    .bdy-section-heading { display: flex; justify-content: space-between; align-items: baseline; gap: 12px; margin-bottom: 11px; }
    .bdy-section-heading h2 { margin: 0; color: var(--bdy-ink); font-size: 18px; font-weight: 700; }
    .bdy-section-count { color: var(--bdy-muted); font-size: 13px; }
    .bdy-rows { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 9px 12px; }
    .bdy-row { display: grid; grid-template-columns: minmax(140px, 1fr) minmax(190px, 1.1fr); align-items: center; gap: 16px; min-height: 86px; padding: 14px 16px; border: 1px solid var(--bdy-line); border-radius: 11px; background: #fff; box-shadow: 0 1px 4px #243e5c07; }
    .bdy-row:hover { border-color: #b6cbe5; box-shadow: 0 3px 10px #243e5c0d; }
    .bdy-bank-line { display: flex; align-items: center; flex-wrap: wrap; gap: 8px; }
    .bdy-bank-name { color: var(--bdy-ink); font-size: 16px; font-weight: 700; line-height: 1.3; }
    .bdy-server-status { font-size: 17px; font-weight: 800; line-height: 1; }
    .bdy-kind { display: block; margin-top: 4px; color: var(--bdy-muted); font-size: 13px; }
    .bdy-file-input { display: block; width: 100%; min-width: 0; color: #486079; font-size: 13px; }
    .bdy-file-input::file-selector-button { margin-right: 11px; padding: 9px 12px; border: 1px solid #c8d9ee; border-radius: 8px; background: #f8fbff; color: #1256ac; font: 700 13px "Segoe UI", Arial, sans-serif; cursor: pointer; }
    .bdy-file-input:hover::file-selector-button { border-color: #8eb8ed; background: #edf5ff; }
    .bdy-file-input:disabled { opacity: .6; }
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
    @media (max-width: 850px) {
        .bdy { padding: 20px 20px 0; }
        .bdy-overview { grid-template-columns: repeat(2, minmax(0, 1fr)); }
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
        .bdy-row { grid-template-columns: 1fr; gap: 10px; padding: 13px; }
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
        </div>
        <div class="bdy-overview-card">
            <span class="bdy-overview-label">Günlük dosya türü</span>
            <div class="bdy-overview-value"><strong>8</strong><span>alan</span></div>
        </div>
        <div class="bdy-overview-card">
            <span class="bdy-overview-label">Dönemlik dosya türü</span>
            <div class="bdy-overview-value"><strong>13</strong><span>alan</span></div>
        </div>
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
                <asp:FileUpload ID="AkbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Akbank günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="FinansbankLbl" runat="server" CssClass="bdy-bank-name" Text="Finansbank"></asp:Label>
                        <asp:Label ID="FinansbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="FinansbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Finansbank günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="GarantiLbl" runat="server" CssClass="bdy-bank-name" Text="Garanti Bankası"></asp:Label>
                        <asp:Label ID="GarantiOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="GarantiFU" runat="server" CssClass="bdy-file-input" ToolTip="Garanti Bankası günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="HalkbankLbl" runat="server" CssClass="bdy-bank-name" Text="Halkbank"></asp:Label>
                        <asp:Label ID="HalkbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="HalkbankFU" runat="server" CssClass="bdy-file-input" ToolTip="Halkbank günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="IsbankLbl" runat="server" CssClass="bdy-bank-name" Text="İş Bankası"></asp:Label>
                        <asp:Label ID="IsbankOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="IsbankFU" runat="server" CssClass="bdy-file-input" ToolTip="İş Bankası günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="TebLbl" runat="server" CssClass="bdy-bank-name" Text="TEB"></asp:Label>
                        <asp:Label ID="TebOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="TebFU" runat="server" CssClass="bdy-file-input" ToolTip="TEB günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="VakifbankGunlukLbl" runat="server" CssClass="bdy-bank-name" Text="Vakıfbank"></asp:Label>
                        <asp:Label ID="VakifbankGunlukOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="VakifbankGunlukFU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıfbank günlük dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Bankası"></asp:Label>
                        <asp:Label ID="ZiraatOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Günlük dosya</span>
                </div>
                <asp:FileUpload ID="ZiraatFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Bankası günlük dosya seçiniz" />
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
                <asp:FileUpload ID="AkbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Akbank ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="EDevletLbl" runat="server" CssClass="bdy-bank-name" Text="E-Devlet ile Bağış"></asp:Label>
                        <asp:Label ID="EDevletOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="EDevletFU" runat="server" CssClass="bdy-file-input" ToolTip="E-Devlet ile Bağış dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="FinansbankEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Finansbank"></asp:Label>
                        <asp:Label ID="FinansbankEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <asp:FileUpload ID="FinansbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Finansbank ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="GarantiEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Garanti Bankası"></asp:Label>
                        <asp:Label ID="GarantiEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <asp:FileUpload ID="GarantiEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Garanti Bankası ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="Halkbank2Lbl" runat="server" CssClass="bdy-bank-name" Text="Halkbank 2"></asp:Label>
                        <asp:Label ID="Halkbank2OkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="Halkbank2FU" runat="server" CssClass="bdy-file-input" ToolTip="Halkbank 2 dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="IsbankEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="İş Bankası"></asp:Label>
                        <asp:Label ID="IsbankEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <asp:FileUpload ID="IsbankEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="İş Bankası ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="SMSVakifLbl" runat="server" CssClass="bdy-bank-name" Text="SMS Vakıfbank"></asp:Label>
                        <asp:Label ID="SMSVakifOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="SMSVakifFU" runat="server" CssClass="bdy-file-input" ToolTip="SMS Vakıfbank dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="Vakifbank2Lbl" runat="server" CssClass="bdy-bank-name" Text="Vakıfbank 2"></asp:Label>
                        <asp:Label ID="Vakifbank2OkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="Vakifbank2FU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıfbank 2 dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="VakifKatilimLbl" runat="server" CssClass="bdy-bank-name" Text="Vakıf Katılım"></asp:Label>
                        <asp:Label ID="VakifKatilimOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="VakifKatilimFU" runat="server" CssClass="bdy-file-input" ToolTip="Vakıf Katılım dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="YKBEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Yapı Kredi Bankası"></asp:Label>
                        <asp:Label ID="YKBEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <asp:FileUpload ID="YKBEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Yapı Kredi Bankası ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatEkstreLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Bankası"></asp:Label>
                        <asp:Label ID="ZiraatEkstreOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Ekstre</span>
                </div>
                <asp:FileUpload ID="ZiraatEkstreFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Bankası ekstre seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="ZiraatKatilimLbl" runat="server" CssClass="bdy-bank-name" Text="Ziraat Katılım"></asp:Label>
                        <asp:Label ID="ZiraatKatilimOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="ZiraatKatilimFU" runat="server" CssClass="bdy-file-input" ToolTip="Ziraat Katılım dönemlik dosya seçiniz" />
            </div>
            <div class="bdy-row">
                <div>
                    <div class="bdy-bank-line">
                        <asp:Label ID="AlbarakaLbl" runat="server" CssClass="bdy-bank-name" Text="Albaraka"></asp:Label>
                        <asp:Label ID="AlbarakaOkLbl" runat="server" CssClass="bdy-server-status"></asp:Label>
                    </div>
                    <span class="bdy-kind">Dönemlik dosya</span>
                </div>
                <asp:FileUpload ID="AlbarakaFU" runat="server" CssClass="bdy-file-input" ToolTip="Albaraka dönemlik dosya seçiniz" />
            </div>
        </div>
    </section>

    <footer class="bdy-footer">
        <div class="bdy-footer-copy"><strong>Dosyaları yüklemeye hazır mısınız?</strong><span>Dosyaları ilgili banka satırından seçip Kaydet'e basın.</span></div>
        <div class="bdy-actions">
            <asp:LinkButton ID="NextBtn" runat="server" CssClass="bdy-action bdy-action-secondary" Text="Ekstre listesine git →" OnClick="NextBtn_Click" />
            <asp:LinkButton ID="KaydetBtn" runat="server" CssClass="bdy-action bdy-action-primary" Text="Seçilen dosyaları yükle" OnClick="KaydetBtn_Click" />
        </div>
    </footer>
</div>
