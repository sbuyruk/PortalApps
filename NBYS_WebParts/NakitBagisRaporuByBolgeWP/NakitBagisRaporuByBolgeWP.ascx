<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NakitBagisRaporuByBolgeWP.ascx.cs" Inherits="NBYS_WebParts.NakitBagisRaporuByBolgeWP.NakitBagisRaporuByBolgeWP" %>
<script>
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<div class="container ">

    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" runat="server" Text="Bölgelere Göre Nakit Bağış Raporu"></asp:Label>
            </h3>
        </div>
        <div class="row m-2">
            <div class="col-md-2 col-2">
                <label for="YilDDL" class="form-label fw-semibold">Yıl</label>
                <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-select form-select-lg"
                    OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>
        </div>


        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                <div class="card-body">
                    <asp:Table ID="NBTable" runat="server" class="loader table table-bordered table-hover table-striped">
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell ID="TableHeaderCell" runat="server" CssClass="btn-light fw-bold" ColumnSpan="13"></asp:TableCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell CssClass="btn-light fw-bold" RowSpan="2">Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">Ankara Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">İstanbul Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">İzmir Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">Mersin Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">Erzurum Bölge</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">Yurtdışı</asp:TableCell>
                            <asp:TableCell CssClass="btn-light fw-bold" ColumnSpan="2">Toplam</asp:TableCell>

                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Adet</asp:TableCell>
                            <asp:TableCell CssClass="btn-light">Tutar</asp:TableCell>

                        </asp:TableHeaderRow>
                    </asp:Table>

                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="YilDDL" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-warning float-end" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />

        </div>
    </div>

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
