<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AileGirisiWP.ascx.cs" Inherits="IKYS_WebParts.AileGirisiWP.AileGirisiWP" %>
<div class="container shadow w-75">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Aile Bilgileri Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="PersonelIdLbl" Visible="false" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="3" runat="server" ></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Table ID="AileTable" runat="server" class="table table-bordered table-hover table-sm">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="HeaderCell0" CssClass="btn-primary">Sıra</asp:TableCell>
                            <asp:TableCell ID="HeaderCell1" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell2" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell3" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell4" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell5" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell6" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell7" CssClass="btn-primary" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell9" CssClass="btn-primary"></asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                    <div>
                        <div id="EkleDiv1" class="row alignCenter nopadding">
                            <div class="form-group col-sm-2 nopadding">
                                <label for="YakAdiTxt" class="col-form-label">Adi</label>
                                <div>
                                    <asp:TextBox ID="YakAdiTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-sm-2 nopadding">
                                <label for="YakSoyadiTxt" class="col-form-label">Soyadı</label>
                                <div>
                                    <asp:TextBox ID="YakSoyadiTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-sm-2 nopadding">
                                <label for="YakTcKimlikNoTxt" class="col-form-label">TC Kimlik No</label>
                                <div>
                                    <asp:TextBox ID="YakTcKimlikNoTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-sm-2 nopadding">
                                <label for="YakDerecesiDDL" class="col-form-label">Yakınlık</label>
                                <div>
                                    <asp:DropDownList ID="YakDerecesiDDL" runat="server" class="form-control "></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col-sm-4 nopadding">
                                <label for="OkulTxt" class="col-form-label">Okul</label>
                                <div>
                                    <asp:TextBox ID="OkulTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div id="EkleDiv2" class="row alignCenter nopadding">
                            <div class="form-group col-sm-2 nopadding">
                                <label class="col-form-label">Dog.Tarihi</label>
                                <div>
                                    <input runat="server" type="text" id="YakDogumTarTxt" name="YakDogumTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                </div>
                            </div>
                            <div class="form-group col-sm-2 nopadding">
                                <label class="col-form-label" for="YakMeslekDDL">Meslek</label>
                                <div>
                                    <asp:DropDownList ID="YakMeslekDDL" runat="server" class="form-control "></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col-sm-4 nopadding">
                                <label for="YakTelefonTxt" class="col-form-label">Telefon</label>
                                <div>
                                    <asp:TextBox ID="YakTelefonTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-sm-2">
                                <label class="col-form-label"></label>
                                <div>
                                    <asp:LinkButton ID="YakinEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Ekle" OnClick="YakinEkleBtn_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="YakinEkleBtn" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
        </div>
    </div>
</div>
