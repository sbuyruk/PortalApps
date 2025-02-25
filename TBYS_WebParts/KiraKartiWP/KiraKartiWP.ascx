<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraKartiWP.ascx.cs" Inherits="TBYS_WebParts.KiraKartiWP.KiraKartiWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<div class="container shadow">
    <div class="card">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Kiracı Kartı"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label text-danger"></asp:Label>
            </h3>
        </div>
        <div class="card-body">

            <asp:Table ID="KiraTable" runat="server" class="table ">
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell CssClass="fw-bold" ColumnSpan="7" Font-Bold="True" Font-Size="XX-Large">TSKGV KİRA KARTI</asp:TableCell>
                    <asp:TableCell CssClass="fw-bold float-end" ID="KartNoHdrCell">KART NO</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1" CssClass="bg-light fw-bold" RowSpan="3" >KİRACININ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold" >ADI SOYADI</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="6" ID="AdiCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">İLETİŞİM</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="4" ID="TelNoCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="2" ID="EpostaCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">KİMLİK NO</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="2" ID="TCKimlikNoCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">VERGİ DAİ.</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="VergiDairesiCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">VERGİ No.</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="VergiNoCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">KONTRAT TARİHİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="7" ID="SozlesmeTarCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold" RowSpan="3">KİRALANANIN</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">İLİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="2" ID="IliCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">İLÇESİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="3" ID="IlcesiCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">SEMTİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="2" ID="SemtCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">ADRESİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="3" ID="AdresCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">Kiralama Amacı</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="KiralamaAmaciCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold" >NİTELİĞİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="NiteligiCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">YÜZÖLÇÜMÜ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ColumnSpan="2" ID="YuzolcumuCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light  fw-bold">KİRA TEMINAT TARİHİ</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white " ID="TeminatTarihiCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light  fw-bold" ID="KiraTeminatiCell" runat="server">TEMİNAT</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white " ID="TeminatTutariCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light  fw-bold">ÖDENEN TEMİNAT:</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="OdenenTeminatTutariCell" runat="server"></asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light  fw-bold">KALAN TEMİNAT:</asp:TableCell>
                    <asp:TableCell BorderStyle="Solid" BorderWidth="1"  CssClass="bg-white" ID="KalanTeminatTutariCell" runat="server"></asp:TableCell>
                </asp:TableRow>
                <%--                <asp:TableRow HorizontalAlign="Left">
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">DÖNEM</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">KİRA TUTARI</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">ÖDENEN TARİH</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">ÖDENEN TUTAR</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">KALAN ANAPARA</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">GECİKME FAİZİ</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">FAİZLİ BAKİYE</asp:TableCell>
                    <asp:TableCell  BorderStyle="Solid" BorderWidth="1"  CssClass="bg-light fw-bold">AÇIKLAMA</asp:TableCell>
                </asp:TableRow>--%>
            </asp:Table>
        </div>
        <div class="card-footer">
            
            <asp:LinkButton CssClass="btn btn-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton ID="KiraciListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Kiracı Listesi" OnClick="KiraciListesiBtn_Click" />
            <asp:CheckBox ID="TumunuSecChk" CssClass="float-end mr-4" AutoPostBack="true" runat="server" Text="Tüm Sözleşmeleri Göster" Checked="false" OnCheckedChanged="TumunuSecChk_CheckedChanged" TextAlign="Right" />
        </div>
    </div>
</div>
