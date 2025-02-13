<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazResimleriWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazResimleriWP.TasinmazResimleriWP" %>
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
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Resimleri"></asp:Label>
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
                                 <div class="col-2">
                                     <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil1Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil1Btn_Click" />
                                 </div>
                            </div>
                            <asp:FileUpload ID="FileUpload0" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image1')" />
                        </div>
                       <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image2" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil2Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil2Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload1" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image2')" />
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image3" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto2_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto2_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil3Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil3Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload2" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image3')" />
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image4" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TahkikatFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TahkikatFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil4Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil4Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload3" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image4')" />
                        </div>
                    </div>
                    <hr />
                    <div class="row">

                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image5" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/KrokiFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/KrokiFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil5Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil5Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload4" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image5')" />
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image6" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TapuFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TapuFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil6Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil6Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload5" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image6')" />
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image7" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil7Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil7Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload6" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image7')" />
                        </div>
                        <div class="col border border-dark p-2">
                            <div class="row">
                                <div class="col-10">
                                    <asp:Image ID="Image8" ClientIDMode="Static" runat="server" ImageUrl="../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg" CssClass="img-thumbnail" onclick="OpenModal(this)" onerror="this.src='../TasinmazResimleri/_t/TasinmazFoto1_jpg.jpg';" Style="height: 150px" />
                                </div>
                                <div class="col-2">
                                    <asp:LinkButton CssClass="btn btn-outline-danger float-end mr-2" ID="ResimSil8Btn" runat="server" Text="Resmi Sil" CausesValidation="false" OnClick="ResimSil8Btn_Click" />
                                </div>
                            </div>
                            <asp:FileUpload ID="FileUpload7" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'Image8')" />
                        </div>
                    </div>
                    <hr />
                    <hr />
                    <div class="row">
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label font-weight-bold" ID="Label2" runat="server">Emlak Beyanı (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="EmlakBeyaniDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Emlak Beyanı 
                                </a>
                                <asp:LinkButton ID="EmlakBeyaniSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="EmlakBeyaniSilBtn_Click"
                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                            </div>
                            <div class="form-group">
                                <asp:FileUpload ID="EmlakBeyaniYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklemek İçin PDF Dosya Seçiniz" type="text" />
                            </div>
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label font-weight-bold" ID="Label1" runat="server">Yapı Kayıt Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="YapiKayitDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Yapı Kayıt Belgesi 
                                </a>
                                <asp:LinkButton ID="YapiKayitSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="YapiKayitSilBtn_Click"
                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                            </div>
                            <div class="form-group">
                                <asp:FileUpload ID="YapiKayitYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklemek İçin PDF Dosya Seçiniz" type="text" />
                            </div>
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label font-weight-bold" ID="Label3" runat="server">Tapu Kayıt Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="TapuKayitDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">Tapu Kayıt Belgesi 
                                </a>
                                <asp:LinkButton ID="TapuKayitSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="TapuKayitSilBtn_Click"
                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                            </div>
                            <div class="form-group">
                                <asp:FileUpload ID="TapuKayitYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklemek İçin PDF Dosya Seçiniz" type="text" />
                            </div>
                        </div>
                        <div class="col-3 border border-dark p-2">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label font-weight-bold" ID="Label4" runat="server">İmar Durumu Belgesi (pdf)</asp:Label>
                            </div>
                            <div class="form-group text-center">
                                <a id="ImarDurumuDosyaLnk" runat="server" class="btn btn-outline-primary" data-fancybox data-type="pdf" data-width="960" data-height="720" href="#">İmar Durumu Belgesi 
                                </a>
                                <asp:LinkButton ID="ImarDurumuSilBtn" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="ImarDurumuSilBtn_Click"
                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
                            </div>
                            <div class="form-group">
                                <asp:FileUpload ID="ImarDurumuYukleFU" CssClass="btn btn-danger form-control" runat="server" ToolTip="Yüklemek İçin PDF Dosya Seçiniz" type="text" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card-footer">
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Resimleri Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="SaveBtn" />
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
