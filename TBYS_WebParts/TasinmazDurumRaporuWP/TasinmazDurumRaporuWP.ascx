<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazDurumRaporuWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazDurumRaporuWP.TasinmazDurumRaporuWP" %>
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
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Taşınmaz Durum Raporu"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server" >
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
                
                <asp:TableRow HorizontalAlign="Center" >
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
                <strong>Eml.Bey.Değeri : </strong>
                <input class="input-money text-end" id="EmlakBeyanTopTxt" runat="server" readonly />
                <strong>Tah.Ray.Değeri : </strong>
                <input class="input-money text-end" id="TahminiRayicTopTxt" runat="server" readonly />
                <strong>Muh.Kayıtlı Değer : </strong>
                <input class="input-money text-end" id="MuhasebeyeKayitliTopTxt" runat="server" readonly />
                <strong>Yak.Piy.Değeri : </strong>
                <input class="input-money text-end" id="TahminiPiyasaTopTxt" runat="server" readonly />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>