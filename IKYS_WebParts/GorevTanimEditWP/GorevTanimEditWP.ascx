<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GorevTanimEditWP.ascx.cs" Inherits="IKYS_WebParts.GorevTanimEditWP.GorevTanimEditWP" %>
<div class="container shadow w-50">

    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label  btn-outline-primary mb-1" ID="TitleLbl" runat="server" Text="Görev/Kadro Düzenleme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="PersonelIdLbl" Visible="false" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label text-secondary float-end" ID="EkranNo" Text="13" runat="server" ></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel" runat="server" >
                <ContentTemplate>
                    <div class="form-group">
                        <div class="card border-0">
                            <div class="form-group m-0">
                                <label class="col-form-label" for="AdiTxt">Görev/Kadro</label>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="AdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                <asp:TextBox ID="AdiTxt" runat="server" class="form-control" ToolTip="Adı" type="text"></asp:TextBox>
                            </div>
                            <div class="form-group m-0">
                                <label class="col-form-label" for="KisaAdiTxt">Görev Kısaltması</label>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="KisaAdiTxt" ForeColor="Red" ErrorMessage="Zorunlu Alan"> </asp:RequiredFieldValidator>
                                <asp:TextBox ID="KisaAdiTxt" runat="server" class="form-control" ToolTip="Soyadı" type="text"></asp:TextBox>
                            </div>

                            <div class="form-group m-0">
                                <label class="col-form-label" for="PersonelDDL">Personel</label> 
                                <asp:DropDownList ID="PersonelDDL" runat="server" class="form-control form-select form-select-lg" style="height:auto" Enabled="false" ToolTip="Personel atamasını kişinin sayfasından yapınız"></asp:DropDownList>
                            </div>
                            <div class="form-group m-0">
                                <label class="col-form-label" for="BirimDDL">Birim/Şube</label>
                                <asp:DropDownList ID="BirimDDL" runat="server" class="form-control form-select form-select-lg" style="height:auto"></asp:DropDownList>
                            </div>
                            <div class="form-group m-0">
                                <label class="col-form-label" for="HarcirahDDL">Birim/Şube</label>
                                <asp:DropDownList ID="HarcirahDDL" runat="server" class="form-control form-select form-select-lg"></asp:DropDownList>
                            </div>
                            <div class="row">
                                <div class="form-group col m-0">
                                    <label class="col-form-label text-white" for="VekilChk">.... ... ...</label>
                                    <asp:CheckBox ID="VekilChk" CssClass="form-control" runat="server" Text=". Vekil" Checked="false" TextAlign="Right"  />     
                                </div>
                                <div class="form-group col m-0">
                                    <label class="col-form-label text-white" for="AktifChk">.... ... ...</label>
                                    <asp:CheckBox ID="AktifChk" CssClass="form-control" runat="server" Text=". Aktif" Checked="true" TextAlign="Right"  />     
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SaveBtn" EventName="Click" />                    
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="GorevTanimListesiBtn" runat="server" Text="Görev Tanım Listesi" CausesValidation="false" OnClick="GorevTanimListesiBtn_Click" />
            <asp:LinkButton ID="SaveBtn" Visible="false" CssClass="btn btn-outline-success" runat="server" Text="Kaydet" OnClick="SaveBtn_Click" />
            <asp:LinkButton ID="UpdateBtn" CssClass="btn btn-outline-primary" runat="server" Text="Güncelle" OnClick="UpdateBtn_Click" />
            <asp:LinkButton ID="DeleteBtn" Visible="false" CssClass="btn btn-danger" runat="server" CausesValidation="false" Text="Sil" OnClick="DeleteBtn_Click"
                OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" />
        </div>
    </div>

</div>