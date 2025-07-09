<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonelGirisiWP.ascx.cs" Inherits="IKYS_WebParts.PersonelGirisiWP.PersonelGirisiWP" %>

<style>
    .custom-file-upload {
        padding: 6px 10px;
        border: 2px solid #0078d7;
        background-color: #f3f3f3;
        color: #333;
        border-radius: 6px;
        font-size: 14px;
        font-family: Segoe UI, sans-serif;
    }
</style>
<script type="text/javascript">

    function setActiveTab(activeTab) {
        $("#" + activeTab).tab("show");
    }
    function clearEvlilikTar() {
        $('#clear-evlilikTar').on('click', function () {
            $("#EvlilikTarihiTxt").val("");
            document.getElementById('<%= EvlilikTarihiTxt.ClientID%>').value = "";
            document.getElementById('<%= EvlilikKutlamaChk.Checked%>').value = false;
        });
    }

    $(document).ready(function () {
        $("#clear-evlilikTar").on("click", function (event) {
            $("#EvlilikTarihiTxt").val("");
        });
    });
    function readURL(personelFU, sender) {
        if (personelFU.files && personelFU.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + sender).attr('src', e.target.result);

            }
            reader.readAsDataURL(personelFU.files[0]);
        }
    }
</script>
<script>
    let cropper;

    function readPictureURL() {

        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const image = document.getElementById('imagePreview');
                image.src = e.target.result;
                image.style.display = 'block';

                // Initialize Cropper.js
                if (cropper) {
                    cropper.destroy(); // Destroy previous instance
                }
                cropper = new Cropper(image, {
                    aspectRatio: 1, // Example: Square crop
                    viewMode: 2,
                });

                document.getElementById('cropButton').style.display = 'inline-block';
            };
            reader.readAsDataURL(file);
        }
    }


    // Crop and upload button click event
    function cropAndUpload() {
        if (cropper) {
            // Get cropped image as a blob
            cropper.getCroppedCanvas().toBlob(async function (blob) {
                const formData = new FormData();
                formData.append('croppedImage', blob, 'cropped-image.jpg');

                // Send cropped image to the server
                const response = await fetch('/Admin/TasinmazBagisci/UploadCroppedImage', {
                    method: 'POST',
                    body: formData,
                });

                if (response.ok) {
                    alert('Image uploaded successfully!');
                } else {
                    alert('Error uploading image.XX');
                }
            }, 'image/jpeg');
        }
    }
