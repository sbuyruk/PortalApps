<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FaaliyetParametreWP.ascx.cs" Inherits="MTS_WebParts.FaaliyetParametreWP.FaaliyetParametreWP" %>
<script>
    function OpenModalOnay() {
        $("#ModalOnay").modal({ backdrop: "static" });
    }
    function CloseModal() {
        $("#ModalOnay").modal('hide');

    }
    function DuzenleSilModalAc(parametreId, islemTipi) {
        document.getElementById('<%= parametreIdLbl.ClientID%>').value = parametreId;
        document.getElementById('<%= paramIslemTipiLbl.ClientID%>').value = islemTipi;
        document.getElementById('<%=DuzenleSilBtn.ClientID%>').click();
    }

</script>
<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Faaliyet Parametreleri"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group col-3">
                <asp:DropDownList ID="FaaliyetGrupDDL" CssClass="form-control" runat="server" Height="34px" OnSelectedIndexChanged="FaaliyetGrupDDL_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
            </div>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Sira</th>
                                    <th>Id</th>
                                    <th>Grup</th>
                                    <th>Deger</th>
                                    <th>Düzenle</th>
                                    <th>Sil</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div class="form-group row">
                        <div class="form-group col-3">
                            <asp:Label ID="Label1" CssClass="col-form-label" runat="server" Text="Grup"></asp:Label>
                            <asp:Label ID="GrupLbl" CssClass="form-control" runat="server"></asp:Label>
                        </div>
                        <div class="form-group col-4">
                            <asp:Label ID="Label2" CssClass="col-form-label" runat="server" Text="Değer"></asp:Label>
                            <asp:TextBox ID="YeniDegerTxt" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-group col-2">
                            <asp:Label ID="Label4" CssClass="col-form-label" runat="server" Text="Sıra"></asp:Label>
                            <asp:TextBox ID="YeniSiraTxt" type="number" min="0" step="1" CssClass="form-control input-integer" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-group col-1">
                            <asp:Label ID="Label3" CssClass="col-form-label text-white" runat="server" Text=".--------."></asp:Label>
                            <asp:LinkButton ID="KaydetBtn" Text="Kaydet" runat="server" class="btn btn-outline-success" OnClick="KaydetBtn_Click"></asp:LinkButton>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="FaaliyetGrupDDL" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">

        </div>
    </div>
    <div id="HiddenDiv" style="display: none">
        <input id="parametreIdLbl" runat="server" type="text" />
        <input id="paramIslemTipiLbl" runat="server" type="text" />
        <asp:LinkButton ID="DuzenleSilBtn" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="DuzenleSilBtn_Click" />
    </div>
     <div class="modal" id="ModalOnay" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <h3>
                                    <asp:Label ID="ModalLbl" class="col-form-label text-primary font-weight-bold" Text="" runat="server"></asp:Label></h3>
                            </div>
                            <div class="card-body">
                                <div class="form-group" id="SilDiv" runat="server" style="display:none">
                                    <asp:Label ID="MessageLbl" runat="server" class="col-form-label">Silmek istediğinizden emin misiniz?</asp:Label>                                    
                                </div>
                                <div id="DuzenleDiv" runat="server" style="display: none">
                                    <div class="row">
                                        <div class="form-group col-8">
                                            <asp:Label CssClass="col-form-label" runat="server" Text="Değer"></asp:Label>
                                            <asp:TextBox ID="ParametreTxt" runat="server" CssClass="form-control "></asp:TextBox>
                                        </div>
                                        <div class="form-group col-2">
                                            <asp:Label CssClass="col-form-label" runat="server" Text="Sıra"></asp:Label>
                                            <asp:TextBox ID="SiraTxt" type="number" step="1" min="0" CssClass="form-control input-integer" runat="server" />
                                        </div>
                                    </div>
                                    
                                    
                                </div>
                            </div>
                            <div class="card-footer">
                                <asp:LinkButton ID="GuncelleNowBtn" Text="Güncelle" runat="server" class="btn btn-outline-primary" OnClick="GuncelleNowBtn_Click"></asp:LinkButton>
                                <asp:LinkButton ID="SilNowBtn" Text="Sil" runat="server" class="btn btn-outline-danger" OnClick="SilNowBtn_Click"></asp:LinkButton>
                                <button type="button" class="btn btn-outline-secondary" data-dismiss="modal">İptal</button>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="DuzenleSilBtn" EventName="click" />
                        
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

</div>