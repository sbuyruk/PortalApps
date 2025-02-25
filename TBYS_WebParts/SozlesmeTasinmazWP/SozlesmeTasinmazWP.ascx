<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SozlesmeTasinmazWP.ascx.cs" Inherits="TBYS_WebParts.SozlesmeTasinmazWP.SozlesmeTasinmazWP" %>
<style>
    .ui-datatable tbody td {
        white-space: normal;
    }
</style>
<script type="text/javascript">
    function TasinmazSecimiModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('TasinmazSecimiModal'));
        myModalInstance.show();
    }
    function EnvanterdeOlmayanTasinmazSecimiModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('EnvanterdeOlmayanTasinmazSecimiModal'));
        myModalInstance.show();
    }
    function KiraciSecimiModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('KiraciSecDiv'));
        myModalInstance.show();
    }
</script>

<div class="container shadow">
    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Kiraya Verilen Taşınmazlar"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">
            <div class="form-group nopadding">
                <div class="form-group">
                    <asp:Label ID="KiraciAdiSoyadiLbl" runat="server" CssClass="col-form-label" BorderColor="Blue" BorderStyle="Inset"></asp:Label>
                </div>
            </div>
            <div class="table ">
                <asp:Table ID="PopUpTable" runat="server" class="table table-bordered table-hover table-sm">
                    <asp:TableHeaderRow>
                        <asp:TableCell ID="SiraNoCell" CssClass="btn-primary">Sıra</asp:TableCell>
                        <asp:TableCell ID="AdresCell" CssClass="btn-primary" >Adres</asp:TableCell>
                        <asp:TableCell ID="SilCell" CssClass="btn-primary">Çıkar</asp:TableCell>
                    </asp:TableHeaderRow>
                </asp:Table>

            </div>

        </div>
        <div class="card-footer">
            <asp:LinkButton ID="KiraciSecBtn" runat="server" CssClass="btn btn-outline-secondary" Text="Kiracı Seç" OnClick="KiraciSecBtn_Click" />
            <asp:LinkButton ID="TasinmazEkleBtn" runat="server" CssClass="btn btn-outline-secondary" Text="Taşınmaz Ekle" OnClick="TasinmazEkleBtn_Click" />
            <asp:LinkButton ID="EnvanterdeOlmayanTasinmazEkleBtn" runat="server" CssClass="btn btn-outline-secondary" Text="Envanterde Olmayan Taşınmaz Ekle" OnClick="EnvanterdeOlmayanTasinmazEkleBtn_Click" />
            <asp:LinkButton ID="TamamBtn" runat="server" CssClass="btn btn-outline-primary" Text="Tamam" Visible="false" OnClick="TamamBtn_Click"></asp:LinkButton>

        </div>
    </div>
</div>
<!-- Modal -->
<div class="modal" id="TasinmazSecimiModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content" style="width: 1030px;">
            <div class="modal-body ">
                <div class="card">
                    <div class="card-header text-danger">
                        <h3>Taşınmaz Seçimi
                        </h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="upPanel" runat="server">
                            <ContentTemplate>
                                <div class="form-group">
                                    <table id="TasinmazModalDataTable" class="table table-striped row-border" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Kullanım Şekli</th>
                                                <th>Mülkiyet Şekli</th>
                                                <th>Kira Durumu</th>
                                                <th>Adres</th>
                                                <th>Bölüm</th>
                                                <th>Sözleşmeye Ekle</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
<div class="modal" id="EnvanterdeOlmayanTasinmazSecimiModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content" style="width: 1030px;">
            <div class="modal-body ">
                <div class="card">
                    <div class="card-header text-danger">
                        <h3>Taşınmaz Seçimi
                        </h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="form-group">
                                    <table id="EnvanterdeOlmayanTasinmazModalDataTable" class="table table-striped row-border" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Adres</th>
                                                <th>İl</th>
                                                <th>İlçe</th>
                                                <th>Sözleşmeye Ekle</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
<div class="modal" id="KiraciSecimiModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content" style="width: 1030px;">
            <div class="modal-body ">
                <div class="card">
                    <div class="card-header text-danger">
                        <h3>Kiracı Seçimi
                        </h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div class="form-group">
                                    <table id="KiraciModalDataTable" class="table table-striped row-border" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Kiraci</th>
                                                <th>Adres</th>
                                                <th>İl İlçe</th>
                                                <th>Kiracıyı Seç</th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
