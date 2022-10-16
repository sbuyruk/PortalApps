<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraSozlesmeSiralamaWP.ascx.cs" Inherits="TBYS_WebParts.KiraSozlesmeSiralamaWP.KiraSozlesmeSiralamaWP" %>

<style>
    .ui-datatable tbody td{
        white-space:normal;
    }
    .ui-datatable .ui-state-highlight {
        background-image: none;
        background-color: steelblue;
        color:white;
    }
</style>
<script>

    function storeTblValues() {
        var tableData = "";
        $('#tblfilter tr').each(function (row, tr) {
            tableData += 
                $(tr).find('td:eq(1)').text()+","; //kiraSozlesmeId lerini "," ile ayırarak ekle
        });
        return tableData;
    }
    function SiralamayiKaydet() {
        var tableData = storeTblValues();
        document.getElementById('<%= paramDosyaNoArray.ClientID%>').value = tableData;
        document.getElementById('<%= KaydetNowBtn.ClientID%>').click();
    }
</script>


<div class="container shadow">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>


    <div class="card">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Sözleşme Sıralama"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="table loader table-hower table-bordered table-responsive">
                <div id="tblfilter"></div>
                <div id="messages"></div>
            </div>
        </div>
        <div class="card-footer">
            <button id="kaydetBtn" class="btn btn-outline-primary" type="button" onclick="SiralamayiKaydet();">Sıralamayı Kaydet</button>
        </div>
        <div style="display:none">
            <input id="paramDosyaNoArray"  runat="server" type="text" />
            <asp:LinkButton ID="KaydetNowBtn" runat="server" OnClick="KaydetNowBtn_Click">Kaydet</asp:LinkButton>
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
</div>
