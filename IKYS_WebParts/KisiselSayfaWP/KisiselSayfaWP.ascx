<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KisiselSayfaWP.ascx.cs" Inherits="IKYS_WebParts.KisiselSayfaWP.KisiselSayfaWP" %>
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

<div class="container-fluid">
    <div class="card shadow">
        <div class="card-header">
            <div class="row">
                <div class="col">
                    <h3>
                        <asp:Label ID="KisiselSayfaLbl" CssClass="col-form-label  btn-outline-primary" runat="server" Text="Kişisel Sayfa"></asp:Label>
                    </h3>
                </div>
                <div class="col-auto">
                    <img id="PersonelImg" class="img-thumbnail" src="../PersonelResimleri/_t/personel_jpg.jpg" runat="server" style="height:190px;width:140px;">
                </div>
                <div class="col-auto">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="card-body">
            <div class="card">
                <div class="bg-secondary text-center">
                    <a class="text-white " data-bs-toggle="collapse" data-bs-target="#KimlikDiv" aria-expanded="false" aria-controls="KimlikDiv" style="font-weight: bold">Kimlik Bilgileri</a>
                </div>
            </div>
            <div class="card">
                <div class="collapse" id="KimlikDiv">
                    <asp:Table ID="KimlikTable" runat="server" class="table table-bordered">
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR1H1" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H2" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H3" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H4" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR1H5" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H6" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H7" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR1H8" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR2H1" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H2" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H3" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H4" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR2H5" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H6" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H7" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR2H8" CssClass="" runat="server"></asp:TableCell>

                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR3H1" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H2" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H3" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H4" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="KimlikR3H5" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H6" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H7" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="KimlikR3H8" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
            </div>
            <div class="card">
                <div class="bg-secondary text-center">
                    <a class="text-white" data-bs-toggle="collapse" data-bs-target="#IsBilgileriDiv" aria-expanded="false" aria-controls="IsBilgileriDiv" style="font-weight: bold">İş Bilgileri</a>
                </div>
            </div>
            <div class="card">
                <div class="collapse" id="IsBilgileriDiv">
                    <asp:Table ID="IsTable" runat="server" class="table table-bordered">
                        <asp:TableRow>
                            <asp:TableCell ID="IsR1H1" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR1H2" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IsR1H3" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR1H4" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="IsR1H5" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR1H6" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IsR1H7" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR1H8" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="IsR1H9" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR1H10" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IsR2H1" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR2H2" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="IsR2H3" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR2H4" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IsR2H5" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR2H6" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ID="IsR2H7" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR2H8" CssClass="" runat="server"></asp:TableCell>
                            <asp:TableCell ID="IsR2H9" CssClass="" runat="server" Font-Bold="True"></asp:TableCell>
                            <asp:TableCell ID="IsR2H10" CssClass="" runat="server"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>

            </div>
            <div class="card">
                <div class="bg-secondary text-center">
                    <a class="text-white" data-bs-toggle="collapse" data-bs-target="#IletisimDiv" aria-expanded="false" aria-controls="IletisimDiv" style="font-weight: bold">İletisim Bilgileri</a>
                </div>
            </div>
            <div class="card">
                <div class="collapse" id="IletisimDiv">
                    <asp:Table ID="IletisimTable" runat="server" class="table table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="card">
                <div class="bg-secondary text-center">
                    <a class="text-white" data-bs-toggle="collapse" data-bs-target="#AileDiv" aria-expanded="false" aria-controls="AileDiv" style="font-weight: bold">Aile Bilgileri</a>
                </div>
            </div>
            <div class="card">
                <div class="collapse" id="AileDiv">
                    <asp:Table ID="AileTable" runat="server" class="table table-bordered">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="HeaderCell0">Sıra</asp:TableCell>
                            <asp:TableCell ID="HeaderCell1" CssClass="btn-default" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell2" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell3" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell4" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell5" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell6" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell7" Visible="false"></asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
            </div>
            <div class="card">
                <div class="bg-secondary text-center">
                    <a class="text-white" data-bs-toggle="collapse" data-bs-target="#IzinDiv" aria-expanded="true" aria-controls="IzinDiv" style="font-weight: bold">İzin Bilgileri</a>
                </div>
            </div>
            <div class="card">
                <div class="collapse" id="IzinDiv">
                    <div class="card">
                        <div class="card-body">
                            <%--<asp:Timer ID="Timer1" runat="server" Interval="20000" OnTick="Timer1_Tick"></asp:Timer>--%>
                            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-6">
                                            <div class="card">
                                                <div class="card-header">
                                                    <a style="font-weight: bold">Ücretli İzin Dönemleri</a>
                                                </div>
                                                <div class="card-body" style="overflow: auto; max-height: 400px;">
                                                    <asp:Table ID="UcretliIzinDonemleriTable" runat="server" class="table ">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                            <div class="card">
                                                <div class="card-header">
                                                    <a style="font-weight: bold">Kullanılan Ücretli İzinler</a>
                                                </div>
                                                <div class="card-body" style="overflow: auto; max-height: 400px;">
                                                    <asp:Table ID="UcretliIzinHareketTable" runat="server" class="table ">
                                                    </asp:Table>
                                                </div>
                                                <div class="card-footer">
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col-6">
                                            <div class="card">
                                                <div class="card-header">
                                                    <a style="font-weight: bold">Mazeret İzin Dönemleri</a>
                                                </div>
                                                <div class="card-body" style="overflow: auto; max-height: 400px;">
                                                    <asp:Table ID="MazeretIzinDonemleriTable" runat="server" class="table ">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                            <div class="card">
                                                <div class="card-header">
                                                    <a style="font-weight: bold">Kullanılan Mazeret İzinleri</a>
                                                </div>
                                                <div class="card-body" style="overflow: auto; max-height: 400px;">
                                                    <asp:Table ID="MazeretIzinHareketTable" runat="server" class="table ">
                                                    </asp:Table>
                                                </div>
                                                <div class="card-footer">
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <br>
                                    <div class="card">
                                        <div class="card-header">
                                            <a style="font-weight: bold">Kullanılan Diğer İzinler</a>
                                        </div>
                                         <div class="card-body" style="overflow: auto; max-height: 400px;">
                                             <asp:Table ID="DigerIzinlerTable" runat="server" class="table ">
                                            </asp:Table>
                                         </div>
                                    </div>
                                    <br>
                                    <div class="card">
                                        <div class="card-header">
                                            <a style="font-weight: bold">İzin Talepleri</a>
                                        </div>
                                        <div class="card-body" style="overflow: auto; max-height: 400px;">
                                            <asp:Table ID="IzinTalepTable" runat="server" class="table ">
                                                <asp:TableHeaderRow>
                                                    <asp:TableCell ID="IzinTalepH0" Font-Bold="True">Sıra</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH1" Font-Bold="True">İzin Tipi</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH2" Font-Bold="True">Başlangıç Tar.</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH3" Font-Bold="True">Bitiş Tar.</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH4" Font-Bold="True">Vekil</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH5" Font-Bold="True">Amir</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH6" Font-Bold="True">Onay</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH7" Font-Bold="True">Adres</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH8" Font-Bold="True">Onay Durumu</asp:TableCell>
                                                    <asp:TableCell ID="IzinTalepH9" Font-Bold="True">Sil</asp:TableCell>
                                                </asp:TableHeaderRow>
                                            </asp:Table>
                                        </div>
                                        <div class="card-footer">
                                        </div>

                                    </div>
                                    <br>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="PersonelListesiBtn" runat="server" Text="Personel Listesi" CausesValidation="false" OnClick="PersonelListesiBtn_Click" />
        </div>
    </div>
</div>

