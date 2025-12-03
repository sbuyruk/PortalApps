<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazGirisiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazGirisiWP.TasinmazGirisiWP" %>
<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    function OpenSerhBeyanIrtifakModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('SerhBeyanIrtifakModalDiv'));
        myModalInstance.show();
    }
    function setActiveTab(activeTab) {
        var tabTrigger = document.querySelector('#' + activeTab);
        if (tabTrigger) {
            bootstrap.Tab.getOrCreateInstance(tabTrigger).show();
        }
    }
    function DuzenleSilModalAc(parametreId, islemTipi) {
        document.getElementById('<%= parametreIdLbl.ClientID%>').value = parametreId;
        document.getElementById('<%= paramIslemTipiLbl.ClientID%>').value = islemTipi;
        document.getElementById('<%=DuzenleSilBtn.ClientID%>').click();
    }
    function clearSBITarihi() {
        $('#clear-SBITarihi').on('click', function () {
            $("#TarihTxt").val("");
            document.getElementById('<%= TarihTxt.ClientID%>').value = "";
        });
    }
</script>
<div class="container-fluid">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="form-label text-success fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz İşlemleri"></asp:Label>
                        <asp:Label CssClass="form-label " ID="AdiLbl" runat="server"></asp:Label>
                        <div class="form-group m-0 float-end me-2">
                            <asp:Label CssClass="form-control fw-semibold" ID="IdLbl" runat="server"></asp:Label>
                        </div>
                        <div class="form-group m-0 float-end">
                            <asp:Label CssClass="form-control fw-semibold" ID="SorumluBolgeTxt" runat="server"></asp:Label>
                        </div>
                        <div class="form-group m-0 float-end">
                            <asp:Label CssClass="form-control fw-semibold" ID="SorumluBolgeLbl" runat="server">Sorumlu Bölge:</asp:Label>
                        </div>
                    </h3>
                </div>
                <div class="card-body" id="MainCardDiv" runat="server">

                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs" role="tablist">
                        <li class="nav-item" runat="server" id="TapuSenedi1Nav">
                            <a class="nav-link active" id="TapuSenedi1Li" data-bs-toggle="tab" href="#TapuSenedi1Div">Tapu Senedi -1-</a>
                        </li>
                        <li class="nav-item" runat="server" id="SerhBeyanIrtifakNav">
                            <a class="nav-link" id="SerhBeyanIrtifakLi" data-bs-toggle="tab" href="#SerhBeyanIrtifakDiv" role="tab">Şerh/Beyan/İrtifak</a>
                        </li>
                        <li class="nav-item" runat="server" id="TasinmazBilgileriNav">
                            <a class="nav-link" id="TasinmazBilgileriLi" data-bs-toggle="tab" href="#TasinmazBilgileriDiv">Taşınmaz Bilgileri</a>
                        </li>

                        <li runat="server" class="nav-item" id="DegerlemeNav">
                            <a class="nav-link" id="DegerlemeLi" data-bs-toggle="tab" href="#DegerlemeDiv">Değerleme Bilgileri</a>
                        </li>
                    </ul>

                    <!-- Tab panes -->
                    <div class="tab-content" runat="server">
                        <!-- 1.Tab Tapu Senedi 1 -->
                        <div class="tab-pane active" role="tabpanel" id="TapuSenedi1Div">
                            <div class="row">
                                <!-- Panel - 1. sütun Tapu Tescil  -->
                                <div class="col-5">
                                    <div class="card d-flex flex-row" style="height: 510px">
                                        <!-- Dikey Label -->
                                        <div class="bg-primary text-white d-flex justify-content-center align-items-center px-2"
                                            style="writing-mode: vertical-rl; text-orientation: mixed; transform: rotate(180deg);">
                                            Taşınmaz Bilgileri
                                        </div>

                                        <!-- Taşınmaz Bilgileri  İçeriği -->
                                        <div class="card-body">

                                            <div class="row">
                                                <div class="col">
                                                    <div class="row">
                                                        <div class="col">
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="IliDDL">İl</label>
                                                                <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" />
                                                            </div>

                                                        </div>
                                                        <div class="col">
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="IlcesiDDL">İlçe</label>
                                                                <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control form-select form-select-lg"></asp:DropDownList>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="MahalleTxt">Mahalle</label>
                                                        <asp:TextBox ID="MahalleTxt" runat="server" class="form-control" ToolTip="Mahalle"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="KoyTxt">Köy</label>
                                                        <asp:TextBox ID="KoyTxt" runat="server" class="form-control" ToolTip="Köy"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="MevkiTxt">Mevki/Semt</label>
                                                        <asp:TextBox ID="MevkiTxt" runat="server" class="form-control" ToolTip="Mevki/Semt"></asp:TextBox>
                                                    </div>

                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="CaddeTxt">Cadde</label>
                                                        <asp:TextBox ID="CaddeTxt" runat="server" class="form-control" ToolTip="Cadde"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="SokakTxt">Sokak</label>
                                                        <asp:TextBox ID="SokakTxt" runat="server" class="form-control" ToolTip="Sokak"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="AdresTxt">Adres</label>
                                                        <asp:TextBox ID="AdresTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="3" ToolTip="Taşınmaz adresi"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <div class="row">
                                                        <div class="col">

                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="CinsiTxt">Niteliği (Cinsi)</label>
                                                                <asp:TextBox ID="CinsiTxt" runat="server" class="form-control" ToolTip="Taşınmazın cinsi/niteliği"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="AdaNoTxt">Ada No</label>
                                                                <asp:TextBox ID="AdaNoTxt" runat="server" CssClass="form-control" ToolTip=">Ada No"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="ParselNoTxt">Parsel No</label>
                                                                <asp:TextBox ID="ParselNoTxt" runat="server" class="form-control" ToolTip="Parsel No"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="PaftaNoTxt">Pafta No</label>
                                                                <asp:TextBox ID="PaftaNoTxt" runat="server" CssClass="form-control" ToolTip="Pafta No"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col">
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="YuzolcumuTxt">Yüzölçümü</label>
                                                                <asp:TextBox ID="YuzolcumuTxt" runat="server" class="form-control" ToolTip="Yüzölçümü"></asp:TextBox>
                                                            </div>

                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="SahifeNoTxt">Sahife No</label>
                                                                <asp:TextBox ID="SahifeNoTxt" runat="server" CssClass="form-control" ToolTip="Sahife No"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="CiltNoTxt">Cilt No</label>
                                                                <asp:TextBox ID="CiltNoTxt" runat="server" CssClass="form-control" ToolTip="Cilt No"></asp:TextBox>
                                                            </div>
                                                            <div class="form-group m-0 ">
                                                                <label class="form-label fw-semibold" for="YevmiyeNoTxt">Yevmiye No</label>
                                                                <asp:TextBox ID="YevmiyeNoTxt" runat="server" CssClass="form-control" ToolTip="Yevmiye No"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%--Bağımsız Bölüm--%>
                                <div class="col">
                                    <div class="card d-flex flex-row" style="height: 500px">
                                        <!-- Dikey Label -->
                                        <div class="bg-primary text-white d-flex justify-content-center align-items-center px-2"
                                            style="writing-mode: vertical-rl; text-orientation: mixed; transform: rotate(180deg);">
                                            Bağımsız Bölüm 
                                        </div>

                                        <!-- Panel İçeriği -->
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BagimsizBolumNoTxt">Bağ.Böl.No</label>
                                                        <asp:TextBox ID="BagimsizBolumNoTxt" runat="server" class="form-control" ToolTip="Bağımsız bölüm no"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BlokTxt">Blok</label>
                                                        <asp:TextBox ID="BlokTxt" runat="server" class="form-control" ToolTip="Blok"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="GirisTxt">Giriş</label>
                                                        <asp:TextBox ID="GirisTxt" runat="server" class="form-control" ToolTip="Giriş"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="ProjeM2Txt">ProjeM2</label>
                                                        <asp:TextBox ID="ProjeM2Txt" runat="server" class="form-control" ToolTip="ProjeM2"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="MetrekareTxt">Metrekare</label>
                                                        <asp:TextBox ID="MetrekareTxt" runat="server" class="form-control input-money" ToolTip="Metrekaresi"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="ToplamMetrekareTxt">Toplam Metrekare</label>
                                                        <asp:TextBox ID="ToplamMetrekareTxt" runat="server" class="form-control input-money" ToolTip="Toplam Metrekaresi"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BBBrutAlanTxt">BB Brüt alan</label>
                                                        <asp:TextBox ID="BBBrutAlanTxt" runat="server" class="form-control input-money" ToolTip="Bağımsız Bölüm Brüt Alan "></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BBNetAlanTxt">BB Net Alan</label>
                                                        <asp:TextBox ID="BBNetAlanTxt" runat="server" class="form-control input-money" ToolTip="Bağımsız Bölüm Net Alan "></asp:TextBox>
                                                    </div>

                                                </div>
                                                <div class="col">

                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="HisseMiktariPayTxt">Hisse Pay</label>
                                                        <asp:TextBox ID="HisseMiktariPayTxt" runat="server" class="form-control" ToolTip="Hisse Miktarı Pay"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="HisseMiktariPaydaTxt">Hisse Payda</label>
                                                        <asp:TextBox ID="HisseMiktariPaydaTxt" runat="server" class="form-control" ToolTip="Hisse Miktarı Payda"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="ArsaPayiTxt">Arsa Payı</label>
                                                        <asp:TextBox ID="ArsaPayiTxt" runat="server" CssClass="form-control" ToolTip="Arsa Payı"></asp:TextBox>

                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="TamHisseTxt">Tam Hisse</label>
                                                        <asp:TextBox ID="TamHisseTxt" runat="server" class="form-control" ToolTip="Tam Hisse"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="VakifHissesiTxt">Vakıf His.</label>
                                                        <asp:TextBox ID="VakifHissesiTxt" runat="server" class="form-control" ToolTip="Vakif Hissesi"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BBNitelikTxt">BB Nitelik</label>
                                                        <asp:TextBox ID="BBNitelikTxt" runat="server" class="form-control" ToolTip="Bağımsız bölüm Niteliği"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="AnaTasinmazNitelikTxt">Ana Taş.Nitelik</label>
                                                        <asp:TextBox ID="AnaTasinmazNitelikTxt" runat="server" class="form-control" ToolTip="Ana Taşınmaz Nitelik"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%--Tescil Bilgileri--%>
                                <div class="col">
                                    <div class="card d-flex flex-row" style="height: 500px">
                                        <!-- Dikey Label -->
                                        <div class="bg-primary text-white d-flex justify-content-center align-items-center px-2"
                                            style="writing-mode: vertical-rl; text-orientation: mixed; transform: rotate(180deg);">
                                            Tescil Bilgileri
                                        </div>

                                        <!-- Panel İçeriği -->
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="TapuTasinmazNoTxt">Tapu Taş.No</label>
                                                        <input class="form-control text-end" id="TapuTasinmazNoTxt" runat="server" />
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <div class="checkbox">
                                                            <label>
                                                                <asp:CheckBox ID="KatMulkiyetiChk" runat="server" Checked="True" ToolTip="Kat Mülkiyetli tapu ise işaretleyiniz" />
                                                                Kat Mülkiyeti
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <div class="checkbox">
                                                            <label>
                                                                <asp:CheckBox ID="KatIrtifakiChk" runat="server" Checked="True" ToolTip="Kat İrtifaklı tapu ise işaretleyiniz" />
                                                                Kat İrtifakı
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <div class="checkbox">
                                                            <label>
                                                                <asp:CheckBox ID="AltBolumChk" runat="server" Checked="True" AutoPostBack="true" OnCheckedChanged="AltBolumChk_CheckedChanged" ToolTip="Taşınmazın üstünde alt bölümler varsa işaretleyiniz" />
                                                                Alt Bölüm var mı
                                                            </label>
                                                        </div>
                                                        <asp:LinkButton ID="BagimsizBolumBtn" CssClass="btn btn-outline-warning" runat="server" Text="Alt Bölümler" Visible="false" OnClick="BagimsizBolumBtn_Click" />
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="ZeminTipiTxt">Zemin Tipi</label>
                                                        <input class="form-control" id="ZeminTipiTxt" name="ZeminTipiTxt" runat="server" tooltip="Zemin tipi" />
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="ZeminHisseTxt">Zemin Hisse</label>
                                                        <input class="form-control" id="ZeminHisseTxt" name="ZeminHisseTxt" runat="server" tooltip="Zemin tipi" />
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="EdinmeSekliDDL">Edinme Nedeni</label>
                                                        <asp:DropDownList ID="EdinmeSekliDDL" runat="server" class="form-control form-select form-select-lg"></asp:DropDownList>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="BagisYiliTxt">Bağış Yılı</label>
                                                        <asp:TextBox ID="BagisYiliTxt" runat="server" CssClass="form-control" ToolTip="Bağış Yılı"></asp:TextBox>
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="EnvantereGirisTarihiTxt">Env.Gir.Tar.</label>
                                                        <input runat="server" type="text" id="EnvantereGirisTarihiTxt" name="EnvantereGirisTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                    </div>

                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="TapuTarihiTxt">Tapu Tarihi</label>
                                                        <input runat="server" type="text" id="TapuTarihiTxt" name="TapuTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                    </div>
                                                    <div class="form-group m-0 ">
                                                        <label class="form-label fw-semibold" for="TapuIslemTarihiTxt">Tapu İşlem Tarihi</label>
                                                        <input runat="server" type="text" id="TapuIslemTarihiTxt" name="TapuIslemTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                    </div>

                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>



                            </div>

                            <div class="card">
                                <div class="card-header">
                                </div>
                            </div>
                        </div>
                        <!-- Şerh Beyan İrtifak-->
                        <div class="tab-pane" role="tabpanel" id="SerhBeyanIrtifakDiv" ClientIDMode="Static">
                            <%-- Şerh Şerh Beyan İrtifak ve --%>
                            <div class="row">
                                <div class="col card">
                                    <div class="card-header bg-primary text-white d-flex justify-content-center align-items-center mt-2">
                                        Taşınmaza Ait Şerh Beyan İrtifak Bilgileri
                                    </div>
                                    <div class="card-body">
                                        <!-- Şerh Beyan İrtifak içeriği buraya -->
                                        <div class="form-group">
                                            <table id="SerhBeyanIrtifakDataTable" class="table table-striped row-border" width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>No</th>
                                                        <th>Ş/B/İ</th>
                                                        <th>Açıklama</th>
                                                        <th>Malik/Lehtar</th>
                                                        <th>Tesis Kurum - Tarih - Yevmiye -</th>
                                                        <th>Terkin Sebebi </th>
                                                        <th>Düzenle</th>
                                                        <th>Sil</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="SerhBeyanIrtifakEkleBtn" CssClass="btn btn-success float-end  col-2 me-5" runat="server" Text="Ş/B/İ Ekle" OnClick="SerhBeyanIrtifakEkleBtn_Click" />
                                        <%--<asp:LinkButton ID="SerhBeyanIrtifakDuzenleBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Ş/B/İ Düzenle" OnClick="SerhBeyanIrtifakDuzenleBtn_Click" Visible="false" />--%>
                                        <%--<asp:LinkButton ID="SerhBeyanIrtifakSilBtn" CssClass="btn btn-outline-danger" runat="server" Text="Ş/B/İ Sil" OnClick="SerhBeyanIrtifakSilBtn_Click" Visible="false" />--%>
                                    </div>
                                </div>
                                <div id="HiddenDiv" style="display: none">
                                    <input id="parametreIdLbl" runat="server" type="text" />
                                    <input id="paramIslemTipiLbl" runat="server" type="text" />
                                    <asp:LinkButton ID="DuzenleSilBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="DuzenleSilBtn_Click" />
                                </div>
                            </div>
                        </div>
                        <!-- 3.Tab Taşınmaz Bilgileri -->
                        <div class="tab-pane" role="tabpanel" id="TasinmazBilgileriDiv">
                            <div class="row">

                                <div class="col-2">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="KullanimSekliDDL">Kullanım Şekli</label>
                                        <asp:DropDownList ID="KullanimSekliDDL" runat="server" CssClass="form-control form-select form-select-lg" ToolTip="Kullanim Şekli" Style="height: auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="MulkiyetSekliDDL">Mülkiyet Şekli</label>
                                        <asp:DropDownList ID="MulkiyetSekliDDL" runat="server" class="form-control form-select form-select-lg" Style="height: auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="EmlakSicilNoTxt">Emlak Sic.No</label>
                                        <asp:TextBox ID="EmlakSicilNoTxt" runat="server" class="form-control" ToolTip="Emlak Sicil No"></asp:TextBox>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="SigortaDDL">Sigorta Dur.</label>
                                        <asp:DropDownList ID="SigortaDDL" runat="server" CssClass="form-control form-select form-select-lg" ToolTip="Sigorta Durumu" Style="height: auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="InsaYiliTxt">İnşa Yılı</label>
                                        <input class="form-control text-end" id="InsaYiliTxt" runat="server" />
                                    </div>
                                </div>
                                <div class="col-2">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="KirayaUygunlukDDL">Kiraya Uygunluk</label>
                                        <asp:DropDownList ID="KirayaUygunlukDDL" runat="server" class="form-control form-select form-select-lg" Style="height: auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="KiraDurumuDDL">Kira Durumu</label>
                                        <asp:DropDownList ID="KiraDurumuDDL" runat="server" class="form-control form-select form-select-lg" Style="height: auto"></asp:DropDownList>
                                    </div>


                                </div>
                                <div class="col-2">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="ToplamKatSayisiTxt">Toplam Kat</label>
                                        <asp:TextBox ID="ToplamKatSayisiTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="BulunduguKatTxt">Bulunduğu Kat</label>
                                        <asp:TextBox ID="BulunduguKatTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="NitelikTxt">Niteliği</label>
                                        <asp:TextBox ID="NitelikTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="NitelikTxt">Bağ.Böl.Sayısı</label>
                                        <asp:TextBox ID="BagimsizBolumSayisiTxt" runat="server" class="form-control" ToolTip="Kaç bağımsız bölüm var">1</asp:TextBox>
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="NitelikTxt">Kat Maliki Sayısı</label>
                                        <asp:TextBox ID="KatMalikiSayisiTxt" runat="server" class="form-control" ToolTip="Kat Maliki kaç kişi">1</asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <div class="form-group m-0 ">
                                    <label class="form-label fw-semibold" for="AciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="4" ToolTip="Açıklama"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <!-- 4.Tab Değerleme Bilgileri -->
                        <div class="tab-pane" role="tabpanel" id="DegerlemeDiv">
                            <div class="row p-1">
                                <div class="col-4 ">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="EmlakBeyanDegeriTxt">Eml.Bey.Değ.</label>
                                        <input class="form-control input-money text-end" id="EmlakBeyanDegeriTxt" runat="server" />
                                    </div>
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold text-end" for="TahminiRayicDegeriTxt">Tah.Rayiç Değ.</label>
                                        <input class="form-control input-money text-end" id="TahminiRayicDegeriTxt" runat="server" />
                                    </div>
                                </div>

                            </div>

                        </div>

                    </div>

                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil"
                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" OnClick="DeleteBtn_Click" />
                    <asp:LinkButton ID="BagisciBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Bağışçı" Visible="false" OnClick="BagisciBtn_Click" />

                    <asp:LinkButton ID="SigortaBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Sigorta" Visible="false" OnClick="SigortaBtn_Click" />
                    <asp:LinkButton ID="TasinmazKartiBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Taşınmaz Kartı" Visible="false" OnClick="TasinmazKartiBtn_Click" />
                    <asp:LinkButton ID="OnarimlarBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Onarımlar" Visible="false" OnClick="OnarimlarBtn_Click" />
                    <asp:LinkButton ID="ResimlerBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Resimler" Visible="false" OnClick="ResimlerBtn_Click" />
                    <asp:LinkButton ID="EnvanterdenCikarBtn" CssClass="btn btn-outline-danger" runat="server" Text="Envanterden Çıkar" Visible="false" OnClick="EnvanterdenCikarBtn_Click" />
                    <asp:LinkButton ID="KopyalaBtn" CssClass="btn btn-outline-danger" runat="server" Text="Yeni Taşınmaz Olarak Kopyala" Visible="false" OnClick="KopyalaBtn_Click" />
                    <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-end mr-2" runat="server" Text="Sonraki=>" Visible="false" OnClick="NextBtn_Click" />
                    <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-end mr-2" runat="server" Text="<=Önceki" Visible="false" OnClick="PrevBtn_Click" />
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog modal-dialog-centered"">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <div class="modal-body">
                            <div style="display: none">
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label CssClass="form-label fw-semibold text-danger" runat="server" Text="Lütfen Dikkat!"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="MesajLbl" CssClass="form-label fw-semibold" runat="server" Text="Taşınmaza bağlı Bağışçı, Kira Sözleşmesi, Ödeme Planı, Sigorta ve Onarım işlemleri gibi bilgiler aktarılacak."></asp:Label>
                                    <asp:Label ID="MesajLbl1" CssClass="form-label fw-semibold text-danger" runat="server" Text="Bu taşınmazdan kopyalanarak yeni bir taşınmaz yaratılmasını onaylıyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="KopyalaNowBtn" runat="server" Text="Kopyala ve Yeni Taşınmaz Oluştur" OnClick="KopyalaNowBtn_Click" Visible="false" />
                            <asp:LinkButton CssClass="btn btn-danger" ID="SBIDeleteBtn" runat="server" Text="Sil" OnClick="SBIDeleteBtn_Click" ClientIDMode="Static" Visible="False" />
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>

                    </div>
                </div>
            </div>
            <div class="modal" id="SerhBeyanIrtifakModalDiv" role="dialog" ClientIDMode="Static">
                <div class="modal-dialog modal-dialog-centered">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 id="ModalBaslikLbl" class="modal-title fw-semibold text-success" runat="server">Şerh/Beyan/İrtifak</h4>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="SerhBeyanIrtifakDDL">Ş./B./İ.</label>
                                        <asp:DropDownList ID="SerhBeyanIrtifakDDL" runat="server" class="form-control form-select form-select-lg"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="MalikLehtarTxt">Malik/Lehtar</label>
                                        <asp:TextBox ID="MalikLehtarTxt" runat="server" CssClass="form-control" ToolTip="Malik Lehtar"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="TesisKurumTxt">Tesis Kurum</label>
                                        <asp:TextBox ID="TesisKurumTxt" runat="server" CssClass="form-control" ToolTip="Tesis eden kurum"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="TarihTxt">Tarih</label>
                                        <input runat="server" type="text" id="TarihTxt" name="SBITarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        <input type="button" id="clear-SBITarihi" value="Sil" onclick="clearSBITarihi()" />
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="SBIYevmiyeNoTxt">Terkin Sebebi</label>
                                        <asp:TextBox ID="TerkinSebebiTxt" runat="server" CssClass="form-control" ToolTip="Terkin Sebebi"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group m-0 ">
                                        <label class="form-label fw-semibold" for="YevmiyeTxt">Yevmiye No</label>
                                        <asp:TextBox ID="YevmiyeTxt" runat="server" CssClass="form-control" ToolTip="ŞBİ Yevmiye No"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="form-label fw-semibold" for="SBIAciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="SBIAciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="8" ToolTip="Ş/B/İ Açıklama"></asp:TextBox>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="SBIKaydetBtn" runat="server" Text="Kaydet" CssClass="btn btn-success" OnClick="SBIKaydetBtn_Click" ClientIDMode="Static"  Visible="False" />
                            <asp:LinkButton ID="SBIGuncelleBtn" runat="server" Text="Güncelle" CssClass="btn btn-primary" OnClick="SBIGuncelleBtn_Click" ClientIDMode="Static" Visible="False" />
                            
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

