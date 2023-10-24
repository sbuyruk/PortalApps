<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeTasinmazListesiWP.ascx.cs" Inherits="BTYS_Webparts.BolgeTasinmazListesiWP.BolgeTasinmazListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    table tr td th {
        font-size: small;
    }
</style>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{
        "Id": "", "KullanimSekli": "", "MulkiyetSekli": "", "IliIlcesi": "", "Adres": "", "Bagisci": "", "BagisYili": "",
        "SorumluBolge": "", "EmlakSicilNo": "", "AdaNo": "", "ParselNo": "", "PaftaNo": "", "YevmiyeNo": "", "CiltNo": "", "SahifeNo": "", "Cinsi": "", "KiraDurumu": "",
        "TasinmazKarti": "", "Duzenle": ""
    }];
    jQuery(document).ready(function () {

        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "Id" },
                { data: "KullanimSekli" },
                { data: "MulkiyetSekli" },
                { data: "IliIlcesi" },
                { data: "Adres" },
                { data: "Bagisci" },
                { data: "BagisYili" },
                { data: "SorumluBolge" },
                { data: "EmlakSicilNo" },
                { data: "AdaNo" },
                { data: "ParselNo" },
                { data: "PaftaNo" },
                { data: "YevmiyeNo" },
                { data: "CiltNo" },
                { data: "SahifeNo" },
                { data: "Cinsi" },
                { data: "KiraDurumu" },
                { data: "TasinmazKarti" },
            ],
            'order': [[0, 'asc']],//Id Sıralı
            "language": {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                "decimal": ",",
                "thousands": "."
            },
            responsive: true,
            dom: 'Bfrtip',
            //colon resizable
            //initComplete: function (settings) {
            //    $('#CustomDataTable').colResizable({ liveDrag: true });
            //},
            buttons: [
                {
                    extend: 'print',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'excel',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'pdf',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'copy',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                , 'pageLength', "colvis"
            ],
            columnDefs: [
                {
                    targets: [8,9,10,11,12,13,14,15,16],
                    visible: false
                }
            ]
        });
    });
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
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>T.No</th>
                            <th>Kullanım Şekli</th>
                            <th>Mülkiyet Şekli</th>
                            <th>İl/İlçe</th>
                            <th>Adres</th>
                            <th>Bagışçı</th>
                            <th>Bagış Yılı</th>
                            <th>SorumluBolge</th>
                            <th>Emlak Sicil No</th>
                            <th>AdaNo</th>
                            <th>ParselNo</th>
                            <th>PaftaNo</th>
                            <th>YevmiyeNo</th>
                            <th>CiltNo</th>
                            <th>SahifeNo</th>
                            <th>Cinsi</th>
                            <th>KiraDurumu</th>
                            <th>Taşınmaz Kartı</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>