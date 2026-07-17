<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevOnayGirisiWP.ascx.cs" Inherits="IKYS_WebParts.GorevOnayGirisiWP.GorevOnayGirisiWP" %>

<style>
    .disabled-look {
        background-color: #e9ecef !important; /* Bootstrap'ın disabled rengi */
        color: #6c757d !important; /* Gri yazı rengi */
        pointer-events: auto; /* Kullanıcı etkileşimi aktif */
    }
    /* Summernote içeriğindeki ul/ol stili zorla */
    .note-editor .note-editable ul {
        list-style-type: disc !important; /* maddelerin daire olmasını sağlar */
        list-style-position: outside !important; /* dışta bırak, daha görünür olur */
        margin-left: 1.25rem !important; /* girinti */
        padding-left: 1.25rem !important;
        color: inherit !important; /* sembol rengini içeriğin rengiyle eşitle */
    }

        /* içindeki li öğelerinin doğru davranması için */
        .note-editor .note-editable ul li {
            display: list-item !important;
        }

            /* alternatif: marker stilini modern tarayıcılarda ayarlamak istersen */
            .note-editor .note-editable ul li::marker {
                color: inherit !important;
            }

    /* ol için (sayıların davranışını garanti et) */
    .note-editor .note-editable ol {
        list-style-type: decimal !important;
        margin-left: 1.25rem !important;
        padding-left: 1.25rem !important;
    }
</style>
<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }

    function CalculateFullDateTimeDiff() {
        var startDateStr = $('#BaslangicTarihiTxt').val();
        var endDateStr = $('#BitisTarihiTxt').val();
        var startTimeStr = $('#BasSaatDDL').val();
        var endTimeStr = $('#BitSaatDDL').val();

        if (!startDateStr || !endDateStr || !startTimeStr || !endTimeStr) {
            return;
        }

        var startParts = startDateStr.split('.');
        var endParts = endDateStr.split('.');
        var startDateTime = new Date(`${startParts[2]}-${startParts[1]}-${startParts[0]}T${startTimeStr}`);
        var endDateTime = new Date(`${endParts[2]}-${endParts[1]}-${endParts[0]}T${endTimeStr}`);

        var diffMs = endDateTime - startDateTime;
        if (diffMs < 0) {
            $('#SureGunTxt').val("0");
            $('#SureSaatTxt').val("0");
            $('#SureDakikaTxt').val("0");
            var saveBtnId = '<%= SaveBtn.ClientID %>';
            $('#' + saveBtnId).hide();
            return;
        }

        var saveBtnId = '<%= SaveBtn.ClientID %>';
        $('#' + saveBtnId).show();

        var diffMins = Math.floor(diffMs / (1000 * 60));
        var days = Math.floor(diffMins / (60 * 24));
        var hours = Math.floor((diffMins % (60 * 24)) / 60);
        var minutes = diffMins % 60;

        $('#SureGunTxt').val(days);
        $('#SureSaatTxt').val(hours);
        $('#SureDakikaTxt').val(minutes);
        $('#SureSaatDakikaTxt').val(hours + ' Saat ' + minutes + ' Dakika');
        $('#SureGunStrTxt').val(days + " Gün ");
        __doPostBack('BaslangicTarihiTxt', '');
        __doPostBack('BitisTarihiTxt', '');
        __doPostBack('BasSaatDDL', '');
        __doPostBack('BitSaatDDL', '');
    }

    function setupDatepickers() {
        var common = {
            dateFormat: 'dd.mm.yy',
            changeMonth: true,
            changeYear: true,
            firstDay: 1,
            dayNamesMin: ['Paz', 'Pts', 'Sal', 'Çar', 'Per', 'Cum', 'Cts'],
            monthNames: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'],
            monthNamesShort: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'],
            onSelect: function () { CalculateFullDateTimeDiff(); }
        };

        $('#BaslangicTarihiTxt').datepicker(common);
        $('#BitisTarihiTxt').datepicker(common);
    }

    function RegisterDDLChangeHandlers() {
        $('#BasSaatDDL, #BitSaatDDL, #BaslangicTarihiTxt, #BitisTarihiTxt').off('change').on('change', function () {
            CalculateFullDateTimeDiff();
        });
    }

    // Initialize or re-initialize Summernote safely
    function InitializeSummernote() {
        try {
            var $editor = $('#GorevinSebebiTxt');
            if ($editor.length === 0) return;

            // Eğer zaten init edilmişse destroy et (temiz state)
            if ($editor.next().hasClass('note-editor')) {
                try { $editor.summernote('destroy'); } catch (e) { /* ignore */ }
            }

            $editor.summernote({
                lang: 'tr-TR',
                height: 130,
                focus: false, // önemli: selection bazlı active state'in gelmesini engeller
                toolbar: [
                    ['style', ['bold', 'italic', 'underline']],
                    ['para', ['ul', 'ol', 'paragraph']]
                ],
                popover: { image: [], link: [], air: [] }
            });

            // küçük gecikmeyle toolbar üzerindeki kalan 'active' sınıflarını temizle
            setTimeout(function () {
                $('.note-toolbar .note-btn.active').removeClass('active');
            }, 60);

        } catch (ex) {
            console && console.error && console.error('InitializeSummernote error', ex);
        }
    }

    // Tek bir yerden başlangıç - hem ilk yükleme hem UpdatePanel sonrası için güvenli
    $(function () {
        setupDatepickers();
        RegisterDDLChangeHandlers();
        CalculateFullDateTimeDiff();
        InitializeSummernote();

        // PageRequestManager ile partial postbackleri ele al
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            // Partial postback başlamadan önce editörü destroy et - böylece DOM/State kalmaz
            prm.add_beginRequest(function () {
                try {
                    var $ed = $('#GorevinSebebiTxt');
                    if ($ed.length && $ed.next().hasClass('note-editor')) {
                        $ed.summernote('destroy');
                    }
                } catch (e) { /* ignore */ }
            });

            // Partial postback bittikten sonra yeniden setup yap
            prm.add_endRequest(function () {
                try {
                    // Tarih/saat handler'larını tekrar bağla
                    setupDatepickers();
                    RegisterDDLChangeHandlers();

                    // yeniden init editor
                    InitializeSummernote();

                    // ve ekstra temizleme (küçük gecikmeyle)
                    setTimeout(function () {
                        $('.note-toolbar .note-btn.active').removeClass('active');
                    }, 60);
                } catch (e) { console && console.error && console.error(e); }
            });
        }
    });
