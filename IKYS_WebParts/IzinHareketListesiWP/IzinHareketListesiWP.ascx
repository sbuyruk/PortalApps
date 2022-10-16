<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IzinHareketListesiWP.ascx.cs" Inherits="IKYS_WebParts.IzinHareketListesiWP.IzinHareketListesiWP" %>

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
        "IzinHareketId": "", "AdiSoyadi": "", "IzinDonemi": "", "IzinTipi": "", "BaslangicTarihi": "", "BaslangicSaati": "", "BitisTarihi": "", "BitisSaati": "", "SureStr": "", "DuzenleLink": "", "YazdirLink": ""
    }];
    jQuery(document).ready(function () {
        jQuery.fn.dataTable.moment('DD.MM.YYYY');//sort date
        jQuery('#CustomDataTable').DataTable({
            'initComplete': function (settings, json) {//tablo yüklendiğinde
                var api = this.api();
                var row = api.row(function (idx, data, node) { //secilen Id'ye gider
                    return data['Secildi'] == true;
                });
                if (row.length > 0) {
                    row.select()
                        .show()
                        .draw(false);
                }
            },
            data: myjsons,
            columns: [
                { data: "IzinHareketId" },
                { data: "AdiSoyadi" },
                { data: "IzinDonemi" },
                { data: "IzinTipi" },
                { data: "BaslangicTarihi" },
                { data: "BaslangicSaati" },
                { data: "BitisTarihi"},
                { data: "BitisSaati"},
                { data: "SureStr" },
                { data: "DuzenleLink" },
                { data: "YazdirLink" },

            ],
            'order': [[4, 'desc'], [5, 'desc'], [0, 'desc']],
            "language": {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                "decimal": ",",
                "thousands": "."
            },
            responsive: true,
            dom: 'Bfrtip',
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

<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label  btn-outline-primary" runat="server" Text="Kullanılan İzinler Listesi"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped table-hover table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>İzin No</th>
                            <th>Adı Soyadı</th>
                            <th>İzin Dönemi</th>
                            <th>İzin Tipi</th>
                            <th>Başlangıç Tarihi</th>
                            <th>Başlangıç Saati</th>
                            <th>Bitiş Tarihi</th>
                            <th>Bitiş Saati</th>
                            <th>İzin Süresi</th>
                            <th>Düzenle</th>
                            <th>Yazdır</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
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