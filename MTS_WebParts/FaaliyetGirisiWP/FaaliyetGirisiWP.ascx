<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaaliyetGirisiWP.ascx.cs" Inherits="MTS_WebParts.FaaliyetGirisiWP.FaaliyetGirisiWP" %>

<style>
    .ui-datatable tbody td {
        white-space: normal;
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

<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 15em;
        font-size: small;
    }

    .disabled-item {
        background-color: rgba(206, 220, 229, 0.40) !important;
        color: darkgrey !important;
    }
</style>
<%-- Başlangıç bitiş Tarihi --%>
<script type="text/javascript">
    //On Page Load.<a href="{4BB30692-604D-4C8D-ADF7-3A049AD260F3}|MTS_WebParts\MTS_WebParts.csproj|c:\users\taylis\source\repos\portalapps\mts_webparts\kisilistesiwp\">{4BB30692-604D-4C8D-ADF7-3A049AD260F3}|MTS_WebParts\MTS_WebParts.csproj|c:\users\taylis\source\repos\portalapps\mts_webparts\kisilistesiwp\</a>
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {
        $("#BitisTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true,
            inline: true,
            altField: "#BitisTarihiTxt",
            beforeShow: function (input, inst) {
                var mindate = $('#BaslangicTarihiTxt').datepicker('getDate');
                $(this).datepicker('option', 'minDate', mindate);
                var newDate = new Date($('#BaslangicTarihiTxt').datepicker('getDate'));
                newDate.setDate(newDate.getDate() + 30);
                $(this).datepicker('option', 'maxDate', newDate);
            },
            beforeShowDay: function (date) {
                $('#ui-datepicker-div').css('clip', 'auto');
                return [true, '', ''];
            }
        });
        $("#BaslangicTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true,
            inline: true,
            altField: "#BaslangicTarihiTxt",
            beforeShow: function (input, inst) {
                var mindate = new Date();
                $(this).datepicker('option', 'minDate', mindate);
            },
        }).on("change", function () {

            var dateMin = $('[id$=BaslangicTarihiTxt]').datepicker("getDate");
            var rMin = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate());
            var rMax = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate());

            $('[id$=BitisTarihiTxt]').datepicker("option", "minDate", rMin);
            $('[id$=BitisTarihiTxt]').datepicker("option", "maxDate", rMax);

            var newDate = new Date($('#BaslangicTarihiTxt').datepicker('getDate'));
            newDate.setDate(newDate.getDate());

        });


    }
</script>

<%-- Katılımcı/itribat personeli ekleme / çıkartma --%>
<script type="text/javascript">
    function OpenSilModal() {
        $("#ModalSilDiv").modal({ backdrop: true });
    }
    function KatilimciSecimiModal() {
        $("#KatilimciSecimiModal").modal({ backdrop: false });
    }
    $("#KatilimciSecimiModal").draggable({
        handle: ".modal-dialog"
    });
    function KatilimciSecildiBtnClick(katilimciId, faaliyetId, katilimciTipi) {
        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= SecilenKatilimciyiKaydetNowBtn.ClientID%>').click();
    }
    function IrtibatSecBtnClick(kisiId, faaliyetId, katilimciTipi) {
        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = kisiId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%=IrtibatSecBtn.ClientID%>').click();
    }
    function KatilimciCikarBtnClick(katilimId) {
        document.getElementById('<%= paramFaaliyetKatilimIdLbl.ClientID%>').value = katilimId;
        document.getElementById('<%= KatilimciCikarBtn.ClientID%>').click();
    }
</script>
<%--Excell--%>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<%--Anı Objesi Seçimi--%>
<%--Modal açıldığında gelen aniobjesi checkbox'ları check edildiğide seçilenlerin bir array'de (aniObjesiIdArray) adetlerini de başka bir array'de (aniObjesiIdArray) tut 
    Kaydet'e basıldığında bu javascript arrayi codebehind'dan erişilebilen paramArray nesnesine ver
    secilenleriKaydetBtn.click eventini çalıştır, codebehind'da secilenleri AniObjesiDagitim_Table'a kaydet bitti ;-)
