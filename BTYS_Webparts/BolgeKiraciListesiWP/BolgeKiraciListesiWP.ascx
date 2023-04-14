<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeKiraciListesiWP.ascx.cs" Inherits="BTYS_Webparts.BolgeKiraciListesiWP.BolgeKiraciListesiWP" %>
<style>
    .pasif-kiraci {
        background-color: lightgrey !important;
        color: black !important;
    }
</style>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="col-xl">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Kiracı Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body mt-1">
            <div class="row form-group m-0 mb-3 p-2 border">
                <label class="col-form-label col-2" for="KiraciDDL">Kiracı Seçimi</label>
                <div class="col-3">
                    <asp:DropDownList ID="KiraciSecimiDDL" runat="server" class="form-control" Style="height: auto" AutoPostBack="true" OnSelectedIndexChanged="KiraciSecimiDDL_SelectedIndexChanged"></asp:DropDownList>
                </div>
            </div>
            <div class="form-group">
                            <asp:UpdatePanel ID="upPanel" runat="server">
                <ContentTemplate>
                <table id="CustomDataTable" class="table table-hover row-border" width="100%">
                    <thead>
                        <tr>
                            <th>Kiracı No</th>
                            <th>Adı/Ünvanı</th>
                            <th>Adresi</th>
                            <th>İlçe/İl</th>
                            <th>KiraBedeli</th>
                            <th>Ödeme Şekli</th>
                            <th>Kira Kartı</th>
                        </tr>
                    </thead>
                </table>
            </div>
                                    </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="KiraciSecimiDDL" EventName="SelectedIndexChanged" />
                </Triggers>
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
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
