<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraciAylikOdemeWP.ascx.cs" Inherits="TBYS_WebParts.KiraciAylikOdemeWP.KiraciAylikOdemeWP" %>


<script type="text/javascript">

    
    function OpenKiraciSecModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('KiraciSecDiv'));
        myModalInstance.show();
    }
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnay'));
        myModalInstance.show();
    }
    function CallButtonClick(kiraciId) {
        document.getElementById('<%= paramKiraciIdLbl.ClientID%>').value = kiraciId;
        document.getElementById('<%= KiraciSecNowBtn.ClientID%>').click();
    }
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{ "DosyaNo": "0", "KiraciAdiSoyadi": "A", "TasinmazAdresi": "A","KiraBedeli": "1", "OdenenTutar": "1", "OdemeTarihi": "01.01.2021", "IlkSozlesmeTar": "01.01.2021", "ArtisAyi": "test","Sozlesme": "test", "VadeBitTar": "01.01.2021", "Aciklama": "www.google.com", "Bolge": "istanbul", "KiralamaAmaci": "Mesken", "Duzenle": "Duzenle" }];
    jQuery(document).ready(function () {
        jQuery('#CustomDataTable').DataTable({
            data: myjsons,
            columns: [
                { data: "DosyaNo" },
                { data: "Bolge" },
                { data: "KiraciAdiSoyadi" },
                { data: "TasinmazAdresi" },
                { data: "KiralamaAmaci" },
                { data: "IlkSozlesmeTar" },
                { data: "Sozlesme" },
                { data: "VadeBitTar" },
                { data: "ArtisAyi" },
                { data: "OdemeSekli" },
                { data: "KiraBedeli", type: "decimal" },
                { data: "OdemeTarihi" },
                { data: "OdenenTutar", type: "decimal" },
                { data: "Aciklama" },
                { data: "Duzenle" },
            ],
            "columnDefs": [
                { className: "text-end", "targets": [10] },
                { className: "text-end", "targets": [12] }
            ],
            "language": {
                "url": "http://tskgv-portal/OrtakBelgeler/Turkish.txt",
                "decimal": ",",
                "thousands": "."
            },
            "order": [[11, "desc"], [0, "asc"], [2, "asc"]],
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
                        columns: ':visible',
                        format: {
                            body: function (data, row, column, node) {
                               
                                if (column === 3) {
                                    var kirabedeli = data.replace('.', '#');
                                    kirabedeli = kirabedeli.replace(',', '.');
                                    kirabedeli = kirabedeli.replace('#', ',');
                                    return kirabedeli;
                                } else if (column === 4) {
                                    var odenenTutar = data.replace('.', '#');
                                    odenenTutar = odenenTutar.replace(',', '.');
                                    odenenTutar = odenenTutar.replace('#', ',');
                                    return odenenTutar;
                                }
                                else {
                                    return data.replace(/(&nbsp;|<([^>]+)>)/ig, "");//html'i soy //regex to strip the HTML
                                }

                            }
                        }
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
<div class="container col-xl ">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Aylık Ödemeler"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body " id="MainCardDiv" runat="server">
            <div class="form-group" style="display: block" runat="server">
                <div class="form-group row">
                    <div class="form-group col-2">
                        <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Ay :</asp:Label>
                        <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                    </div>
                    <div class="form-group col-2">
                        <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Yıl :</asp:Label>
                        <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                    </div>
                    <div class="col row">
                        <div class="form-group col-6">
                            <asp:Label CssClass="col-form-label" runat="server" Font-Bold="True">Kiraci</asp:Label>
                            <asp:TextBox ID="KiraciTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label text-secondary" runat="server" Font-Bold="True">. .  .</asp:Label>
                            <asp:LinkButton ID="KiraciSecBtn" CssClass="btn btn-outline-primary " runat="server" Text="Kiracı Seç" OnClick="KiraciSecBtn_Click" />
                        </div>
                        <div class="form-group col-2">
                            <asp:Label CssClass="col-form-label text-secondary" runat="server" Font-Bold="True">. . .</asp:Label>
                            <asp:LinkButton ID="HepsiBtn" CssClass="btn btn-outline-primary " runat="server" Text="Tüm Kiracılar" OnClick="HepsiBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped table-bordered table-sm " width="100%">
                    <thead>
                        <tr>
                            <th>DNo</th>
                            <th>Bölge</th>
                            <th>Kiracı</th>
                            <th>Taşınmaz Adresi</th>
                            <th>Kir. Amaci</th>
                            <th>İlk Söz. Tarihi</th>
                            <th>Sözleşme</th>
                            <th>Vade Tarihi</th>
                            <th>Artış Ayı</th>
                            <th>Ödeme Şekli</th>
                            <th>Kira Bedeli</th>
                            <th>Ödeme Tarihi</th>
                            <th>Ödenen Tutar</th>
                            <th>Açıklama</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-success" ID="YeniOdemeGirisiBtn" runat="server" Text="Yeni Ödeme Girişi" OnClick="YeniOdemeGirisiBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton ID="OdemePlaniBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Son Ödeme Plani" OnClick="OdemePlaniBtn_Click" />
            <asp:LinkButton ID="SozlesmeBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Sözleşme" OnClick="SozlesmeBtn_Click" />
            <asp:LinkButton ID="KiraciBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Kiraci" OnClick="KiraciBtn_Click" />
            <asp:LinkButton ID="OdemePlaniListBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Ödeme Planı Listesi" OnClick="OdemePlaniListBtn_Click" />
            <asp:LinkButton ID="SozlesmeListBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Sözleşme Listesi" OnClick="SozlesmeListBtn_Click" />
            <asp:LinkButton ID="KiraciListBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Kiraci Listesi" OnClick="KiraciListBtn_Click" />
            <asp:LinkButton ID="BakiyeDevirBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Bakiye Devir İşlemleri" OnClick="BakiyeDevirBtn_Click" />
        </div>
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
            </ContentTemplate>
        </asp:UpdatePanel>
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
    <div class="modal" id="KiraciSecDiv" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content" style="width: 1000px;">
                <div class="modal-body">
                    <div style="display: none">
                        <input id="paramKiraciIdLbl" runat="server" type="text" />
                        <asp:LinkButton ID="KiraciSecNowBtn" runat="server" OnClientClick="{return true;};" OnClick="KiraciSecNowBtn_Click"></asp:LinkButton>
                    </div>
                    <div>
                        <div class="text-center">
                            <h3>
                                <asp:Label ID="Label1" class="col-form-label " runat="server" Text="Kiracı Listesi"></asp:Label></h3>
                        </div>
                        <div class="card-body">
                                <div class="form-group">
                                    <table id="CustomModalDataTable" class="table table-striped table-bordered table-sm small" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Kiracı No</th>
                                                <th>Adı Soyadi</th>
                                                <th>TCKimlikNo</th>
                                                <th>İl/İlçe</th>
                                                <th>Adres</th>
                                                <th>Seç</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal" id="ModalOnay" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <h3>
                                    <asp:Label ID="ModalLbl" class="col-form-label text-danger fw-bold" Text="Ödeme Silinecek" runat="server"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <asp:Label ID="MessageLbl" runat="server" class="col-form-label"></asp:Label>
                                    <asp:HiddenField ID="OdemeIdHdn" runat="server" />
                                    <asp:HiddenField ID="OdemePlaniIdIdHdn" runat="server" />
                                    <asp:HiddenField ID="SozlesmeIdHdn" runat="server" />
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton ID="SilNowBtn" Text="Ödemeyi Sil" runat="server" class="btn btn-outline-danger" OnClick="SilNowBtn_Click"></asp:LinkButton>
                                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">İptal</button>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <!-- Kiracı Seçimi Modal -->
   
</div>
