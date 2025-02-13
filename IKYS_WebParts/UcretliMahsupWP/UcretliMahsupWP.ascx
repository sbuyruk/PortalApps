<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcretliMahsupWP.ascx.cs" Inherits="IKYS_WebParts.UcretliMahsupWP.UcretliMahsupWP" %>
<script>
    function OpenModalOnay() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
    //ekrandan secilen onayla vb butonun serverside'da oncliclkini calistirsin
    function CallButtonClick(onay) {

        var button = document.getElementById('<%= OnaylaBtn.ClientID%>');
        if (button != null) {
            button.click();
        }
    }
    function OpenModal(izinHareketId) {
        document.getElementById('<%= paramIzinHareketIdLbl.ClientID%>').value = izinHareketId;
        document.getElementById('<%= ModalInfoBtn.ClientID%>').click();
        OpenModalOnay();
    }
</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3>
                <asp:Label CssClass="col-form-label  btn-outline-primary" runat="server" Text="Ücretli İzin Mahsubu"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="19" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group row">
                <div class="form-group col-3" id="PersonelDiv" runat="server" style="display: block;">
                    <label class="col-form-label" for="PersonelDDL">Personel </label>
                    <asp:DropDownList ID="PersonelDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="PersonelDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
                <div class="form-group col-3" id="DonemDiv" runat="server" style="display: block;">
                    <label class="col-form-label" for="DonemDDL">Dönem </label>
                    <asp:DropDownList ID="DonemDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DonemDDL_SelectedIndexChanged" Style="height: auto" />
                </div>
            </div>
            <div class="form-group">
                <table id="CustomDataTable" class="table table-bordered table-striped" width="100%">
                    <thead>
                        <tr>
                            <th>Adı Soyadı</th>
                            <th>İzin Dönemi</th>
                            <th>İzin Başlangıç Tarihi</th>
                            <th>İzin Bitiş tarihi</th>
                            <th>Süre</th>
                            <th>Mahsup</th>
                        </tr>
                    </thead>
                </table>
            </div>
            <div style="display: none">
                <input id="paramIzinHareketIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <input id="paramPersonelIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="OnaylaBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="OnaylaBtn_Click" />
            </div>
        </div>
        <div class="card-footer">
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
<div class="modal" id="ModalOnayDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 550px;">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-body">
                        <div style="display: none">
                            <asp:LinkButton ID="ModalInfoBtn" CssClass="btn btn-info" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalInfoBtn_Click" />
                        </div>
                        <div>
                            <div class="text-center">
                                <h3>
                                    <asp:Label ID="PersonelAdiLbl" class="label label-primary " runat="server" Text="..."></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <asp:Table ID="IzinBilgileriTable" runat="server" class="table table-sm small">
                                </asp:Table>
                            </div>
                            <div class="card-body">
                                <div class="form-group" id="Div1" runat="server" style="display: block;">
                                    <label class="col-form-label" for="DonemDDL">Mahsup Edilebilecek Dönemler </label>
                                    <asp:DropDownList ID="ModalDonemDDL" runat="server" class="form-control" Style="height: auto" />
                                </div>
                            </div>
                            <div class="card-body">
                                <label class="col-form-label">Mahsup Açıklaması</label>
                                <asp:TextBox ID="AciklamaTxt" TextMode="MultiLine" Rows="3" runat="server" class="form-control" type="text" />
                                <asp:Label ID="OnayLbl" runat="server" CssClass="col-form-label text-danger"></asp:Label>
                            </div>
                        </div>

                    </div>

                    <div class="modal-footer">
                        <button id="MahsupEtModalBtn" runat="server" class="btn btn-success" onclick="CallButtonClick('onay')" visible="False">Mahsup Et</button>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>

