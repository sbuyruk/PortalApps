<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazArmaganWP.ascx.cs" Inherits="NBYS_WebParts.TasinmazArmaganWP.TasinmazArmaganWP" %>
<div class="container shadow">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-info font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Armağan"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-4">
                            <div class="card">
                                <div class="card-header">
                                    <asp:Label ID="BagisciLbl" runat="server" CssClass="col-form-label" Text="Bağışçı"></asp:Label>
                                </div>
                                <div class="card-body">
                                    <div class="form-group">
                                        <asp:Label ID="Label1" runat="server" CssClass="col-form-label" Text="Adı-Soyadı"></asp:Label>
                                        <asp:TextBox ID="AdiSoyadiTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label2" runat="server" CssClass="col-form-label" Text="Telefon"></asp:Label>
                                        <asp:TextBox ID="TelefonTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label14" runat="server" CssClass="col-form-label" Text="İkamet İli"></asp:Label>
                                        <asp:TextBox ID="IliTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label3" runat="server" CssClass="col-form-label" Text="Adresi"></asp:Label>
                                        <asp:TextBox ID="BagisciAdresiTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="card">
                                <div class="card-header">
                                    <asp:Label ID="Label10" runat="server" CssClass="col-form-label" Text="Taşınmaz"></asp:Label>
                                </div>
                                <div class="card-body">
                                    <div class="form-group">
                                        <asp:Label ID="Label4" runat="server" CssClass="col-form-label" Text="Cinsi"></asp:Label>
                                        <asp:TextBox ID="TasinmazCinsiTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label5" runat="server" CssClass="col-form-label" Text="Tahmini Rayiç"></asp:Label>
                                        <asp:TextBox ID="TahminiRayicTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label13" runat="server" CssClass="col-form-label" Text="Bağış Tarihi"></asp:Label>
                                        <asp:TextBox ID="BagisTarihiTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label6" runat="server" CssClass="col-form-label" Text="Adresi"></asp:Label>
                                        <asp:TextBox ID="TasinmazAdresiTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="card alert-success">
                                <div class="card-header">
                                    <asp:Label ID="Label11" runat="server" CssClass="col-form-label" Text="Armağan"></asp:Label>
                                </div>
                                <div class="card-body">
                                    <div class="form-group">
                                        <asp:Label ID="Label7" runat="server" CssClass="col-form-label" Text="Belge No"></asp:Label>
                                        <asp:TextBox ID="ArmaganIdTxt" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label8" runat="server" CssClass="col-form-label" Text="Armağan Durumu"></asp:Label>
                                        <asp:DropDownList ID="DurumDDL" runat="server" CssClass="form-control" style="height:auto" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label12" runat="server" CssClass="col-form-label" Text="Belge Tarihi"></asp:Label>
                                        <input type="text" id="ArmaganTarihiTxt" name="ArmaganTarihiTxt" class="form-control DateTimePickerV1 " runat="server" readonly="readonly" />
                                        <asp:RequiredFieldValidator ID="rfvBagis" runat="server" ControlToValidate="ArmaganTarihiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="Label9" runat="server" CssClass="col-form-label" Text="Açıklama"></asp:Label>
                                        <asp:TextBox ID="AciklamaTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="KaydetBtn" runat="server" OnClick="KaydetBtn_Click">Kaydet</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-outline-primary" ID="BelgeBasimiBtn" runat="server" OnClick="BelgeBasimiBtn_Click" CausesValidation="false">Belge Basımı</asp:LinkButton>
                    <asp:LinkButton CssClass="btn btn-outline-secondary" ID="ArmaganListBtn" runat="server" OnClick="ArmaganListBtn_Click" CausesValidation="false">Taşınmaz Armağan Listesi</asp:LinkButton>
                    
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="updateProgress" runat="server">
        <ProgressTemplate>
            <div class='loaderMainContainer'>
                <div class='loaderContainer'>
                    <div class='loaderCircle'></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</div>
