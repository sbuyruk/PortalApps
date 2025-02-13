<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuyuruListesiWP.ascx.cs" Inherits="Portal_WebParts.DuyuruListesiWP.DuyuruListesiWP" %>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }

    function OkuyanlarClicked(duyuruId) {
        document.getElementById('<%= paramDuyuruIdLbl.ClientID%>').value = duyuruId;

        OpenOkuyanlarModal(duyuruId);
        document.getElementById('<%= HiddenOkuyanlarBtn.ClientID%>').click();
    }
    function OpenOkuyanlarModal(duyuruId) {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OkuyanlarModalDiv'));
        myModalInstance.show();
    }
</script>
<div class="container shadow">

    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Duyuru Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body" id="MainCardDiv" runat="server">
            <div class="form-group">
                <table id="CustomDataTable" class="table table-hover table-striped table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>Başlık</th>
                            <th>Yayın Tarihi</th>
                            <th>Bitiş Tarihi</th>
                            <th>Tekrar</th>
                            <th>Popup Pencere</th>
                            <th>Okunma</th>
                            <th>Duyuru</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success" ID="YeniDuyuruBtn" ClientIDMode="Static" runat="server" Text="Yeni Duyuru" OnClick="YeniDuyuruBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>

</div>


<div class="modal" id="OkuyanlarModalDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-header">
                        <h4>
                            <asp:Label CssClass="text-primary" ID="DuyuruLbl" Text="" runat="server" />
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <input id="paramDuyuruIdLbl" runat="server" type="text" />
                            <asp:LinkButton ID="HiddenOkuyanlarBtn" runat="server" OnClientClick="{return true;};" OnClick="HiddenOkuyanlarBtn_Click"></asp:LinkButton>
                        </div>
                        <div class="form-group">
                            <table id="CustomModalDataTable" class="table table-hover table-striped table-bordered" width="100%">
                                <thead>
                                    <tr>
                                        <th>Adi</th>
                                        <th>Soyadi</th>
                                        <th>Okuma Tarihi</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
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
