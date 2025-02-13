<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisciTalepleriWP.ascx.cs" Inherits="TBYS_WebParts.BagisciTalepleriWP.BagisciTalepleriWP" %>

<script>
    function OpenModalTalep(id) {
        document.getElementById('<%= ParamTalepIdLbl.ClientID%>').value = id;
        document.getElementById('<%= TalepModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('TalepModalUrlDiv'));
        myModalInstance.show();
    }

    function OpenTalepSilModal(id) {
        document.getElementById('<%= ParamTalepIdLbl.ClientID%>').value = id;

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalTalepSilDiv'));
        myModalInstance.show();
    }

</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışçi Talepleri"></asp:Label>
                <asp:Label ID="BagisciIdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="card">
                <h4>
                    <asp:Label ID="TalepTableLbl" runat="server" class="col-form-label font-weight-bold">Bağışçının Talepleri</asp:Label>
                </h4>
                <div class="table" >
                    <a href="#" onclick="OpenModalTalep(0);" class="btn btn-outline-success m-1">Talep Ekle</a>
                    <asp:Table ID="BagisciTalepleriTable" runat="server" CssClass="table table-striped table-bordered table-hover">
                    </asp:Table>
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="BagisciBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Bağışçıya Git" OnClick="BagisciBtn_Click" />
        </div>
    </div>
</div>
<div class="modal" id="TalepModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-sm">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="Label2" runat="server" Text="Talep Ekleme"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        <div style="display: none">
                            <input id="ParamTalepIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                            <asp:LinkButton ID="TalepModalDoldurBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text="" OnClick="TalepModalDoldurBtnBtn_Click" />
                        </div>
                        <div class="form-group">
                            <div class="form-group">
                                <label class="col-form-label font-weight-bold" for="TalepTxt">Talep</label>
                                <asp:TextBox ID="TalepTxt" runat="server" class="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label font-weight-bold" for="IrtibatTxt">İrtibat</label>
                                <asp:TextBox ID="IrtibatTxt" runat="server" class="form-control " ToolTip="Soyadı" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label font-weight-bold" for="AdiTxt">Zamanı</label>
                                <asp:TextBox ID="TarihTxt" runat="server" class="form-control" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="col-form-label font-weight-bold" for="TalepAciklamaTxt">Açıklama</label>
                                <asp:TextBox ID="TalepAciklamaTxt" runat="server" class="form-control small" TextMode="MultiLine" Rows="3" ToolTip="Talep Açıklaması"></asp:TextBox>
                            </div>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="TalepKaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Talep Kaydet" OnClick="TalepKaydetBtn_Click" />
                        <asp:LinkButton ID="TalepGuncelleBtn" runat="server" CssClass="btn btn-primary" CausesValidation="false" Text=" Talep Güncelle" OnClick="TalepGuncelleBtn_Click" />
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>

<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="ModalTalepSilDiv" role="dialog">
            <div class="modal-dialog modal-dialog-centered">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <h3>
                            <asp:Label ID="SilModalBaslikLbl" runat="server" Text="Talep Silinecek"></asp:Label>
                        </h3>
                    </div>
                    <div class="modal-body">
                        
                        <div class="m-1 text-center" id="MesajDiv">
                            <div class="form-group">
                                <asp:Label ID="SilMesajiLbl" CssClass="col-form-label" runat="server" Text=" Talep Silinsin mi?"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="TalepSilNowBtn" CssClass="btn btn-outline-danger" runat="server" Text="Sil" OnClick="TalepSilNowBtn_Click"></asp:LinkButton>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

