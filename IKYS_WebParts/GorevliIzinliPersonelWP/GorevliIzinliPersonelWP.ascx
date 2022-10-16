<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevliIzinliPersonelWP.ascx.cs" Inherits="IKYS_WebParts.GorevliIzinliPersonelWP.GorevliIzinliPersonelWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="container shadow">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-primary mb-1" ID="TitleLbl" runat="server" Text="Görevli/İzinli Personel Listesi"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">
            <div class="card mb-1">
                <div style="max-height: 400px; overflow: auto;">
                    <asp:Table ID="GorevOnayTable" runat="server" CssClass="table table-striped table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="card mb-2">
                <div style="max-height: 400px; overflow: auto;">
                    <asp:Table ID="RaporluPerTable" runat="server" CssClass="table table-striped table-bordered">
                    </asp:Table>
                </div>
            </div>
            <div class="card mb-2">
                <div style="max-height: 400px; overflow: auto;">
                    <asp:Table ID="IzinliPerTable" runat="server" CssClass="table table-striped table-bordered">
                    </asp:Table>
                </div>
                 <div  id="MazeretDiv"  runat="server" style="max-height: 400px; overflow: auto; display: none">
                    <asp:Table ID="MazeretIzinliPerTable" runat="server" CssClass="table table-striped table-bordered">
                    </asp:Table>
                </div>
            </div>
        </div>
        <div class="card-footer" id="ExcelDiv" runat="server" style="display: none">
            <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
