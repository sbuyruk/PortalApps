<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazSigortaGirisWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazSigortaGirisWP.TasinmazSigortaGirisWP" %>
<script>

    function CloseModal() {
        var myModalEl = document.getElementById('ModalOnayDiv');
        var modalInstance = bootstrap.Modal.getInstance(myModalEl);
        if (modalInstance) {
            modalInstance.hide();
        }
    }
    function ModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>

<div id="MainPanel" class="container shadow w-75" runat="server">

    <div id="BagisciMainPanel" class="card" runat="server">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Sigorta Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row ">

                <div class="col border m-2">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <div class="form-group">
                                <label class="col-form-label " for="SigortaCinsiDDL">Sigorta Cinsi</label>
                                <asp:DropDownList ID="SigortaCinsiDDL" runat="server" CssClass="form-control " Style="height: auto" />
                            </div>
                            <div class="form-group">
                                <label class="col-form-label " for="BagimsizBolumDDL">Bölüm Id</label>
                                <asp:DropDownList ID="BagimsizBolumDDL" runat="server" CssClass="form-control " Style="height: auto" OnSelectedIndexChanged="BagimsizBolumDDL_SelectedIndexChanged" AutoPostBack="True" />
                            </div>
                            <div class="form-group">
                                <label class="col-form-label " for="BagimsizBolumDDL">Bağımsız Bölüm</label>
                                <asp:TextBox ID="BagimsizBolumNoTxt" runat="server" CssClass="form-control" type="text" Enabled="false"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label " for="AdresKoduTxt">Adres Kodu</label>
                                <asp:TextBox ID="AdresKoduTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label " for="PoliceNoTxt">Poliçe No</label>
                                <asp:TextBox ID="PoliceNoTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label " for="DaskPoliceNoTxt">DASK Poliçe No</label>
                                <asp:TextBox ID="DaskPoliceNoTxt" runat="server" CssClass="form-control" type="text"></asp:TextBox>
                            </div>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </div>
                <div class="col border m-2">
                    <div class="form-group">
                        <label class="col-form-label " for="SigBasTarTxt">Sig.Baş.Tar.</label>
                        <asp:TextBox ID="SigBasTarTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" AutoPostBack="True" OnTextChanged="SigBasTarTxt_TextChanged" placeholder="gg.aa.yyyy"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="SigBitTarTxt">Sig.Bit.Tar.</label>
                        <asp:TextBox ID="SigBitTarTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy"></asp:TextBox>

                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="YapiTarziTxt">Yapı Tarzı</label>
                        <asp:TextBox ID="YapiTarziTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>


                    <div class="form-group">
                        <label class="col-form-label " for="BBBrutAlanTxt">Brüt Alan</label>
                        <asp:TextBox ID="BBBrutAlanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="BBNetAlanTxt">Net Alan</label>
                        <asp:TextBox ID="BBNetAlanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="row">
                        <div class="col form-group">
                            <label class="col-form-label " for="KullanimAmaciTxt">Kullanım Şekli</label>
                            <asp:TextBox ID="KullanimAmaciTxt" runat="server" CssClass="form-control" ToolTip="Kullanım Şekli"></asp:TextBox>
                        </div>
                        <div class="col form-group">
                            <label class="col-form-label " for="KullanimAmaciBtn">Kullanım Şekli</label>
                            <asp:LinkButton ID="KullanimAmaciGetirBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Getir" OnClick="KullanimAmaciGetirBtn_Click"/>
                        </div>
                    </div>
                </div>
                <div class="col border m-2">
                    <div class="form-group">
                        <label class="col-form-label " for="BulunduguKatTxt">Bulunduğu Kat</label>
                        <asp:TextBox ID="BulunduguKatTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="ToplamKatSayisiTxt">Toplam Kat Sayısı</label>
                        <asp:TextBox ID="ToplamKatSayisiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="InsaYiliTxt">Bina İnşa Yılı</label>
                        <asp:TextBox ID="InsaYiliTxt" runat="server" CssClass="form-control" ToolTip=" Bina İnşa Yılı"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="SigortaBedeliTxt">Sigorta Bedeli</label>
                        <asp:TextBox ID="SigortaBedeliTxt" runat="server" CssClass="form-control input-money text-end"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="col-form-label " for="PrimTxt">Prim</label>
                        <asp:TextBox ID="PrimTxt" runat="server" CssClass="form-control input-money text-end" type="text"></asp:TextBox>
                    </div>                    
                    <div class="form-group">
                        <label class="col-form-label " for="PrimTxt">Tapu Taşınmaz No</label>
                        <asp:Label ID="TapuTasinmazNoTxt" runat="server" CssClass="form-control" ></asp:Label>
                    </div>
                </div>
                <div class="col border m-2">
                    <div class="form-group">
                        <label class="col-form-label fw-bold ">Teminatlar</label>
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="DepremChk" CssClass="form-control mr-2 " runat="server" Text="Deprem " Checked="false" TextAlign="Right" ToolTip="Deprem" />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="YanginChk" CssClass="form-control mr-2 " runat="server" Text="Yangın, yıldırım, infilak ... " Checked="false" TextAlign="Right" ToolTip="Yangın, yıldırım, infilak, sel, su baskını, dahili su, fırtına, yer kayması, duman, cam kırılması, kar ağırlığı, kara-hava taşıtları çarpması, yangın mali sorumluluğu, grev, lokavt, kargaşa, halk hareketleri, kötü niyetli hareketler, terör, kira kaybı ve sabit tesisat (3.000 TL)." />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="Makine100000Chk" CssClass="form-control mr-2 " runat="server" Text="Makine-Tesisat (100.000 TL) ... " Checked="false" TextAlign="Right" ToolTip="Makine-Tesisat (100.000 TL), Demirbaş (100.000 TL), Elektronik Cihaz (350.000TL), Nakit Para ve Kıymetli Evrak (20.000 TL), Taşınan Para Hırsızlık (20.000 TL), Emniyeti Suistimal (20.000 TL),  Kasa (10.000 TL)" />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="Makine5000Chk" CssClass="form-control mr-2 " runat="server" Text="Makine-Tesisat (5.000 TL) ... " Checked="false" TextAlign="Right" ToolTip="Makine-Tesisat (5.000 TL), Demirbaş (10.000 TL), Elektronik Cihaz (10.000TL), Nakit Para ve Kıymetli Evrak (2.000 TL), Taşınan Para Hırsızlık (2.000 TL). Emniyeti Suistimal (2.000 TL)" />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="JeneratorChk" CssClass="form-control mr-2 " runat="server" Text="Jeneratör (30.000 TL) " Checked="false" TextAlign="Right" ToolTip="Jeneratör (30.000 TL)" />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="AsansorChk" CssClass="form-control mr-2 " runat="server" Text="Asansör (50.000 TL) " Checked="false" TextAlign="Right" ToolTip="Asansör (50.000 TL) " />
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="KazanChk" CssClass="form-control mr-2 " runat="server" Text="Kazan Dairesi Ekipmanı ..." Checked="false" TextAlign="Right" ToolTip="Kazan Dairesi Ekipmanı (30.000 TL)" />
                    </div>
                </div>
            </div>
            <div class="form-group border border-dark p-2 text-center" style="background-color: aliceblue">
                <div class="form-group">
                    <asp:Label CssClass="col-form-label fw-bold" ID="Label1" runat="server">Poliçe Formu</asp:Label>
                </div>
                <div class="form-group text-center">
                    <a id="DosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Poliçeyi Görüntüle
                    </a>
                    <asp:LinkButton ID="BelgeSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="BelgeSilBtn_Click"
                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                </div>
                <div class="form-group">
                    <asp:FileUpload ID="BelgeYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Dosyayı Seçiniz" type="text" />
                </div>
            </div>
            <div class="form-group">
                <label class="col-form-label " for="AciklamaTxt">Açıklama</label>
                <asp:TextBox ID="AciklamaTxt" TextMode="MultiLine" Rows="3" runat="server" CssClass="form-control " />
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-end mr-2" runat="server" Text="Sonraki=>" Visible="false" OnClick="NextBtn_Click" />
            <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-end mr-2" runat="server" Text="<=Önceki" Visible="false" OnClick="PrevBtn_Click" />
            <asp:LinkButton ID="BackBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Geri" Visible="false" OnClick="BackBtn_Click" />
            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" Width="150px" Visible="False" />
            <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" Width="150px" />
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="TasinmazBtn" runat="server" Text="Taşınmaza Git" CausesValidation="false" OnClick="TasinmazBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="TasinmazListesiBtn" runat="server" Text="Taşınmaz Listesi" CausesValidation="false" OnClick="TasinmazListesiBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-secondary" ID="SigortaListesiBtn" runat="server" Text="Sigorta Listesi" CausesValidation="false" OnClick="SigortaListesiBtn_Click" />
            <asp:LinkButton ID="SilBtn" Visible="false" CssClass="btn btn-danger ml-4 mr-2" runat="server" Text="Sigortayı Sil" OnClick="SilBtn_Click" />
        </div>
    </div>
    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="modal-body">
                                    <div class="card" runat="server" id="OnayDiv">
                                        <div class="card-header text-center">
                                            <h3>
                                                <asp:Label ID="Label3" class="col-form-label" runat="server" Text="Sigorta Silinecek"></asp:Label></h3>
                                        </div>
                                        <div class="card-body">
                                            <asp:Label ID="SilmeMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label><br />
                                            <asp:Label ID="OnayMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Sigortanın silinmesini onaylıyor musunuz?"></asp:Label>
                                        </div>
                                        <div class="card-footer">
                                            <asp:LinkButton CssClass="btn btn-danger" ID="SilNowBtn" runat="server" CausesValidation="false" Text="Sigortayı Sil" OnClientClick="{return true;};" OnClick="SilNowBtn_Click" />
                                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                                        </div>
                                    </div>
                                    <div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>

