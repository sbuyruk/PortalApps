<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciBagislariWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciBagislariWP.NakitBagisciBagislariWP" %>

<script type="text/javascript">
    function BagisListesiGoster(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;
        document.getElementById('<%= BagisListesiGosterBtn.ClientID%>').click();
    }

    //Excel'e export ettikten sonra dönüp kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
</style>
<div class="container">

    <div style="display: none">
        <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
    </div>
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-info" runat="server" Text="Bağışçı Bazında Nakit Bağışlar"></asp:Label>
            </h3>
        </div>
        <div class="card-body border border-default" runat="server" id="PUTableDiv">
            <div class="form-group border border-info" id="BagisciAraDiv" runat="server" style="display: block">
                <div class="input-group col-6">
                    <label class="col-form-label m-1" for="BagisciAraTxt">Bağışçı Ara :</label>
                    <asp:TextBox ID="BagisciAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="BagisciAraTxt_TextChanged" />
                    <asp:LinkButton CssClass="btn btn-info m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="BagisciAraBtn_Click" />
                </div>
            </div>

            <div class="form-group border border-info" id="BagisciSecTableDiv" runat="server" style="display: none">
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                        <thead>
                            <tr>
                                <th>Adı</th>
                                <th>TC Kimlik No</th>
                                <th>İli</th>
                                <th>İlçesi</th>
                                <th>Telefon</th>
                                <th>Adres</th>
                                <th>Seç</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
            <div style="display: none">
                <asp:LinkButton ID="BagisListesiGosterBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="BagisListesiGosterBtn_Click" />
            </div>
            <div class="form-group border border-info" id="BagisTableDiv" runat="server" style="display: none">
                <div class="form-group">
                    <asp:Label ID="BagisciAdiLbl" CssClass="fw-bold" runat="server" Text="Label"></asp:Label>
                    <asp:Label ID="BagisBilgileriLbl" runat="server" Text="Label"></asp:Label>
                    <table id="BagisDataTable" class="table table-bordered table-striped" width="100%">
                        <thead>
                            <tr>
                                <th>Bağış Tarihi</th>
                                <th>Bağış Miktarı</th>
                                <th>Banka</th>
                                <th>Armağan</th>
                                <th>Armağan Tutarı</th>
                                <th>Armağan Durumu</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>

        </div>
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


</div>

