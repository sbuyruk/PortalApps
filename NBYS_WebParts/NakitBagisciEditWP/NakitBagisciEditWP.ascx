<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciEditWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciEditWP.NakitBagisciEditWP" %>

<style>
    .nb-edit-wrap { --nb-primary: #2a8fd6; }
/*    .nb-edit-wrap .card { border: none; border-radius: 10px; overflow: hidden; }
    .nb-edit-wrap .card-header { background: linear-gradient(90deg, #1b6fb5, #2a8fd6); border-bottom: none; padding: 14px 20px; }
    .nb-edit-wrap .card-header .close { color: #fff; opacity: .85; text-shadow: none; float: right; }
    .nb-edit-wrap .card-header .close:hover { opacity: 1; }
    .nb-edit-wrap .card-header h3 { margin: 0; }
    .nb-edit-wrap .card-header #TitleLbl { color: #fff !important; }
    .nb-edit-wrap .card-header #IdLbl { background: rgba(255,255,255,.2); border-radius: 20px; padding: 2px 10px; font-size: 13px; margin-left: 8px; }
    .nb-edit-wrap .card-body-custom { background: #fff; padding: 22px 26px 6px 26px; }*/
    .nb-edit-wrap .form-group.row { margin-bottom: 16px; }
    .nb-edit-wrap label.form-control-label { font-weight: 600; color: #33475b; text-align: left; }
    .nb-edit-wrap .form-control { border-radius: 6px; border: 1px solid #cfd7e3; }
    .nb-edit-wrap .form-control:focus { border-color: var(--nb-primary); box-shadow: 0 0 0 3px rgba(42,143,214,.15); }
    .nb-edit-wrap .durum-section { border-top: 1px solid #eef1f5; margin-top: 6px; padding-top: 14px; }
    .nb-edit-wrap .durum-section .section-title { text-transform: uppercase; font-size: 12px; letter-spacing: .5px; color: #7d8ea3; margin-bottom: 10px; }
    .nb-edit-wrap .durum-section .form-check { display: flex; align-items: center; gap: 8px; margin-bottom: 10px; }
    .nb-edit-wrap .durum-section .form-check input[type=checkbox] { width: 18px; height: 18px; accent-color: var(--nb-primary); margin: 0; }
    .nb-edit-wrap .durum-section .form-check label { margin: 0; font-weight: 500; color: #33475b; }
    /*.nb-edit-wrap .card-footer { background: #fff; border-top: 1px solid #eef1f5; padding: 16px 26px; }*/
</style>
<div class="container w-50 nb-edit-wrap">
    <div class="card row shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="LinkButton1" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışçı Bilgileri Değiştirme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-group card-body-custom text-end">
            <div class="col me-3">
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Ad/Ünvan</label>
                    <div class="col-8">
                        <asp:TextBox ID="AdiTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">TC Kimlik No</label>
                    <div class="col-8">
                        <asp:TextBox ID="TCKimlikNoTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Meslek</label>
                    <div class="col-8">
                        <asp:TextBox ID="MeslekTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row mb-1">
                    <label class="col-4 col-form-label form-control-label">Adres</label>
                    <div class="col-8">
                        <asp:TextBox ID="AdresTxt" TextMode="MultiLine" Rows="6" runat="server" class="form-control" type="text" />
                    </div>
                </div>

            </div>
            <div class="col me-3">
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">İl</label>
                    <div class="col-8">
                        <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">İlçe</label>
                    <div class="col-8">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control form-select form-select-lg fw-bold" style="height:auto"></asp:DropDownList>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="IliDDL" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Telefon1</label>
                    <div class="col-8">
                        <asp:TextBox ID="Telefon1Txt" runat="server" class="form-control" type="text" />
                    </div>
                </div>

                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Telefon2</label>
                    <div class="col-8">
                        <asp:TextBox ID="Telefon2Txt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">EPosta</label>
                    <div class="col-8">
                        <asp:TextBox ID="EPostaTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Posta Kodu</label>
                    <div class="col-8">
                        <asp:TextBox ID="PostaKoduTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
            </div>
            <div class="col-12 durum-section text-start">
                <div class="section-title">Durum Bilgileri</div>
                <div class="row">
                    <div class="col-6 col-md-4 form-check">
                        <asp:CheckBox ID="UlasilamiyorChk" runat="server" />
                        <label for="UlasilamiyorChk">Ulaşılamıyor</label>
                    </div>
                    <div class="col-6 col-md-4 form-check">
                        <asp:CheckBox ID="TuzelKisiChk" runat="server" />
                        <label for="TuzelKisiChk">Tüzel Kişi</label>
                    </div>
                    <div class="col-6 col-md-4 form-check">
                        <asp:CheckBox ID="BelgeIstemiyorChk" runat="server" />
                        <label for="BelgeIstemiyorChk">Belge İstemiyor</label>
                    </div>
                    <div class="col-6 col-md-4 form-check">
                        <asp:CheckBox ID="SagChk" runat="server" />
                        <label for="SagChk">Sağ mı</label>
                    </div>
                    <div class="col-6 col-md-4 form-check">
                        <asp:CheckBox ID="DergiGonderilmesinChk" runat="server" />
                        <label for="DergiGonderilmesinChk">Dergi Gönderilmesin</label>
                    </div>
                </div>
            </div>
            <div class="col-12 text-start mt-2">
                <div class="form-group row mb-1">
                    <label class="col-2 col-form-label form-control-label">Açıklama</label>
                    <div class="col-10">
                        <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="4" class="form-control" type="text" />
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <%--<asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="NakitBagisciListesiBtn" runat="server" Text="Nakit Bağışçı Listesi" OnClick="NakitBagisciListesiBtn_Click" />--%>
        </div>
    </div>
</div>
