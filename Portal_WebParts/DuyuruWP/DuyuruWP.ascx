<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuyuruWP.ascx.cs" Inherits="Portal_WebParts.DuyuruWP.DuyuruWP" %>
<script type="text/javascript">
    /* SET COOKIE */
    function OpenDuyuruModal(clicked) {
        $("#DuyuruPopupDiv").modal({ backdrop: false });
    }

</script>

<div id="announcementSliderContainer" class="cf" onclick="OpenDuyuruModal(true);">
    <div class="title">
        <h4>
            <asp:Label ID="lblTitle" runat="server">Duyuru</asp:Label></h4>
    </div>
    <div class="inner">
        <div class="announcement-slider"  onclick="OpenDuyuruModal(true);">
            <asp:Repeater runat="server" ID="DuyuruRepeater">
                <ItemTemplate>
                    <div class="item">
                        <a href='<%#((System.Data.DataRowView)Container.DataItem)["Link"]%>' target="_popup"><%#((System.Data.DataRowView)Container.DataItem)["Subject"]%></a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <asp:Panel ID="EklePanel" runat="server">

        <div id="announcementSlideAdd" class="cf">
            <asp:HyperLink CssClass="ann-add-button" Target="_popup" ID="YeniHyperLink" runat="server">Duyuru Ekle</asp:HyperLink>
        </div>

    </asp:Panel>


</div>
<asp:Label ID="lblHata" runat="server" Visible="False"></asp:Label><br />

<div id="DuyuruPopupDiv" class="modal" role="dialog">
    <div class="modal-dialog">

        <%-- Duyuru Popup--%>
        <div class="card">
            <div class="card-header">
                <div class="form-group">
                    <h5>
                        <asp:Label CssClass="col-form-label text-success font-weight-bold" runat="server" Text="Duyuru"></asp:Label>
                    </h5>
                </div>
            </div>
            <div class="card-body">
                <div class="card" id="DuyuruDiv" runat="server" style="display: none">
                    <div class="card-header">
                        <div class="form-group">
                            <h5>
                                <asp:Label ID="DuyuruTitleLbl" CssClass="col-form-label text-success font-weight-bold" runat="server"></asp:Label>
                            </h5>
                        </div>
                    </div>
                    <div class="card-body">
                        <div id="ImageDiv" class="text-center">
                            <img id="DuyuruImg" class="rounded img-fluid" runat="server" src="~/OrtakResimler/Duyuru.jpg" />
                        </div> 
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
