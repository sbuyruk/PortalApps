<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeArmaganRaporuWP.ascx.cs" Inherits="NBYS_WebParts.BolgeArmaganRaporuWP.BolgeArmaganRaporuWP" %>
<script>
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="container shadow">
    <div class="card">
        <div class="card-header ">
            <asp:LinkButton ID="LinkButton1" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-info font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Bölgelere Göre Armağan Raporu"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="row mt-2 ">
            <div class="form-group form-group-sm col-sm-3">
                <div class="row">
                    <label for="BasAyDDL" class="col-6 col-form-label text-right ">Başlangıç Ay</label>
                    <div class="col-6">
                        <asp:DropDownList ID="BasAyDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="BasAyDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="form-group form-group-sm col-sm-3">
                <div class="row">
                    <label for="BasYilDDL" class="col-6 col-form-label text-right">Yıl</label>
                    <div class="col-6">
                        <asp:DropDownList ID="BasYilDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="BasAyDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mb-2">
            <div class="form-group form-group-sm col-sm-3">
                <div class="row">
                    <label for="BitAyDDL" class="col-6 col-form-label text-right ">Bitiş Ay</label>
                    <div class="col-6">
                        <asp:DropDownList ID="BitAyDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="BasAyDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="form-group form-group-sm col-sm-3">
                <div class="row">
                    <label for="BitYilDDL" class="col-6 col-form-label text-right">Yıl</label>
                    <div class="col-6">
                        <asp:DropDownList ID="BitYilDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="BasAyDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                <div class="card-body">
                    <asp:Table ID="NBTable" runat="server" class="loader table table-bordered table-hover table-striped">
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="TableHeaderCell" runat="server" CssClass="btn-light font-weight-bold" ColumnSpan="6"></asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell CssClass="btn-light font-weight-bold">Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light font-weight-bold">Altın Madalya</asp:TableCell>
                            <asp:TableCell CssClass="btn-light font-weight-bold">Gümüş Madalya</asp:TableCell>
                            <asp:TableCell CssClass="btn-light font-weight-bold">Bronz Madalya</asp:TableCell>
                            <asp:TableCell CssClass="btn-light font-weight-bold">Teşekkür Belgesi</asp:TableCell>
                            <asp:TableCell CssClass="btn-light font-weight-bold">Bölge Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="GMBaslikCell" CssClass="font-weight-bold">Gn.Md.lük</asp:TableCell>
                            <asp:TableCell ID="GMAltinAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="GMGumusAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="GMBronzAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="GMTesAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="GmAdetTopCell" CssClass="font-weight-bold"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="IstBaslikCell" CssClass="font-weight-bold">İstanbul Bölge</asp:TableCell>
                            <asp:TableCell ID="IstAltinAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IstGumusAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IstBronzAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IstTesAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IstAdetTopCell" CssClass="font-weight-bold"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="IzmBaslikCell" CssClass="font-weight-bold">İzmir Bölge</asp:TableCell>
                            <asp:TableCell ID="IzmAltinAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IzmGumusAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IzmBronzAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IzmTesAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IzmAdetTopCell" CssClass="font-weight-bold"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="MerBaslikCell" CssClass="font-weight-bold">Mersin Bölge</asp:TableCell>
                            <asp:TableCell ID="MerAltinAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="MerGumusAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="MerBronzAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="MerTesAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="MerAdetTopCell" CssClass="font-weight-bold"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="YurtDisiBaslikCell" CssClass="font-weight-bold">Yurtdışı</asp:TableCell>
                            <asp:TableCell ID="YurtDisiAltinAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="YurtDisiGumusAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="YurtDisiBronzAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="YurtDisiTesAdetCell" runat="server"></asp:TableCell>
                            <asp:TableCell ID="YurtDisiAdetTopCell" CssClass="font-weight-bold"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow HorizontalAlign="Center">
                            <asp:TableCell ID="TopBaslikCell" CssClass="font-weight-bold">Toplam</asp:TableCell>
                            <asp:TableCell ID="AltinAdetToplamCell" CssClass="font-weight-bold" runat="server"></asp:TableCell>
                            <asp:TableCell ID="GumusAdetToplamCell" CssClass="font-weight-bold" runat="server"></asp:TableCell>
                            <asp:TableCell ID="BronzAdetToplamCell" CssClass="font-weight-bold" runat="server"></asp:TableCell>
                            <asp:TableCell ID="TesAdetToplamCell" CssClass="font-weight-bold" runat="server"></asp:TableCell>
                            <asp:TableCell ID="TopAdetTopCell" CssClass="font-weight-bold text-danger"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                    <div>
                        <asp:Label Text="* İl bilgisi olmayan bağışçıların armağanları Genel Müd.lük sayılarına dahil edilmiştir." runat="server" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
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
