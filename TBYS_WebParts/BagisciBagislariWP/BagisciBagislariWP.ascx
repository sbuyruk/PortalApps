<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagisciBagislariWP.ascx.cs" Inherits="TBYS_WebParts.BagisciBagislariWP.BagisciBagislariWP" %>
<script type="text/javascript">
    function OpenModalOnay() {
        $("#ModalOnayDiv").modal({ backdrop: false });
    }
    function OpenModal() {
        $("#ModalTasinmazListesiDiv").modal({ backdrop: false });
    }
    function CallButtonClick(bagisciId, tasinmazId) {
        document.getElementById('<%= paramTasinmazIdLbl.ClientID%>').value = tasinmazId;
        document.getElementById('<%= TasinmazEkleNowBtn.ClientID%>').click();
    }
</script>
<div class="container col-xl">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Bağışlar"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="table">
                        <asp:Table ID="BagisTasinmazLarTable" runat="server" class="table">
                        </asp:Table>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="TasinmazEkleBtn" CssClass="btn btn-outline-primary float-left" runat="server" Text="Taşınmaz Ekle" OnClick="TasinmazEkleBtn_Click" />
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog ">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">

                        <div class="modal-body">
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="CikarLbl" class="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Taşınmaz Bağışlardan Çıkarılacak"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="TasinmazAdresLbl" CssClass="col-form-label" runat="server" Text=""></asp:Label>
                                    <asp:Label ID="CikarMesajiLbl" CssClass="col-form-label text-danger" runat="server" Text="Seçilen Taşınmazı Bağışlardan Çıkarmak İstiyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="CikarNowBtn" runat="server" CausesValidation="false" Text="Bağışlardan Çıkar" OnClientClick="{return true;};" OnClick="CikarNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default float-right" data-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal" id="ModalTasinmazListesiDiv" role="dialog">
                <div class="modal-dialog modal-lg">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 1000px;">

                        <div class="modal-body">
                            <div style="display: none">
                                <input id="paramTasinmazIdLbl"  runat="server" type="text" />
                                <asp:LinkButton ID="TasinmazEkleNowBtn" runat="server" OnClientClick="{return true;};" OnClick="TasinmazEkleNowBtnBtn_Click" ></asp:LinkButton>
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label ID="Label1" class="col-form-label " runat="server" Text="Taşınmaz Listesi"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <div class="form-group">
                                        <table id="CustomModalDataTable" class="table table-striped table-bordered" width="100%">
                                            <thead>
                                                <tr>
                                                    <th>Sıra</th>
                                                    <th>Mülkiyet Şekli</th>
                                                    <th>Kullanım Şekli</th>
                                                    <th>İl</th>
                                                    <th>İlce</th>
                                                    <th>Adres</th>
                                                    <th>Seç</th>
                                                </tr>
                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>