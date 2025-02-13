<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AcikTarihliFaaliyetListesiWP.ascx.cs" Inherits="MTS_WebParts.AcikTarihliFaaliyetListesiWP.AcikTarihliFaaliyetListesiWP" %>

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
        "FaaliyetId": "", "FaaliyetYeri": "", "FaaliyetKonusu": "", "FaaliyetAmaci": "", "FaaliyetDurumu": "", "FaaliyetTipi": "", "Katilimci": "", "Aciklama": "", "OlusturmaTarihi": "", "Duzenle": ""
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
                { data: "FaaliyetYeri" },
                { data: "FaaliyetKonusu" },
                { data: "FaaliyetAmaci" },
                { data: "FaaliyetDurumu" },
                { data: "FaaliyetTipi" },
                { data: "Katilimci" },
                { data: "Aciklama" },
                { data: "OlusturmaTarihi" },
                { data: "Duzenle" },

            ],
            columnDefs: [
                { type: 'turkish', targets: [1,2,3,4,5,6] }
            ],
            'order': [[8, 'desc']],//sort date desc
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
<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Açık Tarihli Faaliyet Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group alert-secondary p-2">
                <div class="form-group ">
                    <div class="row">
                        <div class="col-2">
                            <div class="form-group col checkbox" id="Div1" runat="server" style="display: block;">
                                <label>
                                    <asp:CheckBox ID="ToplantiChk" runat="server" ToolTip="Toplantıları göstermek için seçiniz." Checked="false" OnCheckedChanged="ToplantiChk_CheckedChanged" AutoPostBack="true" />
                                   Toplantı
                                </label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group col checkbox" id="Div2" runat="server" style="display: block;">
                                <label>
                                    <asp:CheckBox ID="ZiyaretChk" runat="server" ToolTip="Toplantıları göstermek için seçiniz." Checked="false" OnCheckedChanged="ZiyaretChk_CheckedChanged" AutoPostBack="true" />
                                    Ziyaret
                                </label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group col checkbox" id="AcikTarhliDiv" runat="server" style="display: block;">
                                <label>
                                    <asp:CheckBox ID="GorusmeChk" runat="server" ToolTip="Görüşmeleri göstermek için seçiniz." Checked="false" OnCheckedChanged="GorusmeChk_CheckedChanged" AutoPostBack="true" />
                                    Görüşme
                                </label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group col checkbox" id="Div3" runat="server" style="display: block;">
                                <label>
                                    <asp:CheckBox ID="SeyahatChk" runat="server" ToolTip="Seyahatleri göstermek için seçiniz." Checked="false" OnCheckedChanged="SeyahatChk_CheckedChanged" AutoPostBack="true" />
                                    Seyahat
                                </label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group col checkbox" id="Div4" runat="server" style="display: block;">
                                <label>
                                    <asp:CheckBox ID="DavetChk" runat="server" ToolTip="Davetleri göstermek için seçiniz." Checked="false" OnCheckedChanged="DavetChk_CheckedChanged" AutoPostBack="true" />
                                    Davet
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>F.No</th>
                            <th>Yeri</th>
                            <th>Konusu</th>
                            <th>Amacı</th>
                            <th>Durumu</th>
                            <th>Tipi</th>
                            <th>Katılımcılar</th>
                            <th>Açıklama</th>
                            <th>Oluşturma Tarihi</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Faaliyet" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="FaaliyetTakvimiBtn" CssClass="btn btn-outline-info float-end" runat="server" Text="Faaliyet Takvimi" OnClick="FaaliyetTakvimiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="FaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Faaliyet Listesi" OnClick="FaaliyetListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="AcikTarihliFaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Açık Tarihli Faal. List." OnClick="AcikTarihliFaaliyetListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>

        </div>

    </div>
</div>
