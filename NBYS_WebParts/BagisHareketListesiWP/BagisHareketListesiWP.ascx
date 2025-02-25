<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisHareketListesiWP.ascx.cs" Inherits="NBYS_WebParts.BagisHareketListesiWP.BagisHareketListesiWP" %>
<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

</script>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
</script>
<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Bağış Listesi (Bağış Hareketleri)"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                
                <div class="card-body mt-1">
                    <div class="row">
                        <div class="form-group col">
                            <label for="AyDDL" class="col-form-label text-end ">Ay</label>
                            <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col">
                            <label for="YilDDL" class="col-form-label text-end">Yıl</label>
                            <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col">
                            <label for="IlilDDL" class="col-form-label text-end">İl</label>
                            <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" style="height:auto"></asp:DropDownList>
                        </div>
                        <div class="form-group col">
                            <div class="float-end">
                                <asp:Label ID="RowCountLbl" runat="server" Text="" ForeColor="Blue"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>Bağışçı No</th>
                                    <th>Adı Soyadı</th>
                                    <th>Bağış Tarihi</th>
                                    <th>Bağış Tutarı</th>
                                    <th>İl</th>
                                    <th>TC Kimlik No</th>
                                    <th>Telefon</th>
                                    <th>Banka</th>
                                    <th>Açıklama</th>
                                </tr>
                            </thead>
                        </table>
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
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
