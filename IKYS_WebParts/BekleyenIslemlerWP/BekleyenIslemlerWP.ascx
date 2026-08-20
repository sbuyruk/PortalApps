<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BekleyenIslemlerWP.ascx.cs" Inherits="IKYS_WebParts.BekleyenIslemlerWP.BekleyenIslemlerWP" %>

<link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">

<script>
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    function OpenModalReddet() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalReddetDiv'));
        myModalInstance.show();
    }
    function OpenModalIncele() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalInceleDiv'));
        myModalInstance.show();
    }
    function OpenOnayla(gorevOnayId) {
        document.getElementById('<%= paramGorevOnayIdLbl.ClientID%>').value = gorevOnayId;
        document.getElementById('<%= ModalOnaylaBtn.ClientID%>').click();
        OpenModalOnay();
    }
    function OpenReddet(gorevOnayId) {
        document.getElementById('<%= paramGorevOnayIdLbl.ClientID%>').value = gorevOnayId;
        document.getElementById('<%= ModalReddetBtn.ClientID%>').click();
        OpenModalReddet();
    }
    function OpenIncele(gorevOnayId) {
        document.getElementById('<%= paramGorevOnayIdLbl.ClientID%>').value = gorevOnayId;
        document.getElementById('<%= ModalInceleBtn.ClientID%>').click();
        OpenModalIncele();
    }
</script>
<style>
    
    .section-title {
        font-weight: 600;
        color: #198754;
        margin-bottom: .75rem;
        display: flex;
        align-items: center;
        gap: .5rem;
        border-bottom: 1px solid #e9ecef;
        padding-bottom: .4rem;
    }
</style>
<div class="container small">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-primary fw-bold mb-1" Text="Bekleyen İşlemler"></asp:Label>
                <asp:Label ID="EkranNo" runat="server" CssClass="col-form-label text-secondary float-end" Text=""></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="section-title"><i class="bi bi-person-badge"></i> Amir Onayı Bekleyen Yurt İçi / Yurt Dışı GörevlerX</div>
            <div class="form-group" id="MesajDiv" runat="server" style="display: none;">
                <asp:Label ID="MesajLbl" runat="server" CssClass="col-form-label text-secondary" Text="Bekleyen işleminiz bulunmamaktadır."></asp:Label>
            </div>
            <div class="form-group m-4" id="TabloDiv" runat="server">
                <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                    <thead>
                        <tr>
                            <th>Adı Soyadı</th>
                            <th>Başlangıç Tarihi</th>
                            <th>Bitiş Tarihi</th>
                            <th>Görevin Sebebi</th>
                            <th>Görevin Yeri</th>
                            <th>Ulaşım Aracı</th>
                            <th>Transfer</th>
                            <th>Konaklama</th>
                            <th>Açıklama</th>
                            <th>Onayla</th>
                            <th>Reddet</th>
                            <th>İncele</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div style="display: none">
                <input id="paramGorevOnayIdLbl" runat="server" style="border-style: none;" />
                <asp:LinkButton ID="ModalOnaylaBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalOnaylaBtn_Click" />
                <asp:LinkButton ID="ModalReddetBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalReddetBtn_Click" />
                <asp:LinkButton ID="ModalInceleBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalInceleBtn_Click" />
            </div>
        </div>
        <div class="card-footer">
        </div>
    </div>

    <%--Onayla modal--%>
    <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">
                <div class="modal-header">
                    <h5 class="modal-title">Görev Onayı</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="OnayLbl" runat="server" CssClass="col-form-label"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Vazgeç</button>
                    <asp:LinkButton ID="OnaylaBtn" CssClass="btn btn-primary" runat="server" CausesValidation="false" Text="Onayla" OnClick="OnaylaBtn_Click" />
                </div>
            </div>
        </div>
    </div>
    <%--Reddet Modal--%>
    <div class="modal" id="ModalReddetDiv" role="dialog">
        <div class="modal-dialog modal-dialog-centered">
            <!-- Modal content-->
            <div class="modal-content" style="width: 550px;">
                <div class="modal-header">
                    <h5 class="modal-title">Görev Onayı</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox ID="ReddetAciklamaTxt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" Placeholder="Reddetme sebebini giriniz..."></asp:TextBox>
                    <asp:Label ID="ReddetLbl" runat="server" CssClass="col-form-label"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Vazgeç</button>
                    <asp:LinkButton ID="ReddetBtn" CssClass="btn btn-danger" runat="server" CausesValidation="false" Text="Reddet" OnClick="ReddetBtn_Click" />
                </div>
            </div>
        </div>
    </div>
    <!-- İncele Modal -->
     <div class="modal" id="ModalInceleDiv" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Görev Onayı Detayı</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Kapat"></button>
                </div>
                <div class="modal-body">
                    <div class="Table">
                        <table id="GorevInfoTable" runat="server" class="table table-bordered table-striped" width="100%"></table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
                </div>
            </div>
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
