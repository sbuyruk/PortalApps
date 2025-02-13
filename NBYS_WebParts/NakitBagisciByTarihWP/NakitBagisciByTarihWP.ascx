<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciByTarihWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciByTarihWP.NakitBagisciByTarihWP" %>
<script type="text/javascript">

    //On Page Load.
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {
        $("[id$=BitTarTxt]").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            showAnim: "fold",
            changeMonth: true,
            changeYear: true,
            beforeShow: function (input, inst) {
                var mindate = $('.DateTimePickerV1').datepicker('getDate');
                $(this).datepicker('option', 'minDate', mindate);
                var maxSure = 365;//en fazla 1 yıl 
                var newDate = new Date($('.DateTimePickerV1').datepicker('getDate'));
                newDate.setDate(newDate.getDate() + maxSure);
                $(this).datepicker('option', 'maxDate', newDate);
            },
            beforeShowDay: function (date) {
                $('#ui-datepicker-div').css('clip', 'auto');
                return [true, '', ''];
            }
        });
    }
    //On UpdatePanel Refresh.
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    if (prm != null) {
        prm.add_endRequest(function (sender, e) {
            if (sender._postBackSettings.panelsToUpdate != null) {
                SetDatePicker();
            }
        });
    };
    
</script>
<div class="container shadow">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Tarih Bazlı Bağışçı Listesi (Eski-Yeni)"></asp:Label>
                        <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="23" runat="server" ></asp:Label>
                    </h3>
                </div>
                <div class="card-body alert-secondary" id="MainCardDiv" runat="server">
                    <asp:UpdatePanel ID="upPanel" runat="server">
                        <ContentTemplate>
                            <div class="card-body p-0">
                                <div class="input-group">
                                    <div class="input-group">
                                        <div class="form-group col-2">
                                            <label class="col-form-label" for="BasTarTxt">Başlangıç Tarihi</label>
                                            <input runat="server" type="text" id="BasTarTxt" name="BasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" style="width:130px" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group col-3" id="BitTarDiv" runat="server" style="display: block;">
                                            <label class="col-form-label" for="BitTarTxt">Bitiş Tarihi</label>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BitTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                            <input runat="server" type="text" id="BitTarTxt" name="BitTarTxt" class="form-control DatePicker2" readonly="readonly" style="width:130px"/>
                                        </div>
                                        <div class="form-group col-3">
                                            <label class="col-form-label col" for="ListeleBtn">.</label>
                                            <asp:LinkButton ID="ListeleBtn" CssClass="btn btn-outline-primary" runat="server" class="form-control" Text="Listele" OnClick="ListeleBtn_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="table loader">
                                    <asp:Table ID="SonucTable" runat="server" CssClass="table table-bordered table-striped">
                                    </asp:Table>
                                </div>
<%--                                <div class="table loader">
                                    <asp:Table ID="SMSTable" runat="server" class="table table-bordered table-striped">
                                    </asp:Table>
                                </div>--%>
                            </div>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </div>
                <div class="card-footer">
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
