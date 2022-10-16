<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBagisWP.TasinmazBagisWP" %>
<div class="container">
    <div class="card">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <a class=" btn btn-outline-primary float-right mr-4" runat="server" id="YonergeLnk"
                    data-fancybox
                    data-type="pdf"
                    data-width="960"
                    data-height="720"
                    href="">
                    <i class="fa fa-book" aria-hidden="true"></i>
                </a>
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Bağış Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">

            </div>
            <div class="table loader">
                <asp:Table ID="BagisTable" runat="server" class="table table-striped table-bordered">
                </asp:Table>
            </div>
            <div class="row">
                <div class="col">
                    <div class="table">
                        <table id="CustomDataTableBagisci" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>B.No</th>
                                    <th>Adı </th>
                                    <th>Soyadı</th>
                                    <th>Adres</th>
                                    <th>İl</th>
                                    <th>İlçe</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="col">
                    <div class="table">
                        <table id="CustomDataTableTasinmaz" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>T.No</th>
                                    <th>Cinsi</th>
                                    <th>Adres</th>
                                    <th>İl</th>
                                    <th>İlçe</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-footer">

        </div>
    </div>
</div>