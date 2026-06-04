<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DevirGerekenSozlesmelerWP.ascx.cs" Inherits="TBYS_WebParts.DevirGerekenSozlesmelerWP.DevirGerekenSozlesmelerWP" %>

<style>
    #DevirAlOnayModalDiv {
        z-index: 99999 !important;
    }

    .modal-backdrop {
        z-index: 99998 !important;
    }
</style>

<script>

    function DevirAlModalKapat() {
        var modalEl = document.getElementById('DevirAlOnayModalDiv');
        var modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
        modalInstance.hide();

        // UpdatePanel senaryosunda backdrop ve body class'ını zorla temizle
        modalEl.classList.remove('show');
        modalEl.style.display = 'none';
        document.querySelectorAll('.modal-backdrop').forEach(function (el) { el.remove(); });
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('overflow');
        document.body.style.removeProperty('padding-right');
    }

    // UpdatePanel her güncellendiğinde modal artıklarını temizle
    if (typeof Sys !== 'undefined') {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            if (document.querySelectorAll('.modal.show').length === 0) {
                document.querySelectorAll('.modal-backdrop').forEach(function (el) { el.remove(); });
                document.body.classList.remove('modal-open');
                document.body.style.removeProperty('overflow');
                document.body.style.removeProperty('padding-right');
            }
        });
    }
    function DevirAlModalOnay(id) {
        document.getElementById('<%= HiddenSecilenId.ClientID %>').value = id;
        var modalEl = document.getElementById('DevirAlOnayModalDiv');

        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }
</script>


<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h3 class="mb-0">
                        <asp:Label ID="TitleLbl" runat="server"
                            CssClass="col-form-label text-danger fw-bold mb-0"
                            Text="Devir Gereken Sözleşmeler"></asp:Label>
                    </h3>

                </div>
                <div class="card-body">
                    </hr>
                    <div class="text-center">
                        <asp:LinkButton ID="LoadDataBtn" runat="server"
                            CssClass="btn btn-primary"
                            Text="Devir Gerektiren Sözleşmeleri Getir"
                            OnClick="LoadDataBtn_Click" />
                    </div>
                    </hr>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%" style="display: none">
                            <thead>
                                <tr>
                                    <th colspan="9">
                                        <h4 class="text-center">Devir Tutarı Farklı Olan Aktif Sözleşmeler</h4>
                                    </th>
                                </tr>
                                <tr>
                                    <th>Kiracı</th>
                                    <th>Dosya No</th>
                                    <th>Tarih Aralığı</th>
                                    <th>Kira Bedeli</th>
                                    <th>Devir Anapara</th>
                                    <th>Devir Faiz</th>
                                    <th>Devir Faizli Bakiye</th>
                                    <th>Devir Al</th>
                                    <th>Sözleşme</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
                <div class="card-footer">
                    <button type="button" class="btn btn-danger" onclick="DevirAlModalOnay()">Tümünü Devir Al</button>
                    <asp:LinkButton ID="OdemePlanlariniGuncelleBtn" runat="server"
                        CssClass="btn btn-outline-primary float-end"
                        Text="Ödeme Planlarını Güncelle"
                        OnClick="OdemePlanlariniGuncelleBtn_Click" />
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="TumunuDevirAlNowBtn" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>


    <asp:UpdateProgress ID="updateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel" DisplayAfter="0">
        <ProgressTemplate>
            <div class='loaderMainContainer'>
                <div class='loaderContainer'>
                    <div class='loaderCircle'></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <%-- Modal Onay - UpdatePanel dışında, backdrop sorunu önlemek için --%>
    <asp:HiddenField ID="HiddenSecilenId" runat="server" />
    <div class="modal" id="DevirAlOnayModalDiv" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-body">
                    <div class="text-center">
                        <h3>
                            <asp:Label ID="DevirAlModalBaslikLbl" CssClass="col-form-label text-danger fw-bold" runat="server" Text="Tüm Bakiyeler Devir Alınacak"></asp:Label></h3>
                    </div>
                    <div class="card-body text-center">
                        <div class="form-group">
                            <asp:Label ID="DevirAlMesajiLbl" CssClass="col-form-label" runat="server"
                                Text="Bu işlem geri alınamaz. Devam etmek istiyor musunuz?">
                            </asp:Label>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="TumunuDevirAlNowBtn" CssClass="btn btn-danger" runat="server"
                        CausesValidation="false" Text="Devir Al" OnClick="TumunuDevirAlNowBtn_Click"
                        OnClientClick="DevirAlModalKapat();" />
                    <button type="button" class="btn btn-secondary" onclick="DevirAlModalKapat()">İptal</button>
                </div>
            </div>
        </div>
    </div>
</div>

