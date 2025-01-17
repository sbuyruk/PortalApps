<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArmaganAraWP.ascx.cs" Inherits="NBYS_WebParts.ArmaganAraWP.ArmaganAraWP" %>

<script type="text/javascript">
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;

        $("#ModalUrlDiv").modal({ backdrop: false });
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }
    
</script>

<div class="container ">
    <div style="display: none">
        <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
    </div>
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label btn-outline-primary" runat="server" Text="Armağan Ara"></asp:Label>

            </h3>
        </div>
        <div class="card-body border border-default" runat="server" id="PUTableDiv">
            <div class="form-group border border-primary" id="ArmaganAraDiv" runat="server" style="display: block">
                <div class="input-group col-6">
                    <label class="col-form-label m-1" for="ArmaganAraTxt">Armağan Ara :</label>
                    <asp:TextBox type="number"  ID="ArmaganAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="ArmaganAraTxt_TextChanged"  
                        placeholder="Armağan Belge No " 
                        title="Armağan Belge No yazarak arayabilirsiniz"  ToolTip="Armağan Belge No yazarak arayabilirsiniz" />
                    <asp:LinkButton CssClass="btn btn-primary m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="ArmaganAraBtn_Click" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label CssClass="text-danger font-weight-bold" runat="server">Armağan Belge Numaraları Nisan 2018 tarihinde NBYS yazılımının devreye girmesinden itibaren düzenli olarak verilmektedir. 
                    Daha eski tarihli belgelere ait numaralar manüel verilmiştir ve farklılık gösterebilir.
                </asp:Label>
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Belge No</th>
                            <th>Adı Soyadı</th>
                            <th>Armağan</th>
                            <th>Belge Tarihi</th>
                            <th>TC Kimlik No</th>
                            <th>İl</th>
                            <th>İlçe</th>
                            <th>Telefon</th>
                            <th>Adres</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
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
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="ArmaganAdiLbl" runat="server" Text="Bağışçı Bilgileri" Font-Bold="True"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClick="ModalDoldurBtn_Click" />
                        </div>

                        <div class="m-1 text-center" id="ArmaganDiv">
                            <asp:Table CssClass="table text-center table-bordered table-striped" ID="BagisciTable" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Ad/Ünvan</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>TC Kimlik No</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Adres</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>İli/İlçesi</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Telefon</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Tüzel Kişi</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                        <div>
                            <h3>
                                <br />
                                <asp:Label ID="BagisBilgileriLbl" runat="server" Text="Bağışçının Yaptığı Nakit Bağışlar" Font-Bold="True"></asp:Label>
                            </h3>
                        </div>
                        <div class="form-group">
                            <table id="CustomModalDataTable" class="table table-bordered table-striped" width="100%">
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
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
