<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaaliyetListesiWP.ascx.cs" Inherits="MTS_WebParts.FaaliyetListesiWP.FaaliyetListesiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
    .alinan {
        background-color: #fff3cd !important;
    }
</style>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
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
        "FaaliyetId": "", "BaslangicTarihi": "", "BitisTarihi": "", "FaaliyetYeri": "", "FaaliyetKonusu": "", "FaaliyetAmaci": "", "FaaliyetDurumu": "", "FaaliyetTipi": "", "Katilimci": "", "Duzenle": ""
    }];
    jQuery(document).ready(function () {
        jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date
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
                { data: "FaaliyetId" },
                { data: "BaslangicTarihi" },
                { data: "BitisTarihi" },
                { data: "FaaliyetYeri" },
                { data: "FaaliyetKonusu" },
                { data: "FaaliyetAmaci" },
                { data: "FaaliyetDurumu" },
                { data: "FaaliyetTipi" },
                { data: "Katilimci" },
                { data: "Duzenle" },

            ],
            columnDefs: [
                { type: 'turkish', targets: [3,4,5,6,7] }
            ],
            'order': [[1, 'desc']],//sort date desc
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
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.FaaliyetTipi == "Alınan Randevu") {
                    if (data['Secildi'] == false)
                        $(row).addClass('alinan');

                }
            },//set row color 
        });

        <%--jQuery('#' + '<%: VisibleChk.ClientID %>').addClass("custom-control-input");--%>
    });

</script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Faaliyet Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>F.No</th>
                            <th>Başlangıç</th>
                            <th>Bitiş</th>
                            <th>Yeri</th>
                            <th>Konusu</th>
                            <th>Amacı</th>
                            <th>Durumu</th>
                            <th>Tipi</th>
                            <th>Katılımcılar</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Faaliyet" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton ID="FaaliyetTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Faaliyet Takvimi" OnClick="FaaliyetTakvimiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="FaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Faaliyet Listesi" OnClick="FaaliyetListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>

        </div>

    </div>
</div>
