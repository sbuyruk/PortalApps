<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YoneticiOzetiWP.ascx.cs" Inherits="Portal_WebParts.YoneticiOzetiWP.YoneticiOzetiWP" %>

<style>
    /* Yeni stil: Taşınmaz Haritası kartı için görsel iyileştirme */
    .map-card {
        border-radius: 8px;
        overflow: hidden;
        box-shadow: 0 10px 20px rgba(0,0,0,0.08);
        background: #fff;
    }

    /* Taşınmaz Durumu kartı */
    .status-card {
        border-radius: 8px;
        overflow: hidden;
        box-shadow: 0 10px 20px rgba(0,0,0,0.06);
        background: #fff;
    }

    /* header gradient ve beyaz metin */
    .map-card-header {
        background: linear-gradient(90deg, #2366d1 0%, #5aa1ff 100%);
        color: #ffffff;
        padding: 0.75rem 1rem;
    }

    .status-card-header {
        background: linear-gradient(90deg, #117a37 0%, #3fc071 100%);
        color: #ffffff;
        padding: 0.75rem 1rem;
    }

    /* footer hafif arka plan */
    .map-card-footer,
    .status-card-footer {
        border-top: 1px solid rgba(0,0,0,0.04);
    }

    /* Kartların aynı yükseklikte olması için flex düzeni */
    .cards-row {
        display: flex;
        gap: 12px;
        flex-wrap: wrap;
        align-items: stretch; /* çocukları aynı yüksekliğe uzatır */
    }

    /* Her .card sütun düzeni alsın, gövde (body) esnek kısımdır */
    .cards-row > .card {
        display: flex;
        flex-direction: column;
        min-width: 220px;
        max-width: 260px;
        /* istenirse sabit height verilebilir:
           height: 320px;
           veya min-height ile tutarlılık sağlanabilir
        */
    }

    /* card gövdesi kalan alanı kaplasın, footer sabit kalsın */
    .cards-row > .card .card-body,
    .cards-row > .card .status-body {
        flex: 1 1 auto;
    }

    .status-body {
        padding: 0.75rem 1rem;
    }

    .info-row {
        display: flex;
        justify-content: space-between;
        padding: 6px 0;
        border-bottom: 1px dashed rgba(0,0,0,0.04);
        font-family: Arial, Helvetica, sans-serif;
    }

    .info-row:last-child {
        border-bottom: none;
    }

    .info-label {
        color: #6c757d;
        font-size: 0.95rem;
    }

    .info-value {
        font-weight: 700;
        color: #222;
    }

    /* responsive davranış */
    @media (max-width: 576px) {
        .map-card,
        .status-card {
            max-width: 100%;
        }
        .cards-row {
            flex-direction: column;
            gap: 10px;
        }
    }
</style>

<div class="mt-4 mb-4">
    <div class="card shadow">
        <div class="card-header">
            <h2 runat="server" id="TitleLbl" class=" text-center">Özet Bilgiler</h2>
        </div>
        <div class="card-body">
            <div class="cards-row">
                <!-- Taşınmaz Haritası kartı -->
                <div class="status-card card shadow-sm" style="margin-top: 18px;">
                    <div class="card-header status-card-header text-center">
                        <h3>Taşınmaz Haritası</h3>
                    </div>
                    <div class="card-body" style="padding: 0.5rem;">
                        <a href="/sayfalar/TasinmazBagisHaritasi.aspx" aria-label="Taşınmaz Haritasını Aç">
                            <img alt="Taşınmaz Haritası" src='/PublishingImages/tskgv-bolge-sm.png' style="width: 100%; height: auto; display: block; border-radius: 4px;" />
                        </a>
                    </div>
                    <div class="card-footer map-card-footer" style="text-align: center; background: #fbfbfb; padding: 0.5rem 0;">
                        <a href="/sayfalar/TasinmazBagisHaritasi.aspx" aria-label="Taşınmaz Haritasını Aç"> Haritayı Görüntüle</a>
                    </div>
                </div>

                <!-- Yeni: Taşınmaz Durumu kartı (resimsiz) -->
                <div class="status-card card shadow-sm" style="margin-top: 18px;">
                    <div class="card-header status-card-header text-center">
                        <h3>Taşınmaz Durumu</h3>
                    </div>
                    <div class="status-body">
                        <div class="info-row">
                            <div class="info-label">Çıplak Mülkiyetli Toplam</div>
                            <div class="info-value"><asp:Label ID="CiplakMulkToplamLbl" runat="server" Text="0"></asp:Label></div>
                        </div>
                        <div class="info-row">
                            <div class="info-label">Tam Mülkiyetli Toplam</div>
                            <div class="info-value"><asp:Label ID="TamMulkToplamLbl" runat="server" Text="0"></asp:Label></div>
                        </div>
                        <hr />
                        <div class="info-row">
                            <div class="info-label">Toplam Taşınmaz</div>
                            <asp:Label ID="ToplamTasinmazLbl" runat="server" class="fw-bold"></asp:Label>
                        </div>
                    </div>
                    <div class="card-footer status-card-footer" style="text-align: center; background: #fbfbfb; padding: 0.5rem 0; font-size: 0.85rem; color: #6c757d;">
                        <asp:LinkButton CssClass="btn btn-link" ID="TasinmazDurumuBtn" runat="server" Text="Taşınmaz Durumunu Görüntüle" OnClick="TasinmazDurumuBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                    </div>
                </div>

                <!-- Kira Durumu kartı -->
                <div class="status-card card shadow-sm" style="margin-top: 18px;">
                    <div class="card-header status-card-header text-center">
                        <h3>Kira Durumu</h3>
                    </div>
                    <div class="status-body">
                        <div class="info-row">
                            <div class="info-label">Kiralanabilir Ana Taşınmaz</div>
                            <div class="info-value">
                                <asp:Label ID="KirayaUygunAnaTasinmazLbl" runat="server" Text="0"></asp:Label></div>
                        </div>
                        <div class="info-row">
                            <div class="info-label">Kiralanabilir Alt Bölüm</div>
                            <div class="info-value">
                                <asp:Label ID="KirayaUygunAltBolumLbl" runat="server" Text="0"></asp:Label></div>
                        </div>
                        <div class="info-row">
                            <div class="info-label" data-bs-toggle="tooltip" data-bs-placement="top" 
                                title="Envanterde olmayıp kiralanabilen taşınmazlar, marina vb tesisler bu kapsamda ">
                            Envanter Olmayan Taşınmaz
                            </div>
                            <div class="info-value">
                                <asp:Label ID="EnvanterDisiLbl" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <hr />
                        <div class="info-row">
                            <div class="info-label">Kiralanabilir Toplam Taşınmaz</div>
                            <asp:Label ID="ToplamKirayaUygunLbl" runat="server" class="fw-bold"></asp:Label>
                        </div>
                    </div>
                    <div class="card-footer status-card-footer" style="text-align: center; background: #fbfbfb; padding: 0.5rem 0; font-size: 0.85rem; color: #6c757d;">
                        <asp:LinkButton CssClass="btn btn-link" ID="KiraDurumuBtn" runat="server" Text="Kiralabilir Taşınmazlar" OnClick="KiraDurumuBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                        <asp:LinkButton CssClass="btn btn-link" ID="KirayaUygunOlmayanBtn" runat="server" Text="Kiraya Uygun Olmayan Taşınmazlar" OnClick="KirayaUygunOlmayanDurumuBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                    </div>
                </div>
                <!-- Yeni: Kira Gelirleri kartı -->
                <div class="status-card card shadow-sm" style="margin-top: 18px;">
                    <div class="card-header status-card-header text-center">
                        <h3>Kira Gelirleri</h3>
                    </div>
                    <div class="status-body">
                        <div class="info-row">
                            <div class="info-label">Aylık Güncel Kira Geliri</div>
                            <div class="info-value">
                                <asp:Label ID="AylikKiraGeliriLbl" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="info-row">
                            <div class="info-label">Yıllık Tahmini Kira Geliri</div>
                            <div class="info-value">
                                <asp:Label ID="YillikTahminiKiraLbl" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="info-row">
                            <div class="info-label">Toplam Tahakkuk</div>
                            <div class="info-value">
                                <asp:Label ID="ToplamTahakkukLbl" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <hr />
                        <div class="info-row">
                            <div class="info-label">Toplam Tahsilat</div>
                            <asp:Label ID="ToplamTahsilatLbl" runat="server" class="fw-bold"></asp:Label>
                        </div>
                    </div>
                    <div class="card-footer status-card-footer" style="text-align: center; background: #fbfbfb; padding: 0.5rem 0; font-size: 0.85rem; color: #6c757d;">
                        <asp:LinkButton CssClass="btn btn-link" ID="KiraGelirleriBtn" runat="server" Text="Kira Gelirlerini Görüntüle" OnClick="KiraGelirleriBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                    </div>
                </div>
                <!-- Yapım Aşamasında kartı -->
                <div class="status-card card shadow-sm" style="margin-top: 18px;">
                    <div class="card-header status-card-header text-center">
                        <h3>Nakit Bağışlar</h3>
                    </div>
                    <div class="card-body" style="padding: 0.5rem;">
                        <a aria-label="Yapım Aşamasında">
                            <img alt="Yapım Aşamasında" src='/PublishingImages/yapim-asamasinda.png' style="width: 100%; height: auto; display: block; border-radius: 4px;" />
                            <img alt="Yapım Aşamasında" src='/PublishingImages/coding.gif' style="width: 100%; height: auto; display: block; border-radius: 4px;" />
                        </a>
                    </div>
                    <div class="card-footer map-card-footer" style="text-align: center; background: #fbfbfb; padding: 0.5rem 0;">
                        <%--<a href="/sayfalar/TasinmazBagisHaritasi.aspx" aria-label="Taşınmaz Haritasını Aç"> Haritayı Görüntüle</a>--%>
                    </div>
                </div>

                <!-- Ek kartlar eklendiğinde aynı yükseklikte görünmeleri için sadece .card sınıfını kullanın -->

            </div>
        </div>

        <div class="card-footer">
            <p style='color: gray; font-family: arial; font-size: xx-small;'>TBYS ve NBYS Kayıtlarından Alınmıştır.  Bilgi Sistemleri Kısmı &trade; </p>
        </div>
    </div>
</div>

<!-- Modal: Taşınmaz Durum Raporu (Bootstrap) -->
<div class="modal" id="TasinmazDurumuModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <h3 class="modal-title text-center" id="TasinmazDurumuModalLabel">Taşınmaz Durumu</h3>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <asp:Table ID="TasDurTable" runat="server" class="table table-bordered table-hover table-striped">
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="btn-primary" RowSpan="2">Bölge</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary" ColumnSpan="2">Mülkiyet</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary" ColumnSpan="5">Taşınmazın Cinsi</asp:TableCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="btn-primary">Tam Mülkiyet</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Çıplak Mülkiyet</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Bina</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Mesken</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">İşyeri</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Arsa</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Tarla</asp:TableCell>
                                <%--<asp:TableCell CssClass="btn-primary">Müstakil Ev</asp:TableCell>--%>
                            </asp:TableHeaderRow>

                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell ID="TopBaslikCell" CssClass="btn-primary" BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Toplam</asp:TableCell>
                                <asp:TableCell ID="TopTMCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopCMCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopTMCMTopCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopAptCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopMesCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopIsyCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopArsCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="TopTarCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <%--<asp:TableCell ID="TopMevCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>--%>
                            </asp:TableRow>
                        </asp:Table>
                        <div class="float">
                            <strong>Emlak Beyan Değeri : </strong>
                            <input class="input-money text-end" id="EmlakBeyanTopTxt" runat="server" readonly />
                            <strong>Tahmini Rayiç Değeri : </strong>
                            <input class="input-money text-end" id="TahminiRayicTopTxt" runat="server" readonly />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<div class="modal" id="KiraDurumuModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <h3 class="modal-title text-center" id="KiraDurumuModalLabel">Kiralanabilir Taşınmazlar</h3>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <asp:Table ID="KiraDurTable" runat="server" class="table table-bordered table-hover table-striped">
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="btn-primary" RowSpan="2">Bölge</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary" ColumnSpan="5">Taşınmazın Cinsi</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="btn-primary">Bina</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Mesken</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">İşyeri</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Arsa</asp:TableCell>
                                <asp:TableCell CssClass="btn-primary">Tarla</asp:TableCell>
                                <%--<asp:TableCell CssClass="btn-primary">Müstakil Ev</asp:TableCell>--%>
                            </asp:TableHeaderRow>

                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell ID="KTopBaslikCell" CssClass="btn-primary" BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Toplam</asp:TableCell>

                                <asp:TableCell ID="KTopAptCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="KTopMesCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="KTopIsyCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="KTopArsCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="KTopTarCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <asp:TableCell ID="KTopCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>
                                <%--<asp:TableCell ID="TopMevCell" BorderStyle="Solid" BorderWidth="2"></asp:TableCell>--%>
                            </asp:TableRow>
                        </asp:Table>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<script type="text/javascript">
    /**
     * Opens a Bootstrap modal by id or element.
     * - Accepts a string id (e.g. 'TasinmazDurumuModal') or the element itself.
     * - Falls back to 'TasinmazDurumuModal' when no parameter is provided for backward compatibility.
     */
    function OpenModal(modal) {
        // Backward compatible default
        var modalId = modal || 'TasinmazDurumuModal';

        // If a string was provided, treat it as an id
        var el = (typeof modalId === 'string') ? document.getElementById(modalId) : modalId;

        if (!el) {
            console.warn('OpenModal: modal element not found for', modalId);
            return;
        }

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(el);
        myModalInstance.show();
    }
</script>