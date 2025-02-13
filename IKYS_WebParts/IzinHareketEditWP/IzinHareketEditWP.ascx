<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IzinHareketEditWP.ascx.cs" Inherits="IKYS_WebParts.IzinHareketEditWP.IzinHareketEditWP" %>
<style>
    .ui-datepicker {
        width: 18em;
        font-size:small;
    }
</style>
<script type="text/javascript">
    //On Page Load.
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {
        $("[id$=IzinBitTarTxt]").datepicker({
            inline: true,
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
                var izinTipi = document.getElementById('<%= IzinTipiIdLbl.ClientID%>').value;
                var maxSure = 365;//en fazla 1 yıl ücretsiz izin
                if (izinTipi == "1") {//ücretli
                    maxSure = 45;//26 max izin + 10 gün hafta sonu + 9 gün bayram vs
                } else if (izinTipi == "4") {//Evlenme
                    maxSure = 14; //3 +2 hafta sonu+ 9 bayram vs
                } else if (izinTipi == "5") {//Doğum
                    maxSure = 140;//18 hafta + 2gun haftasonu +9 bayram vs
                } else if (izinTipi == "6") {//Babalık
                    maxSure = 18;//5 + 4 haftasonu +9 bayram vs
                } else if (izinTipi == "7") {//Ölüm
                    maxSure = 14;//3 +2 hafta sonu+ 9 bayram vs
                }
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
    function CallButtonClick(onay) {

        var button = document.getElementById('<%= DeleteNowBtn.ClientID%>');
        button.click();
    }
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    $(function () {
        $("#datepicker").datepicker({
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
        });
    });
</script>
<div class="container " style="width: 900px;">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="İzin Düzenleme"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IzinHareketIdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="PersonelAdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="alert-secondary" id="IzinTalepDiv" runat="server">
                    <div class="form-group">
                        <div class="row">
                            <div class="form-group col">
                                <label class="col-form-label" for="IzinTipiLbl">İzin Tipi</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:TextBox ID="IzinTipiLbl" runat="server" class="form-control" ReadOnly="true" />
                                <div style="display: none">
                                    <asp:TextBox ID="IzinTipiIdLbl" runat="server" class="form-control" ReadOnly="true" />
                                </div>
                            </div>
                            <div class="form-group col">
                                <label class="col-form-label" for="IzinBasTarTxt">Başlangıç Tarihi</label>
                                <input runat="server" type="text" id="IzinBasTarTxt" name="IzinBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="IzinBasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col" id="IzinBasSaatDiv" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinBasSaatDDL" style="height: auto">Başlangıç Saati</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="IzinBasSaatDDL" runat="server" class="form-control" OnSelectedIndexChanged="IzinBasSaatDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                            </div>
                            <div class="form-group col" id="IzinBitTarDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="IzinBitTarTxt">Bitiş Tarihi</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinBitTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <input runat="server" type="text" id="IzinBitTarTxt" name="IzinBitTarTxt" class="form-control DatePicker2" readonly="readonly" />
                            </div>
                            <div class="form-group col" id="IzinBitSaatDiv" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinBitSaatDDL">Bitiş Saati</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="IzinBitSaatDDL" runat="server" class="form-control " Style="height: auto" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col-4" id="VekilDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="VekilImzaDDL">Vekil</label>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="VekilImzaDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                <asp:DropDownList ID="VekilImzaDDL" runat="server" class="form-control" Style="height: auto" />
                            </div>
                            <div class="form-group col-4" id="AmirDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="AmirImzaDDL">Amir</label>
                                <asp:DropDownList ID="AmirImzaDDL" runat="server" class="form-control" Style="height: auto" />
                            </div>
                            <div class="form-group col-4" id="OnayDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="OnayImzaDDL">Onay</label>
                                <asp:DropDownList ID="OnayImzaDDL" runat="server" class="form-control" Style="height: auto" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group col" id="AdresDiv" runat="server" style="display: block">
                                <label class="col-form-label">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                            </div>
                            <div class="form-group col">
                                <label class="col-form-label">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" class="form-control" type="text" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="KisiselSayfaBtn" runat="server" Text="Kişisel Sayfa" CausesValidation="false" OnClick="KisiselSayfaBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="IzinHareketListesiBtn" runat="server" Text="Kullanılan İzinler" CausesValidation="false" OnClick="IzinHareketListesiBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary float-left" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger float-left ml-2" runat="server" Text="Sil" OnClick="DeleteBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-primary" ID="IzinDegisDilekceBtn" runat="server" Text="İzin Değişiklik Dilekçesi" CausesValidation="false" OnClick="IzinDegisDilekceBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-outline-primary" ID="IzinIptalDilekceBtn" runat="server" Text="İzin Iptal Dilekçesi" CausesValidation="false" OnClick="IzinIptalDilekceBtn_Click" Visible="false" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="UpdateBtn" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
</div>
 <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-body">
                            <div style="display: none">
                                <asp:LinkButton ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" />
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="SilLbl" class="label label-primary " runat="server" Text="İzin Kaydı Silinecek"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <label class="col-form-label">İzin Silme Sebebi</label>
                                    <asp:TextBox ID="SilmeSebebiTxt" TextMode="MultiLine" Rows="3" runat="server" class="form-control" type="text" />
                                    <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Geçerli İzni Silmeyi Onaylıyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button ID="DeleteModalBtn" runat="server" class="btn btn-danger" onclick="CallButtonClick()" >Sil</button>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
