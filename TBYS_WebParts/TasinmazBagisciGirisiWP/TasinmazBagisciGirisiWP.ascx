<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisciGirisiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBagisciGirisiWP.TasinmazBagisciGirisiWP" %>
<script type="text/javascript">
    function readURL(fileUpload, sender) {
        if (fileUpload.files && fileUpload.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + sender).attr('src', e.target.result);
            }
            reader.readAsDataURL(fileUpload.files[0]);
        }
    }

</script>

<div class="container">

    <div id="BagisciMainPanel" class="card shadow" runat="server">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" CssClass="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3>
                <asp:Label CssClass="col-form-label text-danger font-weight-bold" ID="TitleLbl" runat="server" Text="Taşınmaz Bağışçı Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-10">
                            <div class="row">
                                <div class="form-group col">
                                    <div class="form-group">
                                        <label class="col-form-label" for="AdiTxt">Adı</label>
                                        <asp:TextBox ID="AdiTxt" runat="server" CssClass="form-control" ToolTip="Bağışçının Adı" type="text"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="SoyadiTxt">Soyadı</label>
                                        <asp:TextBox ID="SoyadiTxt" runat="server" CssClass="form-control " ToolTip="Bağışçının Soyadı" type="text"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="AdiTxt">TC Kimlik No</label>
                                        <asp:TextBox ID="TCKimlikNoTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                                    </div>
                                    <div class="checkbox pt-3 text-danger">
                                        <label>
                                            <asp:CheckBox CssClass="" ID="GizliChk" runat="server" Checked="false" ToolTip="Bağışçı yaptığı bağışın gizli tutulmasını istiyor." />
                                            Bağışım Gizli Kalsın
                                        </label>
                                    </div>

                                </div>
                                <div class="form-group col">
                                    <div class="form-group">
                                        <label class="col-form-label" for="DogumYeriTxt">Doğum Yeri</label>
                                        <asp:TextBox ID="DogumYeriTxt" runat="server" CssClass="form-control " ToolTip="Bağışçının Doğum Yeri" type="text"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="DogumTarihiTxt">Doğum Tar.</label>
                                        <input runat="server" type="text" id="DogumTarihiTxt" name="DogumTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="MeslegiTxt">Mesleği</label>
                                        <asp:TextBox ID="MeslegiTxt" runat="server" CssClass="form-control " ToolTip="Bağışçının Mesleği" type="text" ViewStateMode="Inherit"></asp:TextBox>
                                    </div>

                                    <div class="form-group ">
                                        <label class="col-form-label" for="IliDDL">İkamet İli</label>
                                        <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true"  style="height:auto" />
                                    </div>
                                    <div class="form-group">
                                        <label class="col-form-label" for="IlcesiDDL">İkamet İlçesi</label>
                                        <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control"  style="height:auto"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group col">
                                    <div class="form-group ">
                                        <label class="col-form-label" for="SosyalGuvenceDDL">Sosyal Güv.</label>
                                        <asp:DropDownList ID="SosyalGuvenceDDL" runat="server" CssClass="form-control " Height="34px"></asp:DropDownList>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="SorumluBolgeTxt">Sor.Bölge</label>
                                        <asp:TextBox ID="SorumluBolgeTxt" runat="server" CssClass="form-control small" ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                        <asp:TextBox ID="SorumluBolgeIdTxt" runat="server" class="form-control " ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="Telefon1Txt">Telefon(1)</label>
                                        <asp:TextBox ID="Telefon1Txt" runat="server" CssClass="form-control " ToolTip="Bağışçının telefonu"></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="Telefon2Txt">Telefon(2)</label>
                                        <asp:TextBox ID="Telefon2Txt" runat="server" CssClass="form-control" ToolTip="Bağışçının ikinci telefonu"></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <label class="col-form-label" for="EPostaTxt">E-Posta</label>
                                        <asp:TextBox ID="EPostaTxt" runat="server" CssClass="form-control" ToolTip="Bağışçının e-posta adresi"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group col" >
                                    <div class="form-group ">
                                        <label class="col-form-label" for="Sag_vefatDDL">Sağ-Vefat</label>
                                        <asp:DropDownList ID="Sag_vefatDDL" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Sag_vefatDDL_SelectedIndexChanged" style="height:auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group" id="VefatDiv" runat="server">
                                        <div class="form-group" id="VefatTarihiDiv" runat="server">
                                            <label class="col-form-label" for="VefatTarihiTxt">Vefat Tarihi</label>
                                            <input runat="server" type="text" id="VefatTarihiTxt" name="DogumTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        </div>
                                        <div class="form-group">
                                            <label class="col-form-label" for="DefinYeriTxt">Defin Yeri (Mezarlık)</label>
                                             <asp:TextBox ID="DefinYeriTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                                        </div>
                                        <div class="form-group ">
                                            <label class="col-form-label" for="DefinIliDDL">Defin İli</label>
                                            <asp:DropDownList ID="DefinIliDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="DefinIliDDL_SelectedIndexChanged" AutoPostBack="true"  Style="height: auto" />
                                        </div>
                                        <div class="form-group">
                                            <label class="col-form-label" for="DefinIlcesiDDL">Defin İlçesi</label>
                                            <asp:DropDownList ID="DefinIlcesiDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group col">
                                    <label class="col-form-label" for="AdresTxt">Adres</label>
                                    <asp:TextBox ID="AdresTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="6" ToolTip="Bağışçının adresi"></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="6" ToolTip="Açıklama"></asp:TextBox>
                                </div>
                                <div class="form-group col-3" runat="server" id="DefinAciklamaDiv">
                                    <label class="col-form-label" for="DefinAciklamaTxt">Defin Açıklaması</label>
                                    <asp:TextBox ID="DefinAciklamaTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="6" ToolTip="Defin Açıklaması"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group ">
                                <div class="form-group border border-dark p-2" style="background-color:antiquewhite" >
                                    <div class="form-group text-center">
                                        <asp:Image ID="DisplayImage" ClientIDMode="Static" runat="server" ImageUrl="../BagisciResimleri/_t/bagisci_jpg.jpg" CssClass="img-thumbnail" onerror="this.src='../BagisciResimleri/_t/bagisci_jpg.jpg';" Style="height: 150px" />
                                    </div>
                                    <div class="form-group">
                                        <asp:FileUpload ID="ResimYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'DisplayImage')" />
                                    </div>
                                </div>
                                <div class="form-group border border-dark p-2" style="background-color:aliceblue">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-form-label font-weight-bold" ID="Label1" runat="server">Bağışçı Bilgi ve Talep Formu</asp:Label>
                                    </div>
                                    <div class="form-group text-center">
                                        <a id="DosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">
                                            Bilgi ve Talep Formu
                                        </a>
                                        <asp:LinkButton ID="BelgeSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="BelgeSilBtn_Click"
                                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                                    </div>
                                    <div class="form-group">
                                        <asp:FileUpload ID="BelgeYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Dosyayı Seçiniz" type="text" />
                                    </div>
                                </div>
                                <div class="form-group border border-dark p-2" style="background-color:lightgrey">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-form-label font-weight-bold" ID="Label2" runat="server">Bağışçı Taahhüt Formu</asp:Label>
                                    </div>
                                    <div class="form-group text-center">
                                        <a id="TaahhutDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">
