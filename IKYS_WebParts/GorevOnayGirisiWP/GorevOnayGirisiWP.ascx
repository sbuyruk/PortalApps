<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevOnayGirisiWP.ascx.cs" Inherits="IKYS_WebParts.GorevOnayGirisiWP.GorevOnayGirisiWP" %>
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
    };
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
<div class="container ">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label  text-success" ID="TitleLbl" runat="server" Text="Görev Onayı Girişi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="GorevOnayIdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="PersonelAdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div >
                        <div class="form-group p-1" id="PersonelDiv" runat="server">
                            <label class="col-form-label p-1" for="PersonelDDL">Personel Seçimi</label>
                            <asp:DropDownList ID="PersonelDDL" runat="server" CssClass="form-control col-3" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" AutoPostBack="true" style="Height:auto" />
                        </div>
                    </div>
                    <div class="alert-secondary p-2">
                        <div class="row">
                            <div class="col-4">
                                <div class="form-group">
                                    <label class="col-form-label" for="GorevinSebebiTxt">Görevin Sebebi</label>
                                    <asp:TextBox ID="GorevinSebebiTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label" for="GorevinYeriTxt">Görevin Yeri</label>
                                    <asp:TextBox ID="GorevinYeriTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-form-label" for="BasTarTxt">Başlangıç Tarihi</label>
                                        <input runat="server" type="text" id="BasTarTxt" name="BasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="BasTarTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group" id="BasSaatDiv">
                                        <label class="col-form-label" for="BasSaatDDL">Saat</label>
                                        <asp:DropDownList ID="BasSaatDDL" runat="server" CssClass="form-control " style="Height:auto" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group" id="BitTarDiv" runat="server">
                                        <label class="col-form-label" for="BitTarTxt">Bitiş Tarihi</label>
                                        <input runat="server" type="text" id="BitTarTxt" name="BitTarTxt" class="form-control DatePicker2" readonly="readonly" />
                                    </div>
                                    <div class="form-group" id="BitSaatDiv" runat="server">
                                        <label class="col-form-label" for="BitSaatDDL">Saat</label>
                                        <asp:DropDownList ID="BitSaatDDL" runat="server" CssClass="form-control" style="Height:auto"  />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-form-label" for="PerSubeImzaDDL">Per.Ş.Md</label>
                                        <asp:DropDownList ID="PerSubeImzaDDL" runat="server" CssClass="form-control" style="Height:auto"  />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="PersubeVekilChk">Vekil</label>
                                        <asp:CheckBox ID="PersubeVekilChk" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-form-label" for="OnayImzaDDL">İmzalayan</label>
                                        <asp:DropDownList ID="OnayImzaDDL" runat="server" CssClass="form-control"  style="Height:auto" />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="OnayVekilChk">Vekil</label>
                                        <asp:CheckBox ID="OnayVekilChk" runat="server" CssClass="form-control" />
                                    </div>
                                    
                                </div>
                                <div class="row">
                                    <div class="form-group">
                                        <label class="col-form-label" for="GMImzaDDL">Genel Müdür</label>
                                        <asp:DropDownList ID="GMImzaDDL" runat="server" CssClass="form-control"  style="Height:auto" />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="GMVekilChk">Vekil</label>
                                        <asp:CheckBox ID="GMVekilChk" runat="server" CssClass="form-control" />
                                    </div>
                                    
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="row">
                                    <div class="form-group col-6">
                                        <label class="col-form-label" for="SureTxt">Süre</label>
                                        <asp:TextBox ID="SureTxt" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="form-group col-6">
                                        </br>
                                        <asp:Button ID="SureHesaplaBtn" CssClass="btn btn-secondary" runat="server" Text="Hesapla" OnClick="SureHesaplaBtn_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group col-6">
                                        <label class="col-form-label" for="AvansTxt">Avans</label>
                                        <asp:TextBox ID="AvansTxt" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group col-6">
                                        <label class="col-form-label" for="YevmiyeTxt">Yevmiye</label>
                                        <asp:TextBox ID="YevmiyeTxt" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="form-group col-6">
                                        <label class="col-form-label" for="ParaBirimiDDL">Para Birimi</label>
                                        <asp:DropDownList ID="ParaBirimiDDL" runat="server" CssClass="form-control" style="Height:auto"  />
                                    </div>
                                </div>
                                <div class="row">

                                    <div class="form-group col">
                                    <label class="col-form-label" for="UlasimAraciDDL">Ulaşım Aracı</label>
                                    <asp:DropDownList ID="UlasimAraciDDL" runat="server" CssClass="form-control"  style="Height:auto" />
                                    </div>
                                    <div class="form-group col">
                                        <label class="col-form-label" for="AracPlakasiTxt">Araç Plakasi</label>
                                        <asp:TextBox ID="AracPlakasiTxt" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group col">
                                        <label class="col-form-label" for="OnayMakamDDL">İmza Makamı</label>
                                        <asp:DropDownList ID="OnayMakamDDL" runat="server" CssClass="form-control" style="Height:auto" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
            </div>
                <div class="card-footer">
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" Visible="false" />

                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" Visible="false" />
                    <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger ml-2" runat="server" Text="Sil" OnClick="DeleteBtn_Click" />

                    <asp:LinkButton ID="RaporAlBtn" CssClass="btn btn-outline-success float-end" runat="server" Text="Rapor Al" OnClick="RaporAlBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="GorevOnayListesiBtn" runat="server" Text="Görev Onay Listesi" CausesValidation="false" OnClick="GorevOnayListesiBtn_Click" Visible="False" />
                </div>
            </div>
            <div class="card shadow">
                <div class="card-header">
                    <h2 class="col-form-label font-weight-bold">Görev/Onay Listesi</h2>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>Adı Soyadı</th>
                                    <th>Görevin Sebebi</th>
                                    <th>Gidiş Tarihi</th>
                                    <th>Dönüş Tarihi</th>
                                    <th>Görevin Yeri</th>
                                </tr>
                            </thead>
                        </table>
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
                                        <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Görev Onayı Silinecek"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Geçerli Görev Onayını Silmek İstiyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Görev Onayı Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>