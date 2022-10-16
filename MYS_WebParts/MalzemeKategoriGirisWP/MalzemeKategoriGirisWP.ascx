<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MalzemeKategoriGirisWP.ascx.cs" Inherits="MYS_WebParts.MalzemeKategoriGirisWP.MalzemeKategoriGirisWP" %>
<div class="container shadow  w-25">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Malzeme Kategorisi Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="KategoriAdiLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label text-secondary float-right" ID="EkranNo" Text="1" runat="server" ></asp:Label>
                    </h3>
                </div>
                <div class="card-body alert-secondary" id="MainCardDiv" runat="server">
                    <div class="form-group m-0">
                        <label class="col-form-label" for="AdiTxt">Kategori Adi</label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                        <asp:TextBox ID="AdiTxt" runat="server" class="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                    </div>
                    <div class="form-group m-0">
                        <label class="col-form-label" for="AdiTxt">Kategori Kısaltması</label>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                        <asp:TextBox ID="KisaAdiTxt" runat="server" class="form-control" ToolTip="Kısaltması" type="text"></asp:TextBox>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success float-left" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary float-left" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger float-left" runat="server" Text="Sil" OnClick="DeleteBtn_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>