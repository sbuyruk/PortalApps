<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RandevuKartiWP.ascx.cs" Inherits="MTS_WebParts.RandevuKartiWP.RandevuKartiWP" %>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Faaliyet Kartı"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">
            <div class="table">
                <asp:Table ID="RandevuKartiBilgileriTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell ColumnSpan="4" BackColor="Silver">Faaliyet Bilgileri</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </div>
            <div id="AniObjesiDiv" class="table">
                <asp:Table ID="AniObjesiBilgileriTable" runat="server" CssClass="table table-sm table-hover table-striped table-bordered" BorderStyle="Solid">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell ColumnSpan="4" BackColor="Silver">Anı Objesi Bilgileri</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Katılımcılar</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Katılımcı Tipi</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Verilen Anı Objesi</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Getirilen Anı Objesi</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="RandevuyaGitBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Faaliyete Git" OnClick="RandevuyaGitBtn_Click"></asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton ID="RandevuTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Faaliyet Takvimi" OnClick="RandevuTakvimiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="RandevuListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Faaliyet Listesi" OnClick="RandevuListesiBtn_Click"></asp:LinkButton>

        </div>
    </div>
</div>
