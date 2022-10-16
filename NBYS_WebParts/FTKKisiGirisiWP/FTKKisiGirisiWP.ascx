<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FTKKisiGirisiWP.ascx.cs" Inherits="NBYS_WebParts.FTKKisiGirisiWP.FTKKisiGirisiWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }

    .ui-datepicker {
        z-index: 9999 !important;
        width: 19em;
        font-size: small;
    }

    .disabled-item {
        background-color: rgba(206, 220, 229, 0.40) !important;
        color: darkgrey !important;
    }
</style>
<script type="text/javascript">
    function OpenSilModal() {
        $("#SilModalDiv").modal({ backdrop: true });
    }
    function ModalUyelikDurumuDegistirAc() {
        $("#ModalUyelikDurumuDiv").modal({ backdrop: true });
    }
</script>

<link rel="stylesheet"  href="/Style Library/tskgv/css/fancybox.css"/>
<script src="/Style Library/tskgv/js/fancybox.umd.js"></script>
<script src="/Style Library/tskgv/js/fancybox.esm.js"></script>
<div class="container ">
    <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-1">
                        <a class=" btn btn-outline-primary float-right mr-4" runat="server" id="YonergeLnk"
                            data-fancybox
                            data-type="pdf"
                            data-width="960"
                            data-height="720"
                            href="">
                            <i class="fa fa-book" aria-hidden="true"></i>
                        </a>
                        <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger font-weight-bold mb-1" Text="FTK Kişi Girişi"></asp:Label>
                        <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label"></asp:Label>
                    </h3>
                </div>
                <div class="card-body alert-secondary">
                    <div class="row">
                        <div class="col-3">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Adı"></asp:Label>
                                <asp:TextBox ID="AdiTxt" CssClass="form-control" runat="server" ></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Lütfen ad giriniz."> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Soyadı"></asp:Label>
                                <asp:TextBox ID="SoyadiTxt" CssClass="form-control" runat="server" OnTextChanged="SoyadiTxt_TextChanged" AutoPostBack="True"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="SoyadiTxt" ForeColor="Red" ErrorMessage="Lütfen soyadı giriniz."> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group ">
                                <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İli</asp:Label>
                                <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="IliDDL" ForeColor="Red" ErrorMessage="Lütfen il seçiniz."> </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İlçesi</asp:Label>
                                <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IlcesiDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="IlcesiDDL" ForeColor="Red" ErrorMessage="Lütfen ilçe seçiniz."> </asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="col-3">
                            <div class="form-group ">
                                <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">FTK Görevi</asp:Label>
                                <asp:DropDownList ID="FTKGoreviDDL" runat="server" CssClass="form-control" Style="height: auto" OnSelectedIndexChanged="FTKGoreviDDL_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Kart No"></asp:Label>
                                <asp:TextBox ID="KartNoTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                            </div>
                            <div id="ValiChkDiv" class="checkbox pt-3" runat="server">
                                <label>
                                    <asp:CheckBox ID="ValiChk" runat="server" Checked="false" ToolTip="Vali ise işaretleyiniz." Enabled="false" />
                                    Vali
                                </label>
                            </div>
                            <div id="KaymakamChkDiv" class="checkbox pt-3" runat="server">
                                <label>
                                    <asp:CheckBox ID="KaymakamChk" runat="server" Checked="false" ToolTip="Kaymakam ise işaretleyiniz." Enabled="false" />
                                    Kaymakam
                                </label>
                            </div>
                        </div>
                        <div class="col-3">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="TC Kimlik No"></asp:Label>
                                <asp:TextBox ID="TCKimlikNoTxt" CssClass="form-control input-tckimlik" runat="server" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Ünvanı"></asp:Label>
                                <asp:TextBox ID="UnvaniTxt" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Doğum Tarihi"></asp:Label>
                                <asp:TextBox ID="DogumTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" placeholder="dd.mm.yyyy" runat="server" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label input-phone" runat="server" Text="Telefon(1)"></asp:Label>
                                <asp:TextBox ID="Telefon1Txt" CssClass="form-control input-phone" runat="server" placeholder="( _ _ _ ) _ _  _ _" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Telefon(2)"></asp:Label>
                                <asp:TextBox ID="Telefon2Txt" CssClass="form-control input-phone" runat="server" placeholder="( _ _ _ ) _ _  _ _" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-3">
                        <div class="form-group">
                            <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">Üyelik Durumu</asp:Label>
                            <asp:DropDownList ID="UyelikDurumuDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                        </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Adres"></asp:Label>
                                <asp:TextBox ID="AdresTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Açıklama"></asp:Label>
                                <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4" Text=""></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="KaydetBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="GuncelleBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="GuncelleBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="SilBtn" CssClass="btn btn-outline-danger ml-5" runat="server" Text="Sil" OnClick="SilBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success ml-5" runat="server" Text="Yeni Kayıt" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FtkKisiListesiBtn" CssClass="btn btn-outline-secondary float-right" runat="server" Text="FTK Kişi Listesi" OnClick="FtkKisiListesiBtn_Click" CausesValidation="False"></asp:LinkButton>
                    <asp:LinkButton ID="FTKFahriBaskanListesiBtn" CssClass="btn btn-outline-secondary float-right mr-3" runat="server" Text="Fahri Başkan Listesi" OnClick="FTKFahriBaskanListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FTKIslemleriBtn" CssClass="btn btn-outline-secondary float-right mr-3" runat="server" Text="FTK İşlemleri" OnClick="FTKIslemleriBtn_Click" CausesValidation="False"></asp:LinkButton>
                </div>
            </div>
            
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="KaydetBtn" />
            <asp:PostBackTrigger ControlID="GuncelleBtn" />
        </Triggers>
    </asp:UpdatePanel>
</div>
<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="SilModalDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="SilModalBaslikLbl" runat="server" Text="Kişi Silinecek"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:Label ID="ParamVnLbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="m-1 text-center" id="MesajDiv">
                            <div class="form-group">
                                <asp:Label ID="SilMesajiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="SilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Kişiyi Sil" OnClick="SilNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SilBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>