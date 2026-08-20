<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BagimsizBolumWP.ascx.cs" Inherits="TBYS_WebParts.BagimsizBolumWP.BagimsizBolumWP" %>

<script type="text/javascript">
    function BagimsizBolumModal() {
        var myModalInstance = bootstrap.Modal.getOrCreateInstance(document.getElementById('BagimsizBolumModal'));
        myModalInstance.show();
    }
</script>

<div class="col-xl small">
    <div class="card shadow">
        <div class="card-header ">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Bağımsız Bölüm"></asp:Label>
                <asp:Label ID="AdiLbl" runat="server" CssClass="col-form-label"></asp:Label>
                <asp:Label ID="IdLbl" runat="server" CssClass="col-form-label text-white" Visible="true"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
                <ContentTemplate>
                    <div class="form-group">
                        <h4>
                            <asp:Label ID="TableLbl" runat="server" class="col-form-label"></asp:Label>
                        </h4>
                        <div class="table border-bottom" style="max-height: 400px; overflow: auto;">
                            <asp:Table ID="BagimsizBolumTable" runat="server" CssClass="table table-sm table-hover">
                            </asp:Table>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="BagimsizBolumBtn" CssClass="btn btn-outline-success" runat="server" Text="Bağımsız Bölüm Ekle" OnClick="BagimsizBolumBtn_Click" />
            <asp:LinkButton ID="TasinmazaGitBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Taşınmaza Git" OnClick="TasinmazaGitBtn_Click" />
        </div>
    </div>
</div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ViewStateMode="Enabled">
    <ContentTemplate>
        <div class="modal" id="BagimsizBolumModal" role="dialog">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header text-danger">
                        <h3 class="col-form-label fw-bold" id="BagimsizBolumHeaderLbl" runat="server"></h3>
                    </div>
                    <div class="modal-body ">
                        <div class="form-group">
                            <div class="row">
                                
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Bölüm No"></asp:Label>
                                        <asp:TextBox ID="BolumNoTxt" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div> 
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Nitelik"></asp:Label>
                                        <asp:TextBox ID="NitelikTxt" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Brüt Alan"></asp:Label>
                                        <asp:TextBox ID="BBBrutAlanTxt" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Net Alan"></asp:Label>
                                        <asp:TextBox ID="BBNetAlanTxt" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Kullanim Şekli"></asp:Label>
                                        <asp:DropDownList ID="KullanimAmaciDDL" runat="server" CssClass="form-control form-select form-select-lg" ToolTip="Kullanım Şekli" Style="height: auto"></asp:DropDownList>
                                    </div>
                                </div>
                                <div style="display: none">
                                    <asp:Label ID="ParamBagimsizBolumIdLbl" runat="server" Text=""></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label class="form-label fw-bold" runat="server" Text="Muhs.Kayt.Değ."></asp:Label>
                                        <asp:TextBox ID="MuhasebeyeKayitliDegerTxt" class="form-control input-money text-end" runat="server" />
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label class="form-label fw-bold text-end" runat="server" Text="Tah.Rayiç Değ."></asp:Label>
                                        <asp:TextBox ID="TahminiRayicDegeriTxt" class="form-control input-money text-end" runat="server" />
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label class="form-label fw-bold" runat="server" Text="Eml.Bey.Değ."></asp:Label>
                                        <asp:TextBox ID="EmlakBeyanDegeriTxt" class="form-control input-money text-end" runat="server" />
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label class="form-label fw-bold" runat="server" Text="Yak.Piyasa Değ."></asp:Label>
                                        <asp:TextBox ID="YaklasikPiyasaDegeriTxt" class="form-control input-money text-end" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Adres"></asp:Label>
                                        <asp:TextBox ID="AdresTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="3" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col">
                                    <div class="form-group">
                                        <asp:Label CssClass="form-label fw-bold" runat="server" Text="Açıklama"></asp:Label>
                                        <asp:TextBox ID="AciklamaTxt" CssClass="form-control" runat="server" Text="" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div>
                                <asp:Label ID="MessageLbl" runat="server" CssClass="form-label"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div id="BtnDiv" style="display: block">
                            <asp:LinkButton ID="KaydetBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Kaydet " OnClick="KaydetBtn_Click" />
                            <asp:LinkButton ID="GuncelleBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Güncelle " OnClick="GuncelleBtn_Click" />
                            <asp:LinkButton ID="SilBtn" runat="server" CssClass="btn btn-success" CausesValidation="false" Text=" Sil " OnClick="SilBtn_Click" />
                        </div>
                        <button type="button" class="btn btn-default" data-bs-dismiss="modal">Kapat</button>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="KaydetBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="GuncelleBtn" EventName="click" />
        <asp:AsyncPostBackTrigger ControlID="SilBtn" EventName="click" />
    </Triggers>
</asp:UpdatePanel>
