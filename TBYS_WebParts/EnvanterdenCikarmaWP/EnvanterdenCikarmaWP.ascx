<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnvanterdenCikarmaWP.ascx.cs" Inherits="TBYS_WebParts.EnvanterdenCikarmaWP.EnvanterdenCikarmaWP" %>
<script type="text/javascript">
    function OpenModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('ModalOnayDiv'));
        myModalInstance.show();
    }
</script>
<div id="MainPanel" class="container shadow w-75" runat="server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="BagisciMainPanel" class="card" runat="server">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Envanterden Çıkarma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body card-danger">
                    <div class="form-group m-0 ">
                        <div class="form-group">
                            <div class="form-group ">
                                <asp:Label ID="KullanimSekliLbl" runat="server" CssClass="form-control " ToolTip="Taşınmazın Kullanım Şekli"></asp:Label>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="AdresLbl" runat="server" CssClass="form-control font-weight-bold text-danger" ToolTip="Taşınmazın Adresi"></asp:Label>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Il_IlceLbl" runat="server" CssClass="form-control " ToolTip="Taşınmazın Bulunduğu İl-İlçe"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="form-group col-2">
                                <label for="CikarmaSebebiDDL" class="control-label">Çıkarma Sebebi</label>
                                <div>
                                    <asp:DropDownList ID="CikarmaSebebiDDL" runat="server" class="form-control "></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="BedelTxt" class="control-label">Bedel</label>
                                <div>
                                    <input runat="server" type="text" id="BedelTxt" name="BedelTxt" class="form-control input-money text-right" />
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="EnvanterdenCikmaTarTxt" class="control-label">Çıkarma Tarihi</label>
                                <div>
                                    <input runat="server" type="text" id="EnvanterdenCikmaTarTxt" name="EnvanterdenCikmaTarTxt" class="form-control DateTimePickerV1 " readonly="readonly" />
                                </div>
                            </div>
                            <div class="form-group col-6">
                                <label for="AciklamaTxt" class="control-label">Açıklama</label>
                                <div>
                                    <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control " TextMode="MultiLine" Rows="3" ToolTip="Envanterden çıkarmaya dair ayrıntılı bilgi"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton ID="BackBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Geri" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="EnvanterdenCikarBtn" CssClass="btn btn-outline-danger" runat="server" Text="Envanterden Çıkar!" OnClick="EnvanterdenCikarBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" />
                </div>
            </div>
            <div class="modal" id="ModalOnayDiv" role="dialog">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content" style="width: 550px;">
                        <div class="modal-body">
                            <div style="display: none">
                            </div>
                            <div>
                                <div class="text-center">
                                    <h3>
                                        <asp:Label CssClass="col-form-label text-danger" runat="server" Text="Lütfen Dikkat: Taşınmaz Envanterden Çıkarılacak!"></asp:Label></h3>
                                </div>
                                <div class="card-body">
                                    <asp:Label ID="MesajLbl" CssClass="col-form-label text-danger" runat="server" Text="Envanterden Çıkarmayı Onaylıyor musunuz?"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-danger" ID="EnvanterdenCikarNowBtn" runat="server" Text="Envanterden Çıkar!" OnClick="EnvanterdenCikarNowBtn_Click" Visible="false" />
                            <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</div>