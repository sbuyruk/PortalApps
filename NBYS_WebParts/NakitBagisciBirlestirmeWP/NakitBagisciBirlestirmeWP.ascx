<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciBirlestirmeWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciBirlestirmeWP.NakitBagisciBirlestirmeWP" %>
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
</style>
<script>
    function FillAsilBagisciTable(bagisciId) {

        document.getElementById('<%= paramLbl.ClientID%>').value = bagisciId;
        document.getElementById('<%= AsilBagisciHiddenBtn.ClientID%>').click();
    }

    function addBirlesecekBagisciTable(bagisciId) {

        document.getElementById('<%= paramLbl.ClientID%>').value = bagisciId;
        document.getElementById('<%= BirlesecekBagisciHiddenBtn.ClientID%>').click();
    }
    function bagisciDuzenle(bagisciId) {

        document.getElementById('<%= paramLbl.ClientID%>').value = bagisciId;
        document.getElementById('<%= BagisciDuzenleHiddenBtn.ClientID%>').click();
    }
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }

    var tableData = [];

    function addRemoveBagisciToList(nakitBagisciId, chkbox) {
        var isChecked = false;
        if (chkbox.checked)
            isChecked = true;
        var index = tableData.indexOf(nakitBagisciId);
        if (isChecked && (index < 0)) {
            tableData.push(nakitBagisciId);
        } else if (!isChecked && (index > -1)) {
            tableData.splice(index, 1);
        }
        if (tableData.length > 0)
            document.getElementById('<%= BirlesecekBagisciDiv.ClientID%>').style.display = "block";
        else {
            document.getElementById('<%= BirlesecekBagisciDiv.ClientID%>').style.display = "none";
<%--            document.getElementById('<%= FooterDiv.ClientID%>').style.visibility = "none";
            document.getElementById('<%= BirlestirSubDiv.ClientID%>').style.visibility = "none";--%>
        }

    }
    function TamamBtnClicked() {
        document.getElementById('<%= paramArray.ClientID%>').value = tableData;
        document.getElementById('<%= TamamBtn.ClientID%>').click();
        document.getElementById('<%= BirlesecekBagisciDiv.ClientID%>').style.display = "none";
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
                <asp:Label CssClass="col-form-label bagis-header-color" runat="server" Text="Nakit Bağışçı Birleştirme"></asp:Label>
            </h3>
        </div>
        <div class="card-body border border-default" runat="server" id="PUTableDiv">
            <div class="card" id="AsilBagisciDiv" runat="server" style="display: none">
                <div class="card-body">
                    <div style="display: none">
                        <input id="paramLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="AsilBagisciHiddenBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="AsilBagisciHiddenBtn_Click" />
                    </div>
                    <asp:Table ID="AsilBagisciTable" runat="server" class="table table-primary m-0">
                    </asp:Table>
                    <div class="table" id="AsilBagisciBagislariDiv" runat="server" style="max-height: 250px; overflow: auto;" visible="false">
                    </div>
                </div>
                <div class="card-footer">
                </div>
            </div>
            <div class="form-group border border-info" id="AsilBagisciAraDiv" runat="server" style="display: block">
                <div class="input-group col-6">
                    <label class="col-form-label m-1" for="AsilBagisciAraTxt">Asil Bagisci Ara :</label>
                    <asp:TextBox ID="AsilBagisciAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="AsilBagisciAraTxt_TextChanged" />
                    <asp:LinkButton CssClass="btn btn-info m-1" ID="AraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="AsilBagisciAraBtn_Click" />
                </div>
            </div>
            <div class="form-group border bagis-border-color" id="BirlesecekBagisciAraDiv" runat="server" style="display: none">
                <div class="input-group col-6">
                    <label class="col-form-label m-1" for="AranacakBagisciTxt">Birlesecek Bagisci Ara :</label>
                    <asp:TextBox ID="BirlesecekBagisciAraTxt" runat="server" CssClass="form-control m-1" AutoPostBack="true" OnTextChanged="BirlesecekBagisciAraTxt_TextChanged" />
                    <asp:LinkButton CssClass="btn bagis-color m-1" ID="BirlesecekBagisciAraBtn" runat="server" CausesValidation="false" Text="Ara" OnClientClick="{return true;};" OnClick="BirlesecekBagisciAraBtn_Click" />
                </div>
            </div>
            <div class="form-group border bagis-border-color" id="BagisciSecTableDiv" runat="server" style="display: none">
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
                                <th>Düzenle</th>
                                <th>Seç</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
            <div class="card" id="BirlesecekBagisciDiv" runat="server" style="display: none">
                <div class="card-body">
                    <div style="display: none">
                        <input id="Text1" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="BirlesecekBagisciHiddenBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="BirlesecekBagisciHiddenBtn_Click" />
                        <asp:LinkButton ID="BagisciDuzenleHiddenBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="BagisciDuzenleHiddenBtn_Click" />
                    </div>
                    <asp:Table ID="BirlesecekBagisciTable" runat="server" class="table table-warning m-0">
                    </asp:Table>
                </div>
                <div class="card-footer" id="TamamDiv" runat="server">
                    <input id="TamamTriggerBtn" class="btn btn-success" type="button" value="Tamam" onclick="TamamBtnClicked();" />
                    <div style="display: none">
                        <input id="paramArray" runat="server" type="text" />
                        <asp:LinkButton ID="TamamBtn" runat="server" CausesValidation="false" OnClick="TamamBtn_Click">Tamam</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <div>
                <asp:LinkButton ID="BasadonBtn" CssClass="btn btn-secondary float-end" runat="server" CausesValidation="false" Text="Başa Dön" OnClick="BasadonBtn_Click" />
            </div>
            <div id="FooterDiv" runat="server">
                <asp:RadioButtonList ID="BirlestirRBL" runat="server" CssClass="form-check-label" OnSelectedIndexChanged="BirlestirRBL_SelectedIndexChanged" BorderStyle="Solid" RepeatDirection="Horizontal" AutoPostBack="true">
                    <asp:ListItem Text="Birleştirmek İstemiyorum " Value="Birlestirme" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Birleştirmek İstiyorum " Value="Birlestir"></asp:ListItem>
                </asp:RadioButtonList><div id="BirlestirDiv" runat="server">
                    <div id="BirlestirSubDiv" runat="server" visible="false">
                        <label class="col-form-label">Birleştirme Sebebi</label>
                        <asp:TextBox ID="BirlestirmeSebebiTxt" runat="server"></asp:TextBox><p class="text-danger">Lütfen Dikkat!</p>
                        <p class="text-danger">Birleştir düğmesine bastığınızda seçilen kişilere ait tüm bağış kayıtları ASİL olarak seçilen kişi üzerine aktarılacaktır. Yapılan işlem geri alınamaz.</p>
                        <asp:LinkButton ID="BirlestirBtn" CssClass="btn btn-danger" runat="server" Text="Kişileri Birleştir" OnClick="BirlestirBtn_Click" Visible="false" />
                    </div>
                </div>
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
