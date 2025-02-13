<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopluIzinGirisiWP.ascx.cs" Inherits="IKYS_WebParts.TopluIzinGirisiWP.TopluIzinGirisiWP" %>
<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    //On Page Load.
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {


        $("[id$=IzinBitTarTxt]").datepicker({
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
                var maxSure = 3;//en fazla 3 gn
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
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  text-success mb-1" ID="TitleLbl" runat="server" Text="Toplu İzin Girişi"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                <ContentTemplate>
                    <div class="p-2" id="IzinHareketDiv" runat="server">
                        <div class="row">
                            <div class="col">
                                <div class="card">
                                    <div class="card-body">
                                        <div class="row">
                                            <div id="IzinTipiDDLDiv" class="form-group col">
                                                <label class="col-form-label" for="IzinTipiDDL">İzin Tipi</label>
                                                <asp:DropDownList ID="IzinTanimDDL" runat="server" class="form-control" OnSelectedIndexChanged="IzinTanimDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group col">
                                                <label class="col-form-label" for="IzinBasTarTxt">Başlangıç Tarihi</label>
                                                <input runat="server" type="text" id="IzinBasTarTxt" name="IzinBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="IzinBasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                            </div>
                                            <div class="form-group col" id="IzinBasSaatDiv" runat="server" style="display: none">
                                                <label class="col-form-label" for="IzinBasSaatDDL">Başlangıç Saati</label>
                                                <asp:DropDownList ID="IzinBasSaatDDL" runat="server" class="form-control " OnSelectedIndexChanged="IzinBasSaatDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group col" id="IzinBitTarDiv" runat="server" style="display: block;">
                                                <label class="col-form-label" for="IzinBitTarTxt">Bitiş Tarihi</label>
                                                <input runat="server" type="text" id="IzinBitTarTxt" name="IzinBitTarTxt" class="form-control DatePicker2" readonly="readonly" />
                                            </div>
                                            <div class="form-group col" id="IzinBitSaatDiv" runat="server" style="display: none">
                                                <label class="col-form-label" for="IzinBitSaatDDL">Bitiş Saati</label>
                                                <asp:DropDownList ID="IzinBitSaatDDL" runat="server" class="form-control " />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label class="col-form-label" ID="AciklamaLbl" runat="server" for="AciklamaTxt">Açıklama</asp:Label>
                                            <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control text-danger" type="text" />
                                        </div>
                                        <div class="form-group">
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" ID="UyariLbl" runat="server" Text=""></asp:Label>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" ID="UyariLbl1" runat="server" Text=""></asp:Label>
                                            </div>

                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" ID="UyariLbl2" runat="server" Text=""></asp:Label>
                                            </div>

                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" ID="UyariLbl3" runat="server" Text=""></asp:Label>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-5">
                                <div class="card">
                                    <div class="card-header form-group" id="PersonelDiv" runat="server">
                                        <label class="col-form-label" for="PersonelDDL">Personel Seçimi</label>
                                        <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" />
                                    </div>
                                    <div class="card-body">
                                        <div class="form-group" id="HaricTutulanDiv" runat="server" style="display: block">
                                            <label class="col-form-label" for="HaricTutulanTxt">Seçilen Personel Listesi</label>
                                            <div class="table border" style="max-height: 300px; overflow: auto;">
                                                <asp:Table ID="SecilenPersonelTable" runat="server" CssClass="table table-sm small table-hover">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <div class="form-group">
                                            <asp:RadioButtonList ID="UygulanacakGrupRL" runat="server">
                                                <asp:ListItem Value="Tum" runat="server" Selected="True">Tüm Personele İzin Gir (Seçilen personel Hariç)</asp:ListItem>
                                                <asp:ListItem Value="Secilen" runat="server">Sadece Seçilen Personele İzin Gir</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success float-left" runat="server" Text="Toplu İzini Kaydet" OnClick="SaveBtn_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="modal" id="ModalOnayDiv" role="dialog">
                        <div class="modal-dialog">
                            <!-- Modal content-->
                            <div class="modal-content" style="width: 650px;">
                                <div class="modal-body">
                                    <div>
                                        <div class="text-center">
                                            <h3>
                                                <asp:Label ID="ModalTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h3>
                                            <h4>
                                                <asp:Label ID="ModalSubTitleLbl" CssClass="col-form-label" runat="server" Text="Lütfen Dikkat"></asp:Label>
                                            </h4>
                                        </div>
                                        <div class="card-body ">
                                            <div class="form-group">
                                                <asp:Label ID="UyariMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Toplu izin girişi yapılacak. Lütfen değişikliği kaydetmeden önce dikkatle kontrol ediniz."></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:LinkButton CssClass="btn btn-success" ID="SaveNowBtn" runat="server" CausesValidation="false" Text="Toplu İzini Kaydet" OnClientClick="{return true;};" OnClick="SaveNowBtn_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">İptal</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </div>
</div>
