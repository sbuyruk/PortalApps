<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KirayaVerilmeyenTasinmazlarWP.ascx.cs" Inherits="TBYS_WebParts.KirayaVerilmeyenTasinmazlarWP.KirayaVerilmeyenTasinmazlarWP" %>
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
            <asp:LinkButton ID="LinkButton1" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Kiraya Verilmeyen Taşınmazlar"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server">
                <div id="TableContainer"  runat="server">
                    <asp:Table ID="KVTTable" runat="server" class="table table-bordered table-hover table-condensed table-striped">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ID="GMHeaderCell"  ColumnSpan="14" CssClass="text-center" BorderStyle="Solid">Kiraya Verilmeyen Taşınmazlar</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell ID="GMSiraNoCell" RowSpan="2"  BorderStyle="Solid">Sıra</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="KVSCell" RowSpan="2" BorderStyle="Solid">Kiraya Verilmeme Sebebi</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="KSCell" RowSpan="2" BorderStyle="Solid" >Kullanım Şekli</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="GM1Cell" ColumnSpan="2" BorderStyle="Solid" >Ankara</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Ist1Cell" ColumnSpan="2" BorderStyle="Solid" >İstanbul</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Izm1Cell" ColumnSpan="2" BorderStyle="Solid" >İzmir</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Mer1Cell" ColumnSpan="2" BorderStyle="Solid" >Mersin</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="ColTop1Cell" ColumnSpan="3" BorderStyle="Solid" >Toplam</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell ID="GM1TMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="GM1CMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Ist1TMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Ist1CMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Izm1TMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Izm1CMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Mer1TMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="Mer1CMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="ColTop1TMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="ColTop1CMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="ColTop1TMCMCell" BorderStyle="Solid">TM + ÇM</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
            <div>
                <strong>Emlak Beyan Değeri : </strong>
                <input class="input-money text-end" id="EmlakBeyanTopTxt" runat="server" readonly />
                <strong>Tahmini Rayiç Değeri : </strong>
                <input class="input-money text-end" id="TahminiRayicTopTxt" runat="server" readonly />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>