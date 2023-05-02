<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DavaDurusmaListesiWP.ascx.cs" Inherits="BTYS_Webparts.DavaDurusmaListesiWP.DavaDurusmaListesiWP" %>

<div class="col-xl">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Dava Duruşma Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group col-2">
                <asp:Label CssClass="col-form-label" runat="server" for="DavaDurumuDDL">Dava Durumu</asp:Label>
                <asp:DropDownList ID="DavaDurumuDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="DavaDurumuDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
            </div>
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                            <thead>
                                <tr>
                                    <th>Sıra No</th>
                                    <th>Duruşma İli</th>
                                    <th>Esas No</th>
                                    <th>Dava Türü</th>
                                    <th>Mahkeme</th>
                                    <th>Karşı Taraf</th>
                                    <th>Tarih ve Saat</th>
                                    <th>Dava Durumu</th>
                                    <th>Açıklama</th>
                                </tr>
                            </thead>
                        </table>
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
        <div class="card-footer">
        </div>
    </div>

</div>