</script>
<div class="container">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="form-label fw-semibold text-success" ID="TitleLbl" runat="server" Text="Görev Onayı Girişi"></asp:Label>
                        <asp:Label CssClass="form-label fw-semibold text-white" ID="GorevOnayIdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="form-label fw-semibold " ID="PersonelAdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row gap-3 m-2">
                        <div class="row form-group p-1 col-4 m-0" id="PersonelDiv" runat="server">
                            <label class="col form-label fw-semibold p-1" for="PersonelDDL">Personel Seçimi</label>
                            <asp:DropDownList ID="PersonelDDL" runat="server" CssClass="col form-control form-select form-select-lg" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" AutoPostBack="true" />
                        </div>

                        <div class="col checkbox">
                            <label>
                                <asp:CheckBox ID="HarcirahHesaplansinChk" runat="server" Checked="True" ToolTip="Harcırah hesaplanmaması için işareti kaldırınız." OnCheckedChanged="HarcirahHesaplansinChk_CheckedChanged" AutoPostBack="true" />
                                Harcırah Hesaplansın
                            </label>
                        </div>
                        <div class="row form-group p-1 col-4 m-0" id="GorevGrubuDiv" runat="server">
                            <label class="col form-label fw-semibold p-1" for="GorevGrubuTxt">Harcırah Grubu</label>
                            <asp:TextBox ID="GorevGrubuTxt" runat="server" CssClass="form-control" ReadOnly="True" />
                        </div>
                    </div>
                    <div class="form-group p-2 m-2">
                        <div class="row m-0">
                            <div class="col border p-2 m-0">
                                <div class="row">
                                    <div class="col-4">
                                        <div class="form-group form-label">
                                            <label class="form-label fw-semibold w-sem" for="BaslangicTarihi">Başlangıç Tarihi</label>
                                            <asp:TextBox ID="BaslangicTarihiTxt" runat="server" CssClass="form-control disabled-look"
                                                ClientIDMode="Static" OnTextChanged="BaslangicTarihiTxt_TextChanged" AutoPostBack="True" placeholder="gg.aa.yyyy"></asp:TextBox>
                                        </div>
                                        <div class="form-group form-label" id="BitTarDiv" runat="server">
                                            <label class="form-label fw-semibold" for="BitisTarihiTxt">Bitiş Tarihi</label>
                                            <asp:TextBox ID="BitisTarihiTxt" runat="server" CssClass="form-control disabled-look"
                                                ClientIDMode="Static" OnTextChanged="BitisTarihiTxt_TextChanged" AutoPostBack="True" placeholder="gg.aa.yyyy"></asp:TextBox>
                                        </div>


                                    </div>
                                    <div class="col-3">
                                        <div class="form-group form-label" id="BasSaatDiv">
                                            <label class="form-label fw-semibold" for="BasSaatDDL">Baş.Saat</label>
                                            <asp:DropDownList ID="BasSaatDDL" runat="server" CssClass="form-control form-select form-select-lg" ClientIDMode="Static"
                                                OnSelectedIndexChanged="BasSaatDDL_SelectedIndexChanged" AutoPostBack="true" />
                                        </div>
                                        <div class="form-group form-label" id="BitSaatDiv" runat="server">
                                            <label class="form-label fw-semibold" for="BitSaatDDL">Bit.Saat</label>
                                            <asp:DropDownList ID="BitSaatDDL" runat="server" CssClass="form-control form-select form-select-lg" ClientIDMode="Static"
                                                OnSelectedIndexChanged="BitSaatDDL_SelectedIndexChanged" AutoPostBack="true" />
                                        </div>

                                        <div class="form-group form-label fw-semibold" style="display: none">
                                            <label class="form-label fw-semibold" for="SureSaatTxt">Süre Saat</label>
                                            <asp:TextBox ID="SureSaatTxt" runat="server" CssClass="form-control" ClientIDMode="Static" />

                                        </div>
                                    </div>
                                    <div class="col-4">
                                        <div class="form-group form-label">
                                            <label class="form-label fw-semibold" for="SureGunStrTxt">Süre Gün</label>
                                            <asp:TextBox ID="SureGunStrTxt" runat="server" CssClass="form-control disabled-look"
                                                ClientIDMode="Static" placeholder="Süre (gün)"></asp:TextBox>
                                        </div>
                                        <div class="form-group form-label" style="display: none">
                                            <label class="form-label" for="SureGunTxt">Süre Gün</label>
                                            <asp:TextBox ID="SureGunTxt" runat="server" CssClass="form-control"
                                                ClientIDMode="Static" placeholder="Süre (gün)"></asp:TextBox>
                                            <asp:TextBox ID="SureDakikaTxt" runat="server" CssClass="form-control"
                                                ClientIDMode="Static" placeholder="Süre (gün)"></asp:TextBox>
                                        </div>
                                        <div class="form-group form-label">
                                            <label class="form-label fw-semibold" for="SureSaatTxt">Süre Saat/Dk</label>
                                            <asp:TextBox ID="SureSaatDakikaTxt" runat="server" CssClass="form-control disabled-look" ClientIDMode="Static" />
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="col-3 border p-2 m-0">
                                <div class="form-group form-label">
                                     <label class="form-label fw-semibold" for="TransferDDL">Transfer</label>
                                    <asp:DropDownList ID="TransferDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="TransferDDL_SelectedIndexChanged" AutoPostBack="true" />
                                </div>
                                <div class="form-group form-label">
                                    <label class="form-label fw-semibold" for="UlkeDDL">Ülke</label>
                                    <asp:DropDownList ID="UlkeDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="UlkeDDL_SelectedIndexChanged" AutoPostBack="true" />
                                </div>
                                <div class="row">
                                    <div class="col form-group form-label">
                                        <label class="form-label fw-semibold" for="GunlukYevmiyeTxt">Günlük Yev.</label>
                                        <asp:TextBox ID="GunlukYevmiyeTxt" runat="server" CssClass="form-control disabled-look" />
                                    </div>
                                    <div class="col form-group form-label">
                                        <label class="form-label fw-semibold" for="ParaBirimiTxt">Para Birimi</label>
                                        <asp:TextBox ID="ParaBirimiTxt" runat="server" CssClass="form-control" ReadOnly="True" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-2 border p-2 m-0">
                                <div class="form-group form-label">
                                    <label class="form-label fw-semibold" for="AvansTxt">Avans</label>
                                    <asp:TextBox ID="AvansTxt" runat="server" CssClass="form-control" />
                                </div>

                                <div class="form-group form-label col">
                                    <label class="form-label fw-semibold" for="UlasimAraciDDL">Ulaşım Aracı</label>
                                    <asp:DropDownList ID="UlasimAraciDDL" runat="server" CssClass="form-control form-select form-group-select-lg" />
                                </div>
                                <div class="form-group form-label col">
                                    <label class="form-label fw-semibold" for="AracPlakasiTxt">Araç Plakasi</label>
                                    <asp:TextBox ID="AracPlakasiTxt" runat="server" CssClass="form-control" />
                                </div>

                            </div>

                            <div class="col-3 border p-2 m-0">
                                <div class="row">
                                    <div class="col">
                                        <div class="form-group form-label">
                                            <label class="form-label fw-semibold" for="PerSubeImzaDDL">Per.Dir.</label>
                                            <asp:DropDownList ID="PerSubeImzaDDL" runat="server" CssClass="form-control form-select form-select-lg" />
                                        </div>
                                        <div class="form-group form-label">
                                            <label class="form-label fw-semibold" for="OnayImzaDDL">Per.Uzm.</label>
                                            <asp:DropDownList ID="OnayImzaDDL" runat="server" CssClass="form-control form-select form-select-lg" />
                                        </div>
                                    </div>

                                    <div class="col-3 form-group ">
                                        <label class="form-label fw-semibold" for="PersubeVekilChk">Vekil</label>
                                        <asp:CheckBox ID="PersubeVekilChk" runat="server" CssClass="form-control" />
                                    </div>
                                </div>


                            </div>
                        </div>
                    </div>
                    <div class="row m-2">
                        <div class="col-6 form-group m-0">
                            <label class="col-form-label fw-semibold" for="GorevinSebebiTxt">Görevin Sebebi</label>
                            <asp:TextBox ID="GorevinSebebiTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" ClientIDMode="Static" />
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label class="col-form-label fw-semibold" for="GorevinYeriTxt">Görevin Yeri</label>
                                <asp:TextBox ID="GorevinYeriTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                            </div>
                            <div class="form-group">
                                <label class="form-label fw-semibold" for="AciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                            </div>
                        </div>
                    </div>
                    <div class="row gap-3 m-2">

                        <!-- Harcırah Hesaplanan bölüm-->
                        <div class="col-8 border p-2 text-center m-0">
                            <!-- Ortalamak için text-center -->
                            <div class="col form-group m-2">
                                <label class="form-label fw-semibold text-primary d-block" for="YevmiyeTxt">Hesaplanan Harcırah</label>
                            </div>

                            <div class="row justify-content-center border m-2">
                                <!-- Ortalamak için justify-content-center -->
                                <div class="col-3 form-group">
                                    <label class="form-label fw-semibold" for="YevmiyeTxt">Hakedilen Yevmiye</label>
                                    <asp:TextBox ID="YevmiyeTxt" runat="server" CssClass="form-control text-center fw-bold text-primary" ReadOnly="True" />
                                </div>
                                <div class="col-3 form-group">
                                    <label class="form-label fw-semibold" for="YevmiyeParaBirimiTxt">Para Birimi</label>
                                    <asp:TextBox ID="YevmiyeParaBirimiTxt" runat="server" CssClass="form-control text-center fw-bold text-primary" ReadOnly="True" />
                                </div>
                                <div class="col-3 form-group">
                                    <label class="form-label fw-semibold" for="SureTxt">Hakedilen Gün</label>
                                    <asp:TextBox ID="SureTxt" runat="server" CssClass="form-control text-center fw-bold text-primary" ReadOnly="True" />
                                </div>
                            </div>

                        </div>
                        <div class="col">
                            <asp:TextBox ID="HesapAciklamaTxt" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" type="text" enabled="false"/>
                        </div>
                    </div>

                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-success col-2 me-5" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" Visible="false" />

                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-primary col-2 me-5" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" Visible="false" />
                    <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-danger col-2 me-5" runat="server" Text="Sil" OnClick="DeleteBtn_Click" />
                    <label class="form-label fw-semibold text-danger" for="YevmiyeTxt">(Yaptığınız değişikliklerin geçerli olması için lütfen Kaydet veya Güncelle düğmesine basınız.)</label>

                    <asp:LinkButton ID="RaporAlBtn" CssClass="btn btn-secondary col-2 float-end m-2" runat="server" Text="Rapor Al" OnClick="RaporAlBtn_Click" Visible="false" />
                    <asp:LinkButton CssClass="btn btn-secondary col-2 float-end m-2" ID="GorevOnayListesiBtn" runat="server" Text="Görev Onay Listesi" CausesValidation="false" OnClick="GorevOnayListesiBtn_Click" />
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog modal-dialog-centered">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">

                        <div class="modal-body">
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="MessageTitleLbl" class="form-label fw-semibold text-primary" runat="server" Text="Lütfen Dikkat: Görev Onayı Silinecek"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="MessageTextLbl" CssClass="form-label fw-semibold " runat="server" Text="Geçerli Görev Onayını Silmek İstiyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="DeleteNowBtn" runat="server" CausesValidation="false" Text="Görev Onayı Sil" OnClientClick="{return true;};" OnClick="DeleteNowBtn_Click" Visible="false" />
                            <asp:LinkButton CssClass="btn btn-success" ID="KaydetNowBtn" runat="server" CausesValidation="false" Text="Görevi kaydet" OnClientClick="{return true;};" OnClick="KaydetNowBtn_Click" />
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
     <asp:UpdateProgress ID="updateProgress1" runat="server">
     <ProgressTemplate>
         <div class='loaderMainContainer'>
             <div class='loaderContainer'>
                 <div class='loaderCircle'></div>
             </div>
         </div>
     </ProgressTemplate>
 </asp:UpdateProgress>
</div>
