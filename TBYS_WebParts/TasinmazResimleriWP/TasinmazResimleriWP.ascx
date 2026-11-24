<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazResimleriWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazResimleriWP.TasinmazResimleriWP" %>
<style>
    .tbys-property {
        --tbys-page: #f3f6f9;
        --tbys-surface: #fff;
        --tbys-line: #dce5ec;
        --tbys-ink: #1b2d40;
        --tbys-muted: #516477;
        --tbys-accent: #07595c;
        --tbys-accent-soft: #e7f3f1;
        --tbys-danger: #923236;
        color: var(--tbys-ink);
        font-size: 16px;
    }
    .tbys-property .tbys-screen {
        background: var(--tbys-page);
        border: 1px solid var(--tbys-line);
        border-radius: 1rem;
    }
    .tbys-property .tbys-toolbar,
    .tbys-property .tbys-footer {
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: .75rem;
        padding: .75rem clamp(.75rem, 2vw, 1.5rem);
        background: var(--tbys-surface);
    }
    .tbys-property .tbys-toolbar { border-bottom: 1px solid var(--tbys-line); border-radius: 1rem 1rem 0 0; }
    .tbys-property .tbys-footer { border-top: 1px solid var(--tbys-line); border-radius: 0 0 1rem 1rem; }
    .tbys-property .tbys-route { color: var(--tbys-muted); font-size: 14px; }
    .tbys-property .tbys-route strong { color: var(--tbys-ink); }
    .tbys-property .tbys-toolbar-actions,
    .tbys-property .tbys-update-info { display: flex; flex-wrap: wrap; align-items: center; gap: .5rem; }
    .tbys-property .tbys-update-info,
    .tbys-property .tbys-record-id { color: var(--tbys-muted); font-size: 14px; }
    .tbys-property .tbys-toolbar-actions .btn,
    .tbys-property .tbys-save { font-size: 14px; }
    .tbys-property .tbys-save { margin-inline-start: auto; color: #fff; background: var(--tbys-accent); border-color: var(--tbys-accent); }
    .tbys-property .tbys-save:hover { color: #fff; background: #064749; border-color: #064749; }
    .tbys-property .tbys-main { padding: clamp(.75rem, 2vw, 1.5rem); }
    .tbys-property .tbys-summary,
    .tbys-property .tbys-panel {
        min-width: 0;
        background: var(--tbys-surface);
        border: 1px solid var(--tbys-line);
        border-radius: .75rem;
    }
    .tbys-property .tbys-summary { padding: 1rem 1.125rem; margin-bottom: 1rem; }
    .tbys-property .tbys-title { color: var(--tbys-ink); font-size: clamp(20px, 2vw, 25px); margin: 0 0 .25rem; font-weight: 700; }
    .tbys-property .tbys-location {
        display: flex;
        flex-wrap: wrap;
        gap: .65rem 1.5rem;
        margin: .8rem 0 0;
        padding-top: .75rem;
        border-top: 1px solid var(--tbys-line);
    }
    .tbys-property .tbys-location > div { flex: 1 1 6rem; min-width: 0; }
    .tbys-property .tbys-location .tbys-address { flex: 4 1 16rem; }
    .tbys-property .tbys-location dt { color: var(--tbys-muted); font-size: 14px; font-weight: 400; }
    .tbys-property .tbys-location dd { margin: .15rem 0 0; font-weight: 600; overflow-wrap: anywhere; }
    .tbys-property .tbys-layout {
        display: flex;
        flex-wrap: wrap;
        gap: 12px;
        align-items: flex-start;
    }
    .tbys-property .tbys-gallery-panel { flex: 8 1 500px; }
    .tbys-property .tbys-documents-panel { flex: 1 1 300px; }
    .tbys-property .tbys-panel-header {
        display: flex;
        align-items: baseline;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: .35rem .75rem;
        padding: .875rem 1rem;
        border-bottom: 1px solid var(--tbys-line);
    }
    .tbys-property .tbys-panel-header h2 { margin: 0; font-size: 18px; font-weight: 700; color: var(--tbys-ink); }
    .tbys-property .tbys-count { color: var(--tbys-muted); font-size: 13px; white-space: nowrap; }
    .tbys-property .tbys-photo-grid {
        --tbys-photo-gap: 8px;
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(min(100%, max(165px, calc((100% - 3 * var(--tbys-photo-gap)) / 4))), 1fr));
        gap: var(--tbys-photo-gap);
        margin: 0;
        padding: .8rem;
        list-style: none;
    }
    .tbys-property .tbys-photo { min-width: 0; border: 1px solid var(--tbys-line); border-radius: .55rem; overflow: hidden; }
    .tbys-property .tbys-photo-preview { display: block; width: 100%; padding: 0; border: 0; background: var(--tbys-page); }
    .tbys-property .tbys-photo-preview img { display: block; width: 100%; aspect-ratio: 16 / 9; max-height: 120px; object-fit: cover; }
    .tbys-property .tbys-photo-empty { display: none; min-height: 2.5rem; padding: .45rem; place-items: center; background: var(--tbys-page); color: var(--tbys-muted); font-size: 13px; }
    .tbys-property .tbys-photo:has(img[data-photo-stored="false"]:not([data-photo-preview])) .tbys-photo-preview { display: none; }
    .tbys-property .tbys-photo:has(img[data-photo-stored="false"]:not([data-photo-preview])) .tbys-photo-empty { display: grid; }
    .tbys-property .tbys-photo:has(img[data-photo-stored="false"]) .tbys-remove { display: none; }
    .tbys-property .tbys-photo-body { display: grid; gap: .35rem; padding: .55rem; }
    .tbys-property .tbys-photo-label { display: flex; align-items: center; justify-content: space-between; gap: .35rem; font-size: 14px; font-weight: 600; }
    .tbys-property .tbys-photo-label label { margin: 0; overflow-wrap: anywhere; }
    .tbys-property .tbys-documents { list-style: none; margin: 0; padding: 0 1rem; }
    .tbys-property .tbys-document {
        display: flex;
        flex-wrap: wrap;
        align-items: center;
        justify-content: space-between;
        gap: .35rem .75rem;
        padding: .65rem 0;
        min-height: 3.5rem;
        border-bottom: 1px solid var(--tbys-line);
    }
    .tbys-property .tbys-document:last-child { border-bottom: 0; }
    .tbys-property .tbys-document-name { flex: 1 1 120px; min-width: 0; font-size: 15px; font-weight: 600; overflow-wrap: anywhere; }
    .tbys-property .tbys-document-status { display: block; color: var(--tbys-muted); font-size: 13px; font-weight: 400; }
    .tbys-property .tbys-document-ready { display: none; color: var(--tbys-accent); }
    .tbys-property .tbys-document:has(.tbys-document-open) .tbys-document-ready { display: block; }
    .tbys-property .tbys-document:has(.tbys-document-open) .tbys-document-missing { display: none; }
    .tbys-property .tbys-document-actions { display: flex; align-items: center; flex-wrap: wrap; justify-content: end; gap: .35rem; max-width: 100%; margin-inline-start: auto; }
    .tbys-property .tbys-document-actions .tbys-file { width: 145px; }
    .tbys-property .tbys-file { display: block; width: 100%; min-width: 0; max-width: 100%; color: var(--tbys-muted); font-size: 13px; }
    .tbys-property .tbys-file::file-selector-button {
        padding: .35rem .5rem;
        margin-inline-end: .35rem;
        border: 1px solid var(--tbys-line);
        border-radius: .35rem;
        background: var(--tbys-surface);
        color: var(--tbys-ink);
    }
    .tbys-property .tbys-document-open,
    .tbys-property .tbys-remove {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-height: 26px;
        padding: .2rem .45rem;
        border-radius: .35rem;
        font-size: 13px;
        font-weight: 600;
        color: var(--tbys-accent);
        text-decoration: none;
        white-space: nowrap;
    }
    .tbys-property .tbys-remove { color: var(--tbys-danger); }
    .tbys-property .tbys-document-open:hover { background: var(--tbys-accent-soft); }
    .tbys-property .tbys-remove:hover { background: #fbecee; }
    .tbys-property :focus-visible { outline: 2px solid var(--tbys-accent); outline-offset: 2px; }
    .tbys-property .tbys-photo-preview:focus-visible { outline-offset: -3px; }
    .tbys-property .tbys-modal-image { display: block; width: 100%; max-height: 76vh; object-fit: contain; }
</style>
<script type="text/javascript">
    function readURL(input, sender) {
        var image = document.getElementById(sender);
        if (!image) return;

        if (!input.files || !input.files[0]) {
            if (image.dataset.originalSrc) {
                image.src = image.dataset.originalSrc;
            } else if (image.hasAttribute('data-original-src')) {
                image.removeAttribute('src');
            }
            delete image.dataset.photoPreview;
            return;
        }

        if (!image.hasAttribute('data-original-src')) {
            image.dataset.originalSrc = image.getAttribute('src') || '';
        }
        var selectedFile = input.files[0];
        var reader = new FileReader();
        reader.onload = function (event) {
            if (input.files[0] !== selectedFile) return;

            image.dataset.photoPreview = "true";
            image.src = event.target.result;
        };
        reader.readAsDataURL(selectedFile);
    }

    function OpenModal(clickedImg) {
        if (!clickedImg || !clickedImg.getAttribute('src')) return;

        var modalImage = document.getElementById('MaximizedImg');
        modalImage.src = clickedImg.src;
        modalImage.alt = clickedImg.alt;
        document.getElementById('ResimAcModalBaslik').textContent = clickedImg.alt;
        bootstrap.Modal.getOrCreateInstance(document.getElementById('ResimAcModal')).show();
    }

    function showDefaultImage(img) {
        img.onerror = null;
        var baseUrl = document.getElementById('TasinmazResimBaseUrl').value;
        if (baseUrl) {
            img.src = new URL('/OrtakResimler/TasinmazFoto.jpg', baseUrl).href;
        }
    }
</script>
<asp:HiddenField ID="TasinmazResimBaseUrl" runat="server" ClientIDMode="Static" />
<div class="tbys-property">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="tbys-screen">
                <div class="tbys-toolbar" id="CardHeader" runat="server">
                    <div class="tbys-route"><strong>TBYS</strong> <span aria-hidden="true">›</span> Taşınmazlar</div>
                    <div class="tbys-toolbar-actions">
                        <asp:LinkButton ID="BackBtn" CssClass="btn btn-sm btn-outline-secondary" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                        <asp:LinkButton ID="CloseBtn" CssClass="btn btn-sm btn-outline-secondary" runat="server" Text="Kapat" CausesValidation="false" OnClick="CloseBtn_Click" />
                    </div>
                </div>
                <div class="tbys-main" id="MainCardDiv" runat="server">
                    <section class="tbys-summary" aria-label="Taşınmaz bilgileri">
                        <h1 class="tbys-title"><asp:Label ID="TitleLbl" runat="server" Text="Taşınmaz Resimleri ve Belgeleri" /></h1>
                        <asp:Label ID="IdLbl" CssClass="tbys-record-id" runat="server" />
                        <dl class="tbys-location">
                            <div class="tbys-address">
                                <dt>Adres</dt>
                                <dd><asp:Label ID="AdiLbl" runat="server" /></dd>
                            </div>
                            <div>
                                <dt>İlçe</dt>
                                <dd><asp:Label ID="IlceLbl" runat="server" /></dd>
                            </div>
                            <div>
                                <dt>İl</dt>
                                <dd><asp:Label ID="IlLbl" runat="server" /></dd>
                            </div>
                        </dl>
                    </section>
                    <div class="tbys-layout">
                        <section class="tbys-panel tbys-gallery-panel" aria-label="Fotoğraflar">
                            <div class="tbys-panel-header">
                                <h2>Fotoğraflar</h2>
                                <asp:Label ID="FotografSayisiLbl" CssClass="tbys-count" runat="server" />
                            </div>
                            <ul class="tbys-photo-grid">
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Taşınmaz fotoğrafı önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image1" ClientIDMode="Static" runat="server" AlternateText="Taşınmaz fotoğrafı" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload0" Text="Taşınmaz fotoğrafı" />
                                        <asp:LinkButton ID="ResimSil1Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Taşınmaz fotoğrafı dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil1Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload0" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image1')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Taşınmaz fotoğrafı 2 önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image2" ClientIDMode="Static" runat="server" AlternateText="Taşınmaz fotoğrafı 2" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload1" Text="Taşınmaz fotoğrafı 2" />
                                        <asp:LinkButton ID="ResimSil2Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Taşınmaz fotoğrafı 2 dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil2Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload1" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image2')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Taşınmaz fotoğrafı 3 önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image3" ClientIDMode="Static" runat="server" AlternateText="Taşınmaz fotoğrafı 3" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload2" Text="Taşınmaz fotoğrafı 3" />
                                        <asp:LinkButton ID="ResimSil3Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Taşınmaz fotoğrafı 3 dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil3Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload2" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image3')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Tahkikat fotoğrafı önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image4" ClientIDMode="Static" runat="server" AlternateText="Tahkikat fotoğrafı" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload3" Text="Tahkikat fotoğrafı" />
                                        <asp:LinkButton ID="ResimSil4Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Tahkikat fotoğrafı dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil4Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload3" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image4')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Kroki önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image5" ClientIDMode="Static" runat="server" AlternateText="Kroki" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload4" Text="Kroki" />
                                        <asp:LinkButton ID="ResimSil5Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Kroki dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil5Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload4" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image5')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Tapu fotoğrafı önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image6" ClientIDMode="Static" runat="server" AlternateText="Tapu fotoğrafı" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload5" Text="Tapu fotoğrafı" />
                                        <asp:LinkButton ID="ResimSil6Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Tapu fotoğrafı dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil6Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload5" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image6')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Taşınmaz fotoğrafı 4 önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image7" ClientIDMode="Static" runat="server" AlternateText="Taşınmaz fotoğrafı 4" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload6" Text="Taşınmaz fotoğrafı 4" />
                                        <asp:LinkButton ID="ResimSil7Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Taşınmaz fotoğrafı 4 dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil7Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload6" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image7')" />
                                </div>
                            </li>
                            <li class="tbys-photo">
                                <div class="tbys-photo-media">
                                    <button type="button" class="tbys-photo-preview" aria-label="Taşınmaz fotoğrafı 5 önizlemesini aç" onclick="OpenModal(this.querySelector('img'))">
                                        <asp:Image ID="Image8" ClientIDMode="Static" runat="server" AlternateText="Taşınmaz fotoğrafı 5" data-photo-stored="false" onerror="showDefaultImage(this)" />
                                    </button>
                                    <span class="tbys-photo-empty">Fotoğraf yok</span>
                                </div>
                                <div class="tbys-photo-body">
                                    <div class="tbys-photo-label">
                                        <asp:Label runat="server" AssociatedControlID="FileUpload7" Text="Taşınmaz fotoğrafı 5" />
                                        <asp:LinkButton ID="ResimSil8Btn" CssClass="tbys-remove" runat="server" Text="Sil" aria-label="Taşınmaz fotoğrafı 5 dosyasını sil" CausesValidation="false"
                                            OnClick="ResimSil8Btn_Click" OnClientClick="return confirm('Fotoğrafı silmek istiyor musunuz?');" />
                                    </div>
                                    <asp:FileUpload ID="FileUpload7" CssClass="tbys-file" runat="server" accept=".jpg" onchange="readURL(this,'Image8')" />
                                </div>
                            </li>
                            </ul>
                        </section>
                        <section class="tbys-panel tbys-documents-panel" aria-label="Belgeler">
                            <div class="tbys-panel-header">
                                <h2>Belgeler</h2>
                                <asp:Label ID="BelgeSayisiLbl" CssClass="tbys-count" runat="server" />
                            </div>
                            <ul class="tbys-documents">
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label runat="server" Text="Tapu Senedi (PDF)" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="TapuSenediDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Tapu Senedi (PDF) belgesini aç">Aç</a>
                                    <asp:LinkButton ID="TapuSenediSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Tapu Senedi (PDF) belgesini sil"
                                        OnClick="TapuSenediSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="TapuSenediYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Tapu Senedi (PDF) için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label ID="Label2" runat="server" Text="Emlak Beyanı" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="EmlakBeyaniDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Emlak Beyanı belgesini aç">Aç</a>
                                    <asp:LinkButton ID="EmlakBeyaniSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Emlak Beyanı belgesini sil"
                                        OnClick="EmlakBeyaniSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="EmlakBeyaniYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Emlak Beyanı için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label ID="Label1" runat="server" Text="Yapı Kayıt Belgesi" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="YapiKayitDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Yapı Kayıt Belgesi belgesini aç">Aç</a>
                                    <asp:LinkButton ID="YapiKayitSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Yapı Kayıt Belgesi belgesini sil"
                                        OnClick="YapiKayitSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="YapiKayitYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Yapı Kayıt Belgesi için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label ID="Label3" runat="server" Text="Tapu Kayıt Belgesi" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="TapuKayitDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Tapu Kayıt Belgesi belgesini aç">Aç</a>
                                    <asp:LinkButton ID="TapuKayitSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Tapu Kayıt Belgesi belgesini sil"
                                        OnClick="TapuKayitSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="TapuKayitYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Tapu Kayıt Belgesi için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label ID="Label4" runat="server" Text="İmar Durumu Belgesi" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="ImarDurumuDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="İmar Durumu Belgesi belgesini aç">Aç</a>
                                    <asp:LinkButton ID="ImarDurumuSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="İmar Durumu Belgesi belgesini sil"
                                        OnClick="ImarDurumuSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="ImarDurumuYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="İmar Durumu Belgesi için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label ID="Label5" runat="server" Text="Taşınmaz Raporu" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="TasinmazRaporuDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Taşınmaz Raporu belgesini aç">Aç</a>
                                    <asp:LinkButton ID="TasinmazRaporuSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Taşınmaz Raporu belgesini sil"
                                        OnClick="TasinmazRaporuSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="TasinmazRaporuYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Taşınmaz Raporu için PDF dosyası seç" />
                                </div>
                            </li>
                            <li class="tbys-document">
                                <div class="tbys-document-name">
                                    <asp:Label runat="server" Text="Vakıflar Taşınmaz Bildirimi" />
                                    <span class="tbys-document-status tbys-document-missing">Dosya yok</span>
                                    <span class="tbys-document-status tbys-document-ready">Yüklü</span>
                                </div>
                                <div class="tbys-document-actions">
                                    <a id="VakiflarTasinmazBildirimiDosyaLnk" runat="server" class="tbys-document-open" data-fancybox data-type="pdf" data-width="960" data-height="720"
                                        href="#" visible="false" aria-label="Vakıflar Taşınmaz Bildirimi belgesini aç">Aç</a>
                                    <asp:LinkButton ID="VakiflarTasinmazBildirimiSilBtn" CssClass="tbys-remove" runat="server" CausesValidation="false" Text="Sil" aria-label="Vakıflar Taşınmaz Bildirimi belgesini sil"
                                        OnClick="VakiflarTasinmazBildirimiSilBtn_Click" OnClientClick="return confirm('Belgeyi silmek istiyor musunuz?');" Visible="false" />
                                    <asp:FileUpload ID="VakiflarTasinmazBildirimiYukleFU" CssClass="tbys-file" runat="server" accept=".pdf" aria-label="Vakıflar Taşınmaz Bildirimi için PDF dosyası seç" />
                                </div>
                            </li>
                            </ul>
                        </section>
                    </div>
                </div>
                <div class="tbys-footer">
                    <div class="tbys-update-info">
                        <asp:Label ID="GuncellemeTarihiLbl" runat="server" />
                        <asp:Label ID="GuncelleyenLbl" runat="server" />
                    </div>
                    <asp:LinkButton ID="SaveBtn" CssClass="btn tbys-save" runat="server" Text="Resimleri/Belgeleri Kaydet" OnClick="SaveBtn_Click" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="SaveBtn" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="updateProgress" runat="server">
        <ProgressTemplate>
            <div class="loaderMainContainer">
                <div class="loaderContainer">
                    <div class="loaderCircle"></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div class="modal fade" id="ResimAcModal" tabindex="-1" aria-labelledby="ResimAcModalBaslik" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="modal-title fs-5" id="ResimAcModalBaslik">Fotoğraf</h2>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                </div>
                <div class="modal-body">
                    <img class="tbys-modal-image" id="MaximizedImg" alt="" />
                </div>
            </div>
        </div>
    </div>
</div>
