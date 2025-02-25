<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraArtisYazisiWP.ascx.cs" Inherits="TBYS_WebParts.KiraArtisYazisiWP.KiraArtisYazisiWP" %>

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
        "KiraciAdi": "", "KiralamaAmaci": "", "Bolge": "", "SozlesmeTarihi": "", "ArtisAyi": "", "TamAdres": "", "KiraBedeli": "", "Tufe": "", "YeniKiraBedeli": "", "YenilendiMi": ""}];
    jQuery(document).ready(function () {

        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "KiraciAdi" },
                { data: "KiralamaAmaci" },
                { data: "Bolge" },
                { data: "SozlesmeTarihi" },
                { data: "ArtisAyi" },
                { data: "TamAdres", "width": "25%" },
                { data: "KiraBedeli", type: "decimal", class: "text-end"},
                { data: "Tufe" },
                { data: "YeniKiraBedeli", type: "decimal", class: "text-end" },
                { data: "YenilendiMi" },

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
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Artış Yazısı Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row border m-1">
                        <div class="form-group col-2 ">
                            <div class="form-group col" >
                                <label class="col-form-label" for="AyDDL">Ay </label>
                                <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group" style="display:none">
                                <label class="col-form-label" for="YilDDL">Yıl </label>
                                <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="BolgeDDL">Bölge </label>
                                <asp:DropDownList ID="BolgeDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="BolgeDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group">
                                <label for="TufeTxt" class="col-form-label">TÜFE (%)</label>
                                <asp:TextBox ID="TufeTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-2">
                            <div class="form-group">
                                <label for="EvrakSayisiYiliTxt" class="col-form-label">Evrak Sayısı Yılı</label>
                                <asp:TextBox ID="EvrakSayisiYiliTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="EvrakTarihiTxt" class="col-form-label">Evrak Tarihi</label>
                                <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            
                        </div>
                        <div class="form-group col-3">
                            <div class="form-group">
                                <label for="Parafe1Txt" class="col-form-label">Parafe Eden (1)</label>
                                <asp:TextBox ID="Parafe1Txt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="Parafe2Txt" class="col-form-label">Parafe Eden (2)</label>
                                <asp:TextBox ID="Parafe2Txt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-3">
                            <div class="form-group">
                                <label for="KoordineTxt" class="col-form-label">Koordine</label>
                                <asp:TextBox ID="KoordineTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-2">
                            <div class="form-group">
                                <label for="ImzalayanTxt" class="col-form-label">İmza (Adi Soyadı)</label>
                                <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="ImzalayanMakamTxt" class="col-form-label">İmza (Makam)</label>
                                <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="form-group border m-1" >
<%--                        <asp:UpdatePanel runat="server" ID="UpdatePanel">
                            <ContentTemplate>--%>
                                <div class="form-group">
                                    <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Kiracı</th>
                                                <th>Cinsi</th>
                                                <th>Bölge</th>
                                                <th>İlk Söz. Tar.</th>
                                                <th>Artış Ayı</th>
                                                <th>Adres</th>
                                                <th>Artıştan Önceki Kira</th>
                                                <th>Artış Oranı (%)</th>
                                                <th>Artıştan Sonraki Kira</th>
                                                <th>Durumu</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
<%--                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="AyDDL" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>--%>

                        <asp:UpdateProgress ID="updateProgress" runat="server">
                            <ProgressTemplate>
                                <div class='loaderMainContainer'>
                                    <div class='loaderContainer'>
                                        <div class='loaderCircle'></div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div class="form-group">
                            <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Kira Borcu Dosyasını Aç</asp:HyperLink>
                        <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiket Dosyasını Aç</asp:HyperLink>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="YaziyiOlusturBtn" runat="server" CssClass="btn btn-outline-success" OnClick="YaziyiOlusturBtn_Click">Yazıyı oluştur </asp:LinkButton>
                </div>
            </div>
        
</div>
