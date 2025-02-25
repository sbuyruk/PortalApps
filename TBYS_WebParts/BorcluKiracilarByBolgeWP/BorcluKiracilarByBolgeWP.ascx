<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BorcluKiracilarByBolgeWP.ascx.cs" Inherits="TBYS_WebParts.BorcluKiracilarByBolgeWP.BorcluKiracilarByBolgeWP" %>
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
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Borçlu Kiracıların Bölgelere Dağılımı"></asp:Label>
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
            <div class="form-group">
                <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="OdemePlanlariniGuncelleBtn" runat="server" Text="Ödeme Planlarını Güncelle" OnClick="OdemePlanlariniGuncelleBtn_Click" />
            </div>
            <asp:Table ID="BorcluKiracilarTable" runat="server" CssClass="table table-bordered table-hover table-striped" >
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">SIRA NO</asp:TableHeaderCell>
                    <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">BÖLGELER</asp:TableHeaderCell>
                    <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">KİRACI SAYISI</asp:TableHeaderCell>
                    <asp:TableHeaderCell ColumnSpan="4" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">KİRA BORCU</asp:TableHeaderCell>
                    <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">TOPLAM</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">1 Ay</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">2 Ay</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">3 Ay</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İcra Takibi</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="AnkSiraCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">1</asp:TableCell>
                    <asp:TableCell ID="AnkBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Ankara Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="AnkKiraciSayisiCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black" runat="server"></asp:TableCell>
                    <asp:TableCell ID="Ank1AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ank2AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ank3AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ank4AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="AnkTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IstSiraCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">2</asp:TableCell>
                    <asp:TableCell ID="IstBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İstanbul Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="IstKiraciSayisiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ist1AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ist2AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ist3AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Ist4AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IstTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IzmSiraCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">3</asp:TableCell>
                    <asp:TableCell ID="IzmBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">İzmir Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="IzmKiraciSayisiCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Izm1AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Izm2AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Izm3AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Izm4AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="IzmTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="MerSiraCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">4</asp:TableCell>
                    <asp:TableCell ID="MerBaslikCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Mersin Temsilciliği</asp:TableCell>
                    <asp:TableCell ID="MerKiraciSayisiCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black" runat="server"></asp:TableCell>
                    <asp:TableCell ID="Mer1AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Mer2AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Mer3AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Mer4AyCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="MerTopCell" runat="server" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableRow>

                <asp:TableFooterRow HorizontalAlign="Center">
                    <asp:TableCell ID="TopBaslikCell" ColumnSpan="2" BorderStyle="Solid" BorderWidth="1" BorderColor="Black">Genel Toplam</asp:TableCell>
                    <asp:TableCell ID="TopKiraciSayisiCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Top1AyCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Top2AyCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Top3AyCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="Top4AyCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                    <asp:TableCell ID="GenTopCell" BorderStyle="Solid" BorderWidth="1" BorderColor="Black"></asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>

        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
