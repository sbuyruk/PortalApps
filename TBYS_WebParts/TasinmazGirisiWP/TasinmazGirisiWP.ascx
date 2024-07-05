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
        $("#ModalOnayDiv").modal({ backdrop: false });
    }
</script>
<div class="container col-xl">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-success font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Girişi"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body row" id="MainCardDiv" runat="server">
                    <div class="col-4">
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KullanimSekliDDL">Kullanım Şekli</label>
                                    <asp:DropDownList ID="KullanimSekliDDL" runat="server" CssClass="form-control" ToolTip="Kullanim Şekli" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="SorumluBolgeTxt">Sor.Bölge</label>
                                    <asp:TextBox ID="SorumluBolgeTxt" runat="server" class="form-control" ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                    <asp:TextBox ID="SorumluBolgeIdTxt" runat="server" class="form-control" ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="EmlakSicilNoTxt">Emlak Sic.No</label>
                                    <asp:TextBox ID="EmlakSicilNoTxt" runat="server" class="form-control" ToolTip="Emlak Sicil No"></asp:TextBox>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="IliDDL">Bulunduğu İl</label>
                                    <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="IlcesiDDL">Bulunduğu İlçe</label>
                                    <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>

                            </div>
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="MukliyetSekliDDL">Mülkiyet Şekli</label>
                                    <asp:DropDownList ID="MukliyetSekliDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="CinsiTxt">Cinsi</label>
                                    <asp:TextBox ID="CinsiTxt" runat="server" CssClass="form-control" ToolTip="Cinsi"></asp:TextBox>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="EdinmeSekliDDL">Edinme Şekli</label>
                                    <asp:DropDownList ID="EdinmeSekliDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="BagisYiliTxt">Bağış Yılı</label>
                                    <asp:TextBox ID="BagisYiliTxt" runat="server" CssClass="form-control" ToolTip="Bağış Yılı"></asp:TextBox>
                                </div>

                            </div>
                        </div>
                        <div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="MahalleTxt">Mahalle</label>
                                <asp:TextBox ID="MahalleTxt" runat="server" class="form-control" ToolTip="Mahalle"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="CaddeTxt">Cadde</label>
                                <asp:TextBox ID="CaddeTxt" runat="server" class="form-control" ToolTip="Cadde"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="AdresTxt">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="4" ToolTip="Taşınmaz adresi"></asp:TextBox>
                            </div>

                        </div>
                    </div>
                    <div class="col-4">
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KirayaUygunlukDDL">Kiraya Uygunluk</label>
                                    <asp:DropDownList ID="KirayaUygunlukDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KiraDurumuDDL">Kira Durumu</label>
                                    <asp:DropDownList ID="KiraDurumuDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="EnvantereGirisTarihiTxt">Env.Gir.Tarihi</label>
                                    <input runat="server" type="text" id="EnvantereGirisTarihiTxt" name="EnvantereGirisTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="EmlakBeyanDegeriTxt">Eml.Bey.Değ.</label>
                                    <input class="form-control input-money text-right" id="EmlakBeyanDegeriTxt" runat="server" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="TapuTasinmazNoTxt">Tapu Taşınmaz No</label>
                                    <input class="form-control text-right" id="TapuTasinmazNoTxt" runat="server" />
                                </div>
                            </div>
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="SigortaDDL">Sigorta Dur.</label>
                                    <asp:DropDownList ID="SigortaDDL" runat="server" CssClass="form-control" ToolTip="Sigorta Durumu" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="TapuTarihiTxt">Tapu Tarihi</label>
                                    <input runat="server" type="text" id="TapuTarihiTxt" name="TapuTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label text-right" for="TahminiRayicDegeriTxt">Tah.Rayiç Değ.</label>
                                    <input class="form-control input-money text-right" id="TahminiRayicDegeriTxt" runat="server" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KatMulkiyetiDDL">Kat Mülk.</label>
                                    <asp:DropDownList ID="KatMulkiyetiDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="KatMulkiyetiDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="InsaYiliTxt">İnşa Yılı</label>
                                    <input class="form-control text-right" id="InsaYiliTxt" runat="server" />
                                </div>
                            </div>

                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="KoyTxt">Köy</label>
                            <asp:TextBox ID="KoyTxt" runat="server" class="form-control" ToolTip="Köy"></asp:TextBox>
                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="SokakTxt">Sokak</label>
                            <asp:TextBox ID="SokakTxt" runat="server" class="form-control" ToolTip="Sokak"></asp:TextBox>
                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                            <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="4" ToolTip="Açıklama"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-4 row">
                        <div class="form-group col">

                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="AdaNoTxt">Ada No</label>
                                <asp:TextBox ID="AdaNoTxt" runat="server" CssClass="form-control" ToolTip=">Ada No"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="ParselNoTxt">Parsel No</label>
                                <asp:TextBox ID="ParselNoTxt" runat="server" class="form-control" ToolTip="Parsel No"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="PaftaNoTxt">Pafta No</label>
                                <asp:TextBox ID="PaftaNoTxt" runat="server" CssClass="form-control" ToolTip="Pafta No"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="YuzolcumuTxt">Yüzölçümü</label>
                                <asp:TextBox ID="YuzolcumuTxt" runat="server" class="form-control" ToolTip="Yüzölçümü"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="YevmiyeNoTxt">Yevmiye No</label>
                                <asp:TextBox ID="YevmiyeNoTxt" runat="server" CssClass="form-control" ToolTip="Yevmiye No"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="CiltNoTxt">Cilt No</label>
                                <asp:TextBox ID="CiltNoTxt" runat="server" CssClass="form-control" ToolTip="Cilt No"></asp:TextBox>
                            </div>
                           
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="MevkiTxt">Mevki/Semt</label>
                                <asp:TextBox ID="MevkiTxt" runat="server" class="form-control" ToolTip="Mevki/Semt"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="ToplamKatSayisiTxt">Toplam Kat</label>
                                <asp:TextBox ID="ToplamKatSayisiTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="BulunduguKatTxt">Bulunduğu Kat</label>
                                <asp:TextBox ID="BulunduguKatTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                            </div>

                        </div>
                        <div class="form-group col">
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="ArsaPayiTxt">Arsa Payı</label>
                                <asp:TextBox ID="ArsaPayiTxt" runat="server" CssClass="form-control" ToolTip="Arsa Payı"></asp:TextBox>

                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="TamHisseTxt">Tam Hisse</label>
                                <asp:TextBox ID="TamHisseTxt" runat="server" class="form-control" ToolTip="Tam Hisse"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="VakifHissesiTxt">Vakıf Hissesi</label>
                                <asp:TextBox ID="VakifHissesiTxt" runat="server" class="form-control" ToolTip="Vakif Hissesi"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="HisseMiktariPayTxt">Hisse Pay</label>
                                <asp:TextBox ID="HisseMiktariPayTxt" runat="server" class="form-control" ToolTip="Hisse Miktarı Pay"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="HisseMiktariPaydaTxt">Hisse Payda</label>
                                <asp:TextBox ID="HisseMiktariPaydaTxt" runat="server" class="form-control" ToolTip="Hisse Miktarı Payda"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="BagimsizBolumNoTxt">Bağımsız Böl.No</label>
                                <asp:TextBox ID="BagimsizBolumNoTxt" runat="server" class="form-control" ToolTip="Adres bölüm no"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="SahifeNoTxt">Sahife No</label>
                                <asp:TextBox ID="SahifeNoTxt" runat="server" CssClass="form-control" ToolTip="Sahife No"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="NitelikTxt">Niteliği</label>
                                <asp:TextBox ID="NitelikTxt" runat="server" class="form-control" ToolTip="Oda+salon sayısı vb. özellikleri"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="NitelikTxt">Metrekare</label>
                                <asp:TextBox ID="MetrekareTxt" runat="server" class="form-control" ToolTip="Metrekaresi"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil"
                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" OnClick="DeleteBtn_Click" />
                    <asp:LinkButton ID="BagisciBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Bağışçı" Visible="false" OnClick="BagisciBtn_Click" />
                    <asp:LinkButton ID="BagimsizBolumBtn" CssClass="btn btn-outline-warning" runat="server" Text="Bağımsız Böl." Visible="false" OnClick="BagimsizBolumBtn_Click" />
                    <asp:LinkButton ID="SigortaBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Sigorta" Visible="false" OnClick="SigortaBtn_Click" />
                    <asp:LinkButton ID="TasinmazKartiBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Taşınmaz Kartı" Visible="false" OnClick="TasinmazKartiBtn_Click" />
                    <asp:LinkButton ID="OnarimlarBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Onarımlar" Visible="false" OnClick="OnarimlarBtn_Click" />
                    <asp:LinkButton ID="ResimlerBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Resimler" Visible="false" OnClick="ResimlerBtn_Click" />
                    <asp:LinkButton ID="EnvanterdenCikarBtn" CssClass="btn btn-outline-danger" runat="server" Text="Envanterden Çıkar" Visible="false" OnClick="EnvanterdenCikarBtn_Click" />
                    <asp:LinkButton ID="KopyalaBtn" CssClass="btn btn-outline-danger" runat="server" Text="Yeni Taşınmaz Olarak Kopyala" Visible="false" OnClick="KopyalaBtn_Click" />
                    <asp:LinkButton ID="NextBtn" CssClass="btn btn-outline-secondary float-right mr-2" runat="server" Text="Sonraki=>" Visible="false" OnClick="NextBtn_Click" />
                    <asp:LinkButton ID="PrevBtn" CssClass="btn btn-outline-secondary float-right mr-2" runat="server" Text="<=Önceki" Visible="false" OnClick="PrevBtn_Click" />
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
                                        <asp:Label CssClass="col-form-label text-danger" runat="server" Text="Lütfen Dikkat!"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="MesajLbl" CssClass="col-form-label" runat="server" Text="Taşınmaza bağlı Bağışçı, Kira Sözleşmesi, Ödeme Planı, Sigorta ve Onarım işlemleri gibi bilgiler aktarılacak."></asp:Label>
                                    <asp:Label ID="MesajLbl1" CssClass="col-form-label text-danger" runat="server" Text="Bu taşınmazdan kopyalanarak yeni bir taşınmaz yaratılmasını onaylıyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="KopyalaNowBtn" runat="server" Text="Kopyala ve Yeni Taşınmaz Oluştur" OnClick="KopyalaNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


</div>
