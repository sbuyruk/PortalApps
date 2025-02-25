<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KisiGirisiWP.ascx.cs" Inherits="MTS_WebParts.KisiGirisiWP.KisiGirisiWP" %>
<script>
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>
<div class="container ">
    <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Kişi Girişi"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-2 ">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Adı"></asp:Label>
                                <asp:TextBox ID="AdiTxt" CssClass="form-control" runat="server" OnTextChanged="AdiTxt_TextChanged" AutoPostBack="True"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Soyadı"></asp:Label>
                                <asp:TextBox ID="SoyadiTxt" CssClass="form-control" runat="server" OnTextChanged="AdiTxt_TextChanged" AutoPostBack="True"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="TC Kimlik No"></asp:Label>
                                <asp:TextBox ID="TCKimlikNoTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Doğum Tarihi"></asp:Label>
                                <asp:TextBox ID="DogumTarihiTxt" CssClass="form-control DateTimePickerV1  input-date" placeholder="dd.mm.yyyy" runat="server" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-5 row">
                            <div class="col-6">
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Kurumu"></asp:Label>
                                    <asp:TextBox ID="KurumuTxt" CssClass="form-control" runat="server" Text="" Enabled="false"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Ünvanı"></asp:Label>
                                    <asp:TextBox ID="UnvaniTxt" CssClass="form-control" runat="server" Text="" Enabled="false"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Görevi"></asp:Label>
                                    <asp:TextBox ID="GoreviTxt" CssClass="form-control" runat="server" Text="" Enabled="false"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Kutlama"></asp:Label>
                                    <asp:CheckBox ID="KutlamaChk" CssClass="form-control" runat="server" Text="Kutlansın" Checked="false"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-5">
                                <div class="form-group ">
                                    <asp:Label CssClass="col-form-label" runat="server" for="MTSKurumTanimDDL">Kurum</asp:Label>
                                    <asp:TextBox ID="MTSKurumTanimTxt" CssClass="form-control" runat="server" Text="" ReadOnly></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-form-label" runat="server" for="MTSUnvanTanimDDL">Ünvan</asp:Label>
                                    <asp:DropDownList ID="MTSUnvanTanimDDL" runat="server" CssClass="form-control " Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-form-label" runat="server" for="MTSGorevTanimDDL">Görev</asp:Label>
                                    <asp:TextBox ID="MTSGorevTanimTxt" CssClass="form-control" runat="server" Text="" ReadOnly></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Randevu Kısıtı"></asp:Label>
                                    <asp:CheckBox ID="RandevuKisitiChk" CssClass="form-control" runat="server" Text="Randevu Kısıtı" Checked="false"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-5 row ">
                            <div class="col-4 ">
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label input-phone" runat="server" Text="Telefon1"></asp:Label>
                                    <asp:TextBox ID="Telefon1Txt" CssClass="form-control input-phone" runat="server" placeholder="( _ _ _ ) _ _  _ _" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Telefon2"></asp:Label>
                                    <asp:TextBox ID="Telefon2Txt" CssClass="form-control input-phone" runat="server" placeholder="( _ _ _ ) _ _  _ _" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Telefon3"></asp:Label>
                                    <asp:TextBox ID="Telefon3Txt" CssClass="form-control input-phone" runat="server" placeholder="( _ _ _ ) _ _  _ _" Text=""></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-3 ">
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label input-phone" runat="server" Text="Dahili1"></asp:Label>
                                    <asp:TextBox ID="Dahili1Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Dahili2"></asp:Label>
                                    <asp:TextBox ID="Dahili2Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Dahili3"></asp:Label>
                                    <asp:TextBox ID="Dahili3Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-5 ">
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Tel Açıklama1"></asp:Label>
                                    <asp:TextBox ID="TelAciklama1Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Tel Açıklama2"></asp:Label>
                                    <asp:TextBox ID="TelAciklama2Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Tel Açıklama3"></asp:Label>
                                    <asp:TextBox ID="TelAciklama3Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                            </div>

                        </div>

                    </div>
                    <div class="row">
                        <div class="col-9 form-group">
                            <asp:Label CssClass="col-from-label" runat="server" Text="Adres"></asp:Label>
                            <asp:TextBox ID="AdresTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="3" Text=""></asp:TextBox>
                        </div>
                        <div class="col-3">
                            <div class="form-group ">
                                <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İli</asp:Label>
                                <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İlçesi</asp:Label>
                                <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control " Height="34px"></asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label CssClass="col-from-label" runat="server" Text="Açıklama"></asp:Label>
                        <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetGirBtn" CssClass="btn btn-outline-success" runat="server" Text="Faaliyet Oluştur" OnClick="FaaliyetGirBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KisiyiSilBtn" CssClass="btn btn-danger" runat="server" Text="Kişiyi Sil" OnClick="KisiyiSilBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetTakvimiBtn" CssClass="btn btn-outline-info float-end" runat="server" Text="Faaliyet Takvimi" OnClick="FaaliyetTakvimiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FaaliyetListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Faaliyet Listesi" OnClick="FaaliyetListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="AramaListesi" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Arama/Görüşme Listesi" OnClick="AramaListesiBtn_Click"></asp:LinkButton>
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog modal-dialog-centered">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3>
                                <asp:Label ID="ModalBaslikLbl" runat="server" Text="Kişi Silinecek"></asp:Label>
                            </h3>
                        </div>
                        <div class="modal-body">
                            <div style="display: none">
                                <asp:Label ID="ParamVnLbl" runat="server" Text=""></asp:Label>
                                <asp:HiddenField ID="kaydetGuncelleSilHdn" runat="server" />
                            </div>
                            <div class="form-group">
                                <asp:Label ID="OnayMesajiLbl" CssClass="col-from-label" runat="server" Text=""></asp:Label>
                                <asp:CheckBox ID="IrtibatPersoneliChk" CssClass="form-control" runat="server" Text=". İrtibat personeli olarak ekle" Checked="false" TextAlign="Right" />
                                <asp:CheckBox ID="FaaliyetKatilimciChk" CssClass="form-control" runat="server" Text=". Katılımcı olarak ekle" Checked="true" TextAlign="Right" />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="OnaylaBtn" Text="Onayla" runat="server" class="btn btn-outline-primary" OnClick="OnaylaBtn_Click" Visible="false"></asp:LinkButton>
                            <asp:LinkButton ID="KisiSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Kişiyi Sil" OnClick="KisiSilNowBtn_Click" Visible="false"></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</div>
