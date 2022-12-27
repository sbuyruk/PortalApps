<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AramaGorusmeListesiWP.ascx.cs" Inherits="MTS_WebParts.AramaGorusmeListesiWP.AramaGorusmeListesiWP" %>

<style>
    .gorusmeSaglanamadi {
        background-color: lightpink !important;
        color: black !important;
    }

    .icKatilimci {
        background-color: lightgray !important;
    }

    .disKatilimci {
        background-color: lightcyan !important;
    }

    .nakitBagisci {
        background-color: lightyellow !important;
    }

    .tasinmazBagisci {
        background-color: wheat !important;
    }
</style>
<%-- Katılımcı/itribat personeli ekleme / çıkartma --%>
<script type="text/javascript">
    function KatilimciSecimiModal() {
        $("#KatilimciSecimiModal").modal({ backdrop: false });
    }
    function KatilimciSecildiBtnClick(katilimciId, katilimciTipi) {
        document.getElementById('<%= paramRandevuKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramRandevuKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= SecilenKatilimciyiGetirBtn.ClientID%>').click();
    }
</script>
<script type="text/javascript">
    //On Page Load.
    //$(function () {
    //    SetDatePicker();
    //});
    ////ikinci tarih için
    //function SetDatePicker() {

    //    $("[id$=BaslangicTarihiTxt],[id$=BitisTarihiTxt]").datepicker({
    //        dateFormat: "dd.mm.yy",
    //        firstDay: 1,
    //        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
    //        monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
    //        dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
    //        dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
    //        showAnim: "fold",
    //        changeMonth: true,
    //        changeYear: true,

    //    }).on("change", function () {
    //        if (this.id == 'BaslangicTarihiTxt') {
    //            var dateMin = $('[id$=BaslangicTarihiTxt]').datepicker("getDate");
    //            var rMin = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate());
    //            var rMax = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate() + 90);

    //            $('[id$=BitisTarihiTxt]').datepicker("option", "minDate", rMin);
    //            $('[id$=BitisTarihiTxt]').datepicker("option", "maxDate", rMax);
    //        }

    //    });

    //}
    function setDataSet(myset) {
        myjsons = myset;
    }
    var myjsons = [{
        "AramaGorusmeId": "", "AdiSoyadi": "", "Tarih": "", "Konu": "", "Kurumu": "", "Randevu": "", "Duzenle": ""
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
                { data: "AramaId" },
                { data: "AdiSoyadi" },
                { data: "Tarih" },
                { data: "Konu" },
                { data: "Kurumu" },
                { data: "Randevu" },
                { data: "Duzenle" },
            ],
            'order': [[2, 'desc']],//AdiSoyadi Sıralı
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
                if (!data.GorusmeSaglandi) {
                    if (data['Secildi'] == false)
                        $(row).addClass('gorusmeSaglanamadi');

                }
            },//set row color 
        });
    });
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Arama/Görüşme Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group row">

                <div class="col-2 form-group">
                    <asp:LinkButton ID="KatilimciSecBtn" runat="server" CssClass="btn btn-outline-primary" Text="İsim Seç" OnClick="KatilimciSecBtn_Click" CausesValidation="false" />
                </div>
                <div class="col-2 form-group">
                    <asp:LinkButton ID="HepsiBtn" runat="server" CssClass="btn btn-outline-primary" OnClick="HepsiBtn_Click">Hepsi</asp:LinkButton>
                </div>
                <div class="col form-group">
                    <asp:HyperLink ID="AdiSoyadiLnk" runat="server" CssClass="col-form-label font-weight-bold" Enabled="True"></asp:HyperLink>
                </div>
            </div>
            <div class="form-group alert-secondary p-2">
                <div class="form-group ">
                    <div class="row">
                        <div class="col-3 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Arama/Görüşme Şekli</asp:Label>
                            <asp:DropDownList ID="GorusmeSekliDDL" runat="server" CssClass="form-control" Height="34px" AutoPostBack="true" OnSelectedIndexChanged="GorusmeSekliDDL_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="GorusmeSekliDDL" ForeColor="Red" ErrorMessage="Arama şeklini seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-2">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Başlangıç</asp:Label>
                                <asp:TextBox ID="BaslangicTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="BaslangicTarihiTxt_TextChanged" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Bitiş</asp:Label>
                                <asp:TextBox ID="BitisTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="BitisTarihiTxt_TextChanged" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <h2 class="font-weight-bold text-center" id="BaslikLbl" runat="server"></h2>
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-striped table-bordered table-hover table-sm" width="100%">
                        <thead>
                            <tr>
                                <th>A/G.No</th>
                                <th>Adı Soyadı</th>
                                <th>Tarih</th>
                                <th>Konu</th>
                                <th>Kurumu</th>
                                <th>Randevu</th>
                                <th>Düzenle</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div id="KatilimciHiddenDiv" style="display: none">
            <input id="paramRandevuKatilimciIdLbl" runat="server" type="text" />
            <input id="paramRandevuKatilimciTipiLbl" runat="server" type="text" />
            <asp:LinkButton ID="SecilenKatilimciyiGetirBtn" runat="server" CausesValidation="false" Text="Randevuya Ekle" OnClientClick="{return true;};" OnClick="SecilenKatilimciyiGetirBtn_Click" />
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniAramaGirisiBtn" CssClass="btn btn-outline-success " runat="server" Text="Yeni Arama Girişi" OnClick="YeniAramaGirisiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="YeniKisiBtn" CssClass="btn btn-outline-secondary " runat="server" Text="Yeni Kişi Girişi" OnClick="YeniKisiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="RandevuTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Faaliyet Takvimi" OnClick="RandevuTakvimiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="RandevuListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Faaliyet Listesi" OnClick="RandevuListesiBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>
        </div>
    </div>
    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
        <ContentTemplate>
            <div class="modal " id="KatilimciSecimiModal" role="dialog">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-body ">
                            <div class="card">
                                <div class="card-header text-danger">
                                    <h3 class="col-form-label font-weight-bold" id="KatiliciSecimiHeaderLbl" runat="server">Katılımcı Seçimi
                                    </h3>
                                </div>
                                <div class="card-body">
                                    <div class="card-body p-0" id="Div1" runat="server">
                                        <div class="form-group">
                                            <table id="CustomModalDataTable" class="table table-striped table-bordered" width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>Adı Soyadi</th>
                                                        <th>Kurumu</th>
                                                        <th>Seç</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>
                    </div>

                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="KatilimciSecBtn" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
</div>
