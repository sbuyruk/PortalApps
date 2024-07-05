<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KiraGelirleriAyDokumuWP.ascx.cs" Inherits="TBYS_WebParts.KiraGelirleriAyDokumuWP.KiraGelirleriAyDokumuWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>
<div id="MainContainer" class="container">
    <asp:UpdatePanel ID="upPanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold text-center" ID="TitleLbl" runat="server" Text="YILI AYLIK KİRA GELİRLERİ"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row">
                        <asp:Label CssClass="col-form-label col-1" runat="server" Font-Bold="True">Yıl :</asp:Label>
                        <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control col-2" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                    </div>
                    
                        <asp:Table ID="KiraGelirleriTable" runat="server" class="table table-bordered table-hover table-striped" >
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">S.NO</asp:TableHeaderCell>
                            <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">AYLAR</asp:TableHeaderCell>
                            <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">BÖLGE</asp:TableHeaderCell>
                            <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">KİRACI SAYISI</asp:TableHeaderCell>
                            <asp:TableHeaderCell ColumnSpan="6" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">KİRA ELDE EDİLEN TAŞINMAZIN TÜRÜ</asp:TableHeaderCell>
                            <asp:TableHeaderCell RowSpan="2" BorderStyle="Solid" BorderWidth="2" BorderColor="Black" style="vertical-align:Middle">KİRA GELİRİ</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableHeaderRow HorizontalAlign="Center">
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Mesken</asp:TableHeaderCell>
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">İşyeri</asp:TableHeaderCell>
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Arsa</asp:TableHeaderCell>
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Tarla</asp:TableHeaderCell>
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Bis</asp:TableHeaderCell>
                            <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">Tesis</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
 
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-right" ID="ExcelBtn" runat="server" Text="Excel'e Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ExcelBtn" />
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