</script>
<div class="container">

    <div class="card shadow">

        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <a class=" btn btn-outline-primary float-end mr-4" runat="server" id="YonergeLnk"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label CssClass="form-label fw-semibold  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Personel Girişi"></asp:Label>
                <asp:Label CssClass="form-label fw-semibold text-white" ID="PersonelIdLbl" Visible="false" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>

                    <div class="row">
                        <div class="form-group col-9">
                            <!-- Nav tabs -->
                            <ul class="nav nav-tabs" role="tablist">
                                <li class="nav-item" runat="server" id="KimlikNav">
                                    <a class="nav-link active" data-bs-toggle="tab" id="KimlikLi" href="#KimlikDiv">Kimlik</a>
                                </li>
                                <li class="nav-item" runat="server" id="IsBilgileriNav">
                                    <a class="nav-link" id="IsBilgileriLi" data-bs-toggle="tab" href="#IsBilgileriDiv">İş Bilgileri</a>
                                </li>
                                <li class="nav-item" runat="server" id="KadrosuzIsBilgileriNav">
                                    <a class="nav-link" id="KadrosuzIsBilgileriLi" data-bs-toggle="tab" href="#KadrosuzIsBilgileriDiv">İş Bilgileri</a>
                                </li>
                                <li runat="server" class="nav-item" id="IletisimNav">
                                    <a class="nav-link" id="IletisimLi" data-bs-toggle="tab" href="#IletisimDiv">İletişim Bilgileri</a>
                                </li>
                                <li runat="server" class="nav-item" id="AileNav">
                                    <a class="nav-link" id="AileLi" data-bs-toggle="tab" href="#AileDiv">Aile</a>
                                </li>
                                <li runat="server" class="nav-item" id="EgitimNav">
                                    <a class="nav-link" id="EgitimLi" data-bs-toggle="tab" href="#EgitimDiv">Eğitim/iş Tecrübesi</a>
                                </li>
                                <li runat="server" class="nav-item" id="IzinNav">
                                    <a class="nav-link" id="IzinLi" data-bs-toggle="tab" href="#IzinDiv">İzin</a>
                                </li>

                            </ul>

                            <!-- Tab panes -->
                            <div class="tab-content" runat="server">
                                <div class="tab-pane active" role="tabpanel" id="KimlikDiv">
                                    <div class="form-group row">
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="AdiTxt">Adı</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="AdiTxt" runat="server" CssClass="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SoyadiTxt">Soyadı</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="SoyadiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="SoyadiTxt" runat="server" CssClass="form-control" ToolTip="Soyadı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <label class="form-label fw-semibold" for="DogumTarihiTxt">Doğum Tarihi</label>
                                                    <input runat="server" type="text" id="DogumTarihiTxt" name="DogumTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                </div>
                                                <div class="col">
                                                    <label class="form-label fw-semibold" for="DogumGunuKutlamaChk">Kutlama</label>
                                                    <asp:CheckBox ID="DogumGunuKutlamaChk" runat="server" CssClass="form-select form-select-lg" Text="    " Checked="True" />
                                                </div>
                                            </div>

                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="DogumIliDDL">Doğ.Yeri İl</label>
                                                <asp:DropDownList ID="DogumIliDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="DogumIliDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="DogumIlceDDL">Doğ.Yeri İlçe</label>
                                                <asp:DropDownList ID="DogumIlceDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="TCKimlikNoTxt">TC Kimlik No</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TCKimlikNoTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="TCKimlikNoTxt" runat="server" CssClass="form-control" ToolTip="TC Kimlik Numarası" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="AnneAdiTxt">Anne Adı</label>
                                                <asp:TextBox ID="AnneAdiTxt" runat="server" CssClass="form-control" ToolTip="Anne Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="BabaAdiTxt">Baba Adı</label>
                                                <asp:TextBox ID="BabaAdiTxt" runat="server" CssClass="form-control" ToolTip="Baba Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="MedeniHaliDDL">Medeni Hali</label>
                                                <asp:DropDownList ID="MedeniHaliDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="MedeniHaliDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div class="row">
                                                <div class="col-8" id="EvlilikTarihiDiv" runat="server">
                                                    <label class="form-label fw-semibold" for="EvlilikTarihiTxt">Evlilik Tarihi</label>
                                                    <div class="input-group">
                                                        <input runat="server" type="text" id="EvlilikTarihiTxt" name="EvlilikTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                        <input type="button" id="clear-evlilikTar" value="Sil" onclick="clearEvlilikTar()" />
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label class="form-label fw-semibold" for="EvlilikKutlamaChk">Kutlama</label>
                                                    <asp:CheckBox ID="EvlilikKutlamaChk" runat="server" CssClass="form-control" Text="    " Checked="True" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SicilNoTxt">Sicil No</label>
                                                <asp:TextBox ID="SicilNoTxt" runat="server" CssClass="form-control" ToolTip="Sicil No" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="CinsiyetDDL">Cinsiyet</label>
                                                <asp:DropDownList ID="CinsiyetDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="KanGrubuDDL">Kan Grubu</label>
                                                <asp:DropDownList ID="KanGrubuDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="AskerSivilDDL">Asker/Sivil</label>
                                                <asp:DropDownList ID="AskerSivilDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="TahsiliDDL">Tahsili</label>
                                                <asp:DropDownList ID="TahsiliDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="KullaniciAdiTxt">Kullanıcı Adı</label>
                                                <asp:TextBox ID="KullaniciAdiTxt" runat="server" CssClass="form-control" ToolTip="Bilgisayar Kullanıcı Adı" type="text"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="KullaniciAdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateKimlikBtn" CssClass="btn btn-outline-primary" runat="server" Text="Kimlik Bilgilerini Kaydet" OnClick="UpdateKimlikBtn_Click" />
                                    </div>

                                </div>
                                <div class="tab-pane" role="tabpanel" id="IsBilgileriDiv">
                                    <div class="form-group row">
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="UnvanTanimDDL">Ünvanı</label>
                                                <asp:DropDownList ID="UnvanTanimDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="GorevTanimDDL">Görevi</label>
                                                <asp:DropDownList ID="GorevTanimDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="GorevTanimDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SubeTxt">Şubesi/Birimi</label>
                                                <asp:TextBox ID="BirimAdiTxt" runat="server" CssClass="form-control" ToolTip="Şubesi/Birimi" type="text" ReadOnly="true"></asp:TextBox>
                                                <div style="display: none">
                                                    <asp:TextBox ID="BirimIdTxt" runat="server" CssClass="form-control form-select form-select-lg" type="text" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="IsbasTarTxt">İşe Baş.Tar.</label>
                                                <input runat="server" type="text" id="IsbasTarTxt" name="IsbasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SGKSicilNoTxt">SGK Sic.No</label>
                                                <asp:TextBox ID="SGKSicilNoTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SGKBasTarTxt">SGK Baş.Tar.</label>
                                                <input runat="server" type="text" id="SGKBasTarTxt" name="SGKBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" tooltip="SGK Başlangıç Tarihi" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="VakifOncesiPrimGunSayisiTxt">Önceki Prim Gün</label>
                                                <asp:TextBox ID="VakifOncesiPrimGunSayisiTxt" runat="server" CssClass="form-control" ToolTip="Vakıf Öncesi SGK Prim Gün Sayısı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="ProtokolSirasiTxt">Prot.Sırası</label>
                                                <asp:TextBox ID="ProtokolSirasiTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div id="IzinDonemiBasTarDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="IzinDonemiBasTarTxt">izin Dönemi Baş.Tar.</label>
                                                <input runat="server" type="text" id="IzinDonemiBasTarTxt" name="IzinDonemiBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div id="EmeklilikTarDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="EmeklilikTarTxt">Emeklilik Tarihi</label>
                                                <input runat="server" type="text" id="EmeklilikTarTxt" name="EmeklilikTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="CalismaDurumuDDL">Çalışma Dur.</label>
                                                <asp:DropDownList ID="CalismaDurumuDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="CalismaDurumuDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div id="AyrilmaTarDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="AyrilmaTarTxt">Ayrılma Tar.</label>
                                                <input runat="server" type="text" id="AyrilmaTarTxt" name="AyrilmaTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div id="AyrilmaSebebiDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="AyrilmaSebebiDDL">Ayrılma Seb.</label>
                                                <asp:DropDownList ID="AyrilmaSebebiDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col">
                                            <div class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="DereceTxt">Derece</label>
                                                <asp:TextBox ID="DereceTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="KademeTxt">Kademe</label>
                                                <asp:TextBox ID="KademeTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="DereceKademeIlerlemeTarihiTxt">Der/Kad. Değ. Tar.</label>
                                                <input runat="server" type="text" id="DereceKademeIlerlemeTarihiTxt" name="DereceKademeIlerlemeTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>

                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label fw-semibold" for="IsBilgileriAciklamaTxt">Açıklama</label>
                                        <asp:TextBox ID="IsBilgileriAciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateIsBilgileriBtn" CssClass="btn btn-outline-primary" runat="server" Text="İş Bilgilerini Kaydet" OnClick="UpdateIsBilgileriBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="KadrosuzIsBilgileriDiv">
                                    <div class="form-group row">
                                        <div class="col">
                                            <div class="form-group">
                                                <label class="form-label fw-semibold" for="BirimDDL">Birim/Şube</label>
                                                <asp:DropDownList ID="BirimDDL" runat="server" class="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="KadrosuzIsbasTarihiTxt">İşe Baş.Tar.</label>
                                                <input runat="server" type="text" id="KadrosuzIsbasTarihiTxt" name="KadrosuzIsbasTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                        </div>
                                        <div class="col" id="Div1" runat="server">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="KadrosuzKurumuTxt">Kurumu</label>
                                                <asp:TextBox ID="KadrosuzKurumuTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                                            </div>

                                        </div>
                                        <div class="col">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="KadrosuzCalismaDurumuDDL">Çalışma Dur.</label>
                                                <asp:DropDownList ID="KadrosuzCalismaDurumuDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="KadrosuzCalismaDurumuDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div id="KadrosuzAyrilmaTarihiDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="KadrosuzAyrilmaTarihiTxt">Ayrılma Tar.</label>
                                                <input runat="server" type="text" id="KadrosuzAyrilmaTarihiTxt" name="KadrosuzAyrilmaTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div id="KadrosuzAyrilmaSebebiDiv" class="form-group m-0" runat="server">
                                                <label class="form-label fw-semibold" for="KadrosuzAyrilmaSebebiDDL">Ayrılma Sebebi</label>
                                                <asp:DropDownList ID="KadrosuzAyrilmaSebebiDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="form-label fw-semibold" for="KadrosuzIsBilgileriAciklamaTxt">Açıklama</label>
                                        <asp:TextBox ID="KadrosuzIsBilgileriAciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="KadrosuzUpdateIsBilgileriBtn" CssClass="btn btn-outline-info" runat="server" Text="İş Bilgilerini Kaydet" OnClick="KadrosuzUpdateIsBilgileriBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="AileDiv">
                                    <div class="card">
                                        <div class="bg-secondary">
                                            <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#AileMainPanel" aria-expanded="false" aria-controls="AileMainPanel" style="font-weight: bold">Aile Bilgileri</a>
                                        </div>
                                    </div>
                                    <div class="card">
                                        <div class="collapse show" id="AileMainPanel">
                                            <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                <asp:Table ID="AileTable" runat="server" CssClass="table table-sm small table-hover">
                                                </asp:Table>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="AileDuzenleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Aile Bilgilerini Düzenle" OnClick="AileDuzenleBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="IletisimDiv">
                                    <div class="row p-1">
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="IkametIliDDL">İkamet İli</label>
                                                <asp:DropDownList ID="IkametIliDDL" runat="server" CssClass="form-control form-select form-select-lg" OnSelectedIndexChanged="IkametIliDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="IkametIlcesiDDL">İkamet İlçe</label>
                                                <asp:DropDownList ID="IkametIlcesiDDL" runat="server" CssClass="form-control form-select form-select-lg"></asp:DropDownList>
                                            </div>

                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="AdresTxt">Adres</label>
                                                <asp:TextBox ID="AdresTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="SemtTxt">Semt</label>
                                                <asp:TextBox ID="SemtTxt" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="PostaKoduTxt">Posta Kodu</label>
                                                <asp:TextBox ID="PostaKoduTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="CepTelefonuTxt">Cep Telefonu</label>
                                                <asp:TextBox ID="CepTelefonuTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="CepTelefonu2Txt">Cep Telefonu 2</label>
                                                <asp:TextBox ID="CepTelefonu2Txt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="EvTelefonuTxt">Ev Telefonu</label>
                                                <asp:TextBox ID="EvTelefonuTxt" runat="server" CssClass="form-control"> </asp:TextBox>
                                            </div>

                                        </div>
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="DahiliTelefonuTxt">Dahili Telefonu</label>
                                                <asp:TextBox ID="DahiliTelefonuTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="IntranetEPostaTxt">EPosta (Intarnet)</label>
                                                <asp:TextBox ID="IntranetEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="IntranetEPostaTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="InternetEPostaTxt">EPosta (Internet)</label>
                                                <asp:TextBox ID="InternetEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="InternetEPostaTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>--%>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="OzelEPostaTxt">EPosta (Özel)</label>
                                                <asp:TextBox ID="OzelEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="form-label fw-semibold" for="PlakaTxt">Plaka</label>
                                                <asp:TextBox ID="PlakaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateIletisimBtn" CssClass="btn btn-outline-primary" runat="server" Text="İletişim Bilgilerini Kaydet" OnClick="UpdateIletisimBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" role="tabpanel" id="EgitimDiv">
                                    <div class="card-body">
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#OkulDiv" aria-expanded="false" aria-controls="OkulDiv" style="font-weight: bold">Mezun Olduğu Okullar</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="OkulDiv">
                                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                    <asp:Table ID="OkulTable" runat="server" CssClass="table table-sm small table-hover">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#KursDiv" aria-expanded="false" aria-controls="KursDiv" style="font-weight: bold">Gördüğü Kurslar</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="KursDiv">
                                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                    <asp:Table ID="KursTable" runat="server" CssClass="table table-sm small table-hover">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#IsTecrubeDiv" aria-expanded="false" aria-controls="IsTecrubeDiv" style="font-weight: bold">İş Tecrübesi</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="IsTecrubeDiv">
                                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                    <asp:Table ID="IsyeriTable" runat="server" CssClass="table table-sm small table-hover">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateEgitimBtn" CssClass="btn btn-outline-primary" runat="server" Text="Eğitim Bilgilerini Düzenle" OnClick="UpdateEgitimBtn_Click" />
                                    </div>

                                </div>
                                <div class="tab-pane" role="tabpanel" id="IzinDiv" >
                                    <div class="card-body">
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#IzinBilgileriDiv" aria-expanded="false" aria-controls="IzinBilgileriDiv" style="font-weight: bold">İzin Bilgileri</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="IzinBilgileriDiv">
                                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                    <asp:Table ID="UcretliIzinDonemleriTable" runat="server" CssClass="table table-sm small table-hover">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#KullanilanIzinlerDiv" aria-expanded="false" aria-controls="KullanilanIzinlerDiv" style="font-weight: bold">Dönem İçinde Kullanılan İzinler</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="KullanilanIzinlerDiv">
                                                <div class="card">
                                                    <div class="card-header">
                                                        <a style="font-weight: bold">Kullanılan Ücretli İzinler</a>
                                                    </div>
                                                    <div class="card-body">
                                                        <asp:Table ID="UcretliIzinHareketTable" runat="server" CssClass="table table-sm small">
                                                        </asp:Table>
                                                    </div>
                                                    <div class="card-footer">
                                                    </div>
                                                </div>
                                                <div class="card">
                                                    <div class="card-header">
                                                        <a style="font-weight: bold">Kullanılan Mazeret İzinleri</a>
                                                    </div>
                                                    <div class="card-body">
                                                        <asp:Table ID="MazeretIzinHareketTable" runat="server" CssClass="table table-sm small">
                                                        </asp:Table>
                                                    </div>
                                                    <div class="card-footer">
                                                    </div>
                                                </div>
                                                <div class="card">
                                                    <div class="card-header">
                                                        <a style="font-weight: bold">Kullanılan Diğer İzinler</a>
                                                    </div>
                                                    <div class="card-body">
                                                        <asp:Table ID="DigerIzinlerTable" runat="server" CssClass="table table-sm small">
                                                        </asp:Table>
                                                    </div>
                                                    <div class="card-footer">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="bg-secondary">
                                                <a class="text-white  text-center " data-bs-toggle="collapse" data-bs-target="#IzinTalepleriDiv" aria-expanded="false" aria-controls="IzinTalepleriDiv" style="font-weight: bold">İzin Talepleri</a>
                                            </div>
                                        </div>
                                        <div class="card">
                                            <div class="collapse" id="IzinTalepleriDiv">
                                                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                                                    <asp:Table ID="IzinTalepTable" runat="server" CssClass="table table-sm small table-hover">
                                                    </asp:Table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-group col-3">
                            <div class="form-group text-center">
                                <div class="form-group">
                                    <asp:Image ID="DisplayImage" ClientIDMode="Static" runat="server" ImageUrl="../PersonelResimleri/_t/personel_jpg.jpg" CssClass="img-thumbnail" Height="170" Width="132" onerror="this.src='../PersonelResimleri/_t/personel_jpg.jpg';" />
                                </div>
                                <div class="form-group">
                                    <asp:FileUpload ID="xFileUpload" CssClass="custom-file-upload" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'DisplayImage')" />
                                </div>
                                <div class="form-group">
                                    <asp:LinkButton ID="ResmiKaydetBtn" ClientIDMode="Static" CssClass="btn btn-outline-primary mt-2" runat="server" Text="Resmi Kaydet" OnClick="ResmiKaydetBtn_Click" Width="140px" />
                                    <%--<button id="cropButton" style="display: none;">Crop and Upload</button>--%>
                                </div>
                                <br />
                                <hr />
<%--                                <div>
                                    <div>

                                        <input type="file" id="fileInput" accept="image/*" onchange="readPictureURL();">
                                    </div>
                                    <div>
                                        <img id="imagePreview"
                                            src="../PersonelResimleri/personel.jpg"
                                            alt="Placeholder Image"
                                            style="max-width: 200px; max-height: 300px;" />
                                    </div>
                                    <div>
                                        <button id="cropButton" style="display: none;"  onclick="cropAndUpload();">Crop and Upload</button>
                                    </div>
                                </div>--%>
                                <div class="form-group">
                                    <label for="PersonelTipiDDL" class="form-label fw-semibold">Personel Tipi</label>
                                    <div>
                                        <asp:DropDownList ID="PersonelTipiDDL" runat="server" class="form-control form-select form-select-lg" OnSelectedIndexChanged="PersonelTipiDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="PersonelListesiBtn" runat="server" Text="Personel Listesi" CausesValidation="false" OnClick="PersonelListesiBtn_Click" />

            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
            <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="DeleteBtn_Click"
                OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" />
        </div>
    </div>

</div>
