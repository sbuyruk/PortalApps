<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraArtisCizelgesiWP.ascx.cs" Inherits="TBYS_WebParts.KiraArtisCizelgesiWP.KiraArtisCizelgesiWP" %>
<style>
    .bes-yil {
        color: orange;
        font-weight: bold;
    }

    .on-yil {
        color: red;
        font-weight: bold;
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
        "KiraciAdi": "", "KiralamaAmaci": "", "Bolge": "", "SozlesmeTarihi": "", "KiraSuresi": "", "ArtisAyi": "", "TamAdres": "", "KiraBedeli": "", "Tufe": "", "YeniKiraBedeli": "", "YenilendiMi": ""
    }];
    if (jQuery.fn.DataTable.isDataTable('#CustomDataTable')) {
        jQuery('#CustomDataTable').DataTable().destroy();
    }
    jQuery('#CustomDataTable tbody').empty();

    jQuery.fn.dataTable.moment('DD.MM.YYYY HH:mm');//sort date

    jQuery(document).ready(function () {

        var table = jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: 'Bolge' },
                { data: 'KiraciAdi', 'width': '20%' },
                { data: 'TamAdres', 'width': '20%' },
                { data: 'KiralamaAmaci' },
                { data: 'SozlesmeTarihi' },
                { data: 'KiraSuresi' },
                { data: 'ArtisAyi' },
                { data: 'KiraBedeli', type: 'string', class: 'text-end' },
                { data: 'Tufe', type: 'decimal' },
                { data: 'YeniKiraBedeli', type: 'decimal', class: 'text-end' },
                { data: 'YenilendiMi' },

            ],
            'order': [[0, 'asc']],//bolge Sıralı
            'language': {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                'decimal': ',',
                'thousands': '.'
            },
            responsive: true,
            destroy: true,
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'print',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'excelHtml5',
                    exportOptions: { orthogonal: 'export' },
                    customize: function (xlsx) {
                        var sheet = xlsx.xl.worksheets['sheet1.xml'];

                        var count = 0;
                        var skippedHeader = 0;
                        $("row c[r^='F']", sheet).each(function () { //F excel kolonu 
                            if (count++ > 0) {
                                var text = $(this).text();
                                var yilInt = text.replace(' Yıl', '');
                                if (yilInt >= 5) {
                                    $(this).attr('s', '11');
                                }
                            }

                        });

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
                'pageLength', 'colvis'
            ],
            'createdRow': function (row, data, dataIndex) {
                if (data.OnYil == 'True') {
                    $(row).addClass('on-yil');
                } else if (data.BesYil == 'True') {
                    $(row).addClass('bes-yil');
                }

            },//set row color
        });
    });            
</script>

<div class="col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Artış Çizelgesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">
            <div class="row">
                <div class="form-group col-2">
                    <asp:Label CssClass="col-form-label" runat="server" Text="Ay"></asp:Label>
                    <asp:DropDownList ID="AyDDL" CssClass="form-control" runat="server" Style="height: auto" Enabled="True" AutoPostBack="true" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="form-group col-2">
                    <asp:Label CssClass="col-form-label" runat="server" Text="Kira Süresi"></asp:Label>
                    <asp:DropDownList ID="KiraSuresiDDL" CssClass="form-control" runat="server" Style="height: auto" Enabled="True" AutoPostBack="true" OnSelectedIndexChanged="KiraSuresiDDL_SelectedIndexChanged"></asp:DropDownList>
                </div>
            </div> 
            <%--            <asp:UpdatePanel runat="server" ID="UpdatePanel">
                <ContentTemplate>--%>

            <div class="form-group" id="TabloDiv" runat="server">
                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>Bölge</th>
                            <th>Kiracı</th>
                            <th>Adres</th>
                            <th>Cinsi</th>
                            <th>İlk Söz. Tar.</th>
                            <th>Kira Süresi</th>
                            <th>Artış Ayı</th>
                            <th>Artıştan Önceki Kira (TL)</th>
                            <th>Artış Oranı (%)</th>
                            <th>Artıştan Sonraki Kira (TL)</th>
                            <th>Durumu</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <%--                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AyDDL" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>--%>

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
        <div class="card-footer">
            <%--<asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />--%>
        </div>
    </div>

</div>
