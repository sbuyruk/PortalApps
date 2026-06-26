<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SMSAylikCizelgeReadOnlyWP.ascx.cs" Inherits="NBYS_WebParts.SMSAylikCizelgeReadOnlyWP.SMSAylikCizelgeReadOnlyWP" %>
<style>
    .border-2 {
        border-width: 2px !important;
        font-weight: bold !important;
        border-style: inset !important;
    }
</style>
<script>
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }

</script>

<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <asp:Label CssClass="col-form-label text-danger float-end" ID="DosyaNoTxt" runat="server"></asp:Label>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="SMS Aylik Bağış Çizelgesi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body " id="MainCardDiv" runat="server">
                    <div class="form-group col-4 row">
                        <asp:Label CssClass="col-form-label col-2" runat="server" Font-Bold="True">Yıl :</asp:Label>
                        <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control col-6" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" Height="34px"></asp:DropDownList>
                    </div>
                    <div id="SMSTableDiv" class="table" runat="server">
                        <asp:Table ID="AyrintiTable" runat="server" class="table table-bordered table-hover table-striped table-sm ">
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableHeaderCell RowSpan="2" >Aylar</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Turkcell</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Vodafone</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Turk Telekom</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Toplam</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                     <div id="ExcelTableDiv" class="table" runat="server" style="display: none" >
                        <asp:Table ID="ExcelTable" runat="server" class="table table-bordered table-hover table-striped table-sm ">
                            <asp:TableHeaderRow HorizontalAlign="Center">
                                <asp:TableHeaderCell RowSpan="2" >Aylar</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Turkcell</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Vodafone</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Turk Telekom</asp:TableHeaderCell>
                                <asp:TableHeaderCell ColumnSpan="2" >Toplam</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Adedi</asp:TableHeaderCell>
                                <asp:TableHeaderCell CssClass="text-end">Bağış Tutarı</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>