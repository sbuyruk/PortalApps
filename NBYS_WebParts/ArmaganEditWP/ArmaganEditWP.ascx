<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArmaganEditWP.ascx.cs" Inherits="NBYS_WebParts.ArmaganEditWP.ArmaganEditWP" %>
<div class="container w-50">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Armağan Kaydı Düzenleme"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body" id="MainCardDiv" runat="server">
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="AdiTxt">Bağışçı</label>
                                    <asp:Label ID="AdiTxt" runat="server" class="form-control bg-secondary text-white" type="text"  />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label ">Belgede Yazan İsim</label>
                                    <asp:TextBox ID="BelgedeYazanIsimTxt" runat="server" class="form-control" type="text" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label">Bağış Miktarı </label>
                                    <asp:TextBox ID="BagisMiktariTxt" runat="server" class="form-control input-money text-end" type="text" />
                                </div>
                            </div>
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label">Armağan</label>
                                    <asp:DropDownList ID="ArmaganDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" OnSelectedIndexChanged="ArmaganDLL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label">Bağış Tarihi</label>
                                    <input runat="server" type="text" id="BagisTarihiTxt" name="DogumTarihiTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label">Durum</label>
                                    <asp:DropDownList ID="DurumDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" OnSelectedIndexChanged="DurumDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                                </div>

                            </div>
                        </div>
                        <div>
                            <div class="checkbox">
                                <label>
                                    <asp:CheckBox ID="BagisMiktariYazmasinChk" runat="server" Checked="false" />
                                    Belgede bağış miktarı yazmasın
                                </label>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label">Açıklama</label>
                                <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" class="form-control" type="text" />
                            </div>

                        </div>
                </div>

                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
                    <asp:LinkButton Visible="false" CssClass="btn btn-outline-secondary float-end " ID="SonrakiBtn" runat="server" Text="Sonraki>>" />
                    <asp:LinkButton Visible="false" CssClass="btn btn-outline-secondary float-end me-5" ID="OncekiBtn" runat="server" Text="<<Önceki" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end me-2" ID="ArmaganListesiBtn" runat="server" Text="Armağan Listesi " OnClick="ArmaganListesiBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-secondary float-end me-10" ID="DuzenliBagisciListesiBtn" runat="server" Text="Düzenli Bağışçı Listesi " OnClick="DuzenliBagisciListesiBtn_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
