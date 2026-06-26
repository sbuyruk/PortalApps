<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciListesiByYilAyWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciListesiByYilAyWP.NakitBagisciListesiByYilAyWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }

    .bolded {
        font-weight: bold;
    }
</style>
<script>

    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        if (jQuery.fn.DataTable.isDataTable('#CustomModalDataTable')) {
            jQuery('#CustomModalDataTable').DataTable().destroy();
        }
        jQuery('#CustomModalDataTable tbody').empty();

        document.getElementById('<%= paramNakitBagisciIdLbl.ClientID%>').value = nakitBagisciId;
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
    }
</script>

<div class="col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Ay-Yıl Bazında Nakit Bağışçı Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="card-body mt-1">
                    <div class="row form-group">
                        <div class="col-2">
                            <label for="AyDDL" class="col-form-label fw-bold">Bağış Ayı: </label>
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                        </div>
                        <div class="col-2">
                            <label for="YilDDL" class="col-form-label fw-bold">Bağış Yılı: </label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                        </div>
                    </div>
                    <div style="display: none">
                        <input id="paramNakitBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-hover table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th id="BaslikTH" colspan="11" class="text-center" runat="server">ccc
                                    </th>
                                </tr>
                                <tr>
                                    <th>Adı Soyadı</th>
                                    <th>Tarih</th>
                                    <th>Tutar</th>
                                    <th>Telefon</th>
                                    <th>Adresi</th>
                                    <th>İl</th>
                                    <th>ilçe</th>
                                    <th>Armağan</th>
                                    <th>Bağış</th>
                                    <th>Armağan Durumu</th>
                                    <th>Belge</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="updateProgress2" runat="server">
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

<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div>
                            <h3>
                                <asp:Label ID="AdiLbl" runat="server" Text="Label"></asp:Label>
                            </h3>
                            <div>
                                <asp:Label ID="TarihLbl" class="col-form-label " runat="server"></asp:Label>
                            </div>
                        </div>

                        <div class="card-body">
                            <div style="display: none">
                                <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />
                            </div>
                            <div class="m-1 text-center" id="NakitBagisciDiv">
                                <table id="CustomModalDataTable" class="table table-hover row-border" width="100%">
                                    <thead>
                                        <tr>
                                            <th>Bağış Tarihi</th>
                                            <th>Bağış Miktarı</th>
                                            <th>Armağan</th>
                                            <th>Bağış</th>
                                            <th>Armağan Durumu</th>
                                        </tr>
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
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