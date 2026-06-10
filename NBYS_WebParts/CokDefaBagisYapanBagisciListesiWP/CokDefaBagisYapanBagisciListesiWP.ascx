<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CokDefaBagisYapanBagisciListesiWP.ascx.cs" Inherits="NBYS_WebParts.CokDefaBagisYapanBagisciListesiWP.CokDefaBagisYapanBagisciListesiWP" %>

<style>
    .table-muted-bg {
        background-color: #f5f6fa !important; /* Soluk bir arka plan */
        color: #6c757d !important;           /* Gri yazı */
    }
    .table-muted-bg th,
    .table-muted-bg td {
        background-color: #f5f6fa !important;
        color: #6c757d !important;
    }
</style>
<script>    
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= hiddenNakitBagisciId.ClientID%>').value = nakitBagisciId;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }
    function OpenArmaganModal(nakitBagisciId, hakedilenarmaganTanimId, sonBagisTarihi) {
        document.getElementById('<%= hiddenNakitBagisciId.ClientID%>').value = nakitBagisciId;
        document.getElementById('<%= hiddenArmaganTanimId.ClientID%>').value = hakedilenarmaganTanimId;
        document.getElementById('<%= hiddenSonBagisTarihi.ClientID%>').value = sonBagisTarihi;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ArmaganModal'));
        myModalInstance.show();
        document.getElementById('<%= ArmaganOlusturModalDoldurBtn.ClientID%>').click();
    }

</script>

<div class="container col-xl">
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
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Çok Defa Bağış Yapan Bağışçılar"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">

            <div style="display: none">
                <input id="hiddenNakitBagisciId" runat="server" text="hiddenNakitBagisciId"  />
                <input id="hiddenArmaganTanimId" runat="server" text="hiddenArmaganTanimId"  />
                <input id="hiddenSonBagisTarihi" runat="server" text="hiddenSonBagisTarihi"  />
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Bağışçı No</th>
                            <th>Adı Soyadı</th>
                            <th>Toplam Bağış Adedi</th>
                            <th>Toplam Bağış Tutarı</th>
                            <th>Son Bağış Tarihi</th>
                            <th>Armağan</th>
                            <th>Öncelik</th>
                        </tr>
                    </thead>
                </table>
                <hr />
                <hr />
                <table id="CustomDataTableVerilen" class="table table-striped row-border table-muted-bg" width="100%">

                    <thead>
                        <tr>
                            <th colspan="6" class="text-center text-white bg-secondary">DAHA ÖNCE VERİLEN ARMAĞANLAR
                            </th>
                        </tr>
                        <tr>
                            <th>Bağışçı No</th>
                            <th>Adı Soyadı</th>
                            <th>Armağan</th>
                            <th>Bağış Adedi</th>
                            <th>Bağış Toplamı</th>
                            <th>Armağan Tarihi</th>
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
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
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
<!-- Modal Armagan Olustur -->

<div class="modal" id="ArmaganModal" role="dialog">
    <div class="modal-dialog  modal-dialog-centered">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="Label1" runat="server" Text="Armağan Oluşturulacak" Font-Bold="True"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ArmaganOlusturModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClick="ArmaganOlusturModalDoldurBtn_Click" />

                        </div>

                        <div class="m-1 text-center" >
                            <asp:Label ID="ArmaganOlusturMessageTxt" runat="server"  CssClass=" text-danger fw-bold text-center"></asp:Label>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="ArmaganOlusturNowBtn" CssClass="btn btn-success" runat="server" CausesValidation="false" Text="Armağan Oluştur" OnClick="ArmaganOlusturNowBtn_Click" />
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>