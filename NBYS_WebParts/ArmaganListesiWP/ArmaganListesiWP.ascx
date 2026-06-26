<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArmaganListesiWP.ascx.cs" Inherits="NBYS_WebParts.ArmaganListesiWP.ArmaganListesiWP" %>

<style>
    .kontrol-edildi {
        background-color: lightyellow;
    }

    .gonderildi {
        background-color: lightCyan;
    }

    .diger {
        background-color: lightgray;
    }
    .afet-ili {
        background-color: yellow !important;
        color: orangered;
        font-weight:bold;
    }
    .belge-gecersiz {
        background-color: black;
        color: lightgray;
    }
    div.dataTables_wrapper  div.dataTables_filter { /*Ara kutusunu sola çekmek için*/
      width: 100%;
      float: none;
      text-align: left;
    }
</style>
<script>
    
    function OpenModal(armaganId) {
        document.getElementById('<%= paramArmaganIdLbl.ClientID%>').value = armaganId;
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
    }
    function CallButtonClick(armaganId) {
        document.getElementById('<%= paramArmaganIdLbl.ClientID%>').value = armaganId;
        document.getElementById('<%= IadeEdildiYapBtn.ClientID%>').click();
    }
</script>

<div class="col-xl">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header ">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <a class=" btn btn-outline-primary float-end me-4" runat="server" id="YonergeLnk"
                            data-fancybox
                            data-type="pdf"
                            data-width="960"
                            data-height="720"
                            href="">
                            <i class="fa fa-book" aria-hidden="true"></i>
                        </a>
                        <asp:Label CssClass="form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Armağan Listesi"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="form-label text-white" Visible="false"></asp:Label>
                        <asp:Label ID="AdiLbl" runat="server" CssClass="form-label"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div style="display: none">
                        <input id="paramArmaganIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="IadeEdildiYapBtn" runat="server" OnClientClick="{return true;};" OnClick="IadeEdildiYapBtn_Click"></asp:LinkButton>
                    </div>
                    <div class="row m-2 ">
                        <div class="form-group col">
                            <label class="form-label" for="AyDDL">Ay </label>
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                        </div>
                        <div class="form-group col">
                            <label class="form-label" for="YilDDL">Yıl </label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                        </div>
                        <div class="form-group col">
                            <label class="form-label" for="ArmaganDDL">Armağan </label>
                            <asp:DropDownList ID="ArmaganDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="ArmaganDDL_SelectedIndexChanged" Style="height: auto" />
                        </div>
                        <div class="form-group col">
                            <label class="form-label" for="DurumDDL">Durum </label>
                            <asp:DropDownList ID="DurumDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="DurumDDL_SelectedIndexChanged" Style="height: auto" />
                        </div>
                        <div class="form-group col">
                            <label class="form-label" for="IliDDL">İl </label>
                            <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" Style="height: auto" />
                        </div>

                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-hover" width="100%">
                            <thead>
                                <tr>
                                    <th>Adı</th>
                                    <th>Belgede Yazan İsim</th>
                                    <th>TC Kimlik</th>
                                    <th>Tutar</th>
                                    <th>Tarih</th>
                                    <th>Armağan</th>
                                    <th>Durumu</th>
                                    <th>Bağış</th>
                                    <th>Düzenle</th>
                                    <th>İade</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="EtiketOlusturBtn" runat="server" CssClass="btn btn-outline-success" Text="Teşekkür Etiketlerini Oluştur" OnClick="EtiketOlusturBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="ArmaganListesiBtn" runat="server" CssClass="btn btn-outline-success" Text="Aylık Armağan Listesi" OnClick="ArmaganListesiBtn_Click" />
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
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content" >
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="BagisciAdiLbl" runat="server" Text="Bağışçı Bilgileri" Font-Bold="True"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClick="ModalDoldurBtn_Click" />
                        </div>

                        <div class="text-center" id="NakitBagisciDiv">
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
                                        <th>Armağan</th>
                                        <th>Armağan Tutarı</th>
                                        <th>Açıklama</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>