--%>
<script type="text/javascript"> 
    function AniObjesiModal() {
        $("#AniObjesiModal").modal({ backdrop: true });
    }
    function StokluAniObjesiModal() {
        $("#StokluAniObjesiModal").modal({ backdrop: false });
    }
    function GetirilenAniObjesiModal() {
        $("#GetirilenAniObjesiModal").modal({ backdrop: true });
    }
    function CloseModals() {
        $("#AniObjesiModal").modal('hide');
        $("#GetirilenAniObjesiModal").modal('hide');
        $("#KatilimciSecimiModal").modal('hide');
        $("#StokluAniObjesiModal").modal('hide');
        $(".modal-backdrop").remove();//ekran modaldan sonra normale dönsün
    }
    function StoksuzAniObjesiBtnClick(katilimciId, faaliyetId, katilimciTipi) {

        document.getElementById('<%= GetirilenAniObjesiTxt.ClientID%>').value = "";
        document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value = "";
        document.getElementById('<%= paramAniObjesiAdetArray.ClientID%>').value = "";


        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= AniObjesiSecBtn.ClientID%>').click();
        ArrayDoldur();
    }
    function StokluAniObjesiBtnClick(katilimciId, faaliyetId, katilimciTipi) {

        document.getElementById("StokluAniObjesiIadeEtTriggerBtn").style.display = "none";
        document.getElementById("StokluAniObjesiKaydetTriggerBtn").style.display = "block";
        document.getElementById('<%= paramStokluAniObjesiDagitimIdLbl.ClientID%>').value = 0;
        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;

        document.getElementById('<%= StokluAniObjesiSecBtn.ClientID%>').click();

    }
    function StokluAniObjesiDuzenleClick(aniObjesiDagitimId) {
        document.getElementById("StokluAniObjesiIadeEtTriggerBtn").style.display = "block";
        document.getElementById("StokluAniObjesiKaydetTriggerBtn").style.display = "none";
        document.getElementById('<%= paramStokluAniObjesiDagitimIdLbl.ClientID%>').value = aniObjesiDagitimId;
        document.getElementById('<%= StokluAniObjesiSecBtn.ClientID%>').click();

    }
    function  GetirilenAniObjesiBtnClick(katilimciId, faaliyetId, katilimciTipi) {
        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= GetirilenAniObjesiSecBtn.ClientID%>').click();        
    }
    function ArrayDoldur() {
        var idString = document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value;
        var adetString = document.getElementById('<%= paramAniObjesiAdetArray.ClientID%>').value;
        var idList = idString.split(',');
        var adetList = adetString.split(',');
        aniObjesiIdArray = [];
        aniObjesiAdetArray = [];
        for (var i = 0; i < idList.length; i++) {
            aniObjesiIdArray.push(idList[i]);
            aniObjesiAdetArray.push(adetList[i]);
        }
    }
    function GetClientID(asp_net_id) {
        return $("[id$=" + asp_net_id + "]").attr("id");
    }
    var aniObjesiIdArray = [];
    var aniObjesiAdetArray = [];
    function EkleCikar(aniObjesiId, isChecked, adet) {
        ArrayDoldur();
        var index = aniObjesiIdArray.indexOf(aniObjesiId.toString());
        if (isChecked) {
            if (index > -1) {
                aniObjesiIdArray.splice(index, 1);
                aniObjesiAdetArray.splice(index, 1);
            }
            aniObjesiIdArray.push(aniObjesiId.toString());
            aniObjesiAdetArray.push(adet);
        } else if (!isChecked && (index > -1)) {
            aniObjesiIdArray.splice(index, 1);
            aniObjesiAdetArray.splice(index, 1);
        }
        document.getElementById('BtnDiv').style.display = "block";
        //seçili olan kaldırılınca da kaydet gözükmeli
        //if (aniObjesiIdArray.length > 0)
        //    document.getElementById('BtnDiv').style.display = "block";
        //else {
        //    document.getElementById('BtnDiv').style.display = "none";

        //}
        document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value = aniObjesiIdArray;
        document.getElementById('<%= paramAniObjesiAdetArray.ClientID%>').value = aniObjesiAdetArray;
    }
    function AddRemoveAniObjesiIdToList(aniObjesiId, chkbox, divId, adet, textId) {

        var id = GetClientID(textId);
        var txtName = document.getElementById(id);
        if (chkbox.checked) {
            if (adet < 1)
                adet = 1;
        } else {
            adet = 0;
        }
        txtName.value = adet;
        txtName.disabled = !chkbox.checked;
        ChangeDivColor(divId, chkbox);

        EkleCikar(aniObjesiId, chkbox.checked, adet);
    }
    function ChangeDivColor(divId, chkbox) {
        var div = GetClientID(divId);
        var divName = document.getElementById(div);

        if (chkbox.checked) {
            divName.className = "checkbox font-weight-bold text-danger";
        } else {
            divName.className = "checkbox text-primary";
        }
    }
    function ChangeAniObjesiAdet(aniObjesiId, chkboxId, adet, textbox, divId) {
        var id = GetClientID(chkboxId);
        var chkName = document.getElementById(id);
        chkName.checked = textbox.value > 0;
        textbox.disabled = !chkName.checked;
        ChangeDivColor(divId, chkName);
        EkleCikar(aniObjesiId, chkName.checked, textbox.value);
    }
    function SecilenleriKaydetTriggerBtnClicked() {
        document.getElementById('<%= paramAniObjesiIdArray.ClientID%>').value = aniObjesiIdArray;
        document.getElementById('<%= paramAniObjesiAdetArray.ClientID%>').value = aniObjesiAdetArray;
        document.getElementById('<%= SecilenleriKaydetBtn.ClientID%>').click();
        CloseModals();
    }
    function GetirilenAniObjesiKaydetTriggerBtnClicked() {
        document.getElementById('<%= GetirilenAniObjesiKaydetBtn.ClientID%>').click();
    }
    function StokluAniObjesiKaydetTriggerBtnClicked() {

        document.getElementById('<%= StokluAniObjesiKaydetBtn.ClientID%>').click();

    }
    function StokluAniObjesiIadeEtTriggerBtnClicked() {

        document.getElementById('<%= StokluAniObjesiIadeEtBtn.ClientID%>').click();
        CloseModals();
    }
