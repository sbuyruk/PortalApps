<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisciEditWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisciEditWP.NakitBagisciEditWP" %>

<div class="container w-50">
    <div class="card row shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="LinkButton1" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışçı Bilgileri Değiştirme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-group mt-1 text-end">
            <div class="col-6 mt-1">
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
                <div class="form-group row mb-1">
                    <label class="col-4 col-form-label form-control-label">Adres</label>
                    <div class="col-8">
                        <asp:TextBox ID="AdresTxt" TextMode="MultiLine" Rows="4" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                 <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Posta Kodu</label>
                    <div class="col-8">
                        <asp:TextBox ID="PostaKoduTxt" runat="server" class="form-control" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Ulaşılamıyor</label>
                    <div class="col-8">
                        <asp:CheckBox ID="UlasilamiyorChk" runat="server" class="form-control custom-checkbox" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Belge İstemiyor</label>
                    <div class="col-8">
                        <asp:CheckBox ID="BelgeIstemiyorChk" runat="server" class="form-control custom-checkbox" type="text" />
                    </div>
                </div>
               

                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Sağ mı</label>
                    <div class="col-8">
                        <asp:CheckBox ID="SagChk" runat="server" class="form-control custom-checkbox" type="text" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">Dergi Gönderilmesin</label>
                    <div class="col-8">
                        <asp:CheckBox ID="DergiGonderilmesinChk" runat="server" class="form-control custom-checkbox" type="text" />
                    </div>
                </div>
            </div>
            <div class="col-6 mt-1">
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">İl</label>
                    <div class="col-8">
                        <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto" />
                    </div>
                </div>
                <div class="form-group row ">
                    <label class="col-4 col-form-label form-control-label">İlçe</label>
                    <div class="col-8">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control" style="height:auto"></asp:DropDownList>
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
                    <label class="col-4 col-form-label form-control-label">Tüzel Kişi</label>
                    <div class="col-8">
                        <asp:CheckBox ID="TuzelKisiChk" runat="server" class="form-control custom-checkbox" type="text" />
                    </div>
                </div>
                <div class="form-group row mb-1">
                    <label class="col-4 col-form-label form-control-label">Açıklama</label>
                    <div class="col-8">
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
