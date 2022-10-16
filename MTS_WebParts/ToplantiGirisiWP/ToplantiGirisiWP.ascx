<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ToplantiGirisiWP.ascx.cs" Inherits="MTS_WebParts.ToplantiGirisiWP.ToplantiGirisiWP" %>

<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }
    /*Dolu olan toplantı saatleri*/
    .disabled-item {
        background-color: rgba(206, 220, 229, 0.40) !important;
        color: darkgrey !important;
    }
    .warning-item {
        background-color: red !important;
        color: white !important;
    }
    /*Katılımcı listesinde hücre yüksekliğini ayarlamak için*/
    table.dataTable tbody th, table.dataTable tbody td {
        padding-bottom: 0px; /* e.g. change 8x to 4px here */
    }

    /*div.dataTables_wrapper { min-height: 360px; }*/
</style>
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
                newDate.setDate(newDate.getDate() + 15);
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
<%-- Katılımcı Seçimi Modal --%>
<script type="text/javascript">
    function OpenSilModal() {
        $("#ModalSilDiv").modal({ backdrop: true });
    }
    function KatilimciSecimiModal() {
        $("#KatilimciSecimiModal").modal({ backdrop: false });
    }


    function KatilimciSecildiBtnClick(thisRow, katilimciId, bilgi) {


        var table = $('#CustomModalDataTable').DataTable();
        var pnum = table.page.info().page;

        var indexes = table.row(thisRow).index();
        table.rows(indexes).remove().draw();


        table.page(pnum).draw(false);

        document.getElementById('<%= paramToplantiKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramBilgiLbl.ClientID%>').value = bilgi;
        document.getElementById('<%= KatilimciEkleBtn.ClientID%>').click();
    }
    function KatilimciCikarBtnClick(katilimciId) {
        document.getElementById('<%= paramToplantiKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= KatilimciCikarBtn.ClientID%>').click();
    }
</script>

