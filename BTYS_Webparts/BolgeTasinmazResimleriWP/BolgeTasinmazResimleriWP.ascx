<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BolgeTasinmazResimleriWP.ascx.cs" Inherits="BTYS_Webparts.BolgeTasinmazResimleriWP.BolgeTasinmazResimleriWP" %>
<script type="text/javascript">
    function readURL(input, sender) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + sender).attr('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
    function OpenModal(clickedImg) {
        $('#MaximizedImg').attr('src', clickedImg.src);
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ResimAcModal'));
        myModalInstance.show();

    }
</script>
<div class="container col-xl">
    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Resimleri"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body alert-secondary" id="MainCardDiv" runat="server">
                    <div class="row m-1">
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image1" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                 
                            </div>
                        </div>
                       <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image2" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg';" Style="height: 150px" />
                                </div>
                                
                            </div>
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image3" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto2_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto2_jpg.jpg';" Style="height: 150px" />
                                </div>
                                
                            </div>
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image4" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TahkikatFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TahkikatFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="row">

                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image5" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/KrokiFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/KrokiFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                
                            </div>
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image6" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TapuFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TapuFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                               
                            </div>
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image7" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                
                            </div>
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image8" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg';" Style="height: 150px" />
                                </div>
                               
                            </div>
                        </div>
                    </div>
                    <hr />
                    <hr />
                    <div class="row">
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label fw-bold" ID="Label2" runat="server">Emlak Beyanı (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="EmlakBeyaniDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Emlak Beyanı 
                                </a>
                               
                            </div>
                           
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label fw-bold" ID="Label1" runat="server">Yapı Kayıt Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="YapiKayitDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Yapı Kayıt Belgesi 
                                </a>
                                
                            </div>
                           
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label fw-bold" ID="Label3" runat="server">Tapu Kayıt Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="TapuKayitDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Tapu Kayıt Belgesi 
                                </a>
                                
                            </div>
                            
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label fw-bold" ID="Label4" runat="server">İmar Durumu Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="ImarDurumuDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">İmar Durumu Belgesi 
                                </a>
                               
                            </div>
                           
                        </div>
                    </div>
                </div>

                <div class="card-footer">
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
</div>
<div class="modal" id="ResimAcModal" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-bs-dismiss="modal">&times</button>
            </div>
            <img class="img-responsive" src="../TasinmazResimleri/TasinmazFoto.jpg" id="MaximizedImg" height="1000" width="1000" />

            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
