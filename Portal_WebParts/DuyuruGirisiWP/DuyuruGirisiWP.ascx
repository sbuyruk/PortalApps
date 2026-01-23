<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuyuruGirisiWP.ascx.cs" Inherits="Portal_WebParts.DuyuruGirisiWP.DuyuruGirisiWP" %>


<script type="text/javascript">
    function ClearDuyuruResmi() {
        $('#SecilenResmiSilBtn').on('click', function () {
            $('#DisplayImage').attr('src', '');
        });
    }
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
        var mindate = new Date();
        //$("[id$=YayinBasTarTxt]").datepicker('option', 'minDate', mindate);
        $("[id$=YayinBitTarTxt]").datepicker({
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
    function readURL(personelFU, sender) {
        if (personelFU.files && personelFU.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + sender).attr('src', e.target.result);

            }
            reader.readAsDataURL(personelFU.files[0]);
        }
    }

    //summernote editor
    $(document).ready(function () {
        if (!$('#MetinTxt').next().hasClass('note-editor')) {
            $('#MetinTxt').summernote({
                lang: 'tr-TR',
                height: 300,
                focus: true,
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['font', ['strikethrough', 'superscript', 'subscript']],
                    ['fontsize', ['fontsize']],
                    ['color', ['color']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['height', ['height']]
                ],
                popover: {
                    image: [], // image popover kapalı
                    link: [],  // link popover kapalı
                    air: []    // air (floating) popover kapalı
                }
            });
        }
    });

</script>


