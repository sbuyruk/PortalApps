<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HarcirahGirisWP.ascx.cs" Inherits="IKYS_WebParts.HarcirahGirisWP.HarcirahGirisWP" %>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Harcırah Tanımları"></asp:Label>
            </h3>
        </div>

        <div class="card-body">
            <div class="row g-2 mb-2">
                <div class="col-2">
                    <label class="col-form-label" for="UlkeFilterDDL">Ülke Filtresi</label>
                    <asp:DropDownList ID="UlkeFilterDDL"
                        runat="server"
                        CssClass="form-control form-select form-select-lg"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="UlkeFilterDDL_SelectedIndexChanged" />
                </div>
                <div class="col-4">
                    <label class="col-form-label" for="TarihSeriDDL">Tarih Aralığı</label>
                    <asp:DropDownList ID="TarihSeriDDL"
                        runat="server"
                        CssClass="form-control form-select form-select-lg"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="TarihSeriDDL_SelectedIndexChanged" />
                </div>

            </div>
            <asp:Table ID="AyrintiTable" runat="server" CssClass="table table-striped table-bordered" />
            <hr />
            <div class="row g-2 border text-center">
                <div class="col-2">
                    <label class="col-form-label" for="YeniBaslangicTarihiTxt">Yeni Geçerli Olacak Harcırah İçin Tarih </label>
                    <asp:TextBox
                        ID="YeniBaslangicTarihiTxt"
                        runat="server"
                        CssClass="form-control input-date"
                        ClientIDMode="Static"
                        placeholder="Tarih Seçiniz" />
                </div>

                <div class="col-2 d-flex align-items-end">
                    <asp:LinkButton ID="EkleBtn"
                        runat="server"
                        CssClass="btn btn-success w-100"
                        Text="Yeni Tarih Aralığı Ekle"
                        OnClick="EkleBtn_Click" />
                </div>
            </div>
        </div>
    </div>
</div>

<style>
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18.5em;
        font-size: small;
    }
</style>

<script type="text/javascript">
    // On Page Load
    $(function () {
        SetHarcirahDatePicker();
    });

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    if (prm != null) {
        prm.add_endRequest(function (sender, e) {
            if (sender._postBackSettings.panelsToUpdate != null) {
                SetHarcirahDatePicker();
            }
        });
    };

    function SetHarcirahDatePicker() {

        var now = new Date();
        var year = now.getFullYear();

        var defaultBas = "01.01." + year;

        if (!$("#YeniBaslangicTarihiTxt").val()) {
            $("#YeniBaslangicTarihiTxt").val(defaultBas);
        }


        $("#YeniBaslangicTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true
        });
    }
</script>