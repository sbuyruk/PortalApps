<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuzenliNakitBagisciListesiWP.ascx.cs" Inherits="NBYS_WebParts.DuzenliNakitBagisciListesiWP.DuzenliNakitBagisciListesiWP" %>
<script>    
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }
    var selectedNakitBagisciId = null;
    var selectedDuzenliBagisciId = null;

    function showDuzenliBagisOnay(nakitBagisciId, duzenliBagisciId) {
        selectedNakitBagisciId = nakitBagisciId;
        selectedDuzenliBagisciId = duzenliBagisciId;
        $('#duzenliBagisOnayModal').modal('show');
    }

    $(document).on('click', '#onaylaBtn', function () {
        $('#duzenliBagisOnayModal').modal('hide');
        DuzenliBagisBelgesiOlustur(selectedNakitBagisciId, selectedDuzenliBagisciId);
    });

    function DuzenliBagisBelgesiOlustur(nakitBagisciId, duzenliBagisciId) {
        // Parametreleri hidden inputlara yaz
        document.getElementById('<%= hdnNakitBagisciId.ClientID %>').value = nakitBagisciId;
        document.getElementById('<%= hdnDuzenliBagisciId.ClientID %>').value = duzenliBagisciId;
        // Sunucu tarafı eventini tetikle
        document.getElementById('<%= ArmaganOlusturBtn.ClientID %>').click();
    }
</script>
<style>
    .table-danger {
        background-color: #ffcccc !important;
        font-weight: bold;
        color: #a94442 !important;
    }
</style>
<div class="col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <a class=" btn btn-outline-primary float-end me-4" runat="server" id="YonergeLnk"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Düzenli Nakit Bağışçı Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body mt-1">
            <div class="row m-2 ">
                <div class="form-group col-2">
                    <label class="form-label" for="AyDDL">Başlangıç Ayı </label>
                    <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
                <div class="form-group col-2">
                    <label class="form-label" for="YilDDL">Başlangıç Yılı </label>
                    <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
                <div class="form-group col-3">
                    <label class="form-label" for="DurumDDL">Durum </label>
                    <asp:DropDownList ID="DurumDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="DurumDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
            </div>
            <div style="display: none">
                <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped small" width="100%">
                    <thead>
                        <tr>
                            <th>Adı Soyadı</th>
                            <th>TCKN</th>
                            <th>Tutar</th>
                            <th>Başlama Tarihi</th>
                            <th>Telefon</th>
                            <th>İl</th>
                            <th>İlçesi</th>
                            <th>Adres</th>
                            <th>İşlem Aciklama</th>
                            <th>Düzenli Bağış Belgesi</th>
                            <th>Bağışçı</th>
                        </tr>
                    </thead>
                </table>
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
        <div class="card-footer">
            
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

<div class="modal" id="duzenliBagisOnayModal" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h5 class="modal-title" id="onayModalLabel">Onay</h5>
                        <button type="button" class="close float-end" data-bs-dismiss="modal" aria-label="Kapat">
                          <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        Düzenli Bağışçı Belgesi oluşturulsun mu?

                    </div>
                    <div style="display: none">
                        <asp:LinkButton ID="ArmaganOlusturBtn" runat="server" CausesValidation="false" Text="" OnClick="ArmaganOlusturBtn_Click" />
                        <input type="hidden" id="hdnNakitBagisciId" runat="server" />
                        <input type="hidden" id="hdnDuzenliBagisciId" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        <button type="button" class="btn btn-success" id="onaylaBtn">Onayla</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>