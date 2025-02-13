<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IzinDonemListesiWP.ascx.cs" Inherits="IKYS_WebParts.IzinDonemListesiWP.IzinDonemListesiWP" %>
<script>
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>
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
                //var maxSure = 10;//en fazla 10 gün

                //var newDate = new Date($('.DateTimePickerV1').datepicker('getDate'));
                //newDate.setDate(newDate.getDate() + maxSure);
                //$(this).datepicker('option', 'maxDate', newDate);
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
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3>
                <asp:Label CssClass="col-form-label  btn-outline-primary" runat="server" Text="İzin Dönemleri Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="22" runat="server" ></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                <ContentTemplate>
                    <div class="card-body p-0">
                        <div class="input-group">
                            <div class="form-group" >
                                <label class="col-form-label" for="PersonelDDL">Personel </label>
                                <asp:DropDownList ID="PersonelDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" />
                            </div>
                        </div>
                        <div class="form-group float-end">
                            <asp:LinkButton CssClass="btn btn-success" ID="DonemEkleBtn" runat="server" CausesValidation="false" Text="Toplu Eski Dönem Ekle" OnClientClick="{return true;};" OnClick="DonemEkleBtn_Click" />
                        </div>
                        <div class="table loader">
                            <asp:Table ID="IzinDonemTable" runat="server" CssClass="table table-striped table-bordered">
                            </asp:Table>
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
                                                <asp:Label ID="ModalSubTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                                <asp:Label ID="IzinDonemIdLbl" CssClass="col-form-label text-secondary" runat="server" Text=""></asp:Label>
                                            </h4>
                                        </div>
                                        <div class="card-body ">
                                            <div class="form-group row " Id="BasBitTarDiv" runat="server" style="display: block;">
                                                <div class="form-group col-3">
                                                    <label class="col-form-label" for="DonemBasTarTxt">Dönem Başı</label>
                                                    <input runat="server" type="text" id="DonemBasTarTxt" name="DonemBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                </div>
                                                <div class="form-group col-3">
                                                    <label class="col-form-label" for="DonemBitTarTxt">Dönem Sonu</label>
                                                    <input runat="server" type="text" id="DonemBitTarTxt" name="DonemBitTarTxt" class="form-control DatePicker2" readonly="readonly" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label class="col-form-label" ID="AciklamaLbl" runat="server" for="AciklamaTxt">Açıklama</asp:Label>
                                                    <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <div class="form-group col">
                                                    <label class="col-form-label" for="IzinHakkiTxt">İzin Hakkı</label>
                                                    <asp:TextBox ID="IzinHakkiTxt" runat="server" CssClass="form-control" />
                                                </div>
                                                <div class="form-group col">
                                                    <label class="col-form-label" for="KullanilanIzinTxt">Kullanılan İzin</label>
                                                    <asp:TextBox ID="KullanilanIzinTxt" runat="server" CssClass="form-control" />
                                                </div>
                                                <div class="form-group col">
                                                    <label class="col-form-label" for="KalanIzinTxt">Kalan İzin</label>
                                                    <asp:TextBox ID="KalanIzinTxt" runat="server" CssClass="form-control" />
                                                </div>
                                            </div>
                                            
                                            <div class="table loader">
                                                <asp:Table ID="IzinHareketTable" runat="server" CssClass="table table-striped table-bordered">
                                                </asp:Table>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="UyariMesajiLbl" CssClass="form-control text-danger" runat="server" Text="İzin dönemi bilgilerinde yapılan değişiklikler veri tutarsızlıklarına yolaçabilir. Lütfen değişikliği kaydetmeden önce dikkatle kontrol ediniz."></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <asp:LinkButton CssClass="btn btn-primary" ID="UpdateNowBtn" runat="server" CausesValidation="false" Text="Güncelle" OnClientClick="{return true;};" OnClick="UpdateNowBtn_Click" />
                                    <asp:LinkButton CssClass="btn btn-success" ID="DonemEkleNowBtn" runat="server" CausesValidation="false" Text="Dönem Ekle" OnClientClick="{return true;};" OnClick="DonemEkleNowBtn_Click" />
                                    <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
        </div>

    </div>
    <asp:UpdateProgress ID="updateProgress" runat="server">
        <ProgressTemplate>
            <div class='loaderMainContainer'>
                <div class='loaderContainer'>
                    <div class='loaderCircle'></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</div>
