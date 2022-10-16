<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MalzemeGirisiWP.ascx.cs" Inherits="MYS_WebParts.MalzemeGirisiWP.MalzemeGirisiWP" %>
<script type="text/javascript">
    function readURL(malzemeFU, sender) {
        if (malzemeFU.files && malzemeFU.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#' + sender).attr('src', e.target.result);

            }
            reader.readAsDataURL(malzemeFU.files[0]);
        }
    }
    function clearTar() {
        $('#clear-Tar').on('click', function () {
            $("#EnvantereGirisTarTxt").val("");
            document.getElementById('<%= EnvantereGirisTarTxt.ClientID%>').value = "";
        });
    }

    $(document).ready(function () {
        $("#clear-Tar").on("click", function (event) {
            $("#EnvantereGirisTarTxt").val("");
        });
    });

   
    function OpenModal(clickedImg) {
        $('#MaximizedImg').attr('src', clickedImg.src);
        $("#PictureModal").modal({ backdrop: false });

    }
    function OpenResimYukleModal() {
        $("#ResimYukleModal").modal({ backdrop: false });

    }
</script>
<style>
    html, body {
        height: 100%;
    }

    .nopadding {
        padding: 0 !important;
        margin: 0 !important;
        /*padding-right: 2px !important;*/
    }

    .alignCenter {
        text-align: center;
    }

    .alignRight {
        text-align: right;
    }

    .alignLeft {
        text-align: left;
    }

    .alignBottom {
        vertical-align: bottom;
    }

    .my-thumbnail {
        width: 230px;
        height: 190px;
        overflow: auto;
    }

    .image-tn {
        width: 221px;
        height: 130px;
        overflow: auto;
    }
</style>
<div class="container shadow">

    <div class="card">

        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Malzeme Girişi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" Visible="false" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="MalzemeLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-right" ID="EkranNo" Text="2" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="form-group row">
                        <div class="form-group col-10 row">
                            <div class="card col-4">
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="AdiTxt">Adı</label>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                    <asp:TextBox ID="AdiTxt" runat="server" class="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="SeriNoTxt">Seri Numarası</label>
                                    <asp:TextBox ID="SeriNoTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="KategoriDDL">Malzeme Kategorisi</label>
                                    <asp:DropDownList ID="KategoriDDL" runat="server" class="form-control " OnSelectedIndexChanged="KategoriDDL_SelectedIndexChanged" AutoPostBack="true" />
                                </div>
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="CinsiDDL">Malzeme Cinsi</label>
                                    <asp:DropDownList ID="CinsiDDL" runat="server" class="form-control " />
                                </div>
                                
                                
                                
                            </div>
                            <div class="card col-4">
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="BirimFiyatTxt">Birim Fiyat</label>
                                    <asp:TextBox ID="BirimFiyatTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="AdetTxt">Adet</label>
                                    <asp:TextBox ID="AdetTxt" runat="server" class="form-control" ToolTip="Anne Adı" type="text"></asp:TextBox>
                                </div>
                                <div class="form-group m-0">
                                    <label class="col-form-label" for="EnvantereGirisTarTxt">Envantere Gir.Tarihi</label>
                                    <div class="input-group">
                                        <input runat="server" type="text" id="EnvantereGirisTarTxt" name="EnvantereGirisTarTxt" class="form-control DateTimePickerV1" readonly="readonly" />
                                        <input type="button" id="clear-Tar" value="sil" onclick="clearTar()" />
                                    </div>
                                </div>
                                <div class="form-group" id="AciklamaDiv" runat="server" style="display: block">
                                    <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" Columns="30" class="form-control" type="text" />
                                </div>
                            </div>
                        </div>

                        <div class="card col-2 ">
                            <div class="form-group">
                                <%--<img src="../MalzemeResimleri/Malzeme.jpg" id="BrowseFoto" class="img-thumbnail" height="190" width="140" />--%>
                               <%-- <asp:Image ID="BrowseFoto" ClientIDMode="Static" runat="server" ImageUrl="../MalzemeResimleri/Malzeme.jpg" class="img-thumbnail" height="190" width="140"/>--%>
                                 <asp:Image ID="DisplayImage" ClientIDMode="Static" runat="server" ImageUrl="../MalzemeResimleri/_t/Malzeme_jpg.jpg" class="img-thumbnail" height="190" width="140" onerror="this.src='../MalzemeResimleri/_t/Malzeme_jpg.jpg';"/>

                                <asp:FileUpload ID="xFileUpload" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="text" onchange="readURL(this,'DisplayImage')" />
                                <%--<input id="xFileUpload" class="btn btn-danger form-control" runat="server" ToolTip="Yüklenecek Resmi Seçiniz" type="file" onchange="readURL(this,'BrowseFoto')" />--%>
                            </div>
                        </div>
                    </div>
                    
                </ContentTemplate>
                <Triggers>
                    <%--<asp:PostBackTrigger ControlID="SaveFotoBtn" />--%>
                    <asp:PostBackTrigger ControlID="SaveBtn" />
                    <asp:PostBackTrigger ControlID="UpdateBtn" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
            <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success float-left" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
            <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary float-left" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" />
            <asp:LinkButton ID="DeleteBtn" CssClass="btn btn-outline-danger float-left" runat="server" Text="Sil" OnClick="DeleteBtn_Click" />
        </div>

    </div>

</div>