<%--                                            <img alt="Bağışçı Bilgi ve Talep Formu (PDF) Görüntüle " src="../TBYSResimleri/pdf-var.png" data-themekey="#" class="img-thumbnail">
                                            <br />--%>
                                            Taahhüt Belgesi
                                        </a>
                                        <asp:LinkButton ID="TaahhutSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="TaahhutSilBtn_Click"
                                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                                    </div>
                                    <div class="form-group">
                                        <asp:FileUpload ID="TaahhutYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Dosyayı Seçiniz" type="text" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer">
                        <asp:LinkButton ID="TasinmazBagisciListBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Taşınmaz Bağışçı Listesi" OnClick="TasinmazBagisciListBtn_Click" />
                        <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                        <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" />
                        <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Bağışçıyı Sil" OnClick="DeleteBtn_Click"
                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" />
                        <asp:LinkButton ID="YakinlariBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Bağışçı Yakınları" Visible="false" OnClick="YakinlariBtn_Click" />
                        <asp:LinkButton ID="TalepleriBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Bağışçı Talepleri" Visible="false" OnClick="TalepleriBtn_Click" />
                        <asp:LinkButton ID="TaahhutleriBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Taahhütler" Visible="false" OnClick="TaahhutleriBtn_Click" />
                        <asp:LinkButton ID="BagislariBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Yaptığı Bağışlar" Visible="false" OnClick="BagislariBtn_Click" />
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="IliDDL" EventName="SelectedIndexChanged" />
                    <asp:PostBackTrigger ControlID="SaveBtn" />
                    <asp:PostBackTrigger ControlID="UpdateBtn" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

</div>
