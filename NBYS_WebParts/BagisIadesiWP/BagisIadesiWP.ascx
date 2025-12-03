<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisIadesiWP.ascx.cs" Inherits="NBYS_WebParts.BagisIadesiWP.BagisIadesiWP" %>

<style>
    .bagis-border-color {
        border: thick;
        border-color: blueviolet;
    }

    .bagis-color {
        background-color: blueviolet;
        color: white;
    }

    .bagis-text-color {
        color: blueviolet;
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
    function OpenModal(armaganId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = armaganId;

        var myModal = new bootstrap.Modal(document.getElementById('ModalUrlDiv'));
        myModal.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();

    }
    function CallButtonClick(bagisHareketId) {
        document.getElementById('<%= paramBagisHareketIdLbl.ClientID%>').value = bagisHareketId;
        document.getElementById('<%= BagisiIadeEtBtn.ClientID%>').click();
    }
    function IadeBilgisiDegistirClick(bagisHareketId) {
        document.getElementById('<%= paramBagisHareketIdLbl.ClientID%>').value = bagisHareketId;
        document.getElementById('<%= IadeDegistirBtn.ClientID%>').click();
    }
    function ParaIadeModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ParaIadeModalOnayDiv'));
        myModalInstance.show();
    }
    function ParaIadeDegistirModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ParaIadeDegistirModalOnayDiv'));
        myModalInstance.show();
    }

</script>


<div class="container ">
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
                <asp:Label CssClass="col-form-label bagis-text-color" runat="server" Text="Bağış İadesi"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div style="display: none">
                <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramBagisHareketIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramArmaganIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />

            </div>
            <div class="form-group border bagis-border-color" id="AraDiv" runat="server" style="display: block">
                <div class="input-group col-6">
                    <label class="col-form-label m-1" for="BagisAraTxt">Aranacak sözcük :</label>
                    <asp:TextBox ID="BagisAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="BagisAraTxt_TextChanged" ToolTip="Ad,TCKimlikNo,Telefon veya Adres yazarak arayabilirsiniz" />
                    <asp:LinkButton CssClass="btn bagis-color m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="AraBtn_Click" />
                    <asp:Label CssClass="btn text-danger m-1" ID="UyariLbl" runat="server" Text=" * Yalnızca son 1 yıl içinde yapılan bağışlar iade edilebilir." />
                </div>
            </div>
            <div class="form-group border bagis-border-color" id="BagisciSecTableDiv" runat="server">
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                        <thead>
                            <tr>
                                <th>Bağışçı Adı</th>
                                <th>Bağış Miktarı</th>
                                <th>Bağış Tarihi</th>
                                <th>Armağan</th>
                                <th>Durum</th>
                                <th>Telefon</th>
                                <th>Adres</th>
                                <th>İade</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="BagisHareketListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Bağış Hareketleri" OnClick="BagisHareketListesiBtn_Click" />
            <asp:LinkButton ID="ArmaganListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Armağan Listesi" OnClick="ArmaganListesiBtn_Click" />
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
                                <asp:Label ID="Label1" runat="server" Text="Bağışçının Yaptığı Nakit Bağışlar" Font-Bold="True"></asp:Label>
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
<div class="modal" id="ParaIadeModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="BagisiIadeEtBtn" runat="server" CausesValidation="false" OnClick="BagisiIadeEtBtn_Click" />
                            <asp:LinkButton ID="IadeDegistirBtn" runat="server" CausesValidation="false" OnClick="IadeDegistirBtn_Click" />
                        </div>
                        <div class="card" runat="server" id="ParaIadeDiv">
                            <div class="card-header text-center">
                                <h3>
                                    <asp:Label ID="Label3" class="col-form-label" runat="server" Text="Para İade Edilecek"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <asp:Label ID="Label2" class="col-form-label" runat="server" Text="İade Sebebi"></asp:Label></h3>
                                    <input id="IadeSebebiTxt" textmode="MultiLine" rows="3" runat="server" placeholder="Para iade sebebini giriniz" class="form-control" type="text" />
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="Label4" class="col-form-label" runat="server" Text="İade Tarihi"></asp:Label>
                                    <asp:TextBox ID="IadeTarihiTxt" CssClass="DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static" ></asp:TextBox>
                                </div>
                                <asp:Label ID="IadeMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                <asp:Label ID="OnayMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Para İadesini Onaylıyor musunuz?"></asp:Label>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton CssClass="btn btn-danger" ID="BagisiIadeEtNowBtn" runat="server" CausesValidation="false" Text="Parayı İade Et" OnClientClick="{return true;};" OnClick="BagisiIadeEtNowBtn_Click" />
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
<div class="modal" id="ParaIadeDegistirModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="false" OnClick="BagisiIadeEtBtn_Click" />
                        </div>
                        <div class="card" runat="server" id="Div1">
                            <div class="card-header text-center">
                                <h3>
                                    <asp:Label ID="Label5" class="col-form-label" runat="server" Text="İade Bilgileri Değişecek"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <asp:Label ID="Label6" class="col-form-label fw-bold" runat="server" Text="İade Sebebi"></asp:Label></h3>
                                    <input id="IadeSebebiDegistirTxt" textmode="MultiLine" rows="3" runat="server" placeholder="Para iade sebebini giriniz" class="form-control" type="text" />
                                </div>
                                <div class="form-group">
                                    <asp:Label ID="Label7" class="col-form-label fw-bold" runat="server" Text="İade Tarihi"></asp:Label>
                                    <asp:TextBox ID="IadeTarihiDegistirTxt" CssClass=" col-6 form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text="" ClientIDMode="Static"></asp:TextBox>
                                </div>
                                <asp:Label ID="IadeMesajiDegistirLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                <asp:Label ID="OnayMesajiDegistirLbl" CssClass="col-form-label text-primary" runat="server" Text="İade Değişikliğini Onaylıyor musunuz?"></asp:Label>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton CssClass="btn btn-primary" ID="IadeDegistirNowBtn" runat="server" CausesValidation="false" Text="Güncelle" OnClientClick="{return true;};" OnClick="IadeDegistirNowBtn_Click" />
                                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                            </div>
                        </div>
                        <div>
                        </div>
                    </div>
                    <div class="modal-footer">
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="IadeDegistirBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
