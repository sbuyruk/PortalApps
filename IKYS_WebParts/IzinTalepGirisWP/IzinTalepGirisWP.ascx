<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IzinTalepGirisWP.ascx.cs" Inherits="IKYS_WebParts.IzinTalepGirisWP.IzinTalepGirisWP" %>

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
                var izinTipi = document.getElementById('<%= IzinTanimDDL.ClientID%>').value;
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
    function TarihKontrolu() {
        document.getElementById('<%= TarihKontrolBtn.ClientID%>').click();
    }
</script>
<div class="container ">
    <div style="display: none">
        <asp:Timer ID="Timer1" runat="server" Interval="10000" OnTick="Timer1_Tick">
        </asp:Timer>
    </div>
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-success fw-bold mb-1" ID="TitleLbl" runat="server" Text="İzin Talebi Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IzinTalepIdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="PersonelAdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="form-group col" id="PersonelDiv" runat="server" style="display: block;">
                    <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control col-6" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" style="height:auto" />
                </div>
                <div class="form-group col checkbox" id="EPostaDiv" runat="server" style="display: block;" >
                    <label>
                        <asp:CheckBox ID="EPostaGonderChk" runat="server" Checked="true" ToolTip="E-Posta Gönderilecekse İşaretlenmelidir." />
                        E-Posta Gönderilsin
                    </label>
                </div>

            </div>
            <div class="alert-secondary p-2" id="IzinTalepDiv" runat="server">
                <div style="display: none">
                    <asp:LinkButton ID="TarihKontrolBtn" runat="server" CausesValidation="false" OnClick="TarihKontrolBtn_Click" />
                </div>
                <div class="row">
                    <div class="col-8">
                        <div class="row">
                            <div id="IzinTipiDDLDiv" class="form-group col" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinTipiDDL">İzin Tipi</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="IzinTanimDDL" runat="server" class="form-control" OnSelectedIndexChanged="IzinTanimDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto" />
                            </div>
                            <div id="IzinTipiLblDiv" class="form-group col" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinTipiLbl">İzin Tipi</label>
                                <asp:TextBox ID="IzinTipiLbl" runat="server" class="form-control" ReadOnly="true" />
                            </div>
                            <div class="form-group col">
                                <label class="col-form-label" for="IzinBasTarTxt">Başlangıç Tarihi</label>
                                <input runat="server" type="text" id="IzinBasTarTxt" name="IzinBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly"  onchange="TarihKontrolu()" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="IzinBasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col" id="IzinBasSaatDiv" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinBasSaatDDL">Başlangıç Saati</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="IzinBasSaatDDL" runat="server" class="form-control " OnSelectedIndexChanged="IzinBasSaatDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto" />
                            </div>
                            <div class="form-group col" id="IzinBitTarDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="IzinBitTarTxt">Bitiş Tarihi</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinBitTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <input runat="server" type="text" id="IzinBitTarTxt" name="IzinBitTarTxt" class="form-control DatePicker2" readonly="readonly" onchange="TarihKontrolu()" />
                            </div>
                            <div class="form-group col" id="IzinBitSaatDiv" runat="server" style="display: none">
                                <label class="col-form-label" for="IzinBitSaatDDL">Bitiş Saati</label>
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IzinTanimDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="IzinBitSaatDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="IzinBitSaatDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto" />
                            </div>

                        </div>
                        <div class="row">
                            <div class="form-group col-4" id="VekilDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="VekilImzaDDL">Vekil</label>
                                <%--<asp:RequiredFieldValidator ID="VekilImzaDDLRequiredFieldValidator"  runat="server" ControlToValidate="VekilImzaDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                <asp:DropDownList ID="VekilImzaDDL" runat="server" class="form-control" style="height:auto" />
                            </div>
                            <div class="form-group col-4" id="AmirDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="AmirImzaDDL">Amir</label>
                                <asp:DropDownList ID="AmirImzaDDL" runat="server" class="form-control" style="height:auto" />
                                <%--<asp:RequiredFieldValidator ID="AmirImzaDDLRequiredFieldValidator"  runat="server" ControlToValidate="VekilImzaDDL" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                            </div>
                            <%-- <div class="form-group col-4" id="OnayDiv" runat="server" style="display: block;">
                                <label class="col-form-label" for="OnayImzaDDL">Onay</label>
                                <asp:DropDownList ID="OnayImzaDDL" runat="server" class="form-control" style="height:auto" />
                            </div>--%>
                        </div>
                        <div class="row">
                            <div class="form-group col">
                                <label class="col-form-label" id="AdresLbl" runat="server" for="AdresTxt">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="AdresTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group col" id="AciklamaDiv" runat="server" style="display: block">
                                <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                            </div>
                        </div>                        
                    </div>
                    <div class="col-4">
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label" ID="IzinSuresiLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label " ID="KullanilanIzinLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <hr />
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label" ID="GecmisDonemlerdenKalanIznLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <hr />
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label fw-bold" ID="KalanIzinLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label" ID="UyariLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label" ID="KullanilmayanLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
                
                <div class="form-group">
                    <asp:LinkButton CssClass="btn btn-outline-primary" ID="MazereteMahsupDilekceBtn" runat="server" Text="Mahsup Dilekçesi" CausesValidation="false" OnClick="MazereteMahsupDilekceBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-outline-primary" ID="UcretliIzinDilekceBtn" runat="server" Text="Ücretli İzin Dilekçesi" OnClick="UcretliIzinDilekceBtn_Click" Visible="false" />
                </div>
            </div>

        </div>
        <div class="card-footer">
            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-success float-left" runat="server" Text="İzin Talebini Gönder" OnClick="SaveBtn_Click" Visible="false" />
            <asp:LinkButton CssClass="btn btn-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
            <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-primary float-left" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" Visible="false" />
        </div>
    </div>
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
        <ContentTemplate>
            <div class="card" Id="IzinTalepTableDiv" runat="server" style="display:none">
                <div class="card-header">
                    <a style="font-weight: bold">Son İzin Talepleri</a>
                </div>
                <div class="card-body" style="overflow: auto; max-height: 400px;">
                    <asp:Table ID="IzinTalepTable" runat="server" class="table ">
                        <asp:TableHeaderRow>
                            <asp:TableCell Font-Bold="True">Sıra</asp:TableCell>
                            <asp:TableCell Font-Bold="True">İzin Tipi</asp:TableCell>
                            <asp:TableCell Font-Bold="True">Başlangıç Tar.</asp:TableCell>
                            <asp:TableCell Font-Bold="True">Bitiş Tar.</asp:TableCell>
                            <asp:TableCell Font-Bold="True">Süre</asp:TableCell>
                            <asp:TableCell Font-Bold="True">Onay Durumu</asp:TableCell>
                            <asp:TableCell Font-Bold="True"></asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableRow>
                            <asp:TableCell ID="SiraR1Cell"></asp:TableCell>
                            <asp:TableCell ID="IzinTipiR1Cell"></asp:TableCell>
                            <asp:TableCell ID="BasTarR1Cell"></asp:TableCell>
                            <asp:TableCell ID="BitTarR1Cell"></asp:TableCell>
                            <asp:TableCell ID="SureR1Cell"></asp:TableCell>
                            <asp:TableCell ID="OnayDurumuR1Cell"></asp:TableCell>
                            <asp:TableCell ID="ButtonR1Cell">
                                <asp:HyperLink ID="YazdirLnk1" runat="server" CssClass="btn btn-outline-primary"></asp:HyperLink>
                                <asp:LinkButton ID="SilBtn1" CssClass="btn btn-danger" runat="server" Text="Talebi Sil" CausesValidation="false" Visible="false" OnClick="SilBtn1_Click" />
                                <asp:HiddenField ID="IzinTalepIdHdn1" runat="server" />
                            </asp:TableCell></asp:TableRow><asp:TableRow>
                            <asp:TableCell ID="SiraR2Cell"></asp:TableCell><asp:TableCell ID="IzinTipiR2Cell"></asp:TableCell><asp:TableCell ID="BasTarR2Cell"></asp:TableCell><asp:TableCell ID="BitTarR2Cell"></asp:TableCell><asp:TableCell ID="SureR2Cell"></asp:TableCell><asp:TableCell ID="OnayDurumuR2Cell"></asp:TableCell><asp:TableCell ID="ButtonR2Cell">
                                <asp:HyperLink ID="YazdirLnk2" runat="server"></asp:HyperLink>
                                <asp:LinkButton ID="SilBtn2" CssClass="btn btn-danger" runat="server" Text="Talebi Sil" CausesValidation="false" Visible="false" OnClick="SilBtn2_Click" />
                                <asp:HiddenField ID="IzinTalepIdHdn2" runat="server" />
                            </asp:TableCell></asp:TableRow><asp:TableRow>
                            <asp:TableCell ID="SiraR3Cell"></asp:TableCell><asp:TableCell ID="IzinTipiR3Cell"></asp:TableCell><asp:TableCell ID="BasTarR3Cell"></asp:TableCell><asp:TableCell ID="BitTarR3Cell"></asp:TableCell><asp:TableCell ID="SureR3Cell"></asp:TableCell><asp:TableCell ID="OnayDurumuR3Cell"></asp:TableCell><asp:TableCell ID="ButtonR3Cell">
                                <asp:HyperLink ID="YazdirLnk3" runat="server"></asp:HyperLink>
                                <asp:LinkButton ID="SilBtn3" CssClass="btn btn-danger" runat="server" Text="Talebi Sil" CausesValidation="false" Visible="false" OnClick="SilBtn3_Click" />
                                <asp:HiddenField ID="IzinTalepIdHdn3" runat="server" />                            
                            </asp:TableCell></asp:TableRow><asp:TableRow>
                            <asp:TableCell ID="SiraR4Cell"></asp:TableCell><asp:TableCell ID="IzinTipiR4Cell"></asp:TableCell><asp:TableCell ID="BasTarR4Cell"></asp:TableCell><asp:TableCell ID="BitTarR4Cell"></asp:TableCell><asp:TableCell ID="SureR4Cell"></asp:TableCell><asp:TableCell ID="OnayDurumuR4Cell"></asp:TableCell><asp:TableCell ID="ButtonR4Cell">
                                <asp:HyperLink ID="YazdirLnk4" runat="server"></asp:HyperLink>
                                <asp:LinkButton ID="SilBtn4" CssClass="btn btn-danger" runat="server" Text="Talebi Sil" CausesValidation="false" Visible="false" OnClick="SilBtn4_Click" />
                                <asp:HiddenField ID="IzinTalepIdHdn4" runat="server" />
                            </asp:TableCell></asp:TableRow><asp:TableRow>
                            <asp:TableCell ID="SiraR5Cell"></asp:TableCell><asp:TableCell ID="IzinTipiR5Cell"></asp:TableCell><asp:TableCell ID="BasTarR5Cell"></asp:TableCell><asp:TableCell ID="BitTarR5Cell"></asp:TableCell><asp:TableCell ID="SureR5Cell"></asp:TableCell><asp:TableCell ID="OnayDurumuR5Cell"></asp:TableCell><asp:TableCell ID="ButtonR5Cell">
                                <asp:HyperLink ID="YazdirLnk5" runat="server"></asp:HyperLink>
                                <asp:LinkButton ID="SilBtn5" CssClass="btn btn-danger" runat="server" Text="Talebi Sil" CausesValidation="false" Visible="false" OnClick="SilBtn5_Click" />
                                <asp:HiddenField ID="IzinTalepIdHdn5" runat="server" />                            
                            </asp:TableCell></asp:TableRow></asp:Table></div><div class="card-footer">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
        </Triggers>
    </asp:UpdatePanel>
</div>
