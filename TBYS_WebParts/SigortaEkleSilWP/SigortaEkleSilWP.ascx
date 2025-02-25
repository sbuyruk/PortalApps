<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SigortaEkleSilWP.ascx.cs" Inherits="TBYS_WebParts.SigortaEkleSilWP.SigortaEkleSilWP" %>
<script type="text/javascript">

    //On Page Load.
    $(function () {
        SetDatePicker();
    });
    //ikinci tarih için
    function SetDatePicker() {
        $("[id$=SigortaBitTarTxt]").datepicker({
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
                var maxSure = 365;//en fazla 1 yıl ücretsiz izin
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

</script>
<div class="container shadow w-75">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Sigortaları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Table ID="SigortaTable" runat="server" class="table table-hover table-sm table-bordered">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="HeaderCell0" CssClass="fw-bold">Sıra</asp:TableCell>
                            <asp:TableCell ID="HeaderCell1" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell2" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell3" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell4" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell5" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell6" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell7" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell8" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell9" CssClass="fw-bold" Visible="false">></asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                    <div class="alert-secondary float-bottom">
                        <div id="EkleDiv1" class="row">
                            <div class="form-group col-2">
                                <label for="SigortaCinsiDDL" class="col-form-label">Sigorta Cinsi</label>
                                <div>
                                    <asp:DropDownList ID="SigortaCinsiDDL" runat="server" class="form-control" Height="34px"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="SigortaCinsiDDL" class="col-form-label">Bağ.Bölüm</label>
                                <div>
                                    <asp:DropDownList ID="BagimsizBolumDDL" runat="server" class="form-control " Height="34px" />
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="SigortaBasTarTxt" class="col-form-label">Başlama Tarihi</label>
                                <div>
                                    <input runat="server" type="text" id="SigortaBasTarTxt" name="OnayTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="SigortaBitTarTxt" class="col-form-label">Bitiş Tarihi</label>
                                <div>
                                    <input runat="server" type="text" id="SigortaBitTarTxt" name="OnayTarihiTxt" class="form-control " readonly="readonly" />
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="SigortaBedeliTxt" class="col-form-label">Sigorta Bed.</label>
                                <div>
                                    <asp:TextBox ID="SigortaBedeliTxt" runat="server" class="form-control input-money text-end"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="PrimTxt" class="col-form-label">Prim</label>
                                <div>
                                    <asp:TextBox ID="PrimTxt" runat="server" class="form-control input-money text-end"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div id="EkleDiv2" class="row  ">
                            <div class="form-group col-2">
                                <label for="AdresKoduTxt" class="col-form-label">Adres Kodu</label>
                                <div>
                                    <asp:TextBox ID="AdresKoduTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="PoliceNoTxt" class="col-form-label">Poliçe No</label>
                                <div>
                                    <asp:TextBox ID="PoliceNoTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="DaskPoliceNoTxt" class="col-form-label">Dask Pol.No</label>
                                <div>
                                    <asp:TextBox ID="DaskPoliceNoTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="YapiTarziTxt" class="col-form-label">Yapı Tarzı</label>
                                <div>
                                    <asp:TextBox ID="YapiTarziTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="InsaYiliTxt" class="col-form-label">İnşa Yılı</label>
                                <div>
                                    <asp:TextBox ID="InsaYiliTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="BulunduguKatTxt" class="col-form-label">Bulunduğu Kat</label>
                                <div>
                                    <asp:TextBox ID="BulunduguKatTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>


                        </div>
                        <div class="form-group float-end">
                            <asp:LinkButton ID="SigortaEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Sigorta Ekle" OnClick="SigortaEkleBtn_Click" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="TasinmazBtn" runat="server" Text="Taşınmaza Git" CausesValidation="false" OnClick="TasinmazBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="TasinmazListesiBtn" runat="server" Text="Taşınmaz Listesi" CausesValidation="false" OnClick="TasinmazListesiBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="SigortaListesiBtn" runat="server" Text="Sigorta Listesi" CausesValidation="false" OnClick="SigortaListesiBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
        </div>
    </div>
</div>
