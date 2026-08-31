<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisciTaahhutleriWP.ascx.cs" Inherits="TBYS_WebParts.BagisciTaahhutleriWP.BagisciTaahhutleriWP" %>
<script>
    function OpenModalTaahhut(id) {
        document.getElementById('<%= ParamTaahhutIdLbl.ClientID%>').value = id;
        document.getElementById('<%= TaahhutModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('TaahhutModalUrlDiv'));
        myModalInstance.show();
    }
    function OpenTaahhutSilModal(id) {
        document.getElementById('<%= ParamTaahhutIdLbl.ClientID%>').value = id;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalTaahhutSilDiv'));
        myModalInstance.show();
    }
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışçıya Verilen Taahhütler"></asp:Label>
                <asp:Label ID="BagisciIdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="card">
                <h4>
                    <asp:Label ID="TaahhutTableLbl" runat="server" class="col-form-label fw-bold">Bağışçıya Verilen Taahhütler</asp:Label>
                </h4>
                <div class="table">
                    <a href="#" onclick="OpenModalTaahhut(0);" class="btn btn-outline-success m-1">Taahhüt Ekle</a>
                    <asp:LinkButton ID="BagisciyiTaahhutListesineEkleBtn" CssClass="btn btn-outline-success float-end" runat="server" Text="Bağışçıyı Taahhüt Listesine Ekle" OnClick="BagisciyiTaahhutListesineEkleBtn_Click" Visible="False" />
                    <asp:Table ID="TaahhutTable" runat="server" CssClass="table table-striped table-bordered table-hover">
                    </asp:Table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="BagisciBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Bağışçıya Git" OnClick="BagisciBtn_Click" />
        </div>
    </div>
</div>

<div class="modal" id="TaahhutModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="Label1" runat="server" Text="Taahhüt Bilgileri"></asp:Label>
                        </h3>
                    </div>
                    <div style="display: none">
                        <input id="ParamTaahhutIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="TaahhutModalDoldurBtn" runat="server" CssClass="btn btn-success" CausesValidation="False" Text="" OnClick="TaahhutModalDoldurBtnBtn_Click" />
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label ID="BagiscciAdiLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group ">
                            <label class="col-form-label fw-bold" for="TasinmazDDL">Taahhüte Tabi Taşınmaz</label>
                            <asp:DropDownList ID="TasinmazDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="AdiTxt">Adı</label>
                                    <asp:TextBox ID="AdiTxt" runat="server" class="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="SoyadiTxt">Soyadı</label>
                                    <asp:TextBox ID="SoyadiTxt" runat="server" class="form-control " ToolTip="Soyadı" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="AdiTxt">TC Kimlik No</label>
                                    <asp:TextBox ID="TCKimlikNoTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="DogumTarihiTxt">Doğum Tarihi</label>
                                    <asp:TextBox ID="DogumTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy"></asp:TextBox>
                                </div>

                            </div>
                            <div class="form-group col">
                                <div class="form-group ">
                                    <label class="col-form-label fw-bold" for="TelefonTxt">Telefon</label>
                                    <asp:TextBox ID="TelefonTxt" runat="server" CssClass="form-control " ToolTip="Bağışçının telefonu"></asp:TextBox>
                                </div>
                                <div class="form-group ">
                                    <label class="col-form-label fw-bold" for="IliDDL">İkamet İli</label>
                                    <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="IlcesiDDL">İkamet İlçesi</label>
                                    <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="AdresTxt">İkamet Adresi</label>
                                    <asp:TextBox ID="AdresTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="4" ToolTip="İkamet Adresi"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col">
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="SagVefatDDL">Sağ-Vefat</label>
                                    <asp:DropDownList ID="SagVefatDDL" CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SagVefatDDL_SelectedIndexChanged" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group" id="VefatTarihiDiv" runat="server" style="display: none">
                                    <label class="col-form-label fw-bold" for="VefatTarihiTxt">Vefat Tarihi</label>
                                    <input id="VefatTarihiTxt" class="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" runat="server" readonly="readonly">
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="EvrakTarihi">Taahhüt Evrakının Tarihi</label>
                                    <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" ToolTip="Taahhüt Evrakının Tarihi"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="col-form-label fw-bold" for="EvrakTarihi">Taahhüt Evrakının Sayısı</label>
                                    <asp:TextBox ID="EvrakSayisiTxt" runat="server" CssClass="form-control" ToolTip="Taahhüt Evrakının Sayısı"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                        <div class="form-group">
                            <label class="col-form-label fw-bold" for="TaahhutAciklamaTxt">Taahhüt Açıklaması</label>
                            <asp:TextBox ID="TaahhutAciklamaTxt" runat="server" CssClass="form-control " TextMode="MultiLine" Rows="8" ToolTip="Taahhüt Açıklaması"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="TaahhutKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text="Taahhüt Kaydet" OnClick="TaahhutKaydetBtn_Click" />
                        <asp:LinkButton ID="TaahhutGuncelleBtn" runat="server" CssClass="btn btn-primary" CausesValidation="false" Text="Taahhüt Güncelle " OnClick="TaahhutGuncelle_Click" />
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>

<asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="ModalTaahhutSilDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="Label3" runat="server" Text="Taahhüt Silinecek"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">

                        <div class="m-1 text-center">
                            <div class="form-group">
                                <asp:Label ID="Label4" CssClass="col-form-label" runat="server" Text=" Taahhüt Silinsin mi?"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="TaahhutSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Sil" OnClick="TaahhutSilNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
