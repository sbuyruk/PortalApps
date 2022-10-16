<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisciYakinlariWP.ascx.cs" Inherits="TBYS_WebParts.BagisciYakinlariWP.BagisciYakinlariWP" %>
<div class="container shadow w-75">
    <div class="card">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışçi Yakınları"></asp:Label>
                <asp:Label ID="BagisciIdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <h4>
                    <asp:Label ID="TableLbl" runat="server" class="label label-default"></asp:Label>
                </h4>
                <div class="table border-bottom" style="max-height: 200px; overflow: auto;">
                    <asp:Table ID="BagisciYakinlariTable" runat="server" CssClass="table table-sm small table-hover">
                    </asp:Table>
                </div>
            </div>
            <div id="EkleDiv" class="card-body row" >
                    <div class="form-group col-sm-3">
                        <label for="AdsoyadTxt" class="control-label">Adı Soyadı</label>
                        <div>
                            <asp:TextBox ID="AdsoyadTxt" runat="server" class="form-control small"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group col-sm-3">
                        <label for="TelefonTxt" class="control-label">Telefon</label>
                        <div>
                            <asp:TextBox ID="TelefonTxt" runat="server" class="form-control small"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group col-sm-4">
                        <label for="YakinlikDerecesiTxt" class="control-label">Yakınlık Derecesi</label>
                        <div>
                            <asp:TextBox ID="YakinlikDerecesiTxt" runat="server" class="form-control small"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="form-group col-sm-2">
                        <label class="control-label"></label>
                        <div>
                            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Yakın Ekle" OnClick="SaveBtn_Click"  />
                        </div>
                    </div>
                    
                </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="BackBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Geri" OnClick="BackBtn_Click" />
        </div>
    </div>
</div>
