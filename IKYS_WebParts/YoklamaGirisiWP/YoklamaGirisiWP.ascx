<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YoklamaGirisiWP.ascx.cs" Inherits="IKYS_WebParts.YoklamaGirisiWP.YoklamaGirisiWP" %>
<script type="text/javascript">
    function OpenModal() {
        $("#ModalOnayDiv").modal({ backdrop: false });
    }
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

                var maxSure = 90;//en fazla 3 ay
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
                        <asp:Label CssClass="col-form-label  text-success " ID="TitleLbl" runat="server" Text="Yoklama Girişi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="YoklamaIdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="PersonelAdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div>
                        <div class="form-group p-1" id="PersonelDiv" runat="server">
                            <label class="col-form-label p-1" for="PersonelDDL">Personel Seçimi</label>
                            <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control col-3" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px" />
                        </div>
                    </div>
                    <div class="alert-secondary p-2">
                        <div class="row col">
                            <div id="BulunmamaSebebiDDLDiv" class="form-group col-3" runat="server">
                                <label class="col-form-label" for="BulunmamaSebebiDDL">Bulunmama Sebebi</label>
                                <asp:DropDownList ID="BulunmamaSebebiDDL" runat="server" class="form-control" OnSelectedIndexChanged="BulunmamaSebebiDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px" />
                                <asp:TextBox ID="GorevliTxt" runat="server" class="form-control" type="text" Text="Şehir İçi Görevi" Visible="false" />
                            </div>
                            <div class="form-group col-2">
                                <label class="col-form-label" for="BasTarTxt">Başlangıç Tarihi</label>
                                <input runat="server" type="text" id="BasTarTxt" name="BasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="BasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col-2" id="BasSaatDiv">
                                <label class="col-form-label" for="BasSaatDDL">Başlama Saati</label>
                                <asp:DropDownList ID="BasSaatDDL" runat="server" class="form-control " OnSelectedIndexChanged="BasSaatDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px" />
                            </div>
                            <div class="form-group col-2" id="BitTarDiv" runat="server">
                                <label class="col-form-label" for="BitTarTxt">Bitiş Tarihi</label>
                                <input runat="server" type="text" id="BitTarTxt" name="BitTarTxt" class="form-control DatePicker2" readonly="readonly" />
                            </div>
                            <div class="form-group col-2" id="BitSaatDiv" runat="server">
                                <label class="col-form-label" for="BitSaatDDL">Bitiş Saati</label>
                                <asp:DropDownList ID="BitSaatDDL" runat="server" class="form-control " Height="34px" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-6">
                                <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                            </div>
                            <div class="form-group col-6">
                                <label class="col-form-label" for="AdresTxt">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                            </div>
                        </div>

                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" Visible="false" />
                    
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" Visible="false" />
                    <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger ml-2" runat="server" Text="Kaydı Sil" OnClick="DeleteBtn_Click" />

                    <asp:LinkButton ID="RaporAlBtn" CssClass="btn btn-outline-success float-right" runat="server" Text="Rapor Al" OnClick="RaporAlBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="YoklamaListesiBtn" runat="server" Text="Yoklama Listesi" CausesValidation="false" OnClick="YoklamaListesiBtn_Click" />
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">

                        <div class="modal-body">
                            <div style="display: none">
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Yoklama Kaydı Silinecek"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Geçerli Yoklama Kaydını Silmeyi Onaylıyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Yoklama Kaydını Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
