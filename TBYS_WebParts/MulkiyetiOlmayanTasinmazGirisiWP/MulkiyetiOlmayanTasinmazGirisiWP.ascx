<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs" Inherits="TBYS_WebParts.MulkiyetiOlmayanTasinmazGirisiWP.MulkiyetiOlmayanTasinmazGirisiWP" %>
<div class="container shadow">
    <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-success font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="Mülkiyeti Olmayan Taşınmaz Girişi"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body row" id="MainCardDiv" runat="server">
                    <div class="col-4">
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KullanimSekliDDL">Kullanım Şekli</label>
                                    <asp:DropDownList ID="KullanimSekliDDL" runat="server" CssClass="form-control" ToolTip="Kullanim Şekli" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="SorumluBolgeTxt">Sor.Bölge</label>
                                    <asp:TextBox ID="SorumluBolgeTxt" runat="server" class="form-control" ToolTip="Sorumlu Bölge/Temsilcilik" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="IliDDL">Bulunduğu İl</label>
                                    <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                </div>
                            </div>
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="CinsiTxt">Cinsi</label>
                                    <asp:TextBox ID="CinsiTxt" runat="server" CssClass="form-control" ToolTip="Cinsi"></asp:TextBox>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="IlcesiDDL">Bulunduğu İlçe</label>
                                    <asp:DropDownList ID="IlcesiDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="MahalleTxt">Mahalle</label>
                                <asp:TextBox ID="MahalleTxt" runat="server" class="form-control" ToolTip="Mahalle"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="CaddeTxt">Cadde</label>
                                <asp:TextBox ID="CaddeTxt" runat="server" class="form-control" ToolTip="Cadde"></asp:TextBox>
                            </div>
                            <div class="form-group m-0 ">
                                <label class="col-form-label" for="AdresTxt">Adres</label>
                                <asp:TextBox ID="AdresTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="4" ToolTip="Taşınmaz adresi"></asp:TextBox>
                            </div>

                        </div>
                    </div>
                    <div class="col-4">
                        <div class="row">
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KullanimDurumuDDL">Kull.Durumu</label>
                                    <asp:DropDownList ID="KullanimDurumuDDL" runat="server" class="form-control" Style="height: auto"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col">
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="SigortaDDL">Sigorta Dur.</label>
                                    <asp:DropDownList ID="SigortaDDL" runat="server" CssClass="form-control" ToolTip="Sigorta Durumu" Style="height: auto"></asp:DropDownList>
                                </div>
                                <div class="form-group m-0 ">
                                    <label class="col-form-label" for="KatMulkiyetiDDL">Kat Mülk.</label>
                                    <asp:DropDownList ID="KatMulkiyetiDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="KatMulkiyetiDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto" />
                                </div>
                            </div>

                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="KoyTxt">Köy</label>
                            <asp:TextBox ID="KoyTxt" runat="server" class="form-control" ToolTip="Köy"></asp:TextBox>
                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="SokakTxt">Sokak</label>
                            <asp:TextBox ID="SokakTxt" runat="server" class="form-control" ToolTip="Sokak"></asp:TextBox>
                        </div>
                        <div class="form-group m-0 ">
                            <label class="col-form-label" for="AciklamaTxt">Açıklama</label>
                            <asp:TextBox ID="AciklamaTxt" runat="server" class="form-control" TextMode="MultiLine" Rows="4" ToolTip="Açıklama"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-4 row">
                        <div class="form-group col">

                        </div>
                        <div class="form-group col">

                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <asp:LinkButton CssClass="btn btn-outline-secondary float-right mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
                    <asp:LinkButton ID="SaveBtn" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
                    <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" Visible="false" OnClick="UpdateBtn_Click" />
                    <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-outline-danger" runat="server" CausesValidation="false" Text="Sil"
                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" OnClick="DeleteBtn_Click" />
                    <asp:LinkButton ID="BagimsizBolumBtn" CssClass="btn btn-outline-warning" runat="server" Text="Bağımsız Böl." Visible="false" OnClick="BagimsizBolumBtn_Click" />
                    <asp:LinkButton ID="SigortaBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Sigorta" Visible="false" OnClick="SigortaBtn_Click" />
                    <asp:LinkButton ID="OnarimlarBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Onarımlar" Visible="false" OnClick="OnarimlarBtn_Click" />
                    <asp:LinkButton ID="ResimlerBtn" CssClass="btn btn-outline-secondary" runat="server" Text="Resimler" Visible="false" OnClick="ResimlerBtn_Click" />

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
