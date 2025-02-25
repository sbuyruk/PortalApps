<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GecikmeZammiWP.ascx.cs" Inherits="TBYS_WebParts.GecikmeZammiWP.GecikmeZammiWP" %>
<script type="text/javascript">
    function EkleBtnEnable() {
        var tarih = $('#<%= YeniBaslangicTarihiTxt.ClientID%>').val().replace(/ /g, '') == '';
        var oran = $('#<%= YeniZamOraniTxt.ClientID%>').val().replace(/ /g, '') == '';
        if (oran || tarih) {
            $("#YeniFaizOraniEkleBtn").attr('class', 'btn btn-outline-secondary');
            $("#YeniFaizOraniEkleBtn").attr('disabled', true);
            $('#<%= EkleBtnDiv.ClientID%>').hide();
        }
        else {
            $("#YeniFaizOraniEkleBtn").attr('class', 'btn btn-outline-success');
            $("#YeniFaizOraniEkleBtn").attr('disabled', false);
            $('#<%= EkleBtnDiv.ClientID%>').show();
        }
    }
</script>
<div class="container shadow w-75">

    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Gecikme Zammı Oranları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body " id="MainCardDiv" runat="server">
            <div class="form-group row border border-dark m-1">
                <div class="form-group col-3">
                    <asp:Label CssClass="col-form-label" runat="server">Başlangıç Tarihi</asp:Label>
                    <input id="YeniBaslangicTarihiTxt" runat="server" class="form-control DateTimePickerV1" readonly="readonly"
                        onchange="EkleBtnEnable()" onkeyup="EkleBtnEnable()" oncut="EkleBtnEnable()" onpaste="EkleBtnEnable()" oninput="EkleBtnEnable()" />
                </div>
                <div class="form-group col-2">
                    <asp:Label CssClass="col-form-label" runat="server">Faiz Oranı</asp:Label>
                    <input id="YeniZamOraniTxt" runat="server" class="form-control input-decimal"
                        onchange="EkleBtnEnable()" onkeyup="EkleBtnEnable()" oncut="EkleBtnEnable()" onpaste="EkleBtnEnable()" oninput="EkleBtnEnable()" />
                </div>
                <div class="form-group col-4">
                    <asp:Label CssClass="col-form-label" runat="server">Açıklama</asp:Label>
                    <input id="YeniAciklamaTxt" runat="server" class="form-control " />
                </div>
                <div class="form-group col-2" style="display: none" id="EkleBtnDiv" runat="server">
                    <asp:Label CssClass="col-form-label text-white" runat="server">".  .  .  .  ."</asp:Label>
                    <asp:LinkButton ID="EkleBtn" CssClass="btn btn-outline-success" Text="Yeni Oran Ekle" runat="server" OnClick="EkleBtn_Click" />
                </div>
            </div>
            <div id="AylikFaizOranlariDiv" class="border border-dark row m-1">
                <asp:Table ID="AyrintiTable" runat="server" class="table table-striped table-hover table-sm table-striped">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Sıra</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Başlangıç Tarihi</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Bitiş Tarihi</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Faiz Oranı</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="200">Açıklama</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Güncelle</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Sil</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </div>
        </div>
        <div class="card-footer">
        </div>
    </div>
</div>