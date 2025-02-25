<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BorcluKiraciTakibiByBolgeWP.ascx.cs" Inherits="TBYS_WebParts.BorcluKiraciTakibiByBolgeWP.BorcluKiraciTakibiByBolgeWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<div id="MainContainer" class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Borçlu Kiracı Takip İşlemleri"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group row">
                <div class="form-group col-4 row">
                    <asp:Label CssClass="col-form-label col-2" runat="server" Font-Bold="True">Yıl :</asp:Label>
                    <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control col-6" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                </div>
                <div class="form-group col-4 row">
                    <asp:Label CssClass="col-form-label col-2" runat="server" Font-Bold="True">Ay :</asp:Label>
                    <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control col-6" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                </div>
            </div>

            <asp:Table ID="BorcluKiracilarTable" runat="server" CssClass="table table-bordered table-hover table-striped" >
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell  ID="GenelBaslikCell" ColumnSpan="5" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Bölgeler</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Uyarı</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Yazılı İhtar</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İcra Takibi</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Toplam</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="AnkBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Ankara Bölge Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="AnkUyariCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="AnkYaziliIhtarCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="AnkIcraTakibiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="AnkTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IstBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İstanbul Bölge Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="IstUyariCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IstYaziliIhtarCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IstIcraTakibiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IstTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IzmBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İzmir Bölge Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="IzmUyariCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IzmYaziliIhtarCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IzmIcraTakibiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IzmTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="MerBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Mersin Bölge Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="MerUyariCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="MerYaziliIhtarCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="MerIcraTakibiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="MerTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>

                <asp:TableFooterRow HorizontalAlign="Center">
                    <asp:TableCell ID="TopBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Genel Toplam</asp:TableCell>
                    <asp:TableCell ID="TopUyariCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="TopYaziliIhtarCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="TopIcraTakibiCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="GenTopCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>

        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
