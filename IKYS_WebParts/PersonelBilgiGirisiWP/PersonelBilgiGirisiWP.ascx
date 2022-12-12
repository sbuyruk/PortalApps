<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonelBilgiGirisiWP.ascx.cs" Inherits="IKYS_WebParts.PersonelBilgiGirisiWP.PersonelBilgiGirisiWP" %>

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

<div class="container">
    <div class="card shadow">
        
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Personel Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="PersonelIdLbl" Visible="false" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-9">
                            <ul class="nav nav-tabs" role="tablist">
                                <li class="nav-item" runat="server" id="KimlikNav">
                                    <a class="nav-link active" data-toggle="tab" id="KimlikLi" href="#KimlikDiv">Kimlik</a>
                                </li>
                                <li class="nav-item"  runat="server" id="IsBilgileriNav">
                                    <a class="nav-link" data-toggle="tab" href="#IsBilgileriDiv">İş Bilgileri</a>
                                </li>
                                <li class="nav-item"  runat="server" id="AileNav">
                                    <a class="nav-link" data-toggle="tab" href="#AileDiv">Aile Bilgileri</a>
                                </li>
                                <li class="nav-item"  runat="server" id="IletisimNav">
                                    <a class="nav-link" data-toggle="tab" href="#IletisimDiv">İletişim Bilgileri</a>
                                </li>
                                <li class="nav-item"  runat="server" id="EgitimNav">
                                    <a class="nav-link" data-toggle="tab" href="#EgitimDiv">Eğitim/İş Tecrübesi</a>
                                </li>
                                <li class="nav-item" runat="server" id="IzinNav">
                                    <a class="nav-link" data-toggle="tab" href="#IzinDiv">İzin Bilgileri</a>
                                </li>
                            </ul>
                            <!-- Tab panes -->
                            <div class="tab-content">
                                <div class="tab-pane active" id="KimlikDiv" role="tabpanel">
                                    <div class="card-columns">
                                        <div class="card border-0">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="AdiTxt">Adı</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="AdiTxt" runat="server" CssClass="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SoyadiTxt">Soyadı</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="SoyadiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="SoyadiTxt" runat="server" CssClass="form-control" ToolTip="Soyadı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <label class="col-form-label" for="DogumTarihiTxt">Doğum Tarihi</label>
                                                    <input runat="server" type="text" id="DogumTarihiTxt" name="DogumTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                </div>
                                                <div class="col">
                                                    <label class="col-form-label" for="DogumGunuKutlamaChk">Kutlama</label>
                                                    <asp:CheckBox ID="DogumGunuKutlamaChk" runat="server" CssClass="form-control" Text="    " Checked="True" />
                                                </div>
                                            </div>

                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="DogumIliDDL">Doğ.Yeri İl</label>
                                                <asp:DropDownList ID="DogumIliDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="DogumIliDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="DogumIlceDDL">Doğ.Yeri İlçe</label>
                                                <asp:DropDownList ID="DogumIlceDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="card border-0">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="TCKimlikNoTxt">TC Kimlik No</label>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TCKimlikNoTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                                <asp:TextBox ID="TCKimlikNoTxt" runat="server" CssClass="form-control" ToolTip="TC Kimlik Numarası" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="AnneAdiTxt">Anne Adı</label>
                                                <asp:TextBox ID="AnneAdiTxt" runat="server" CssClass="form-control" ToolTip="Anne Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="BabaAdiTxt">Baba Adı</label>
                                                <asp:TextBox ID="BabaAdiTxt" runat="server" CssClass="form-control" ToolTip="Baba Adı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="MedeniHaliDDL">Medeni Hali</label>
                                                <asp:DropDownList ID="MedeniHaliDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="MedeniHaliDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div class="row">
                                                <div class="col-8" id="EvlilikTarihiDiv" runat="server">
                                                    <label class="col-form-label" for="EvlilikTarihiTxt">Evlilik Tarihi</label>
                                                    <div class="input-group">
                                                        <input runat="server" type="text" id="EvlilikTarihiTxt" name="EvlilikTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                                        <input type="button" id="clear-evlilikTar" value="Sil" onclick="clearEvlilikTar()" />
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label class="col-form-label" for="EvlilikKutlamaChk">Kutlama</label>
                                                    <asp:CheckBox ID="EvlilikKutlamaChk" runat="server" CssClass="form-control" Text="    " Checked="True" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="card border-0">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SicilNoTxt">Sicil No</label>
                                                <asp:TextBox ID="SicilNoTxt" runat="server" CssClass="form-control" ToolTip="Sicil No" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="CinsiyetDDL">Cinsiyet</label>
                                                <asp:DropDownList ID="CinsiyetDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="KanGrubuDDL">Kan Grubu</label>
                                                <asp:DropDownList ID="KanGrubuDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="AskerSivilDDL">Asker/Sivil</label>
                                                <asp:DropDownList ID="AskerSivilDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="TahsiliDDL">Tahsili</label>
                                                <asp:DropDownList ID="TahsiliDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="KullaniciAdiTxt">Kullanıcı Adı</label>
                                                <asp:TextBox ID="KullaniciAdiTxt" runat="server" CssClass="form-control" ToolTip="Bilgisayar Kullanıcı Adı" type="text"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateKimlikBtn" CssClass="btn btn-outline-primary" runat="server" Text="Kimlik Bilgilerini Kaydet" OnClick="UpdateKimlikBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" id="IsBilgileriDiv" role="tabpanel">
                                    <div class="card-columns">
                                        <div class="card border-0">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="UnvanTanimDDL">Ünvanı</label>
                                                <asp:DropDownList ID="UnvanTanimDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="GorevTanimDDL">Görevi</label>
                                                <asp:DropDownList ID="GorevTanimDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="GorevTanimDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SubeTxt">Şubesi/Birimi</label>
                                                <asp:TextBox ID="BirimAdiTxt" runat="server" CssClass="form-control" ToolTip="Şubesi/Birimi" type="text" ReadOnly="true"></asp:TextBox>
                                                <div style="display: none">
                                                    <asp:TextBox ID="BirimIdTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="IsbasTarTxt">İşe Baş.Tar.</label>
                                                <input runat="server" type="text" id="IsbasTarTxt" name="IsbasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                        </div>
                                        <div class="card border-0">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SGKSicilNoTxt">SGK Sic.No</label>
                                                <asp:TextBox ID="SGKSicilNoTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SGKBasTarTxt">SGK Baş.Tar.</label>
                                                <input runat="server" type="text" id="SGKBasTarTxt" name="SGKBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" tooltip="SGK Başlangıç Tarihi" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="VakifOncesiPrimGunSayisiTxt">Önceki Prim Gün</label>
                                                <asp:TextBox ID="VakifOncesiPrimGunSayisiTxt" runat="server" CssClass="form-control" ToolTip="Vakıf Öncesi SGK Prim Gün Sayısı" type="text"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="ProtokolSirasiTxt">Prot.Sırası</label>
                                                <asp:TextBox ID="ProtokolSirasiTxt" runat="server" CssClass="form-control" type="text" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="card border-0">
                                            <div id="IzinDonemiBasTarDiv" class="form-group m-0" runat="server">
                                                <label class="col-form-label" for="IzinDonemiBasTarTxt">izin Dönemi Baş.Tar.</label>
                                                <input runat="server" type="text" id="IzinDonemiBasTarTxt" name="IzinDonemiBasTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div id="EmeklilikTarDiv" class="form-group m-0" runat="server">
                                                <label class="col-form-label" for="EmeklilikTarTxt">Emeklilik Tarihi</label>
                                                <input runat="server" type="text" id="EmeklilikTarTxt" name="EmeklilikTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="CalismaDurumuDDL">Çalışma Dur.</label>
                                                <asp:DropDownList ID="CalismaDurumuDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="CalismaDurumuDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                            <div id="AyrilmaTarDiv" class="form-group m-0" runat="server">
                                                <label class="col-form-label" for="AyrilmaTarTxt">Ayrılma Tar.</label>
                                                <input runat="server" type="text" id="AyrilmaTarTxt" name="AyrilmaTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                            </div>
                                            <div id="AyrilmaSebebiDiv" class="form-group m-0" runat="server">
                                                <label class="col-form-label" for="AyrilmaSebebiDDL">Ayrılma Seb.</label>
                                                <asp:DropDownList ID="AyrilmaSebebiDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateIsBilgileriBtn" CssClass="btn btn-outline-primary" runat="server" Text="İş Bilgilerini Kaydet" OnClick="UpdateIsBilgileriBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" id="AileDiv" role="tabpanel">
                                    <div class="card">
                                        <div class="btn-secondary">
                                            <a class="text-white  text-center " data-toggle="collapse" data-target="#AileMainPanel" aria-expanded="false" aria-controls="AileMainPanel" style="font-weight: bold">Aile Bilgileri</a>
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
                                <div class="tab-pane" id="IletisimDiv" role="tabpanel">
                                    <div class="row p-1">
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="IkametIliDDL">İkamet İli</label>
                                                <asp:DropDownList ID="IkametIliDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="IkametIliDDL_SelectedIndexChanged" AutoPostBack="true" />
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="IkametIlcesiDDL">İkamet İlçe</label>
                                                <asp:DropDownList ID="IkametIlcesiDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                            </div>

                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="AdresTxt">Adres</label>
                                                <asp:TextBox ID="AdresTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="SemtTxt">Semt</label>
                                                <asp:TextBox ID="SemtTxt" runat="server" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="PostaKoduTxt">Posta Kodu</label>
                                                <asp:TextBox ID="PostaKoduTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="CepTelefonuTxt">Cep Telefonu</label>
                                                <asp:TextBox ID="CepTelefonuTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="CepTelefonu2Txt">Cep Telefonu 2</label>
                                                <asp:TextBox ID="CepTelefonu2Txt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="EvTelefonuTxt">Ev Telefonu</label>
                                                <asp:TextBox ID="EvTelefonuTxt" runat="server" CssClass="form-control"> </asp:TextBox>
                                            </div>

                                        </div>
                                        <div class="col-4 ">
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="DahiliTelefonuTxt">Dahili Telefonu</label>
                                                <asp:TextBox ID="DahiliTelefonuTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="IntranetEPostaTxt">EPosta (Intarnet)</label>
                                                <asp:TextBox ID="IntranetEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="InternetEPostaTxt">EPosta (Internet)</label>
                                                <asp:TextBox ID="InternetEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="form-group m-0">
                                                <label class="col-form-label" for="OzelEPostaTxt">EPosta (Özel)</label>
                                                <asp:TextBox ID="OzelEPostaTxt" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-footer">
                                        <asp:LinkButton ID="UpdateIletisimBtn" CssClass="btn btn-outline-primary" runat="server" Text="İletişim Bilgilerini Kaydet" OnClick="UpdateIletisimBtn_Click" />
                                    </div>
                                </div>
                                <div class="tab-pane" id="EgitimDiv" role="tabpanel">
                                    <div class="card-body">
                                        <div class="card">
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#OkulDiv" aria-expanded="false" aria-controls="OkulDiv" style="font-weight: bold">Mezun Olduğu Okullar</a>
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
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#KursDiv" aria-expanded="false" aria-controls="KursDiv" style="font-weight: bold">Gördüğü Kurslar</a>
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
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#IsTecrubeDiv" aria-expanded="false" aria-controls="IsTecrubeDiv" style="font-weight: bold">İş Tecrübesi</a>
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
                                <div class="tab-pane" id="IzinDiv" role="tabpanel">
                                    <div class="card-body">
                                        <div class="card">
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#IzinBilgileriDiv" aria-expanded="false" aria-controls="IzinBilgileriDiv" style="font-weight: bold">İzin Bilgileri</a>
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
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#KullanilanIzinlerDiv" aria-expanded="false" aria-controls="KullanilanIzinlerDiv" style="font-weight: bold">Dönem İçinde Kullanılan İzinler</a>
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
                                            <div class="btn-secondary">
                                                <a class="text-white  text-center " data-toggle="collapse" data-target="#IzinTalepleriDiv" aria-expanded="false" aria-controls="IzinTalepleriDiv" style="font-weight: bold">İzin Talepleri</a>
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
                        <div class="col-3">
                            <div class="form-group m-0 col-md-2 nopadding">
                                <div class="row form-group m-0">
                                    <div class="row form-group m-0 ">
                                        <asp:Image ID="DisplayImage" ClientIDMode="Static" runat="server" ImageUrl="../PersonelResimleri/_t/personel_jpg.jpg" Height="190" Width="140" onerror="this.src='../PersonelResimleri/_t/personel_jpg.jpg';" />
                                        <%--<div class="controls alignRight">
                                        <asp:Image ID="PersonelFotoImg" runat="server" ImageUrl="/PersonelResimleri/personel.jpg" Height="190" Width="140" />
                                    </div>--%>
                                    </div>
                                    <div class="row form-group m-0 ">
                                        <asp:FileUpload ID="xFileUpload" Width="140" class="btn form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'DisplayImage')" />
                                        <%--<div class="form-group">--%>
                                        <%--<asp:Image ID="PersonelImg" runat="server" />--%>
                                        <%--<asp:FileUpload ID="fileBrowserFoto" class="form-control" runat="server" ToolTip="Personelin Resmi" type="text" onchange="readURL(this,'PersonelFotoImg')" />--%>
                                        <%--</div>--%>
                                    </div>
                                    <div class="row form-group m-0 ">
                                        <asp:LinkButton ID="ResmiKaydetBtn" ClientIDMode="Static" CssClass="btn btn-outline-primary mt-2" runat="server" Text="Resmi Kaydet" OnClick="ResmiKaydetBtn_Click" Width="140px" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                   
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DogumIliDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="GorevTanimDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="IkametIliDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="CalismaDurumuDDL" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="MedenihaliDDL" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="PersonelListesiBtn" runat="server" Text="Personel Listesi" CausesValidation="false" OnClick="PersonelListesiBtn_Click" />

            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
            <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="DeleteBtn_Click"
                OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" />
        </div>
    </div>
</div>