</script>
<%--Faaliyet Yeri Autocomplete--%>
<script>
    var faaliyetYerleri = [
        "TUSAŞ",
        "ASELSAN",
        "HAVELSAN",
        "İŞBİR",
        "ASPİLSAN"
    ];
    $(function () {
        
        $("#FaaliyetYeriTxt").autocomplete({
            source: faaliyetYerleri
        });
        
    });
    function setDataSet(myset) {
        faaliyetYerleri = myset;
    }
</script>

<%-- TakvimDavetiGonder --%>
<script type="text/javascript"> 
    function TakvimDavetiyesiModalAc(katilimId,faaliyetId, katilimciId, katilimciTipi, eposta) {
        document.getElementById('<%= paramFaaliyetKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramFaaliyetKatilimIdLbl.ClientID%>').value = katilimId;
        document.getElementById('<%= paramFaaliyetIdLbl.ClientID%>').value = faaliyetId;
        document.getElementById('<%= paramFaaliyetKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= EPostaAdresiTxt.ClientID%>').value = eposta;
        
        $("#TakvimDavetiModalDiv").modal({ backdrop: true });
    }
    function TakvimDavetiGonderBtnClicked() {
        
        document.getElementById('<%= TakvimDavetiGonderNowBtn.ClientID%>').click();        
        CloseModals();
    }
