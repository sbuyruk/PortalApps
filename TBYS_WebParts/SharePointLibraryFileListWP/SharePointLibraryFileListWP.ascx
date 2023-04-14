<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePointLibraryFileListWP.ascx.cs" Inherits="TBYS_WebParts.SharePointLibraryFileListWP.SharePointLibraryFileListWP" %>
<script>
    function CallButtonClick(fileName) {
        document.getElementById('<%= paramDosyaAdiLbl.ClientID%>').value = fileName;
        document.getElementById('<%= DosyayiSilBtn.ClientID%>').click();
    }
    function OpenModalOnay() {
        $("#ModalOnayDiv").modal({ backdrop: true });
    }
</script>

<div class="container ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Kira Borcu İhbarname Yazıları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div style="display: none">
                <input id="paramDosyaAdiLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="DosyayiSilBtn" runat="server" OnClientClick="{return true;};" OnClick="DosyayiSilBtn_Click"></asp:LinkButton>
            </div>
            <div id="SuzmeBolumuDiv" class="form-group row">
            </div>
            <div class="form-group" id="TabloBolumuDiv">
                <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                    <thead>
                        <tr>
                            <th>Dosya Adı</th>
                            <th>Etiket Dosyası</th>
                            <th>Yazan</th>
                            <th>Tarih</th>
                            <th>Dosyayı Sil</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:Label ID="Label1" runat="server" Text="Label">İstediğiniz dosyayı indirmek için dosya ismine tıklayınız</asp:Label>
        </div>
    </div>
</div>
<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog modal-dialog-centered">
        <!-- Modal content-->
        <div class="modal-content">

            <div class="modal-body">
                <div style="display: none">
                </div>
                <div>
                    <div class="text-center">
                        <h3>
                            <asp:Label ID="SilLbl" class="col-form-label text-danger" runat="server" Text="Dosya Silinecek"></asp:Label></h3>
                    </div>
                    <div class="card-body text-center">
                        <div class="form-group">
                            <asp:Label ID="DosyaAdiLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="SilMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Seçilen Dosyayı Silmek İstiyor musunuz?"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:LinkButton CssClass="btn btn-danger" ID="DosyayiSilNowBtn" runat="server" CausesValidation="false" Text="Dosyayı Sil" OnClientClick="{return true;};" OnClick="DosyayiSilNowBtn_Click" Visible="false" />
                <button type="button" class="btn btn-default float-right" data-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
