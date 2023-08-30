<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TarihBazindaGunlukBagisListesiWP.ascx.cs" Inherits="MFYS_WebParts.TarihBazindaGunlukBagisListesiWP.TarihBazindaGunlukBagisListesiWP" %>
<style>
    /*Tarih seçiminde açılan takvim altta kalmasın*/
    .ui-datepicker {
        z-index: 9999 !important;
        width: 18em;
        font-size: small;
    }
</style>
<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-primary font-weight-bold mb-1" Text="Tarih Bazında Günlük Bağışlar"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <div class="row">
                    <div class="col-2 form-group">
                        <label for="BagisTarihiTxt" class="col-form-label font-weight-bold">Bağış Tarihi: </label>
                        <asp:TextBox ID="BagisTarihiTxt" CssClass="form-control DateTimePickerV1 input-date" runat="server" ClientIDMode="Static" OnTextChanged="BagisTarihiTxt_TextChanged" AutoPostBack="True" placeholder="gg.aa.yyyy"></asp:TextBox>
                    </div>
                    <div class="col-2 form-group ">
                        <label for="BankaDDL" class="col-form-label font-weight-bold">Banka: </label>
                        <asp:DropDownList ID="BankaDDL" runat="server" CssClass="form-control small" Style="height: auto" AutoPostBack="True" OnSelectedIndexChanged="BankaDDL_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-2 form-group ">
                        <label class="col-form-label">Para Birimi</label>
                        <asp:DropDownList ID="DovizCinsiDDL" runat="server" CssClass="form-control small" Style="height: auto" AutoPostBack="True" OnSelectedIndexChanged="DovizCinsiDDLIli_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                    <ContentTemplate>
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Banka</th>
                                    <th>Bağış Tutarı</th>
                                    <th>Bağış Tarihi</th>
                                </tr>
                            </thead>
                        </table>
                        <div class="form-group">
                            <asp:Label ID="ToplamLbl" CssClass="font-weight-bold float-right" runat="server" Text="Label"></asp:Label>
                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="BagisTarihiTxt" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="BankaDDL" EventName="SelectedIndexChanged" />
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
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YenileBtn" CssClass="btn btn-outline-info" runat="server" Text="Yenile" OnClick="YenileBtn_Click"></asp:LinkButton>
        </div>

    </div>
</div>