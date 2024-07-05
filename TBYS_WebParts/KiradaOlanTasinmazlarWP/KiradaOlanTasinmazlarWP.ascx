<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiradaOlanTasinmazlarWP.ascx.cs" Inherits="TBYS_WebParts.KiradaOlanTasinmazlarWP.KiradaOlanTasinmazlarWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    $(document).ready(function () {
        $(function () {
            if ($('.input-money').toArray().forEach(function (field) {
                       new Cleave(field, {
                numeral: true,
                numeralDecimalMark: ',',
                delimiter: '.'
            });
            }));
        });
    });
</script>

<div id="MainContainer" class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="LinkButton1" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold text-center" ID="TitleLbl" runat="server" Text="Kirada Olan Taşınmazlar"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server">
            <div id="TableContainer"  runat="server">
                    <asp:Table ID="KiradaOlanTasinmazlarTable" runat="server" class="table table-bordered table-hover table-sm table-striped">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="AnkHeaderCell" CssClass="btn-primary" ColumnSpan="6" HorizontalAlign="Center">Kirada Olan Taşınmazlar</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="KVSCell" CssClass="btn-primary" >Cinsi</asp:TableCell>
                            <asp:TableCell ID="Ank1Cell" CssClass="btn-primary" >Ankara</asp:TableCell>
                            <asp:TableCell ID="Ist1Cell" CssClass="btn-primary" >İstanbul</asp:TableCell>
                            <asp:TableCell ID="Izm1Cell" CssClass="btn-primary" >İzmir</asp:TableCell>
                            <asp:TableCell ID="Mer1Cell" CssClass="btn-primary" >Mersin</asp:TableCell>
                            <asp:TableCell ID="ColTop1Cell" CssClass="btn-primary" >Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
            <div>
                <strong>Emlak Beyan Değeri : </strong>
                <input class="input-money text-right" id="EmlakBeyanTopTxt" runat="server" readonly />
                <strong>Tahmini Rayiç Değeri : </strong>
                <input class="input-money text-right" id="TahminiRayicTopTxt" runat="server" readonly />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>

</div>