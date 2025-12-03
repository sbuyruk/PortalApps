<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SigortaDurumuDaskWP.ascx.cs" Inherits="TBYS_WebParts.SigortaDurumuDaskWP.SigortaDurumuDaskWP" %>
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
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Sigorta Durumu (DASK)"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server">
            <asp:Table ID="TasDurTable" runat="server" class="table table-bordered table-hover table-sm">
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
                </asp:TableHeaderRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="AnkBaslikCell" CssClass="btn-primary">Ankara</asp:TableCell>
                    <asp:TableCell ID="AnkTMCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkCMCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkTMCMTopCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkAptCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkMesCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkIsyCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkArsCell" runat="server"></asp:TableCell>
                    <asp:TableCell ID="AnkTarCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IstBaslikCell" CssClass="btn-primary">İstanbul</asp:TableCell>
                    <asp:TableCell ID="IstTMCell"></asp:TableCell>
                    <asp:TableCell ID="IstCMCell"></asp:TableCell>
                    <asp:TableCell ID="IstTMCMTopCell"></asp:TableCell>
                    <asp:TableCell ID="IstAptCell"></asp:TableCell>
                    <asp:TableCell ID="IstMesCell"></asp:TableCell>
                    <asp:TableCell ID="IstIsyCell"></asp:TableCell>
                    <asp:TableCell ID="IstArsCell"></asp:TableCell>
                    <asp:TableCell ID="IstTarCell"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="IzmBaslikCell" CssClass="btn-primary">İzmir</asp:TableCell>
                    <asp:TableCell ID="IzmTMCell"></asp:TableCell>
                    <asp:TableCell ID="IzmCMCell"></asp:TableCell>
                    <asp:TableCell ID="IzmTMCMTopCell"></asp:TableCell>
                    <asp:TableCell ID="IzmAptCell"></asp:TableCell>
                    <asp:TableCell ID="IzmMesCell"></asp:TableCell>
                    <asp:TableCell ID="IzmIsyCell"></asp:TableCell>
                    <asp:TableCell ID="IzmArsCell"></asp:TableCell>
                    <asp:TableCell ID="IzmTarCell"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="MerBaslikCell" CssClass="btn-primary">Mersin</asp:TableCell>
                    <asp:TableCell ID="MerTMCell"></asp:TableCell>
                    <asp:TableCell ID="MerCMCell"></asp:TableCell>
                    <asp:TableCell ID="MerTMCMTopCell"></asp:TableCell>
                    <asp:TableCell ID="MerAptCell"></asp:TableCell>
                    <asp:TableCell ID="MerMesCell"></asp:TableCell>
                    <asp:TableCell ID="MerIsyCell"></asp:TableCell>
                    <asp:TableCell ID="MerArsCell"></asp:TableCell>
                    <asp:TableCell ID="MerTarCell"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center">
                    <asp:TableCell ID="ErzBaslikCell" CssClass="btn-primary">Erzurum</asp:TableCell>
                    <asp:TableCell ID="ErzTMCell"></asp:TableCell>
                    <asp:TableCell ID="ErzCMCell"></asp:TableCell>
                    <asp:TableCell ID="ErzTMCMTopCell"></asp:TableCell>
                    <asp:TableCell ID="ErzAptCell"></asp:TableCell>
                    <asp:TableCell ID="ErzMesCell"></asp:TableCell>
                    <asp:TableCell ID="ErzIsyCell"></asp:TableCell>
                    <asp:TableCell ID="ErzArsCell"></asp:TableCell>
                    <asp:TableCell ID="ErzTarCell"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="2">
                    <asp:TableCell ID="TopBaslikCell" CssClass="btn-primary">Toplam</asp:TableCell>
                    <asp:TableCell ID="TopTMCell"></asp:TableCell>
                    <asp:TableCell ID="TopCMCell"></asp:TableCell>
                    <asp:TableCell ID="TopTMCMTopCell"></asp:TableCell>
                    <asp:TableCell ID="TopAptCell"></asp:TableCell>
                    <asp:TableCell ID="TopMesCell"></asp:TableCell>
                    <asp:TableCell ID="TopIsyCell"></asp:TableCell>
                    <asp:TableCell ID="TopArsCell"></asp:TableCell>
                    <asp:TableCell ID="TopTarCell"></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <div>
                <strong>Emlak Beyan Değeri : </strong>
                <input class="input-money text-end" id="EmlakBeyanTopTxt" runat="server" readonly />
                <strong>Tahmini Rayiç Değeri : </strong>
                <input class="input-money text-end" id="TahminiRayicTopTxt" runat="server" readonly />
            </div>
            <div>
                <strong>Sig. Bedeli Toplam  : </strong>
                <input class="input-money text-end" id="SigortaBedeliTopTxt" runat="server" readonly />
                <strong>Prim Toplam         :  </strong>
                <input class="input-money text-end" id="PrimTopTxt" runat="server" readonly />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>

</div>