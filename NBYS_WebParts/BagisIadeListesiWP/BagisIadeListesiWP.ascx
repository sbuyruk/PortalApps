<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisIadeListesiWP.ascx.cs" Inherits="NBYS_WebParts.BagisIadeListesiWP.BagisIadeListesiWP" %>

<script type="text/javascript">
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>

<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="İade Edilen Bağışlar Listesi "></asp:Label>
            </h3>
        </div>
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                <div class="card-body mt-1">
                    <div class="row">
                        <div class="form-group col-2">
                            <label for="AyDDL" class="col-form-label fw-bold">İade Ayı</label>
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select fw-bold" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col-2">
                            <label for="YilDDL" class="col-form-label fw-bold">İade Yılı</label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select fw-bold" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group mt-2">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>İade Tarihi</th>
                                    <th>İade Miktarı</th>
                                    <th>İade Sebebi</th>
                                    <th>Bağışçı</th>
                                    <th>Bağış Tarihi</th>
                                    <th>Banka</th>
                                    <th>Adres</th>
                                    <th>İl</th>
                                    <th>İlçe</th>
                                </tr>
                            </thead>
                        </table>
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
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