</script>
<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">

                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:Label CssClass="col-form-label text-primary font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Faaliyet Düzenleme"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                        <asp:Label ID="lblTime" CssClass="col-form-label text-secondary" runat="server" />
                        <asp:Timer ID="RefreshTimer" runat="server" OnTick="RefreshTimer_Tick" />
                        <div class="form-group text-right text-danger" id="TopBarDiv" runat="server"></div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </h3>
            
        </div>

        <div class="card-body alert-success">
            <div class="form-group">
                <div class="form-group">
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col">
                                    <div class="row">
                                        <div class="col">
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" runat="server" Text="Faaliyet Tipi"></asp:Label>
                                                <asp:DropDownList ID="FaaliyetTipiDDL" CssClass="form-control" runat="server" Style="height: auto" Enabled="True"></asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="FaaliyetTipiDDL" ForeColor="Red" ErrorMessage="Faaliyet Tipi Seçiniz"> </asp:RequiredFieldValidator>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" runat="server" Text="Faaliyet Yeri"></asp:Label>
                                                <div class="ui-widget">
                                                    <asp:TextBox ID="FaaliyetYeriTxt" runat="server" ClientIDMode="Static" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" runat="server" Text="Faaliyet Amacı"></asp:Label>
                                                <asp:DropDownList ID="FaaliyetAmaciDDL" CssClass="form-control" runat="server" Style="height: auto"></asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="FaaliyetAmaciDDL" ForeColor="Red" ErrorMessage="Faaliyet Amacı Seçiniz"> </asp:RequiredFieldValidator>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label CssClass="col-form-label" runat="server" Text="Faaliyet Durumu"></asp:Label>
                                                <asp:DropDownList ID="FaaliyetDurumuDDL" CssClass="form-control" runat="server" Style="height: auto" OnSelectedIndexChanged="FaaliyetDurumuDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                       
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="row">
                                        <div class="form-group col ">
                                            <asp:Label CssClass="col-from-label" runat="server" Text="Başlangıç Tarihi"></asp:Label>
                                            <asp:TextBox ID="BaslangicTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BaslangicTarihiTxt" ForeColor="Red" ErrorMessage="Başlama Tarihi Seçiniz"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group col">
                                            <asp:Label CssClass="col-from-label" runat="server" Text="Bitiş Tarihi"></asp:Label>
                                            <asp:TextBox ID="BitisTarihiTxt" CssClass="form-control input-date " runat="server" ClientIDMode="Static"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BitisTarihiTxt" ForeColor="Red" ErrorMessage="Bitiş Tarihi Seçiniz"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group col">
                                            <div class="checkbox pt-3">
                                                <label>
                                                    <asp:CheckBox ID="TumGunChk" runat="server" Checked="false" ToolTip="Tüm gün geçerli faaliyetlar için işaretleyiniz." />
                                                    Tüm Gün
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="form-group col">
                                            <asp:Label runat="server" CssClass="col-form-label" Text="Başlangıç Saati"></asp:Label>
                                            <asp:DropDownList ID="BasSaatDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="BasSaatDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BasSaatDDL" ForeColor="Red" ErrorMessage="Başlama Saati Seçiniz"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group col">
                                            <asp:Label CssClass="col-form-label" runat="server" Text="Bitiş Saati"></asp:Label>
                                            <asp:DropDownList ID="BitSaatDDL" runat="server" CssClass="form-control " Style="height: auto" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BitSaatDDL" ForeColor="Red" ErrorMessage="Bitiş Saati Seçiniz"> </asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-group col">
                                            <div class="checkbox pt-3">
                                                <label>
                                                    <asp:CheckBox ID="AcikTarihChk" runat="server" Checked="false" ToolTip="Tarihi sonradan belli olacak faaliyetlar için işaretleyiniz." OnCheckedChanged="AcikTarihChk_CheckedChanged" AutoPostBack="true" />
                                                    Açık Tarihli
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">

                                <div class="form-group col">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Faaliyet Konusu"></asp:Label>
                                    <asp:TextBox ID="FaaliyetKonusuTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="FaaliyetKonusuTxt" ForeColor="Red" ErrorMessage="Faaliyet Konusu Giriniz"> </asp:RequiredFieldValidator>
                                </div>
                                <div class="col-4">
                                    <div class="checkbox pt-3">
                                        <label class="float-right">
                                            <asp:CheckBox ID="OzelKalemTakvimiChk" runat="server" Checked="true" ToolTip="Özel Kalemin İnternet Takvimine girecek faaliyetlar için işaretleyiniz." OnCheckedChanged="OzelKalemTakvimiChk_CheckedChanged" AutoPostBack="True" />
                                            Özel Kalem Takvimine İşlensin
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Açıklama"></asp:Label>
                                    <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-form-label font-weight-bold text-danger" runat="server" Text="Yönetici Notu"></asp:Label>
                                    <asp:TextBox ID="YoneticiNotuTxt" CssClass="form-control font-weight-bold text-danger" runat="server" Text="" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                </div>
                            </div>
                        </ContentTemplate>
