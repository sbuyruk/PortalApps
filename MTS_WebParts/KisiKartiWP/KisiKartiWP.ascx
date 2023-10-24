<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KisiKartiWP.ascx.cs" Inherits="MTS_WebParts.KisiKartiWP.KisiKartiWP" %>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="container ">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Kişi Kartı"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body" id="MainCardDiv" runat="server">
                    <div class="table">
                        <asp:Table ID="KisiBilgileriTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell ID="BaslikTH" ColumnSpan="5" BackColor="Silver" >Kişi Bilgileri</asp:TableHeaderCell>
                                <asp:TableCell ID="BaslikTarihCell" BackColor="Silver" HorizontalAlign="Right"></asp:TableCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                    <div id="VerilenFaaliyetDiv" class="table">
                        <asp:Table ID="VerilenFaaliyetBilgileriTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell ColumnSpan="8" BackColor="Silver">Faaliyet Bilgileri (Gelenler)</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell>R.No</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Faaliyet Tarihi</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Yeri/Konusu</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Amacı/Durumu</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Verilen Anı Objesi (Adet)</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Getirilen Anı Objesi (Adet)</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Katılımcılar</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Faaliyet Kartı</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                    <div id="AlinanFaaliyetDiv" class="table">
                        <asp:Table ID="AlınanFaaliyetBilgileriTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell ColumnSpan="8" BackColor="Silver">Faaliyet Bilgileri (Gidilenler)</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell>R.No</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Faaliyet Tarihi</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Yeri/Konusu</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Amacı/Durumu</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Verilen Anı Objesi (Adet)</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Getirilen Anı Objesi (Adet)</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Katılımcılar</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Faaliyet Kartı</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                    <div id="AramaGorusmeDiv" class="table">
                        <asp:Table ID="AramaGorusmeTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell ColumnSpan="6" BackColor="Silver">Aramalar</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell>A/G.No</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Arama/Görüşme</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Arama Tarihi</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Konusu</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Faaliyet</asp:TableHeaderCell>
                                <asp:TableHeaderCell>Görüşme Sağlandı</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                    <asp:LinkButton ID="FaaliyetTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Faaliyet Takvimi" OnClick="FaaliyetTakvimiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Faaliyet Listesi" OnClick="FaaliyetListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="AramaListesi" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Arama/Görüşme Listesi" OnClick="AramaListesiBtn_Click"></asp:LinkButton>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>