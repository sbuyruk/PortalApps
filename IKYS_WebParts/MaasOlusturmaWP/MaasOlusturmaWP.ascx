<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MaasOlusturmaWP.ascx.cs" Inherits="IKYS_WebParts.MaasOlusturmaWP.MaasOlusturmaWP" %>

<style>
    .header-center {
        text-align: center;
        vertical-align: middle!important;
    }
</style>

<script type="text/javascript">
    //On Page Load.
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {
        $("#UygulamaTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true,
            inline: true,
            altField: "#UygulamaTarihiTxt",
            beforeShow: function (input, inst) {
                var mindate = new Date(2025, 4, 1);
                $(this).datepicker('option', 'minDate', mindate);
                var newDate = new Date($('#BaslangicTarihiTxt').datepicker('getDate'));
                newDate.setDate(newDate.getDate() + 15);
                $(this).datepicker('option', 'maxDate', newDate);
            },
            beforeShowDay: function (date) {
                $('#ui-datepicker-div').css('clip', 'auto');
                return [true, '', ''];
            }
        });
        
    }
</script>

<div class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Maaş Oluşturma | "></asp:Label>
                <asp:Label CssClass="form-label fw-light">Seçilen ay sonu itibarı ile, kesintisiz maaş listesi</asp:Label>
            </h3>
        </div>
        <div class="card-body" id="TablesDiv">
            <div class="form-group mb-3">
                <div class="row mb-3">
                    <div class="col-1">
                        <label class="form-label fw-semibold" for="UygulamaTarihiTxt">Tarih</label>
                        <asp:TextBox ID="UygulamaTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static"> </asp:TextBox>
                        
                    </div>
                </div>
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                        <thead>
                            <tr>
                                <th>Sıra</th>
                                <th>Adı Soyadı</th>
                                <th>Kadro ve Ünvanı</th>
                                <th>Kademe İlerleme Tarihi</th>
                                <th>Derece/Kademe</th>
                                <th>Ücret</th>
                                <th>AGİ Yerine İlave Ödeme</th>
                                <th>Toplam Ücret</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>         
        </div>
    </div>
</div>