<%--                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="tick" />
                        </Triggers>--%>
                    </asp:UpdatePanel>
                </div>

                <div class="card" id="KatilimciBilgileriDiv" runat="server" style="display: none">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                            <div class="card-body mb-5">
                                <div class="form-group">
                                    <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Adı Soyadı</th>
                                                <th>Kurumu</th>
                                                <th>Katılımcı</th>
                                                <th>Katilimci Tipi</th>
                                                <th>Anı Objesi </th>
                                                <th>E-posta Daveti</th>
                                                <th>Kişi Kartı</th>
                                                <th>Çıkar</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                                <div class="form-group float-right" id="AramaGorusmeDiv" runat="server" style="display: none">
                                    <asp:Label ID="AramaGorusmeLbl" runat="server" Text=""></asp:Label>
                                    <asp:LinkButton ID="AramaGorusmeBtn" runat="server" CssClass="btn btn-warning" Text="Arama/Görüşmeye Git" OnClick="AramaGorusmeBtn_Click" CausesValidation="false" />
                                </div>
                                <div class="form-group" id="IcIrtibatDiv" runat="server">
                                    <asp:Label ID="IcIrtibatLbl" runat="server" Text=""></asp:Label>
                                    <asp:LinkButton ID="IcIrtibatCikarBtn" runat="server" CssClass="btn btn-outline-danger" Text="Çıkar" OnClick="IcIrtibatCikarBtn_Click" CausesValidation="false" />
                                </div>
                                <div class="form-group" id="DisIrtibatDiv" runat="server">
                                    <asp:Label ID="DisIrtibatLbl" runat="server" Text=""></asp:Label>
                                    <asp:LinkButton ID="DisIrtibatCikarBtn" runat="server" CssClass="btn btn-outline-danger" Text="Çıkar" OnClick="DisIrtibatCikarBtn_Click" CausesValidation="false" />
                                </div>

                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="KatilimciCikarBtn" EventName="click" />
                            <asp:AsyncPostBackTrigger ControlID="IrtibatSecBtn" EventName="click" />
                            <asp:AsyncPostBackTrigger ControlID="SecilenKatilimciyiKaydetNowBtn" EventName="click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div class="card-footer">
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                        <div class="form-group" id="KatilimciEkleBtnDiv" runat="server">
                            <asp:LinkButton ID="KatilimciEkleBtn" runat="server" CssClass="btn btn-outline-success " Text="Katılımcı Ekle" OnClick="KatilimciEkleBtn_Click" CausesValidation="false" />
                        </div>
                            </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="tick" />
                        </Triggers>
                    </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Faaliyeti Kaydet" OnClick="KaydetBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetSilBtn" CssClass="btn btn-outline-danger" runat="server" Text="Faaliyeti Sil" OnClick="FaaliyetSilBtn_Click" Visible="False"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetKartiBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Faaliyet Kartı" OnClick="FaaliyetKartiBtn_Click" Visible="False"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Faaliyet Takvimi" OnClick="FaaliyetTakvimiBtn_Click" CausesValidation="false"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Faaliyet Listesi" OnClick="FaaliyetListesiBtn_Click" CausesValidation="false"></asp:LinkButton>
                     <asp:LinkButton ID="AcikTarihliFaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Açık Tarihli Faal. List." OnClick="AcikTarihliFaaliyetListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click" CausesValidation="false"></asp:LinkButton>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="tick" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <div id="KatilimciHiddenDiv" style="display: none">
        <input id="paramFaaliyetIdLbl" runat="server" type="text" />
        <input id="paramFaaliyetKatilimIdLbl" runat="server" type="text" />
        <input id="paramFaaliyetKatilimciIdLbl" runat="server" type="text" />
        <input id="paramFaaliyetKatilimciTipiLbl" runat="server" type="text" />
        <asp:LinkButton ID="SecilenKatilimciyiKaydetNowBtn" runat="server" CausesValidation="false" Text="Faaliyete Ekle" OnClientClick="{return true;};" OnClick="SecilenKatilimciyiKaydetNowBtn_Click" />
        <asp:LinkButton ID="KatilimciCikarBtn" runat="server" CssClass="btn btn-outline-success" Text="Faaliyetden Çıkar" OnClick="KatilimciCikarBtn_Click" />
        <asp:LinkButton ID="IrtibatSecBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="IrtibatSecBtn_Click" ClientIDMode="Static" />
        <asp:LinkButton ID="AniObjesiSecBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="AniObjesiSecBtn_Click" ClientIDMode="Static" />
        <asp:LinkButton ID="StokluAniObjesiSecBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="StokluAniObjesiSecBtn_Click" ClientIDMode="Static" />
        <asp:LinkButton ID="GetirilenAniObjesiSecBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="GetirilenAniObjesiSecBtn_Click" ClientIDMode="Static" />
    </div>
