<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResmiTatilGirisiWP.ascx.cs" Inherits="IKYS_WebParts.ResmiTatilGirisiWP.ResmiTatilGirisiWP" %>
<style>
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18.5em;
        font-size: small;
    }
</style>
<script type="text/javascript">
    function OpenModal() {        
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    function CloseModal() {
        $("#ModalOnayDiv").modal('hide');

    }
</script>
<script type="text/javascript">
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
<%-- Başlangıç bitiş Tarihi --%>
<script type="text/javascript">
    //On Page Load.
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
                newDate.setDate(newDate.getDate() + 365);
                //$(this).datepicker('option', 'maxDate', newDate);
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

        }).on("change", function () {

            var dateMin = $('[id$=BaslangicTarihiTxt]').datepicker("getDate");
            var rMin = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate());
            var rMax = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate() + 365);

            $('[id$=BitisTarihiTxt]').datepicker("option", "minDate", rMin);
            $('[id$=BitisTarihiTxt]').datepicker("option", "maxDate", rMax);

            var bitis = new Date($('#BitisTarihiTxt').datepicker('getDate'));
            if (bitis < rMin)
                bitis.setDate(rMin);
            else if (bitis > rMax)
                bitis.setDate(rMax);

        });
    <%-- İlan iptal Tarihi --%>

        $("#IptalTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true,
            inline: true,
            altField: "#IptalTarihiTxt",
            beforeShow: function (input, inst) {
                var mindate = $('#IlanTarihiTxt').datepicker('getDate');
                $(this).datepicker('option', 'minDate', mindate);
                var newDate = new Date($('#IlanTarihiTxt').datepicker('getDate'));
                newDate.setDate(newDate.getDate() + 365);
                //$(this).datepicker('option', 'maxDate', newDate);
            },
            beforeShowDay: function (date) {
                $('#ui-datepicker-div').css('clip', 'auto');
                return [true, '', ''];
            }
        });
        $("#IlanTarihiTxt").datepicker({
            dateFormat: "dd.mm.yy",
            firstDay: 1,
            monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
            monthNamesShort: ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"],
            dayNames: ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"],
            dayNamesMin: ["Pz", "Pt", "Sl", "Çr", "Pr", "Cu", "Ct"],
            changeMonth: true,
            changeYear: true,
            inline: true,
            altField: "#IlanTarihiTxt",

        }).on("change", function () {

            var dateMin = $('[id$=IlanTarihiTxt]').datepicker("getDate");
            var rMin = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate());
            var rMax = new Date(dateMin.getFullYear(), dateMin.getMonth(), dateMin.getDate() + 365);

            $('[id$=IptalTarihiTxt]').datepicker("option", "minDate", rMin);
            $('[id$=IptalTarihiTxt]').datepicker("option", "maxDate", rMax);

            var bitis = new Date($('#IptalTarihiTxt').datepicker('getDate'));
            if (bitis < rMin)
                bitis.setDate(rMin);
            else if (bitis > rMax)
                bitis.setDate(rMax);

        });
    }
