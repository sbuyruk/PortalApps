<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TesekkurBasimiWP.ascx.cs" Inherits="NBYS_WebParts.TesekkurBasimiWP.TesekkurBasimiWP" %>
<style>
    
    .durumTable {
        height: 230px;
    }

    input[type=button], input[type=reset], input[type=submit], button {
        font-size: 1.2rem !important;
    }
</style>
<div class="container shadow w-50">
    <div class="card-header ">
        <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
        <h3 class="mb-1">
            <asp:Label CssClass="form-label btn-outline-info" runat="server" Text="Teşekkür Belgesi Basımı"></asp:Label>
        </h3>
    </div>
    <asp:UpdatePanel ID="upPanel" runat="server" class="text-center p-2">
        <ContentTemplate>
            <div class="row mt-2">
                <div class="form-group col">
                    <label class="form-label" for="AyDDL">Ay </label>
                    <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
                <div class="form-group col">
                    <label class="form-label" for="YilDDL">Yıl </label>
                    <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control form-select form-select-lg fw-bold" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto"/>
                </div>
            </div>
            <div class="card bg-info mb-2">
                <div class="card">
                    <div class="card-body ">
                        <asp:Table ID="TesekkurTable" runat="server" class="table table-sm table-striped">
                        </asp:Table>
                    </div>
                    <div class="card-footer">
                        <asp:CheckBox ID="TesekkurDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                        <asp:Button ID="TesekkurBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Teşekkür Belgeleri" OnClick="TesekkurBtn_Click" />
                        <asp:Button ID="AdresEtiketBtn" CssClass="btn btn-outline-successs" runat="server" Enabled="false" Text="Adres Etiketleri" OnClick="AdresEtiketBtn_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AyDDL" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="YilDDL" EventName="SelectedIndexChanged" />
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