</div>
<!-- Silme Onayı Modal -->
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="ModalSilDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="SilModalBaslikLbl" runat="server" Text="Faaliyet Silinecek"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:Label ID="ParamVnLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="m-1 text-center" id="MesajDiv">
                            <div class="form-group">
                                <asp:Label ID="SilMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="FaaliyetSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Faaliyeti Sil" OnClick="FaaliyetSilNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="FaaliyetSilBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="GuncelleBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
<!-- Katılımcı Seçimi Modal -->
<asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal " id="KatilimciSecimiModal" role="dialog">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h3 class="col-form-label font-weight-bold" id="KatiliciSecimiHeaderLbl" runat="server">Katılımcı Seçimi
                        </h3>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body ">

                        <div class="card">
                            <div class="card-body">
                                <div class="card-body p-0" id="Div1" runat="server">
                                    <div class="form-group">
                                        <table id="CustomModalDataTable" class="table table-striped table-bordered table-sm small" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>Adı Soyadi</th>
                                                    <th>Kurumu</th>
                                                    <th>Faaliyete Ekle</th>
                                                    <th>İrtibat Noktası Ekle</th>
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
        <asp:AsyncPostBackTrigger ControlID="KatilimciEkleBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
<!-- Stoksuz Anı Objesi Modal -->
<asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="AniObjesiModal" role="dialog">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header text-danger">
                        <h3 class="col-form-label font-weight-bold" id="AniObjesiHeaderLbl" runat="server">Stoksuz Anı Objesi</h3>
                    </div>
                    <div class="modal-body ">
                        
                        <div id="InvisibleDiv" style="display: none">
                            <input id="paramAniObjesiIdArray" runat="server" type="text" />
                            <input id="paramAniObjesiAdetArray" runat="server" type="text" />
                            <asp:LinkButton ID="SecilenleriKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="SecilenleriKaydetBtn_Click" />
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="Verilen Anı Objesi"></asp:Label>
                            <asp:Table ID="CustomAniObjesiModalDataTable" runat="server" class="table-striped table-bordered" Width="100%"></asp:Table>
                            <asp:PlaceHolder ID="ModalPlaceHolder" runat="server"></asp:PlaceHolder>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                        <div id="BtnDiv" style="display: block">
                            <input id="SecilenleriKaydetTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Kaydet" onclick="SecilenleriKaydetTriggerBtnClicked();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="AniObjesiSecBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="StokluAniObjesiSecBtn" EventName="click" />

    </Triggers>
</asp:UpdatePanel>
<!-- Getirilen Anı Objesi Modal -->
<asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="GetirilenAniObjesiModal" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header text-danger">
                        <h3 class="col-form-label font-weight-bold" id="GetirilenAniObjesiModalTitle" runat="server"></h3>
                    </div>
                    <div class="modal-body ">
                        <div id="InvisibleDiv1" style="display: none">
                            <asp:LinkButton ID="GetirilenAniObjesiKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="GetirilenAniObjesiKaydetBtn_Click" />
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="Getirilen Anı Objesi"></asp:Label>
                            <asp:TextBox ID="GetirilenAniObjesiTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                        <div id="GetirilenAniObjesiKaydetBtnDiv" style="display: block">
                            <input id="GetirilenAniObjesiKaydetTriggerBtn" class="btn btn-success" type="button" value="Kaydet" onclick="GetirilenAniObjesiKaydetTriggerBtnClicked();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="GetirilenAniObjesiSecBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="GetirilenAniObjesiKaydetBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