<div class="col-xl ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-primary font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Toplantı Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body alert-info">
            <div class="row">
                <div class="col-4 ml-2 border border-dark p-2">
                    <div class="form-group">
                        <asp:Label CssClass="col-form-label" runat="server" Text="Toplantı Konusu"></asp:Label>
                        <asp:TextBox ID="ToplantiKonusuTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ToplantiKonusuTxt" ForeColor="Red" ErrorMessage="Toplantı Konusu Giriniz"> </asp:RequiredFieldValidator>
                    </div>
                    <div class="row">
                        <div class="form-group col ">
                            <asp:Label CssClass="col-from-label" runat="server" Text="Başlangıç Tarihi"></asp:Label>
                            <asp:TextBox ID="BaslangicTarihiTxt" CssClass="form-control input-date" runat="server" ClientIDMode="Static" OnTextChanged="BaslangicTarihiTxt_TextChanged" AutoPostBack="True"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BaslangicTarihiTxt" ForeColor="Red" ErrorMessage="Başlama Tarihi Seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col">
                            <asp:Label CssClass="col-from-label" runat="server" Text="Bitiş Tarihi"></asp:Label>
                            <asp:TextBox ID="BitisTarihiTxt" CssClass="form-control input-date " runat="server" ClientIDMode="Static" OnTextChanged="BitisTarihiTxt_TextChanged" AutoPostBack="True" ></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="BitisTarihiTxt" ForeColor="Red" ErrorMessage="Bitiş Tarihi Seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>
                            <div class="row">
                                <div class="form-group col">
                                    <asp:Label runat="server" CssClass="col-form-label" Text="Başlangıç Saati"></asp:Label>
                                    <asp:DropDownList ID="BasSaatDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="BasSaatDDL_SelectedIndexChanged" AutoPostBack="true"  style="height:auto" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="BasSaatDDL" ForeColor="Red" ErrorMessage="Başlama Saati Seçiniz"> </asp:RequiredFieldValidator>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-form-label" runat="server" Text="Bitiş Saati"></asp:Label>
                                    <asp:DropDownList ID="BitSaatDDL" runat="server" CssClass="form-control "  style="height:auto" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="BitSaatDDL" ForeColor="Red" ErrorMessage="Bitiş Saati Seçiniz"> </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="BaslangicTarihiTxt" EventName="TextChanged" />
                            <asp:AsyncPostBackTrigger ControlID="BitisTarihiTxt" EventName="TextChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ToplantiYeriDDL" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <div class="form-group">
                        <asp:Label CssClass="col-form-label" runat="server" Text="Açıklama / Toplantı Gündemi"></asp:Label>
                        <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="11"></asp:TextBox>
                    </div>
                </div>
                <div class="col ml-2 mr-2 border border-dark p-2">
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                        <ContentTemplate>

                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" Text="Toplantı Yeri"></asp:Label>
                                <asp:DropDownList ID="ToplantiYeriDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="ToplantiYeriDDL_SelectedIndexChanged" AutoPostBack="True" style="height:auto" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ToplantiYeriDDL" ForeColor="Red" ErrorMessage="Toplantı Yeri seçiniz"> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group" id="ToplantiYeriDigerDiv" runat="server" style="display: none">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Toplantı Yeri (Diğer)"></asp:Label>
                                <input id="ToplantiYeriDigerTxt" class="form-control" runat="server" placeholder="Toplantı Yeri giriniz..." text="">

                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="form-group">
                        <div class="checkbox pt-3">
                            <label>
                                <asp:CheckBox ID="CevrimIciTplantiChk" runat="server" Checked="false" ToolTip="Çevrim içi (Video Konferans şeklinde) yapılacak toplantılar için işaretleyiniz." />
                                Çevrim içi Toplantı 
                            </label>
                            <label>(Teknik destek gerekiyorsa işaretlenmelidir.)</label>
                        </div>
                        <div class="checkbox pt-3">
                            <label>
                                <asp:CheckBox ID="IkramOnayiChk" runat="server" Checked="false" ToolTip="İkram Onayı alındı ise işaretleyiniz ve İkram malzemelerini giriniz." />
                                İkram Onayı
                            </label>
                            <label>(İkram Onayı alındı ise işaretlenmelidir.)</label>
                            <asp:TextBox ID="IkramMalzemesiTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="4" placeholder="Onaylanan İkram Malzemelerini giriniz."></asp:TextBox>
                        </div>
                    </div>

                </div>
                <div class="col-5 ml-2 mr-2 border border-dark p-2">

                    <div class="card" id="KatilimciBilgileriDiv" runat="server" style="display: block">
                        <div class="card-header">
                            <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="İç Katılımcılar"></asp:Label>
                            <asp:LinkButton ID="KatilimciModalAcBtn" runat="server" CssClass="btn btn-outline-success ml-5" Text="Katılımcı Ekle" OnClick="KatilimciModalAcBtn_Click" CausesValidation="false" />
                        </div>
                        <div class="card-body" style="min-height: 360px;">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                                <ContentTemplate>
                                    <div class="form-group">
                                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>Sıra</th>
                                                    <th>Protokol Sıra No</th>
                                                    <th>Adı Soyadı</th>
                                                    <th>Bilgi / Katılımcı</th>
                                                    <th>Çıkar</th>
                                                </tr>
                                            </thead>
                                        </table>
                                    </div>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="KatilimciCikarBtn" EventName="click" />
                                    <asp:AsyncPostBackTrigger ControlID="KatilimciEkleBtn" EventName="click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                        <div class="card-footer">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label font-weight-bold" runat="server" Text="Dış Katılımcılar"></asp:Label>
                                <asp:TextBox ID="DisKatilimcilarTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="4" placeholder="Vakıf dışı katılımcıları yazınız..."></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Toplantı Kaydet" OnClick="KaydetBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="ToplantiSilBtn" CssClass="btn btn-outline-danger ml-5" runat="server" Text="Toplantı Sil" OnClick="ToplantiSilBtn_Click"></asp:LinkButton>
            <asp:LinkButton ID="ToplantiTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" Text="Toplantı Takvimi" OnClick="ToplantiTakvimiBtn_Click" CausesValidation="False"></asp:LinkButton>
            <asp:LinkButton ID="ToplantiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Toplantı Listesi" OnClick="ToplantiListesiBtn_Click" CausesValidation="False"></asp:LinkButton>
            <asp:LinkButton ID="ToplantiKatilimTutanagiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Toplantı Katılım Tutanagi" OnClick="ToplantiKatilimTutanagiBtn_Click" CausesValidation="False"></asp:LinkButton>
        </div>
    </div>
    <div id="KatilimciHiddenDiv" style="display: none">
        <%--<input id="paramToplantiIdLbl" runat="server" type="text" />--%>
        <input id="paramToplantiKatilimciIdLbl" runat="server" type="text" />
        <input id="paramBilgiLbl" runat="server" type="text" />
        <asp:LinkButton ID="KatilimciEkleBtn" runat="server" CausesValidation="false" Text="Toplantıya Ekle" OnClientClick="{return true;};" OnClick="KatilimciEkleBtn_Click" />
        <asp:LinkButton ID="KatilimciCikarBtn" runat="server" CssClass="btn btn-outline-danger" CausesValidation="false" Text="Toplantıdan Çıkar" OnClick="KatilimciCikarBtn_Click" />
    </div>
</div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="ModalSilDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="SilModalBaslikLbl" runat="server" Text="Toplantı Silinecek"></asp:Label>
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
                        <asp:LinkButton ID="ToplantiSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Toplantıyı Sil" OnClick="ToplantiSilNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ToplantiSilBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="GuncelleBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
<asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal " id="KatilimciSecimiModal" role="dialog">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-body ">

                        <div class="card">
                            <div class="card-header text-danger">
                                <h3 class="col-form-label font-weight-bold" id="KatilimciSecimiHeaderLbl" runat="server">Katılımcı Seçimi
                                </h3>
                            </div>
                            <div class="card-body">
                                <div class="card-body p-0" id="Div1" runat="server">
                                    <div class="form-group">
                                        <table id="CustomModalDataTable" class="table table-striped table-bordered" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>Sıra No</th>
                                                    <th>P.No</th>
                                                    <th>Adı Soyadı</th>
                                                    <th>Toplantıya Ekle</th>
                                                    <th>Bilgiye Ekle</th>
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
        <asp:AsyncPostBackTrigger ControlID="KatilimciModalAcBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
