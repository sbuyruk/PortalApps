<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisciListesiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazBagisciListesiWP.TasinmazBagisciListesiWP" %>

<script>
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    //Eger bagisci resmi yoksa placeholder gostersin
    function imgError(image,url) {
        image.onerror = null;
        image.src = url;
        return true;
    }
    function OpenModalTaahhut(id) {
        document.getElementById('<%= ParamBagisciIdLbl.ClientID%>').value = id;
        document.getElementById('<%= TaahhutModalDoldurBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('TaahhutModalUrlDiv'));
        myModalInstance.show();
    }
</script>

<div class="container col-xl ">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label ID="TitleLbl" runat="server" CssClass="col-form-label text-danger fw-bold mb-1" Text="Taşınmaz Bağışçı Listesi"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="false"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <table id="CustomDataTable" class="table table-striped row-border" width="100%">
                    <thead>
                        <tr>
                            <th>B.No</th>
                            <th>Adı Soyadı</th>
                            <th>Bağış Adedi</th>
                            <th>Sağ</th>
                            <th>Bölge</th>
                            <th>İlçe-İl</th>
                            <th>Bağışçı Bilgi ve Talep Formu</th>
                            <th>Taahhüt Formu</th>
                            <th>Taahhütler</th>
                            <th>Taşınmaz Bağışçı Kartı</th>
                            <th>Düzenle</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="YeniKayitBtn" CssClass="btn btn-outline-success" runat="server" Text="Yeni Bağışçı Girişi" OnClick="YeniKayitBtn_Click"></asp:LinkButton>
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>

    </div>
</div>

<div class="modal" id="TaahhutModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <div class="form-group">
                            <asp:Label ID="BagiscciAdiLbl" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div style="display: none">
                        <input id="ParamBagisciIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                        <asp:LinkButton ID="TaahhutModalDoldurBtn" runat="server" CssClass="btn btn-success" CausesValidation="False" Text="" OnClick="TaahhutModalDoldurBtnBtn_Click" />
                    </div>
                    <div class="modal-body">
                        
                        <div class="form-group ">
                            <h4>
                                <asp:Label ID="TaahhutTableLbl" runat="server" class="col-form-label fw-bold">Bağışçıya Verilen Taahhütler</asp:Label>
                            </h4>
                            <div class="table">
                                <asp:Table ID="TaahhutTable" runat="server" CssClass="table table-striped table-bordered table-hover">
                                </asp:Table>
                            </div>
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