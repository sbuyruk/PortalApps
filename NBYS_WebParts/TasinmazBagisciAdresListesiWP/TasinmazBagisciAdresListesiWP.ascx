<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazBagisciAdresListesiWP.ascx.cs" Inherits="NBYS_WebParts.TasinmazBagisciAdresListesiWP.TasinmazBagisciAdresListesiWP" %>

<script>
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(nakitBagisciId) {
        document.getElementById('<%= paramTasinmazBagisciIdLbl.ClientID%>').value = nakitBagisciId;
        document.getElementById('<%= ModalDoldurBtn.ClientID%>').click();
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalUrlDiv'));
        myModalInstance.show();

    }
</script>
<script type="text/javascript">

</script>
<div class="container">
    <div class="card p-3 shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Bağışçı Adres Listesi"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-3 form-group">
                            <label class="col-form-label fw-bold">Etiket Adedi: </label>
                            <asp:DropDownList ID="EtiketAdediDDL" runat="server" CssClass="form-control" Style="height: auto"></asp:DropDownList>
                        </div>
                        <div class="col-3 checkbox">
                            <label>
                                <asp:CheckBox ID="VefatEdenlerHaricChk" runat="server" Checked="True" AutoPostBack="True" OnCheckedChanged="VefatEdenlerHaricChk_CheckedChanged" ToolTip="Vefat edenleri hariç tutmak için işaretleyiniz." />
                                Vefat Edenler Hariç
                            </label>
                            <label>
                                <asp:CheckBox ID="GizliBagislarHaricChk" runat="server" Checked="True" AutoPostBack="True" OnCheckedChanged="GizliBagislarHaricChk_CheckedChanged" ToolTip="Gizli bağışçıları hariç tutmak için işaretleyiniz." />
                                Gizli Bağışlar Hariç 
                            </label>
                        </div>

                    </div>
                    <div class="row" runat="server">
                        <div style="display: none">
                            <input id="paramTasinmazBagisciIdLbl" runat="server" text="Label" />
                        </div>
                        <div class="form-group">
                            <table id="CustomDataTable" class="table table-striped" width="100%">
                                <thead>
                                    <tr>
                                        <th>T.No</th>
                                        <th>Adı</th>
                                        <th>Taşınmaz Adedi</th>
                                        <th>Tahmini Rayiç</th>
                                        <th>Adres</th>
                                        <th>İli</th>
                                        <th>İlçesi</th>
                                        <th>Telefon</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="form-group">

                        <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiketleri</asp:HyperLink>

                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AdresEtiketiBtn" EventName="Click" />
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
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success" ID="AdresEtiketiBtn" runat="server" Text="Adres Etiketi Oluştur" OnClick="AdresEtiketiBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>


</div>
<div style="display: none">
    <asp:LinkButton ID="ModalDoldurBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="ModalDoldurBtn_Click" />
</div>
<div class="modal" id="ModalUrlDiv" role="dialog">
    <div class="modal-dialog modal-lg">
        <!-- Modal content-->
        <div class="modal-content">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-header">
                        <asp:Label ID="BaslikLbl" runat="server" Text="Bağışçının Yaptığı Taşınmaz Bağışlar" Font-Bold="True"></asp:Label>
                    </div>
                    <div class="modal-body">

                        <div class="m-1 text-center" id="NakitBagisciDiv">
                            <asp:Table CssClass="table table-striped table-bordered text-center" ID="BagisciTable" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Ad/Ünvan</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>TC Kimlik No</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Adres</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>İlçesi / İli</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Telefon</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                        <div class="form-group">
                            <table id="CustomModalDataTable" class="table table-striped" width="100%">
                                <thead>
                                    <tr>
                                        <th>Bağış Yılı</th>
                                        <th>Cinsi</th>
                                        <th>Tahmini Rayiç Değeri</th>
                                        <th>Mülkiyet Şekli</th>
                                        <th>Adres</th>
                                        <th>İlçe-İl</th>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ModalDoldurBtn" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="AdresEtiketiBtn" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
