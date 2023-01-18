<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GerceklesenKiraArtislariWP.ascx.cs" Inherits="TBYS_WebParts.GerceklesenKiraArtislariWP.GerceklesenKiraArtislariWP" %>

<style>
    .bes-yil{ 
        color: orange;
        font-weight:bold;
    }
    .on-yil{ 
        color: red;
        font-weight:bold;
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
        "KiraciAdi": "", "KiralamaAmaci": "", "Bolge": "", "SozlesmeTarihi": "", "KiraSuresi": "", "ArtisAyi": "", "TamAdres": "","OncekiKiraBedeli": "", "YasalArtisOrani": "", "YasalOranaGoreKiraBedeli": "", "KiraBedeli": "", "UygulananArtisOrani": "", "KiraBedeli": ""
    }];
    jQuery(document).ready(function () {

        var table = jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "KiraciAdi", "width": "20%" },
                { data: "KiralamaAmaci" },
                { data: "Bolge" },
                { data: "SozlesmeTarihi" },
                { data: "KiraSuresi" },
                { data: "ArtisAyi" },
                { data: "TamAdres", "width": "20%" },
                { data: "OncekiKiraBedeli", type: "decimal",class:"text-right" },
                { data: "YasalArtisOrani", type: "decimal" },
                { data: "YasalOranaGoreKiraBedeli", type: "decimal", class: "text-right" },
                { data: "UygulananArtisOrani", type: "decimal" },
                { data: "KiraBedeli", type: "decimal",class:"text-right" },

            ],            
            'order': [[2, 'asc']],//AdiSoyadi Sıralı
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
                    extend: 'excelHtml5',
                    customize: function (xlsx) {
                        var sheet = xlsx.xl.worksheets['sheet1.xml'];

                        var count = 0;
                        var skippedHeader = 0;

                        //$('row', sheet).each(function () {
                        //    if (skippedHeader++>1) {
                        //        var text = $('tbody tr:eq(' + parseInt(count) + ') td:eq(4)').text();
                        //        if (text == '5 Yıl') {
                        //            $(this).attr('s', '11');
                        //        }
                        //        else {
                        //            $(this).attr('s', '21');
                        //        }
                                
                        //    }
                        //    count++;
                        //});
                        //count = 0;
                        //skippedHeader = 0;
                        $('row c[r^="E"]', sheet).each(function () {
                            if (count++>0) {
                                var text = $(this).text();
                                var yilInt = text.replace(' Yıl', '');
                                if (yilInt >=5) {
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
                , 'pageLength', "colvis"
            ],
            "createdRow": function (row, data, dataIndex) {
                if (data.OnYil == "True") {
                    $(row).addClass('on-yil');
                } else if (data.BesYil == "True") {
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
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Gerçekleşen Kira Artışları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">

            <asp:UpdatePanel runat="server" ID="UpdatePanel">
                <ContentTemplate>
                    <div class="form-group" id="TabloDiv" runat="server">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Kiracı</th>
                                    <th>Cinsi</th>
                                    <th>Bölge</th>
                                    <th>İlk Söz. Tar.</th>
                                    <th>Kira Süresi</th>
                                    <th>Artış Ayı</th>
                                    <th>Adres</th>
                                    <th>Artıştan Önceki Kira Bedeli (TL)</th>
                                    <th>Yasal Artış Oranı (%)</th>
                                    <th>Yasal Artış Oranına Göre Kira Bedeli (TL)</th>
                                    <th>Uygulanan Artış Oranı (%)</th>
                                    <th>Artıştan Sonraki Kira Bedeli (TL)</th>
                                </tr>
                            </thead>
                        </table>
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
        <div class="card-footer">
            <%--<asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />--%>
        </div>
    </div>
</div>