<div class="container shadow">
    <div class="card">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-primary fw-bold mb-1" ID="TitleLbl" runat="server" Text="Duyuru Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">

            <div class="row">
                <div class="col-7">
                    <div class="card">
                        <div class="card-header text-center p-0">

                                <label class="col-form-label fw-bold">Duyuru Bilgileri</label>

                        </div>

                        <div class="card-body">
                            <div class="row">
                                <div class="col-6">
                                    <div class="form-group">
                                        <asp:Label ID="Label1" runat="server" CssClass="col-form-label" Text="Duyuru Başlığı"></asp:Label>
                                        <asp:TextBox ID="BaslikTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label6" runat="server" CssClass="col-form-label" Text="Resim Seçiniz"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:FileUpload ID="xFileUpload" class="btn form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'DisplayImage')" />
                                    </div>
                                    <div class="form-group">
                                        <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="LinkButton2" runat="server" OnClick="SecilenResmiSilBtn_Click">Varsayılan Resim  -></asp:LinkButton>
                                    </div>
                                    <div class="form-group">
                                        <asp:LinkButton CssClass="form-control btn btn-outline-danger" ID="SecilenResmiSilBtn" runat="server" OnClientClick="ClearDuyuruResmi();">Seçilen Resmi Sil</asp:LinkButton>
                                    </div>
                                </div>
                                <div class="col-6">
                                    <div class="form-group">
                                        <asp:Label ID="Label9" runat="server" CssClass="col-form-label" Text="Seçilen Resim"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Image ID="DisplayImage" ClientIDMode="Static" runat="server" ImageUrl="../DuyuruResimleri/duyuru-resmi-yok.jpg" class="img-thumbnail" Height="190" Width="250" onerror="this.src='../DuyuruResimleri/duyuru-resmi-yok.jpg';" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label2" runat="server" CssClass="col-form-label " Text="Duyuru Metni"></asp:Label>
                                <asp:TextBox ID="MetinTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" ClientIDMode="Static" maxlength="100"></asp:TextBox>

                            </div>
                        </div>

                        
                    </div>

                </div>
                <div class="col-5">
                    <div class="card">
                        <div class="card-header text-center fw-bold p-0">
                            <div class="form-group m-0 p-0">
                                <label class="col-form-label">Duyuru Ayarları</label>
                            </div>
                        </div>
                        <div class="card-body">

                            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="form-group col-6">
                                            <div class="form-group">
                                                <asp:Label ID="Label4" runat="server" CssClass="col-form-label" Text="Yayin Baş.Tarihi"></asp:Label>
                                                <input type="text" id="YayinBasTarTxt" name="YayinBasTarTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="Label7" runat="server" CssClass="col-form-label" Text="Başlama Saati"></asp:Label>
                                                <asp:DropDownList ID="BasSaatDDL" runat="server" class="form-control " OnSelectedIndexChanged="BasSaatDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                            </div>

                                            <div class="checkbox pt-3">
                                                <label>
                                                    <asp:CheckBox ID="AktifChk" runat="server" Checked="true" ToolTip="Duyuru Aktifse işaretli olmalıdır" />
                                                    Duyuru Aktif
                                                </label>
                                            </div>

                                            <div class="form-group">
                                                <asp:Label ID="Label3" runat="server" CssClass="col-form-label" Text="Tekrarlama"></asp:Label>
                                                <asp:DropDownList ID="TekrarlaDDL" runat="server" class="form-control " OnSelectedIndexChanged="TekrarlaDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                            </div>
                                        </div>
                                        <div class="form-group col-6">
                                            <div class="form-group">
                                                <asp:Label ID="Label5" runat="server" CssClass="col-form-label" Text="Yayin Bit.Tarihi"></asp:Label>
                                                <input type="text" id="YayinBitTarTxt" name="YayinBitTarTxt" class="form-control " runat="server" readonly="readonly" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="Label8" runat="server" CssClass="col-form-label" Text="Bitiş Saati"></asp:Label>
                                                <asp:DropDownList ID="BitSaatDDL" runat="server" class="form-control " OnSelectedIndexChanged="BitSaatDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                            </div>
                                            <div class="checkbox pt-3">
                                                <label>
                                                    <asp:CheckBox ID="PopupChk" runat="server" Checked="true" ToolTip="Duyuru penceresi açılacaksa işaretlenmelidir." />
                                                    Popup Pencere Aç
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </div>
                    </div>
                    <div class="card">
                        <div class="card-header text-center fw-bold p-0">
                            <div class="form-group m-0 p-0">
                                <label class="col-form-label">Duyuru Yapılacak Personel</label>
                            </div>
                        </div>
                        <div class="card-body m-0">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="form-group" id="PersonelDiv" runat="server">
                                        <div class="form-group row">
                                            <div class="form-group col-6 ">
                                                <label class="col-form-label" for="PersonelDDL">Personel Seçimi</label>
                                                <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" Style="height: auto" />
                                            </div>
                                            <div class="form-group col-6 ">
                                                <label class="col-form-label text-white" for="HepsiniEkleBtn">Hepsini Ekle</label>
                                                <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="HepsiniEkleBtn" runat="server" OnClick="HepsiniEkleBtn_Click">Hepsini Ekle</asp:LinkButton>
                                            </div>
                                        </div>


                                        <div class="form-group" id="HaricTutulanDiv" runat="server">
                                            <label class="col-form-label" for="HaricTutulanTxt">Seçilen Personel Listesi</label>
                                            <div class="table border" style="max-height: 300px; overflow: auto;">
                                                <asp:Table ID="SecilenPersonelTable" runat="server" CssClass="table table-sm small table-hover">
                                                </asp:Table>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:LinkButton CssClass="btn btn-outline-danger float-end" ID="HepsiniCikar" runat="server" OnClick="HepsiniCikarBtn_Click">Hepsini Çıkar</asp:LinkButton>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>


        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success" ID="KaydetBtn" runat="server" OnClick="KaydetBtn_Click" Visible="False">Kaydet</asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-primary" ID="GuncelleBtn" runat="server" OnClick="GuncelleBtn_Click" Visible="false">Güncelle </asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-danger" ID="SilBtn" runat="server" OnClick="SilBtn_Click" Visible="false">Sil</asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="DuyuruListBtn" runat="server" OnClick="DuyuruListBtn_Click">Duyuru Listesi</asp:LinkButton>
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
                            <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Duyuru Silinecek"></asp:Label></h3>
                    </div>
                    <div class="card-body">
                        <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Duyuruyu Silmek İstiyor musunuz?"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Duyuruyu Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
