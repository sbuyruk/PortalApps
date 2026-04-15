<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisaVesileOlanTesekkurListesiWP.ascx.cs" Inherits="NBYS_WebParts.BagisaVesileOlanTesekkurListesiWP.BagisaVesileOlanTesekkurListesiWP" %>

<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function Sil(id) {
        document.getElementById('<%= HiddenSecilenId.ClientID %>').value = id;
        var modalEl = document.getElementById('SilModalDiv');
        if (modalEl.parentNode !== document.body) {
            document.body.appendChild(modalEl);
        }
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }
</script>

<div class="container col-xl">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="form-label text-info fw-semibold mb-1" ID="TitleLbl" runat="server" Text="Bağışa vesile Olanlara Verilen Teşekkür Belgeleri"></asp:Label>
                <asp:Label CssClass="form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <asp:UpdatePanel ID="upPanel" runat="server">
            <ContentTemplate>
                
                <div class="card-body mt-1">

                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                            <thead>
                                <tr>
                                    <th>Belge No</th>
                                    <th>Adı</th>
                                    <th>Soyadı</th>
                                    <th>Verilme Sebebi</th>
                                    <th>Belge Tarihi</th>
                                    <th>İmzalayan</th>
                                    <th>Açıklama</th>
                                    <th>Düzenle</th>
                                    <th>Sil</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <asp:HiddenField ID="HiddenSecilenId" runat="server" />
                <div class="modal" id="SilModalDiv" role="dialog" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-body">
                                <div class="text-center">
                                    <h3><asp:Label ID="SilModalBaslikLbl" CssClass="col-form-label text-danger fw-bold" runat="server" Text="Silmeyi Onayla"></asp:Label></h3>
                                </div>
                                <div class="card-body text-center">
                                    <div class="form-group">
                                        <asp:Label ID="SilMesajiLbl" CssClass="col-form-label" runat="server"
                                            Text="Seçilen teşekkür belgesi kalıcı olarak silinecektir. Silmek istediğinizden emin misiniz?">
                                        </asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:LinkButton ID="SilNowBtn" CssClass="btn btn-danger" runat="server"
                                    CausesValidation="false" Text="Sil" OnClick="SilNowBtn_Click" />
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">İptal</button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="SilNowBtn" />
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
        <div class="card-footer">
        </div>
    </div>
</div>
