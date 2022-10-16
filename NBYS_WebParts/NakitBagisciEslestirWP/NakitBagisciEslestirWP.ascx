<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciEslestirWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciEslestirWP.NakitBagisciEslestirWP" %>
<script type="text/javascript">
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;

        $("#ModalUrlDiv").modal({ backdrop: false });
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }
</script>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
</style>
<div class="container shadow">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <div style="display: none">
                <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
            </div>
            <div class="card">
                <div class="card-header ">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label CssClass="col-form-label btn-outline-success" runat="server" Text="Nakit Bağışçı Eşleştirme"></asp:Label>
                    </h3>
                </div>
                <div class="card-body border border-default" runat="server" id="PUTableDiv">
                    <div class="form-group border border-success" id="BagisciAraDiv" runat="server" style="display: block">
                        <div class="input-group col-6">
                            <label class="col-form-label m-1" for="BagisciAraTxt">Bağışçı Ara :</label>
                            <asp:TextBox ID="BagisciAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="BagisciAraTxt_TextChanged" ToolTip="Ad,TCKimlikNo,Telefon veya Adres yazarak arayabilirsiniz" />
                            <asp:LinkButton CssClass="btn btn-success m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="BagisciAraBtn_Click" />
                        </div>
                    </div>
                    <div class="form-group border border-success" id="BagisciSecTableDiv" runat="server" style="display: none">
                        <div class="form-group table loader">
                            <input class="form-control col-6" id="globalFilter" placeholder="Aranacak Kelime" size="30" />
                            <div id="tblfilter"></div>
                            <div id="messages"></div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right" ID="EkstreListesiBtn" runat="server" Text="Ekstre Listesi" CausesValidation="false" OnClick="EkstreListesiBtn_Click" />
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
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

<div class="modal alert-secondary" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-xl">
        <!-- Modal content-->
        <div class="modal-content" style="width: 1030px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <asp:Label ID="AdiLbl" runat="server" Text="Label"></asp:Label>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />

                        </div>
                        <div class="m-1 text-center" id="NakitBagisciDiv">
                            <asp:Table CssClass="table text-center" ID="BagisciTable" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableCell>Ad/Ünvan</asp:TableCell>
                                    <asp:TableCell>TC Kimlik No</asp:TableCell>
                                    <asp:TableCell>Telefon</asp:TableCell>
                                    <asp:TableCell>İli/İlçesi</asp:TableCell>
                                    <asp:TableCell>Adres</asp:TableCell>
                                    <asp:TableCell>Tüzel Kişi</asp:TableCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                        <div class="table loader">
                            <div id="modaltblfilter" class="table" style="width: 1000px; height: 400px;"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
