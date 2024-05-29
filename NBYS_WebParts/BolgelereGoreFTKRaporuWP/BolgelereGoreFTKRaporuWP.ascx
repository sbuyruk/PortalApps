<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgelereGoreFTKRaporuWP.ascx.cs" Inherits="NBYS_WebParts.BolgelereGoreFTKRaporuWP.BolgelereGoreFTKRaporuWP" %>
<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }
</style>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <a class=" btn btn-outline-primary float-right mr-4"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="http://tskgv-portal/YonetimBirimleri/BasinTanitimHalklaIliskilerSubesi/NBYSBelgeleri/BolgelereGoreFTKRaporu.pdf">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Bölgelere Göre Kurulu FTK Raporu"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="form-group col-3">
                    <asp:Label CssClass="col-from-label" runat="server" Text="FTK Kurulus Tarihi"></asp:Label>
                    <asp:TextBox ID="KurulusTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static" AutoPostBack="True" OnTextChanged="FTKGuncellemeTarihiTxt_TextChanged"></asp:TextBox>
                </div>
                <div class="form-group col-3">
                    <asp:Label CssClass="col-from-label" runat="server" Text="FTK Güncelleme Tarihi"></asp:Label>
                    <asp:TextBox ID="GuncellemeTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static" AutoPostBack="True" OnTextChanged="FTKGuncellemeTarihiTxt_TextChanged"></asp:TextBox>
                </div>
            </div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="table">
                        <asp:Table ID="TasDurTable" runat="server" class="table table-bordered table-hover table-striped">
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark" RowSpan="2">Sorumlu Bölge</asp:TableCell>
                                <asp:TableCell CssClass="alert-primary border-dark" ColumnSpan="2">Sorumlu Olduğu</asp:TableCell>
                                <asp:TableCell CssClass="alert-success border-dark" ColumnSpan="2">Kurulu Olan</asp:TableCell>
                                <asp:TableCell CssClass="alert-danger border-dark" ColumnSpan="2">Kurulu Olmayan</asp:TableCell>
                                <asp:TableCell CssClass="alert-warning border-dark" RowSpan="2">Güncellenen</asp:TableCell>
                                <asp:TableCell CssClass="alert-success border-dark" RowSpan="2">Yeni Kurulan</asp:TableCell>
                                <asp:TableCell CssClass="alert-info border-dark"  RowSpan="2" >Kurulum Oranı %</asp:TableCell>
                                <asp:TableCell CssClass="alert-warning border-dark" RowSpan="2">Güncelleme Durumu %</asp:TableCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">İl</asp:TableCell>
                                <asp:TableCell CssClass="alert-primary border-dark">İlçe</asp:TableCell>
                                <asp:TableCell CssClass="alert-success border-dark">İl</asp:TableCell>
                                <asp:TableCell CssClass="alert-success border-dark">İlçe</asp:TableCell>
                                <asp:TableCell CssClass="alert-danger border-dark">İl</asp:TableCell>
                                <asp:TableCell CssClass="alert-danger border-dark">İlçe</asp:TableCell>
                                <%--<asp:TableCell CssClass="alert-info border-dark">İlçe %</asp:TableCell>--%>
                            </asp:TableHeaderRow>
                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">Ankara Bölgesi</asp:TableCell>
                                <asp:TableCell ID="AnkSBIlCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkSBIlceCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkKOIlCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkKOIlceCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkKOlmayanIlCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkKOlmayanIlceCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkGuncellenenCell" CssClass="alert-warning border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkYeniKurulanCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkKurulumOraniCell" CssClass="alert-info border-dark"></asp:TableCell>
                                <asp:TableCell ID="AnkGuncellemeDurumuCell" CssClass="alert-warning border-dark"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">İstanbul Bölgesi</asp:TableCell>
                                <asp:TableCell ID="IstSBIlCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstSBIlceCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstKOIlCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstKOIlceCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstKOlmayanIlCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstKOlmayanIlceCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstGuncellenenCell" CssClass="alert-warning border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstYeniKurulanCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstKurulumOraniCell" CssClass="alert-info border-dark"></asp:TableCell>
                                <asp:TableCell ID="IstGuncellemeDurumuCell" CssClass="alert-warning border-dark"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">İzmir Bölgesi</asp:TableCell>
                                <asp:TableCell ID="IzmSBIlCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmSBIlceCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmKOIlCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmKOIlceCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmKOlmayanIlCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmKOlmayanIlceCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmGuncellenenCell" CssClass="alert-warning border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmYeniKurulanCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmKurulumOraniCell" CssClass="alert-info border-dark"></asp:TableCell>
                                <asp:TableCell ID="IzmGuncellemeDurumuCell" CssClass="alert-warning border-dark"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">Mersin Bölgesi</asp:TableCell>
                                <asp:TableCell ID="MerSBIlCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerSBIlceCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerKOIlCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerKOIlceCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerKOlmayanIlCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerKOlmayanIlceCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerGuncellenenCell" CssClass="alert-warning border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerYeniKurulanCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerKurulumOraniCell" CssClass="alert-info border-dark"></asp:TableCell>
                                <asp:TableCell ID="MerGuncellemeDurumuCell" CssClass="alert-warning border-dark"></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableFooterRow HorizontalAlign="Center">
                                <asp:TableCell CssClass="alert-primary border-dark">Toplam</asp:TableCell>
                                <asp:TableCell ID="TopSBIlCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopSBIlceCell" CssClass="alert-primary border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopKOIlCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopKOIlceCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopKOlmayanIlCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopKOlmayanIlceCell" CssClass="alert-danger border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopGuncellenenCell" CssClass="alert-warning border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopYeniKurulanCell" CssClass="alert-success border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopKurulumOraniCell" CssClass="alert-info border-dark"></asp:TableCell>
                                <asp:TableCell ID="TopGuncellemeDurumuCell" CssClass="alert-warning border-dark"></asp:TableCell>
                            </asp:TableFooterRow>
                        </asp:Table>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="KurulusTarihiTxt" EventName="TextChanged" />
                    <asp:AsyncPostBackTrigger ControlID="GuncellemeTarihiTxt" EventName="TextChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:UpdateProgress ID="updateProgress" runat="server">
                <ProgressTemplate>
                    <div class='loaderMainContainer'>
                        <div class='loaderContainer'>
                            <div class='loaderCircle'></div>
                        </div>
                    </div>

                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="FTKListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-right" Text="FTK Listesi" OnClick="FTKListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="FTKIslemleriBtn" CssClass="btn btn-outline-secondary float-right mr-3" runat="server" Text="FTK İşlemleri" OnClick="FTKIslemleriBtn_Click" CausesValidation="False"></asp:LinkButton>
        </div>
    </div>
</div>
