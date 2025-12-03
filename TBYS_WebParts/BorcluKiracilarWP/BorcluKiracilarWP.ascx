<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BorcluKiracilarWP.ascx.cs" Inherits="TBYS_WebParts.BorcluKiracilarWP.BorcluKiracilarWP" %>


<script type="text/javascript">
    //excele export ettikten donup sonra kalmasın diye
    function setFormSubmitToFalse() {
        setTimeout(function () { _spFormOnSubmitCalled = false; }, 3000);
        return true;
    }
    function OpenModal(kiraSozlesmeId) {
        document.getElementById('<%= paramKiraSozlesmeIdLbl.ClientID%>').value = kiraSozlesmeId;
        document.getElementById('<%= OdemePlaniGoruntuleBtn.ClientID%>').click();

        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('OdemePlaniModal'));
        myModalInstance.show();
    }
</script>
<div id="MainContainer" class="container">
    <div class="card shadow">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold text-center" ID="TitleLbl" runat="server" Text="Borçlu Kiracılar"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="form-group">
                <asp:LinkButton CssClass="btn btn-outline-primary float-end" ID="OdemePlanlariniGuncelleBtn" runat="server" Text="Ödeme Planlarını Güncelle" OnClick="OdemePlanlariniGuncelleBtn_Click" />
            </div>
            <div style="display: none">
                <input id="paramKiraSozlesmeIdLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="OdemePlaniGoruntuleBtn" runat="server" OnClientClick="{return true;};" OnClick="OdemePlaniGoruntuleBtn_Click"></asp:LinkButton>
            </div>
            <asp:Table ID="BorcluKiracilarTable" runat="server" class="table table-bordered table-hover table-striped">
                <asp:TableHeaderRow HorizontalAlign="Center">
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">S.NO</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">D.NO</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">BÖLGE</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRACININ ADI VE SOYADI</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">İLK SÖZLEŞME TARİHİ</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRA BEDELİ (TL/AY)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">BORÇ MİKTARI (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">FAİZLİ BAKİYE (TL)</asp:TableHeaderCell>
                    <asp:TableHeaderCell BorderStyle="Solid" BorderWidth="2" BorderColor="Black">KİRA BORCU (AY)</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-end" ID="ExcelBtn" ClientIDMode="Static" runat="server" Text="Excele Aktar" OnClick="ExcelBtn_Click" OnClientClick="javascript:setFormSubmitToFalse()" />
        </div>
    </div>
</div>
<div class="modal" id="OdemePlaniModal" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content" style="width: 650px;">
            <div class="modal-body">
                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div>
                            <h3>Kira Ödeme Planı
								<asp:Label ID="KiraciTitleLbl" runat="server" CssClass="col-form-label fw-bold" Text=""></asp:Label>	
                            </h3>
                        </div>

                        <div class="card-body">
                            
                            <asp:Label ID="DevirLbl" CssClass="col-form-label" runat="server"></asp:Label>
                            <asp:Table ID="OdemePlaniTable" runat="server" CssClass="table table-striped table-bordered">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Sıra No </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Yil </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ay </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Kira Bedeli </asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ödenen Tutar</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Ödeme Tarihi</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="OdemePlaniGoruntuleBtn" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
            </div>
        </div>
    </div>
</div>