</script>
<div class="container">

    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Resmi Tatil Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="ResmiTatilIdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div id="ResmiTatilDiv" runat="server">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                    <ContentTemplate>
                        <div class="row alert-secondary m-2">
                            <div class="form-group col-4">
                                <label class="col-form-label" for="TatilTxt">Tatil</label>
                                <asp:TextBox ID="TatilTxt" runat="server" class="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group col-4">
                                <label class="col-form-label" for="BasSaatDDL">Geçerlilik</label>
                                <asp:DropDownList ID="GecerlilikDDL" runat="server" class="form-control" Style="height: AUTO;" AutoPostBack="True" OnSelectedIndexChanged="GecerlilikDDL_SelectedIndexChanged" />
                            </div>
                            <div class="form-group col-2">
                                <label class="col-form-label" for="IlanTarihiTxt">İlan Tarihi</label>
                                <asp:TextBox ID="IlanTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static" placeholder="Tarih Seçiniz"></asp:TextBox>
                            </div>
                            <div class="form-group col-2">
                                <label class="col-form-label" for="IptalTarihiTxt">İptal Tarihi</label>
                                <asp:TextBox ID="IptalTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static" placeholder="Tarih Seçiniz"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row alert-secondary m-2 pt-4">
                            <div class="form-group col-4">
                                <div class="form-group">
                                    <h4>Baslama Tarihi</h4>
                                </div>
                                <div id="GirilenYilGecerliDiv" runat="server" class="form-group col-6 p-0" style="display: block">
                                    <label class="col-form-label" for="BaslangicTarihiTxt">Gün.Ay.Yıl</label>
                                    <%--<input runat="server" type="text" id="BaslangicTarihiTxt" name="BaslangicTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />--%>
                                    <asp:TextBox ID="BaslangicTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static" placeholder="Tarih Seçiniz"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="BaslangicTarihiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                </div>
                                <div id="TumYillarGecerliDiv" runat="server" class="form-group" style="display: none">
                                    <div class="row">
                                        <div class="form-group col-3">
                                            <label class="col-form-label" for="GunTxt">Gün</label>
                                            <asp:TextBox ID="GunTxt" CssClass="form-control input-integer" runat="server" ClientIDMode="Static" placeholder="Gün" type="number" step="1" min="1" max="31"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="GunTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                        </div>
                                        <div class="form-group col-6">
                                            <label class="col-form-label" for="AyTxt">Ay</label>
                                            <%--<asp:TextBox ID="AyTxt" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="AySeciniz"></asp:TextBox>--%>
                                            <asp:DropDownList ID="BasAyDDL" runat="server" CssClass="form-control" style="height:auto" ></asp:DropDownList>
                                        </div>
                                        <div class="form-group col-3">
                                            <label class="col-form-label" for="YilTxt">Yıl</label>
                                            <asp:TextBox ID="YilTxt" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="YYYY" Enabled="False"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group col-2 border border-dark border-top-0 border-left-0 border-bottom-0">
                                <div class="form-group">
                                    <h4>...</h4>
                                </div>
                                <label class="col-form-label" for="BasSaatDDL">Saat</label>
                                <asp:DropDownList ID="BasSaatDDL" runat="server" CssClass="form-control" Style="height: AUTO;" />

                            </div>
                            <div class="form-group col-4">
                                <div class="form-group">
                                    <h4>Bitiş Tarihi</h4>
                                </div>
                                <div id="GirilenYilGecerliBitTarDiv" runat="server" class="form-group col-6 p-0" style="display: block">
                                    <label class="col-form-label" for="BitisTarihiTxt">Gün.Ay.Yıl</label>
                                    <asp:TextBox ID="BitisTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static" placeholder="Tarih Seçiniz"></asp:TextBox>
                                </div>
                                <div id="TumYillarGecerliBitTarDiv" runat="server" class="form-group" style="display: none">
                                    <div class="row">
                                        <div class="form-group col-3">
                                            <label class="col-form-label" for="GunTxt">Gün</label>
                                            <asp:TextBox ID="BitGunTxt" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="Gün" type="number" step="1" min="1" max="31"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="GunTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                        </div>
                                        <div class="form-group col-6">
                                            <label class="col-form-label" for="AyTxt">Ay</label>
                                            <%--<asp:TextBox ID="BitAyTxt" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="Ay Seçiniz"></asp:TextBox>--%>
                                            <asp:DropDownList ID="BitAyDDL" runat="server" CssClass="form-control" style="height:auto" ></asp:DropDownList>
                                        </div>
                                        <div class="form-group col-3">
                                            <label class="col-form-label" for="YilTxt">Yıl</label>
                                            <asp:TextBox ID="TextBox3" CssClass="form-control" runat="server" ClientIDMode="Static" placeholder="YYYY" Enabled="False"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="form-group col-2">
                                <div class="form-group">
                                    <h4>...</h4>
                                </div>
                                <label class="col-form-label" for="BitSaatDDL">Saat</label>
                                <asp:DropDownList ID="BitSaatDDL" runat="server" CssClass="form-control " Style="height: AUTO;" />
                            </div>

                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end" ID="ResmiTatilListesiBtn" runat="server" Text="Resmi Tatiller" CausesValidation="false" OnClick="ResmiTatilListesiBtn_Click" />
            <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" Visible="false"></asp:LinkButton>
            <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click" Visible="false"></asp:LinkButton>
            <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger ml-2" runat="server" Text="Resmi Tatili Sil" OnClick="DeleteBtn_Click" Visible="false"></asp:LinkButton>
        </div>
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
                            <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Tatil Kaydı Silinecek"></asp:Label></h3>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Söz konusu tarihlerde kullanılan izinler varsa Tatil kaydını sildiğiniz takdirde tutarsızlıklara yol açabilir.Geçerli İzni Silmeyi Onaylıyor musunuz?"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Tatil Kaydını Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                <asp:LinkButton CssClass="btn btn-danger" ID="GuncelleNowBtn" runat="server" CausesValidation="false" Text="Tatil Kaydını Güncelle" OnClientClick="{return true;};" OnClick="GuncelleNowBtn_Click" Visible="false" />
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
