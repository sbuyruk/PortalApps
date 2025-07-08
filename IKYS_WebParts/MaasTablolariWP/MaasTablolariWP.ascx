<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaasTablolariWP.ascx.cs" Inherits="IKYS_WebParts.MaasTablolariWP.MaasTablolariWP" %>

<style>
    .header-center {
        text-align: center;
        vertical-align: middle!important;
    }
</style>


<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>

<div class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Maaş Tabloları"></asp:Label>
            </h3>
        </div>
        <div class="card-body">

            <div class="form-group row">
                <div class="form-group col-3">
                    <asp:Label CssClass="form-label fw-semibold" runat="server">Geçerlilik Tarihi : </asp:Label>
                    <asp:DropDownList ID="TarihDDL" runat="server" class="form-control form-select form-select-lg fw-bold text-success" OnSelectedIndexChanged="TarihDDL_SelectedIndexChanged" AutoPostBack="true" />
                </div>
            </div>

            <div class="form-group">
                <div class="float-end">
                    <asp:LinkButton ID="ExportTablo1ToExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExportTablo1ToExcelBtn_Click" CssClass="btn btn-success" />
                </div>
                <div class="table mb-5" id="TablesDiv" runat="server" >
                    <asp:Table ID="UcretTanimTable1" runat="server" CssClass="table table-bordered">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ColumnSpan="12" CssClass="header-center" runat="server" ID="Table1Title">TSKGV ÜCRET TABLOSU-1</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ColumnSpan="12" CssClass="header-center">DERECE</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell CssClass="header-center">KADEME</asp:TableHeaderCell>
                            <asp:TableHeaderCell ColumnSpan="2" CssClass="header-center">Genel Müdür</asp:TableHeaderCell>
                            <asp:TableHeaderCell ColumnSpan="2" CssClass="header-center">Genel Müdür Yardımcısı</asp:TableHeaderCell>
                            <asp:TableHeaderCell ColumnSpan="2" CssClass="header-center">Direktör/Müdür/ Baş Hukuk Mşvr  /Bölge Temsilcisi</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Baş Uzman/ Hukuk Müşaviri</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Kıdemli Uzman</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Uzman</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Uzman Yardımcısı</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Hizmetli/ Şoför</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                    <br />
                    <asp:Table ID="UcretTanimTable2" runat="server" CssClass="table table-bordered">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ColumnSpan="9" CssClass="header-center" ID="Table2Title" runat="server">TSKGV ÜCRET TABLOSU-2 (TSK'DAN EMEKLİ PERSONEL İÇİN GEÇERLİDİR)</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell ColumnSpan="9" CssClass="header-center">DERECE</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell CssClass="header-center">KADEME</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Genel Müdür</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Genel Müdür Yardımcısı</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Direktör/Müdür/ Baş Hukuk Mşvr  /Bölge Temsilcisi</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Baş Uzman/ Hukuk Müşaviri</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Kıdemli Uzman</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Uzman</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Uzman Yardımcısı</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="header-center">Hizmetli/ Şoför</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
            </div>
           
        </div>
    </div>
</div>
