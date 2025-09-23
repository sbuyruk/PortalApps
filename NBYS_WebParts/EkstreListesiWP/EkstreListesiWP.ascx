<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EkstreListesiWP.ascx.cs" Inherits="NBYS_WebParts.EkstreListesiWP.EkstreListesiWP" %>

<style>
    .uyari {
        color:red;
    }
    .ekstre-aktarildi {
        color: grey;
    }

    .ekstre-aktarilmadi {
        color: black;
    }

    .ekstre-aktarilabilir {
        color: green;
    }

    .cakisma-var {
        color: red;
        font-weight: bold;
    }
    .small-font{
        font-size:small;
    }
</style>

<script type="text/javascript">


    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }

    function CloseModalOnay() {
        $("#ModalOnayDiv").modal('hide');
    }
    var tabledata = [];
    function EkleCikar(ekstreAktarmaId, isChecked) {
        var index = tabledata.indexOf(ekstreAktarmaId.toString());
        if (isChecked && (index < 0)) {
            tabledata.push(ekstreAktarmaId.toString());
        } else if (!isChecked && (index > -1)) {
            tabledata.splice(index, 1);
        }
        if (tabledata.length > 0)
            document.getElementById('BtnDiv').style.display = "block";
        else {
            document.getElementById('BtnDiv').style.display = "none";

        }
    }
    function addRemoveEkstreIdToList(ekstreAktarmaId, chkbox) {
        var isChecked = false;
        if (chkbox.checked)
            isChecked = true;
        EkleCikar(ekstreAktarmaId, isChecked);
    }

    function SecilenleriKaydetTriggerBtnClicked() {
        document.getElementById('<%= paramArray.ClientID%>').value = tabledata;
        document.getElementById('<%= SecilenleriKaydetBtn.ClientID%>').click();
    }
    function SecilenleriSilTriggerBtnClicked() {
        document.getElementById('<%= paramArray.ClientID%>').value = tabledata;
        document.getElementById('<%= SecilenleriSilBtn.ClientID%>').click();
    }

