<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AramaGirisiWP.ascx.cs" Inherits="MTS_WebParts.AramaGirisiWP.AramaGirisiWP" %>

<style>
    .gorusmeSaglanamadi {
        background-color: lightpink !important;
        color: black !important;
    }

    .icKatilimci {
        background-color: lightgray !important;
    }

    .disKatilimci {
        background-color: lightcyan !important;
    }

    .nakitBagisci {
        background-color: lightyellow !important;
    }

    .tasinmazBagisci {
        background-color: wheat !important;
    }
</style>
<script type="text/javascript">
    function OpenModalOnay() {
        $("#ModalOnay").modal({ backdrop: "static" });
    }
    function KatilimciSecimiModal() {
        $("#KatilimciSecimiModal").modal({ backdrop: false });
    }
    function KatilimciSecildiBtnClick(katilimciId, katilimciTipi) {
        document.getElementById('<%= paramRandevuKatilimciIdLbl.ClientID%>').value = katilimciId;
        document.getElementById('<%= paramRandevuKatilimciTipiLbl.ClientID%>').value = katilimciTipi;
        document.getElementById('<%= SecilenKatilimciyiGetirBtn.ClientID%>').click();
    }


</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Yeni Arama/Görüşme Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-1 form-group">
                    <asp:LinkButton ID="KatilimciSecBtn" runat="server" CssClass="btn btn-outline-primary" Text="İsim Seç" OnClick="KatilimciSecBtn_Click" CausesValidation="false" />
                </div>
                <div class="col form-group">
                    <asp:HyperLink ID="AdiSoyadiLnk" runat="server" CssClass="col-form-label font-weight-bold" Enabled="True"></asp:HyperLink>
                </div>

            </div>
            <div class="form-group alert-secondary p-2">
                <div class="form-group ">
                    <div class="row">
                        <div class="col-3 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Arama/Görüşme Şekli</asp:Label>
                            <asp:DropDownList ID="GorusmeSekliDDL" runat="server" CssClass="form-control" style="height:auto"></asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="GorusmeSekliDDL" ForeColor="Red" ErrorMessage="Arama şeklini seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-2 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Tarih</asp:Label>
                            <asp:TextBox ID="TarihTxt" runat="server" CssClass="form-control DateTimePickerV1 input-date" AutoPostBack="True" OnTextChanged="TarihTxt_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="TarihTxt" ForeColor="Red" ErrorMessage="Tarih seçiniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-1 form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Saat </asp:Label>
                            <asp:TextBox ID="SaatTxt" runat="server" class="form-control input-time" placeholder="hh:mm"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="SaatTxt" ForeColor="Red" ErrorMessage="Saat giriniz"> </asp:RequiredFieldValidator>
                        </div>
                        <div class="col form-group">
                            <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Konu</asp:Label>
                            <asp:TextBox ID="KonuTxt" runat="server" CssClass="form-control "></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="KonuTxt" ForeColor="Red" ErrorMessage="Konu Giriniz"> </asp:RequiredFieldValidator>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-3 form-group">
                            <div class="checkbox">
                                <label>
                                    <asp:CheckBox ID="GorusmeSaglandiChk" runat="server" Checked="false" />
                                    Görüşme Sağlandı
                                </label>
                            </div>
                        </div>
                        <div class="col-3 form-group">
                            <div class="checkbox">
                                <label>
                                    <asp:CheckBox ID="RandevuIstendiChk" runat="server" Checked="false" OnCheckedChanged="RandevuIstendiChk_CheckedChanged" AutoPostBack="true" />
                                    Randevu İstendi
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="col-form-label font-weight-bold">Görüşme Açıklaması </asp:Label>
                <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
        </div>
        <div class="card-footer">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
                    <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click" />
                    <asp:LinkButton ID="SilBtn" CssClass="btn btn-outline-danger" runat="server" Text="Sil" OnClick="SilBtn_Click" />
                    <asp:LinkButton ID="RandevuBtn" CssClass="btn btn-outline-secondary" runat="server" CausesValidation="false" Text="Randevu" OnClick="RandevuBtn_Click" Visible="False" />
                    <asp:LinkButton ID="YeniKisiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="Yeni Kişi Girişi" OnClick="YeniKisiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="RandevuTakvimiBtn" CssClass="btn btn-outline-info float-right" runat="server" CausesValidation="false" Text="Faaliyet Takvimi" OnClick="RandevuTakvimiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="RandevuListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" CausesValidation="false" Text="Faaliyet Listesi" OnClick="RandevuListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="KisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" CausesValidation="false" Text="Kişi Listesi" OnClick="KisiListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="AramaListesi" CssClass="btn btn-outline-secondary float-right" runat="server"  CausesValidation="false" Text="Arama/Görüşme Listesi" OnClick="AramaListesiBtn_Click"></asp:LinkButton>
                </ContentTemplate>

            </asp:UpdatePanel>
        </div>
    </div>
    <div id="KatilimciHiddenDiv" style="display: none">
        <input id="paramRandevuKatilimciIdLbl" runat="server" type="text" />
        <input id="paramRandevuKatilimciTipiLbl" runat="server" type="text" />
        <asp:LinkButton ID="SecilenKatilimciyiGetirBtn" runat="server" CausesValidation="false" Text="Faaliyete Ekle" OnClientClick="{return true;};" OnClick="SecilenKatilimciyiGetirBtn_Click" />
    </div>

    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
        <ContentTemplate>
            <div class="modal " id="KatilimciSecimiModal" role="dialog">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-body ">

                            <div class="card">
                                <div class="card-header text-danger">
                                    <h3 class="col-form-label font-weight-bold" id="KatiliciSecimiHeaderLbl" runat="server">Katılımcı Seçimi
                                    </h3>
                                </div>
                                <div class="card-body">
                                    <div class="card-body p-0" id="Div1" runat="server">
                                        <div class="form-group">
                                            <table id="CustomModalDataTable" class="table table-striped table-bordered" width="100%">
                                                <thead>
                                                    <tr>
                                                        <th>Adı Soyadi</th>
                                                        <th>Kurumu</th>
                                                        <th>Seç</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="KatilimciSecBtn" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
    <div class="modal" id="ModalOnay" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <h3>
                                    <asp:Label ID="ModalLbl" class="col-form-label text-primary font-weight-bold" Text="" runat="server"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <asp:Label ID="MessageLbl" runat="server" class="col-form-label"></asp:Label>
                                    <asp:HiddenField ID="kaydetGuncelleSilHdn" runat="server" />
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton ID="OnaylaBtn" Text="Onayla" runat="server" class="btn btn-outline-primary" OnClick="OnaylaBtn_Click"></asp:LinkButton>
                                <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">İptal</button>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GuncelleBtn" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="RandevuBtn" EventName="click" />
                        <asp:AsyncPostBackTrigger ControlID="SilBtn" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</div>

