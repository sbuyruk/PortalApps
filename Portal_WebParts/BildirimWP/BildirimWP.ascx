<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BildirimWP.ascx.cs" Inherits="Portal_WebParts.BildirimWP.BildirimWP" %>
<div>
    <button type="button" id="ModalAcBtn" class="btn btn-primary">Modal Aç</button>
</div>

<%-- Bildirim modal --%>
<div class="modal" id="ModalAcDiv" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Bildirimler</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="KapatX"></button>
            </div>
            <div class="modal-body" style="font-size: 1.15rem;">
                <asp:Label ID="BildirimLbl" runat="server" CssClass="col-form-label" style="font-size: inherit;"></asp:Label>
                <%-- Amir olarak Onaylamanız gereken Görevler --%>
                <asp:Label ID="AmirOnayLbl" runat="server" CssClass="col-form-label" style="font-size: inherit;"></asp:Label>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">VazgeçX</button>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">

// SharePoint web part zone'ları (transform/overflow ile) yeni bir stacking
// context oluşturabildiğinden, modal'ı body'nin altına taşıyıp orijinal
// yerinde Bootstrap'in kendi (hiç bozulmamış) davranışıyla açıyoruz.
(function() {
    function InitBildirimModal() {
        var modalEl = document.getElementById('ModalAcDiv');
        var btnEl = document.getElementById('ModalAcBtn');

        if (!modalEl || !btnEl || modalEl.dataset.bildirimInitialized === 'true') {
            return;
        }

        if (modalEl.parentElement !== document.body) {
            document.body.appendChild(modalEl);
        }

        btnEl.addEventListener('click', function() {
            var myModalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
            myModalInstance.show();
        });

        modalEl.dataset.bildirimInitialized = 'true';
    }

    function ShowBildirimModal() {
        InitBildirimModal();
        var modalEl = document.getElementById('ModalAcDiv');
        if (!modalEl) {
            return;
        }
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
        myModalInstance.show();
    }

    if (document.readyState === 'interactive' || document.readyState === 'complete') {
        InitBildirimModal();
    } else {
        document.addEventListener('DOMContentLoaded', InitBildirimModal);
    }

    if (typeof _spBodyOnLoadFunctionNames !== 'undefined') {
        _spBodyOnLoadFunctionNames.push('InitBildirimModal');
    }

    window.InitBildirimModal = InitBildirimModal;
    window.ShowBildirimModal = ShowBildirimModal;
})();

</script>