</script>
<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3>
                <a class=" btn btn-outline-primary float-end me-4" runat="server" id="YonergeLnk"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label CssClass="form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Ekstre Aktarma Listesi"></asp:Label>
                <asp:Label CssClass="form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group border p-2">

                <div class="row">
                    <div class="form-group col" style="display: block">
                        <label class="col-form-label mr-2 fw-bold" for="IslemTarihiTxt">İşlem Tarihi</label>
                        <asp:TextBox ID="IslemTarihiTxt" runat="server" class="form-control DateTimePickerV1" type="text" AutoPostBack="true" OnTextChanged="IslemTarihiTxt_TextChanged" />
                    </div>
                    <div class="form-group col">
                        <label class="col-form-label" for="BankaDDL">Banka</label>
                        <asp:DropDownList ID="BankaDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="BankaDDL_SelectedIndexChanged" Height="34px"></asp:DropDownList>
                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="AkbankLbl" runat="server" Text="Akbank"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="AkbankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="AkbankEkstreLbl" runat="server" Text="Akbank (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="AkbankEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="FinansbankLbl" runat="server" Text="Finansbank"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="FinansbankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="FinansbankEkstreLbl" runat="server" Text="Finansbank (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="FinansbankEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="EDevletLbl" runat="server" Text="EDevlet"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="EDevletOkLbl" runat="server" Text=""></asp:Label>
                        </div>

                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="GarantiLbl" runat="server" Text="Garanti"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="GarantiOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="GarantiEkstreLbl" runat="server" Text="Garanti (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="GarantiEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="HalkbankLbl" runat="server" Text="Halkbank"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="HalkbankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="Halkbank2Lbl" runat="server" Text="Halkbank 2"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="Halkbank2OkLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="IsbankLbl" runat="server" Text="İşbank"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="IsbankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="IsbankEkstreLbl" runat="server" Text="İşbank (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="IsbankEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="KartIleLbl" runat="server" Text="Kart ile Bağış"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="KartIleOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="KioskLbl" runat="server" Text="Kiosk"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="KioskOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="SMSVakifLbl" runat="server" Text="SMS Vakıf"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="SMSVakifOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="TebLbl" runat="server" Text="TEB"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="TebOkLbl" runat="server" Text=""></asp:Label>
                        </div>

                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="VakifbankLbl" runat="server" Text="Vakıfbank"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="VakifbankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="Vakifbank2Lbl" runat="server" Text="Vakıfbank2"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="Vakifbank2OkLbl" runat="server" Text=""></asp:Label>
                        </div>

                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="VakifKatilimLbl" runat="server" Text="Vakıf Katılım"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="VakifKatilimOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="YKBEkstreLbl" runat="server" Text="Yapı Kredi (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="YKBEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col">
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="ZiraatBankLbl" runat="server" Text="Ziraat Bankası"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="ZiraatBankOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="ZiraatBankEkstreLbl" runat="server" Text="Ziraat B. (Ekstre)"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="ZiraatBankEkstreOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group m-0">
                            <asp:Label CssClass="form-label" ID="ZiraatKatilimLbl" runat="server" Text="Ziraat Katılım"></asp:Label>
                            <asp:Label CssClass="form-label fw-bold" ID="ZiraatKatilimOkLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <div class="col-3">
                        <div class="checkbox">
                            <label>
                                <asp:CheckBox ID="AktarilanlarHaricChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="AktarilanlarHaricChk_CheckedChanged" ToolTip="Aktarilanları görmek için işareti kaldırınız." />
                                Aktarılanları Gösterme
                            </label>
                        </div>
                        <div class="checkbox">
                            <label>
                                <asp:CheckBox ID="TumunuSecChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="TumunuSecChk_CheckedChanged" ToolTip="Bu sayfadakilerin tümünü seç" ClientIDMode="Static" />
                                Sayfanın Tümünü Seç
                            </label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="form-group">
                    <table id="CustomDataTable" class="table table-hover table-bordered table-striped" width="100%">
                        <thead>
                            <tr>
                                <th>SEÇ</th>
                                <th>K.No</th>
                                <th>Banka</th>
                                <th>TC Kimlik</th>
                                <th>Adı Soyadı</th>
                                <th>Telefon</th>
                                <th>Bağış Tarihi</th>
                                <th>Tutar</th>
                                <th>Açıklama</th>
                                <th>Düzenle</th>
                                <th>Eşleştir</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
                
        </div>
        <div class="card-footer">
            <div id="BtnDiv" style="display: none">
                <input id="SecilenleriKaydetTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Kaydet" onclick="SecilenleriKaydetTriggerBtnClicked();" />
                <input id="SecilenleriSilTriggerBtn" class="btn btn-success" type="button" value="Seçilenleri Sil" onclick="SecilenleriSilTriggerBtnClicked();" />
            </div>
            <div id="InvisibleDiv" style="display: none">
                <input id="paramArray" runat="server" type="text" />
                <asp:LinkButton ID="SecilenleriKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="SecilenleriKaydetBtn_Click" />
                <asp:LinkButton ID="SecilenleriSilBtn" runat="server" CssClass="btn btn-danger" CausesValidation="false" Text=" Sil " OnClick="SecilenleriSilBtn_Click" />
            </div>
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>

    </div>
    <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-body">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card">
                                <div class="card-header">
                                    <h3>
                                        <asp:Label ID="ModalTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <div class="form-group">
                                        <h4>
                                            <asp:Label ID="ModalSubTitleLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h4>
                                        <h4>
                                            <asp:Label ID="UyariMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label></h4>
                                        <h4>
                                            <asp:Label ID="OnayMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="İşlemi onaylıyor musunuz?"></asp:Label></h4>
                                    </div>
                                </div>
                                <div class="card-footer">
                                    <asp:LinkButton CssClass="btn btn-success" ID="KaydetNowBtn" runat="server" CausesValidation="false" Text="Seçilenleri Kaydet" 
                                        OnClientClick="this.disabled=true; this.innerHTML='Kaydediliyor...';"
                                        OnClick="KaydetNowBtn_Click" Visible="false" />
                                    <asp:LinkButton CssClass="btn btn-danger" ID="SilNowBtn" runat="server" CausesValidation="false" 
                                        Text="Seçilenleri Sil" OnClientClick="{return true;};" OnClick="SilNowBtn_Click" Visible="false" />
                                    <button type="button" class="btn btn-default float-end" data-bs-dismiss="modal">Kapat</button>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="SecilenleriKaydetBtn" EventName="click" />
                            <asp:AsyncPostBackTrigger ControlID="SecilenleriSilBtn" EventName="click" />
                        </Triggers>
                    </asp:UpdatePanel>
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
            </div>
        </div>
    </div>
</div>