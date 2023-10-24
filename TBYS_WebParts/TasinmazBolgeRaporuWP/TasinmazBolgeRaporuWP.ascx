<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBolgeRaporuWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBolgeRaporuWP.TasinmazBolgeRaporuWP" %>
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
                <asp:Label CssClass="col-form-label text-danger font-weight-bold text-center" ID="TitleLbl" runat="server" Text="Taşınmaz Bölge Raporu"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TasimazDurumuPanel" runat="server">
            <div id="TableContainer">
                <div>
                    <asp:Table ID="GMTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="GMHeaderCell" CssClass="btn-primary" ColumnSpan="17" HorizontalAlign="Center">GENEL MÜDÜRLÜK</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="GMSiraNoCell" CssClass="btn-primary" RowSpan="2">Sıra</asp:TableCell>
                            <asp:TableCell ID="GMIllerCell" CssClass="btn-primary" RowSpan="2">İller</asp:TableCell>
                            <asp:TableCell ID="GMAptCell" CssClass="btn-primary" ColumnSpan="2">Bina</asp:TableCell>
                            <asp:TableCell ID="GMMEvCell" CssClass="btn-primary" ColumnSpan="2">M.Ev</asp:TableCell>
                            <asp:TableCell ID="GMMeskenCell" CssClass="btn-primary" ColumnSpan="2">Mesken</asp:TableCell>
                            <asp:TableCell ID="GMIsyeriCell" CssClass="btn-primary" ColumnSpan="2">İşyeri</asp:TableCell>
                            <asp:TableCell ID="GMArsaCell" CssClass="btn-primary" ColumnSpan="2">Arsa</asp:TableCell>
                            <asp:TableCell ID="GMTarlaCell" CssClass="btn-primary" ColumnSpan="2">Tarla</asp:TableCell>
                            <asp:TableCell ID="GMTMCell" CssClass="btn-primary" RowSpan="2">TM</asp:TableCell>
                            <asp:TableCell ID="GMCMCell" CssClass="btn-primary" RowSpan="2">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMToplamCell" CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="GMAptTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMAptCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMMEvTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMMEvCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMMeskenTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMMeskenCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMIsyeriTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMIsyeriCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMArsaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMArsaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="GMTarlaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="GMTarlaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
                <div>
                    <asp:Table ID="IstTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableCell CssClass="btn-primary" ColumnSpan="17" HorizontalAlign="Center">İSTANBUL BÖLGESİ</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="IstSiraNoCell" CssClass="btn-primary" RowSpan="2">Sıra</asp:TableCell>
                            <asp:TableCell ID="IstIllerCell" CssClass="btn-primary" RowSpan="2">İller</asp:TableCell>
                            <asp:TableCell ID="IstAptCell" CssClass="btn-primary" ColumnSpan="2">Bina</asp:TableCell>
                            <asp:TableCell ID="IstMEvCell" CssClass="btn-primary" ColumnSpan="2">M.Ev</asp:TableCell>
                            <asp:TableCell ID="IstMeskenCell" CssClass="btn-primary" ColumnSpan="2">Mesken</asp:TableCell>
                            <asp:TableCell ID="IstIsyeriCell" CssClass="btn-primary" ColumnSpan="2">İşyeri</asp:TableCell>
                            <asp:TableCell ID="IstArsaCell" CssClass="btn-primary" ColumnSpan="2">Arsa</asp:TableCell>
                            <asp:TableCell ID="IstTarlaCell" CssClass="btn-primary" ColumnSpan="2">Tarla</asp:TableCell>
                            <asp:TableCell ID="IstTMCell" CssClass="btn-primary" RowSpan="2">TM</asp:TableCell>
                            <asp:TableCell ID="IstCMCell" CssClass="btn-primary" RowSpan="2">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstToplamCell" CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="IstAptTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstAptCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstMEvTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstMEvCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstMeskenTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstMeskenCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstIsyeriTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstIsyeriCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstArsaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstArsaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IstTarlaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IstTarlaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>

                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
                <div>
                    <asp:Table ID="IzmTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableCell CssClass="btn-primary" ColumnSpan="17" HorizontalAlign="Center">İZMİR BÖLGESİ</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="IzmSiraNoCell" CssClass="btn-primary" RowSpan="2">Sıra</asp:TableCell>
                            <asp:TableCell ID="IzmIllerCell" CssClass="btn-primary" RowSpan="2">İller</asp:TableCell>
                            <asp:TableCell ID="IzmAptCell" CssClass="btn-primary" ColumnSpan="2">Bina</asp:TableCell>
                            <asp:TableCell ID="IzmMEvCell" CssClass="btn-primary" ColumnSpan="2">M.Ev</asp:TableCell>
                            <asp:TableCell ID="IzmMeskenCell" CssClass="btn-primary" ColumnSpan="2">Mesken</asp:TableCell>
                            <asp:TableCell ID="IzmIsyeriCell" CssClass="btn-primary" ColumnSpan="2">İşyeri</asp:TableCell>
                            <asp:TableCell ID="IzmArsaCell" CssClass="btn-primary" ColumnSpan="2">Arsa</asp:TableCell>
                            <asp:TableCell ID="IzmTarlaCell" CssClass="btn-primary" ColumnSpan="2">Tarla</asp:TableCell>
                            <asp:TableCell ID="IzmTMCell" CssClass="btn-primary" RowSpan="2">TM</asp:TableCell>
                            <asp:TableCell ID="IzmCMCell" CssClass="btn-primary" RowSpan="2">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmToplamCell" CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="IzmAptTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmAptCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmMEvTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmMEvCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmMeskenTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmMeskenCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmIsyeriTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmIsyeriCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmArsaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmArsaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="IzmTarlaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="IzmTarlaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                        </asp:TableHeaderRow>


                    </asp:Table>
                </div>
                <div>
                    <asp:Table ID="MerTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableCell CssClass="btn-primary" ColumnSpan="17" HorizontalAlign="Center">MERSİN BÖLGESİ</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="MerSiraNoCell" CssClass="btn-primary" RowSpan="2">Sıra</asp:TableCell>
                            <asp:TableCell ID="MerIllerCell" CssClass="btn-primary" RowSpan="2">İller</asp:TableCell>
                            <asp:TableCell ID="MerAptCell" CssClass="btn-primary" ColumnSpan="2">Bina</asp:TableCell>
                            <asp:TableCell ID="MerMEvCell" CssClass="btn-primary" ColumnSpan="2">M.Ev</asp:TableCell>
                            <asp:TableCell ID="MerMeskenCell" CssClass="btn-primary" ColumnSpan="2">Mesken</asp:TableCell>
                            <asp:TableCell ID="MerIsyeriCell" CssClass="btn-primary" ColumnSpan="2">İşyeri</asp:TableCell>
                            <asp:TableCell ID="MerArsaCell" CssClass="btn-primary" ColumnSpan="2">Arsa</asp:TableCell>
                            <asp:TableCell ID="MerTarlaCell" CssClass="btn-primary" ColumnSpan="2">Tarla</asp:TableCell>
                            <asp:TableCell ID="MerTMCell" CssClass="btn-primary" RowSpan="2">TM</asp:TableCell>
                            <asp:TableCell ID="MerCMCell" CssClass="btn-primary" RowSpan="2">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerToplamCell" CssClass="btn-primary" RowSpan="2">Toplam</asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="MerAptTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerAptCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerMEvTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerMEvCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerMeskenTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerMeskenCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerIsyeriTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerIsyeriCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerArsaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerArsaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
                            <asp:TableCell ID="MerTarlaTMCell" CssClass="btn-primary">TM</asp:TableCell>
                            <asp:TableCell ID="MerTarlaCMCell" CssClass="btn-primary">ÇM</asp:TableCell>
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