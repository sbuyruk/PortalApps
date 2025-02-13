<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazOnarimListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazOnarimListesiWP.TasinmazOnarimListesiWP" %>
<style>
    .ui-datatable tbody td{
        white-space:normal;
    }
</style>
<script>
    var counter = 0;
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>

<script type="text/javascript">
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{
        "OnarimId": "", "YapilanIs": "", "HarcamaUsulu": "", "OnayTarihi": "", "Tutar": "", "Adres": "", "IliIlcesi": "", "TasinmazKarti": "", "Duzenle": ""
    }];
    jQuery(document).ready(function () {

        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "OnarimId" },
                { data: "YapilanIs" },
                { data: "HarcamaUsulu" },
                { data: "OnayTarihi" },
                { data: "Tutar" },
                { data: "Adres" },
                { data: "IliIlcesi" },
                { data: "TasinmazKarti" },
                { data: "Duzenle" },

            ],
            'order': [[1, 'asc']],//AdiSoyadi Sıralı
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
            ]



        });
    });
</script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Taşınmaz Onarım Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Onarım No</th>
                            <th>Yapılan İş</th>
                            <th>Harcama Usulü</th>
                            <th>Tarih</th>
                            <th>Tutar</th>
                            <th>Adres</th>
                            <th>İl/İlçe</th>
                            <th>Taşınmaz Kartı</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>

    </div>
</div>