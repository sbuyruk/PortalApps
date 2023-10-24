<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeTasinmazRaporuWP.ascx.cs" Inherits="BTYS_Webparts.BolgeTasinmazRaporuWP.BolgeTasinmazRaporuWP" %>
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
                <asp:Label CssClass="col-form-label text-danger font-weight-bold text-center" ID="TitleLbl" runat="server" Text="İllere Göre Taşınmaz Raporu"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server">
            <div id="TableContainer">
                <div>
                    <asp:Table ID="BolgeTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ID="BolgeHeaderCell" BorderStyle="Solid" ColumnSpan="17" CssClass="text-center"></asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell ID="BolgeSiraNoCell" BorderStyle="Solid" RowSpan="2">Sıra</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeIllerCell" BorderStyle="Solid" RowSpan="2">İller</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeAptCell" BorderStyle="Solid" ColumnSpan="2">Bina</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMEvCell" BorderStyle="Solid" ColumnSpan="2">M.Ev</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMeskenCell" BorderStyle="Solid" ColumnSpan="2">Mesken</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeIsyeriCell" BorderStyle="Solid" ColumnSpan="2">İşyeri</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeArsaCell" BorderStyle="Solid" ColumnSpan="2">Arsa</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeTarlaCell" BorderStyle="Solid" ColumnSpan="2">Tarla</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeTMCell" BorderStyle="Solid" RowSpan="2">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeCMCell" BorderStyle="Solid" RowSpan="2">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeToplamCell" BorderStyle="Solid" RowSpan="2">Toplam</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell ID="BolgeAptTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeAptCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMEvTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMEvCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMeskenTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeMeskenCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeIsyeriTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeIsyeriCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeArsaTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeArsaCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeTarlaTMCell" BorderStyle="Solid">TM</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="BolgeTarlaCMCell" BorderStyle="Solid">ÇM</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>

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