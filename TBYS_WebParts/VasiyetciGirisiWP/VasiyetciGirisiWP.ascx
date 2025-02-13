<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VasiyetciGirisiWP.ascx.cs" Inherits="TBYS_WebParts.VasiyetciGirisiWP.VasiyetciGirisiWP" %>
<script>
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
    }

    function OpenSilModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalSilDiv'));
        myModalInstance.show();
    }
</script>

<div class="container">
    <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="Vasiyetçi Girişi"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col border m-2">
                            <div class="row">
                                <div class="col-4">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Adı"></asp:Label>
                                        <asp:TextBox ID="AdiTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Soyadı"></asp:Label>
                                        <asp:TextBox ID="SoyadiTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="TC Kimlik No"></asp:Label>
                                        <asp:TextBox ID="TCKimlikNoTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Doğum Tarihi"></asp:Label>
                                        <input id="DogumTarihiTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly">
                                    </div>

                                </div>
                                <div class="col-4">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Telefon 1"></asp:Label>
                                        <asp:TextBox ID="Telefon1Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Telefon 2"></asp:Label>
                                        <asp:TextBox ID="Telefon2Txt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group ">
                                        <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İkamet İli</asp:Label>
                                        <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control " OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İkamet İlçesi</asp:Label>
                                        <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control " style="height:auto"></asp:DropDownList>
                                    </div>

                                </div>
                                <div class="col-4">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Doğum Yeri"></asp:Label>
                                        <asp:TextBox ID="DogumYeriTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Sorumlu Bölge"></asp:Label>
                                        <input id="SorumluBolgeTxt" class="form-control" runat="server" readonly="readonly">
                                    </div>
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Sağ-Vefat"></asp:Label>
                                        <asp:DropDownList ID="SagVefatDDL" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SagVefatDDL_SelectedIndexChanged" style="height:auto"></asp:DropDownList>
                                    </div>
                                    <div class="form-group" id="VefatTarihiDiv" runat="server" style="display: none">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Vefat Tarihi"></asp:Label>
                                        <input id="VefatTarihiTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly">
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="col border m-2">
                            <div class="row">
                                <div class="col-6">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Vasiyet Tipi"></asp:Label>
                                        <asp:TextBox ID="VasiyetTipiTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>


                                </div>
                                <div class="col-6">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Vasiyetin Durumu"></asp:Label>
                                        <asp:DropDownList ID="VasiyetinDurumuDDL" runat="server" CssClass="form-control" style="height:auto"></asp:DropDownList>
                                    </div>

                                </div>
                            </div>
                             <div class="row">
                                 <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Noter"></asp:Label>
                                        <asp:TextBox ID="NoterTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                 </div>
                                 <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Vasiyet Tarihi"></asp:Label>
                                        <input id="VasiyetTarihiTxt" class="form-control DateTimePickerV1" runat="server" readonly="readonly">
                                    </div>
                                 </div>
                                 <div class="col">

                                    <div class="form-group">
                                        <asp:Label CssClass="col-from-label" runat="server" Text="Yevmiye Numarası"></asp:Label>
                                        <asp:TextBox ID="YevmiyeNumarasiTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                    </div>
                                 </div>
                             </div>
                            <div id="BelgeYukleDiv" runat="server" class="form-group">
                                <div class="form-group">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Vasiyet Dosyası (PDF)"></asp:Label>
                                    <asp:FileUpload ID="BekleYukleFU" class="form-control" runat="server" ToolTip="Yüklenecek Dosyayı PDF formatında Seçiniz" type="file" />
                                </div>
                                <div class="form-group">
                                    <asp:LinkButton ID="YukleBtn" CssClass="btn btn-outline-success" runat="server" Text="Vasiyet Kaydet" OnClick="YukleBtn_Click"></asp:LinkButton>
                                    <asp:HyperLink ID="DosyaLnk" runat="server" data-fancybox data-type=pdf data-width=960 data-height=720 CssClass="btn btn-outline-primary float-end" Visible="false" Font-Size="Small">Vasiyet Görüntüle</asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">

                        <div class="col">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" Text="İkamet Adresi"></asp:Label>
                                <asp:TextBox ID="IkametAdresiTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Vasiyetçinin Talebi"></asp:Label>
                                <asp:TextBox ID="VasiyetcininTalebiTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Açıklama"></asp:Label>
                                <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>

                    </div>
                    <div id="VarlikDiv" runat="server" class="form-group">
                        <div class="card p-1">
                            <asp:Table ID="VasiyeteKonuVarlikTable" runat="server" CssClass="table table-bordered table-hover table-striped">
                                 <asp:TableHeaderRow HorizontalAlign="Center">
                                     <asp:TableHeaderCell ColumnSpan="6" Style="vertical-align: Middle">Vasiyete Konu Varlıklar</asp:TableHeaderCell>
                                 </asp:TableHeaderRow>
                                <asp:TableHeaderRow HorizontalAlign="Center">
                                    <asp:TableHeaderCell Style="vertical-align: Middle">Konusu</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Style="vertical-align: Middle">Cinsi</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Style="vertical-align: Middle">Adet</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Style="vertical-align: Middle">T.Rayiç</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Style="vertical-align: Middle">Aciklama</asp:TableHeaderCell>
                                    <asp:TableHeaderCell Style="vertical-align: Middle">Sil</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                            <div class="card-footer">
                                <asp:LinkButton ID="EkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Varlık Ekle" OnClick="EkleBtn_Click"></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="VasiyetciSilBtn" CssClass="btn btn-outline-danger" runat="server" Text="Vasiyetçiyi Sil" OnClick="VasiyetciSilBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="VasiyetciListesiBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Vasiyetçi Listesi" OnClick="VasiyetciListesiBtn_Click"></asp:LinkButton>

                </div>
            </div>
            <div class="modal" id="ModalSilDiv" role="dialog">
                <div class="modal-dialog modal-dialog-centered">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <h3>
                                <asp:Label ID="SilModalBaslikLbl" runat="server" Text="Vasiyete Konu Varlık Silinecek"></asp:Label>
                            </h3>
                        </div>
                        <div class="modal-body">
                            <div style="display: none">
                                <asp:Label ID="ParamVnLbl" runat="server" Text=""></asp:Label>
                            </div>
                            <div class="m-1 text-center" id="MesajDiv">
                                <div class="form-group">
                                    <asp:Label ID="SilMesajiLbl" CssClass="col-from-label" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton ID="VasiyetciSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Vasiyetçiyi Sil" Visible="False" OnClick="VasiyetciSilNowBtn_Click"></asp:LinkButton>
                            <asp:LinkButton ID="NiteligiSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Vasiyete Konu Varlığı Sil" OnClick="NiteligiSilNowBtn_Click"></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="YukleBtn" />
        </Triggers>
    </asp:UpdatePanel>
</div>
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-dialog-centered modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="VasiyetNiteligiLbl" runat="server" Text="Vasiyete Konu varlık Girişi"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClick="ModalDoldurBtn_Click" />
                        </div>
                        <div class="m-1" id="VasiyetciDiv">
                            <div class="row">
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Konusu"></asp:Label>
                                    <asp:DropDownList ID="VasiyetKonusuDDL" runat="server" CssClass="form-control" style="height:auto" OnSelectedIndexChanged="VasiyetKonusuDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Cinsi"></asp:Label>
                                    <asp:DropDownList ID="VasiyetCinsiDDL" runat="server" CssClass="form-control" style="height:auto"></asp:DropDownList>
                                </div>
                                <div class="form-group col-2">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="Adet/Miktar"></asp:Label>
                                    <asp:TextBox ID="AdetMiktarTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                                <div class="form-group col">
                                    <asp:Label CssClass="col-from-label" runat="server" Text="T.Rayiç"></asp:Label>
                                    <asp:TextBox ID="TahminiRayicTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Açıklama"></asp:Label>
                                <asp:TextBox ID="KonuAciklama" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="VarlikKaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="VarlikKaydetBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
