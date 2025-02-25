<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisHareketSilmeWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisHareketSilmeWP.NakitBagisHareketSilmeWP" %>
<style>
    .bagis-border-color {
        border: thick;
        border-color: blueviolet;
    }

    .bagis-color {
        background-color: blueviolet;
        color: white;
    }

    .bagis-header-color {
        color: blueviolet;
        font-weight: bold;
    }
    /*tabloyu armagan durumuna göre renklendirsin*/
    .kontrol-edildi {
        background-color: lightyellow;
    }

    .gonderildi {
        background-color: lightCyan;
    }

    .diger {
        background-color: lightgray;
    }

    .bagis-iade-edildi {
        background-color: black;
        color: lightgray;
    }
</style>
<script>
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();

    }

    function CloseModal() {
        var myModalEl = document.getElementById('ModalOnayDiv');
        var modalInstance = bootstrap.Modal.getInstance(myModalEl);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
    function CallButtonClick(bagisHareketId) {
        document.getElementById('<%= paramBagisHareketIdLbl.ClientID%>').value = bagisHareketId;
        document.getElementById('<%= BagisSilBtn.ClientID%>').click();
    }
    function ModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>


<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card">
                <div class="card-header ">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label CssClass="col-form-label fw-bold text-info" runat="server" Text="Bağış Silme"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div style="display: none">
                        <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <input id="paramBagisHareketIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <input id="paramArmaganIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                    </div>
                    <div class="form-group row" id="FilterDiv" runat="server">
                        <div class="form-group col-2">
                            <label for="YilDDL" class=" col-form-label">Yıl</label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <label for="AyDDL" class="col-form-label">Ay</label>
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>Bağışçı Adı</th>
                                    <th>TC Kimlik No</th>
                                    <th>Bağış Miktarı</th>
                                    <th>Bağış Tarihi</th>
                                    <th>Banka</th>
                                    <th>İli</th>
                                    <th>Adres</th>
                                    <th>Kayıt Sil</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="BagisHareketListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Bağış Hareketleri" OnClick="BagisHareketListesiBtn_Click" />
                    <asp:LinkButton ID="ArmaganListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Armağan Listesi" OnClick="ArmaganListesiBtn_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="BagisSilBtn" runat="server" CausesValidation="false" OnClick="BagisSilBtn_Click" />
                        </div>
                        <div class="card" runat="server" id="ParaIadeDiv">
                            <div class="card-header text-center">
                                <h3>
                                    <asp:Label ID="Label3" class="col-form-label" runat="server" Text="Bağış Silinecek"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <input id="SilmeSebebiTxt" textmode="MultiLine" rows="3" runat="server" placeholder="Silme sebebini giriniz" class="form-control" type="text" />
                                <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                <asp:Label ID="OnayMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Bağışın silinmesini onaylıyor musunuz?"></asp:Label>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton CssClass="btn btn-danger" ID="BagisSilNowBtn" runat="server" CausesValidation="false" Text="Bağışı Sil" OnClientClick="{return true;};" OnClick="BagisSilNowBtn_Click" />
                                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                            </div>
                        </div>
                        <div>
                        </div>
                    </div>
                    <div class="modal-footer">
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
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

                        <div class="m-1 text-center" id="NakitBagisciDiv">
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
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
