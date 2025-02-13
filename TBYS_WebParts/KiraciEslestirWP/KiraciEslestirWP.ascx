<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraciEslestirWP.ascx.cs" Inherits="TBYS_WebParts.KiraciEslestirWP.KiraciEslestirWP" %>
<script type="text/javascript">
    
    function KiraciSec(kiraciId, sozlesmeId) {
        document.getElementById('<%= paramKiraciIdLbl.ClientID%>').value = kiraciId;
        document.getElementById('<%= KiraciSecBtn.ClientID%>').click();
    }
</script>
<style>
    .pasif-kiraci {
        background-color: lightgrey !important;
        color: black !important;
    }
</style>
<div class="container">
<%--    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>--%>
            <div style="display: none">
                <input id="paramKiraciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramSozlesmeIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="KiraciSecBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="KiraciSecBtn_Click" />
            </div>
            <div class="card shadow">
                <div class="card-header ">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label CssClass="col-form-label btn-outline-danger" runat="server" Text="Kiracı Eşleştirme"></asp:Label>
                    </h3>
                </div>
                <div class="card-body border border-default" runat="server" id="PUTableDiv">
                    <div class="form-group border border-info" id="KiraciAraDiv" runat="server" style="display: block">
                        <div class="input-group col-6">
                            <label class="col-form-label m-1" for="KiraciAraTxt">Kiracı Ara :</label>
                            <asp:TextBox ID="KiraciAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="KiraciAraTxt_TextChanged" ToolTip="Ad,TCKimlikNo,Telefon veya Adres yazarak arayabilirsiniz" />
                            <%--<asp:LinkButton CssClass="btn btn-info m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="KiraciAraBtn_Click" />--%>
                        </div>
                    </div>
                    <div class="form-group border border-info" id="KiraciSecTableDiv" runat="server">
                        <asp:Label ID="GelenOdemeLbl" CssClass="col-form-label font-weight-bold" runat="server" Text="Gelen Ödeme"></asp:Label>
                        <table id="CustomDataTable" class="table table-hover table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Kiracı No</th>
                                    <th>Adı/Ünvanı</th>
                                    <th>Adresi</th>
                                    <th>Bölge</th>
                                    <th>İlçe/İl</th>
                                    <th>Sözleşme Başlangıcı</th>
                                    <th>Sözleşme Bitişi</th>
                                    <th>Kira Bedeli</th>
                                    <th>Seç</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end" ID="EkstreListesiBtn" runat="server" Text="Ekstre Listesi" CausesValidation="false" OnClick="EkstreListesiBtn_Click" />
                </div>
            </div>

<%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
    <asp:UpdateProgress ID="updateProgress" runat="server">
        <ProgressTemplate>
            <div class='loaderMainContainer'>
                <div class='loaderContainer'>
                    <div class='loaderCircle'></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</div>