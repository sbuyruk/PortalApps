<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeBagisciKartiWP.ascx.cs" Inherits="BTYS_Webparts.BolgeBagisciKartiWP.BolgeBagisciKartiWP" %>
<div class="container shadow w-75">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Bağışçı Kartı"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" runat="server" id="BagisciCard">
            <div class="form-group">
                <h4>
                    <asp:Label ID="BagisciBilgileriTableLbl" runat="server" class="fw-bold"></asp:Label>
                </h4>
                <div class="table">
                    <asp:Table ID="BagisciBilgileriTable" runat="server" CssClass="table table-sm table-hover table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="form-group">
                <h4>
                    <asp:Label ID="TasinmazTableLbl" runat="server" class="fw-bold"></asp:Label>
                </h4>
                <div class="table">
                    <asp:Table ID="TasinmazTable" runat="server" CssClass="table table-sm table-hover table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="form-group">
                <h4>
                    <asp:Label ID="BagisciTalepleriTableLbl" runat="server" class="fw-bold"></asp:Label>
                </h4>
                <div class="table">
                    <asp:Table ID="BagisciTalepleriTable" runat="server" CssClass="table table-sm table-hover table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="form-group">
                <h4>
                    <asp:Label ID="BagisciYakinlariTableLbl" runat="server" class="fw-bold"></asp:Label>
                </h4>
                <div class="table">
                    <asp:Table ID="BagisciYakinlariTable" runat="server" CssClass="table table-sm table-hover table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="form-group">
                <h4>
                    <asp:Label ID="TaahhutTableLbl" runat="server" class="fw-bold"></asp:Label>
                </h4>
                <div class="table">
                    <asp:Table ID="TaahhutTable" runat="server" CssClass="table table-sm table-hover table-bordered">
                    </asp:Table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton ID="TasinmazBagisciListesiBtn" CssClass="btn btn-outline-secondary float-end mr-2" runat="server" Text="Taşınmaz Bağışçı Listesi" OnClick="TasinmazBagisciListesiBtn_Click" />
        </div>
    </div>
</div>