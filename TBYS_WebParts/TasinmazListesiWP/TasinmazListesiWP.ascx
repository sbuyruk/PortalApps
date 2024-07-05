<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazListesiWP.TasinmazListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    table tr td th {
        font-size: small;
    }
    thead {
        display: table-header-group;
    }
    tfoot {
        display: none;
    }
</style>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(tasinmazId) {
        document.getElementById('<%= paramTasinmazIdLbl.ClientID%>').value = tasinmazId;
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
        $("#ModalUrlDiv").modal({ backdrop: true });
    }
</script>
<div class="col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Taşınmaz Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div style="display: none">
                <input id="paramTasinmazIdLbl" runat="server" text="Label" />
                <asp:LinkButton ID="xx" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />
            </div>
                                        <div style="display: none">
                                <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />
                            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                    </thead>
                    <tfoot> 
                        <tr >
                            <th>No</th>
                            <th>K. Şekli</th>
                            <th>M. Şekli</th>
                            <th>İl/İlçe</th>
                            <th>Adres</th>
                            <th>Bagışçı</th>
                            <th>"Bagış Yılı"</th>
                            <th>Bölge</th>
                            <th>Taşınmaz Kartı</th>
                            <th>Resimler</th>
                            <th>Düzenle</th>
                            <th>Emlak Beyan Değeri</th>
                            <th>Tahmini Rayiç Değeri</th>
                            <th>Ada No</th>
                            <th>Pafta No</th>
                            <th>ParselNo</th>
                            <th>Yüz Ölçümü</th>
                            <th>Arsa Payı</th>
                            <th>Vakıf Hissesi</th>
                        </tr>
                    </tfoot>
                    <tbody></tbody>
                    
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Taşınmaz Girişi" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>

<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div>
                            <h3>
                                <asp:Label ID="BagisciLbl" CssClass="form-control font-weight-bold" runat="server" Text=""></asp:Label>
                                <asp:Label ID="AdresLbl" CssClass="form-control font-weight-bold"  runat="server" Text=""></asp:Label>
                            </h3>
                        </div>

                        <div class="card-body">

                            <div class="m-1 text-center" id="BagimsizBolumDiv">
                                <table id="CustomModalDataTable" class="table table-hover row-border" width="100%">
                                    <thead>
                                        <tr>
                                            <th>Bölüm Id</th>
                                            <th>Adres</th>
                                            <th>Bölüm</th>
                                            <th>Nitelik</th>
                                            <th>Açıklama</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ModalDoldurBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
             <asp:UpdateProgress ID="updateProgress1" runat="server">
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
</div>