<!-- Stoklu Anı Objesi Modal -->
<div class="modal" id="StokluAniObjesiModal" role="dialog">

    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="modal-header text-danger">
                        <h3 class="col-form-label font-weight-bold" id="StokluAniObjesiModalTitle" runat="server"></h3>
                    </div>
                    <div class="modal-body ">
                        <div id="InvisibleDiv2" style="display: none">
                            <input id="paramStokluAniObjesiDagitimIdLbl" runat="server" type="text" />
                            <input id="paramStokluAniObjesiDepoIdLbl" runat="server" type="text" />
                            <input id="paramStokluAniObjesiAdetLbl" runat="server" type="text" />
                            <asp:LinkButton ID="StokluAniObjesiKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="StokluAniObjesiKaydetBtn_Click" />
                            <asp:LinkButton ID="StokluAniObjesiIadeEtBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Depoya İade Et " OnClick="StokluAniObjesiIadeEtBtn_Click" />
                        </div>
                        <div class="form-group" id="IadeDiv" runat="server" style="display: none">
                            <div class="row">
                                <div class="form-group col-5">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="İade Edilecek Anı Objesi"></asp:Label>
                                    <asp:TextBox ID="IadeEdilecekAniObjesiTxt" CssClass="form-control" runat="server" type="text" Text="" Enabled="False"></asp:TextBox>
                                </div>
                                <div class="form-group col-5">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Odalar"></asp:Label>
                                    <asp:TextBox ID="IadeEdilecekDepoTxt" CssClass="form-control" runat="server" type="text" Text="" Enabled="False"></asp:TextBox>
                                </div>
                                <div class="form-group col-2">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Adet"></asp:Label>
                                    <asp:TextBox ID="IadeEdilecekAdetTxt" CssClass="form-control input-integer" runat="server" type="number" min="1" max="7" step="1" Text="">1</asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="form-group" id="DagitimDiv" runat="server" style="display: block">
                            <div class="row">
                                <div class="form-group col-5">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Anı Objesi"></asp:Label>
                                    <asp:DropDownList ID="StokluAniObjesiDDL" CssClass="form-control" runat="server" Style="height: auto" Enabled="True" AutoPostBack="true" OnSelectedIndexChanged="StokluAniObjesiDDL_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="form-group col-5">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Çıkış Deposu"></asp:Label>
                                    <asp:DropDownList ID="DepoDDL" CssClass="form-control" runat="server" Style="height: auto" Enabled="True" AutoPostBack="true" OnSelectedIndexChanged="DepoDDL_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="form-group col-2">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Adet"></asp:Label>
                                    <asp:TextBox ID="StokluAdetTxt" CssClass="form-control input-integer" runat="server" type="number" min="1" max="7" step="1" Text="">1</asp:TextBox>
                                    <%--<asp:RangeValidator ID="StokluAdetTxtRV" runat="server" Type="Integer" ControlToValidate="StokluAdetTxt" MaximumValue="5" MinimumValue="0" ValidationGroup="form" ForeColor="Red" ErrorMessage="Adet hatası" />--%>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="StokluAniObjesiSecBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="StokluAniObjesiKaydetBtn" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="StokluAniObjesiIadeEtBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                <div id="StokluAniObjesiIadeEtBtnDiv" style="display: block">
                    <input id="StokluAniObjesiIadeEtTriggerBtn" class="btn btn-success" type="button" value="Depoya İade Et" onclick="StokluAniObjesiIadeEtTriggerBtnClicked();" />
                </div>
                <div id="StokluAniObjesiKaydetBtnDiv" style="display: block">
                    <input id="StokluAniObjesiKaydetTriggerBtn" class="btn btn-success" type="button" value="Kaydet" onclick="StokluAniObjesiKaydetTriggerBtnClicked();" />
                </div>
            </div>
        </div>
    </div>
</div>
<!-- TakvimDaveti Modal -->
<asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="TakvimDavetiModalDiv" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header text-danger">
                        <h3 class="col-form-label font-weight-bold" id="H1" runat="server">Takvim Daveti</h3>
                    </div>
                    <div class="modal-body ">
                        <div id="TakvimDavetiGonderNowDiv" style="display: none">
                            <asp:LinkButton ID="TakvimDavetiGonderNowBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Gönder " OnClick="TakvimDavetiGonderNowBtn_Click" />
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="EPosta Adresi"></asp:Label>
                            <asp:TextBox ID="EPostaAdresiTxt" CssClass="form-control" runat="server" Text="" ></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                        <div id="TakvimDavetiGonderBtnDiv" style="display: block">
                            <input id="TakvimDavetiGonderBtn" class="btn btn-success" type="button" value="Gönder" onclick="TakvimDavetiGonderBtnClicked();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="GetirilenAniObjesiSecBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="GetirilenAniObjesiKaydetBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>