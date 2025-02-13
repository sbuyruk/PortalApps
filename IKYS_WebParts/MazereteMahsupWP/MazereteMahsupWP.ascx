<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MazereteMahsupWP.ascx.cs" Inherits="IKYS_WebParts.MazereteMahsupWP.MazereteMahsupWP" %>
<script type="text/javascript">
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
    function OpenModal(personelId) {
        document.getElementById('<%= paramPersonelIdLbl.ClientID%>').value = personelId;
        document.getElementById('<%= IzinBilgileriBtn.ClientID%>').click();
        OpenModalOnay();

    }
</script>
<div class="container shadow">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label  btn-outline-success mb-1" ID="TitleLbl" runat="server" Text="Ücretli izinden Mazerete İznine Mahsup"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="18" runat="server" ></asp:Label>
                    </h3>
                </div>
                <div class="card-body alert-secondary" id="MainCardDiv" runat="server">
                    <div class="table loader">
                        <asp:Table ID="IzinTable" runat="server" class="table table-striped table-bordered">
                        </asp:Table>
                    </div>
                    <div style="display: none">
                        <input id="paramPersonelIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="OnaylaBtn" CssClass="btn btn-secondary" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="OnaylaBtn_Click" />
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-warning float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
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
 <div class="modal" id="ModalOnayDiv" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content" style="width: 750px;">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="modal-body">
                            <div style="display: none">
                                <asp:LinkButton ID="IzinBilgileriBtn" CssClass="btn btn-info" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="IzinBilgileriBtn_Click" />
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="PersonelAdiLbl" class="label label-primary " runat="server" Text="..."></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Table ID="IzinDonemleriTable" runat="server" class="table table-striped">
                                    </asp:Table>
                                </div>
                                <div class="card-body">
                                    <label class="col-form-label">Açıklama</label>
                                    <asp:TextBox ID="AciklamaTxt" TextMode="MultiLine" Rows="3" runat="server" class="form-control" type="text" />
                                    <asp:Label ID="OnayLbl" runat="server" CssClass="col-form-label text-danger" ></asp:Label>
                                </div>
                            </div>
                            
                        </div>

                        <div class="modal-footer">
                            <button ID="OnaylaModalBtn" runat="server" class="btn btn-danger" onclick="CallButtonClick('onay')" Visible="false">Mahsup Et</button>
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>