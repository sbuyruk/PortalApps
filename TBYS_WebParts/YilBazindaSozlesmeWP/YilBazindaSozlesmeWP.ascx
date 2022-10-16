<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YilBazindaSozlesmeWP.ascx.cs" Inherits="TBYS_WebParts.YilBazindaSozlesmeWP.YilBazindaSozlesmeWP" %>
<style>
     /*tblfilter hücre içine sığmazsa wordwrap yapsın*/ 
    .ui-datatable tbody td {
        white-space: normal;
    }
</style>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<asp:UpdatePanel ID="upPanel" runat="server">
    <ContentTemplate>
        <div class="container shadow">

            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Yıla Göre Sözleşme/Tahliye Listesi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body p-0" id="MainCardDiv" runat="server">
                    <div class="form-group row m-2 ">

                        <div class="form-group col-3">
                            <label class="col-form-label" for="YilDDL">Yıl </label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" />
                        </div>
                        <div class="form-group col-3">
                            <label class="col-form-label" for="ArmaganDDL">Sözleşme/Tahliye </label>
                            <asp:DropDownList ID="SozlesmeTahliyeDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="SozlesmeTahliyeDDL_SelectedIndexChanged" />
                        </div>
                    </div>
                    <asp:Label ID="RowCountLbl" runat="server" Text="" CssClass="float-right text-right"></asp:Label>
                    <div class="table loader" id="tbl" runat="server">
                        <input id="globalFilter" placeholder="Aranacak Kelime" size="30" />
                        <div id="tblfilter" class="table"></div>
                        <div id="messages"></div>
                    </div>

                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
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
