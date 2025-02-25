<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TesekkurBelgesiYazisiWP.ascx.cs" Inherits="NBYS_WebParts.TesekkurBelgesiYazisiWP.TesekkurBelgesiYazisiWP" %>



<div class="container shadow">
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Teşekkür Belgesi Oluşturma"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row p-2">
                        <div class="form-group col-3 border border-dark  border-right-0">

                            <div class="form-group " style="display: block">
                                <label class="col-form-label" for="GunDDL">Gün </label>
                                <asp:DropDownList ID="GunDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="GunDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="AyDDL">Ay </label>
                                <asp:DropDownList ID="AyDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="AyDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="YilDDL">Yıl </label>
                                <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                            <div class="form-group ">
                                <label class="col-form-label" for="BolgeDDL">Bölge </label>
                                <asp:DropDownList ID="BolgeDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="BolgeDDL_SelectedIndexChanged" Style="height: auto" />
                            </div>
                        </div>
                        <div class="form-group col-3 border border-dark  border-right-0">
                            <div class="form-group">
                                <label for="ImzalayanTxt" class="col-form-label">İmza (Adi Soyadı)</label>
                                <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="ImzalayanUnvanTxt" class="col-form-label">İmza (Ünvan)</label>
                                <asp:TextBox ID="ImzalayanUnvanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="ImzalayanMakamTxt" class="col-form-label">İmza (Makam)</label>
                                <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col-4 border border-dark  border-right-0">
                            <div class="form-group ">
                                <asp:Table ID="TesekkurTable" runat="server" class="table table-bordered table-striped">
                                </asp:Table>
                            </div>
                        </div>
                        <div class="form-group col-2 border border-dark">
                            <div class="form-group">
                                <label for="EvrakTarihiTxt" class="col-form-label">Evrak Tarihi</label>
                                <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-bordered table-hover small" width="100%">
                            <thead>
                                <tr>
                                    <th>Adı</th>
                                    <th>Belgede Yazan İsim</th>
                                    <th>TC Kimlik</th>
                                    <th>Tarih</th>
                                    <th>Tutar</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="TableDataLbl" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Teşekkür Belgeleri</asp:HyperLink>
                    <asp:HyperLink ID="AdresEtiketLnk" runat="server" CssClass="btn-link m-3" Visible="false">Adres Etiketleri</asp:HyperLink>
                </div>

                <div class="card-footer">
                    <asp:CheckBox ID="TesekkurDurumChk" runat="server" Checked="false" Text=" -Gönderildi- yap" Enabled="False" />
                    <asp:LinkButton ID="DosyaOlusturBtn" runat="server" CssClass="btn btn-outline-success" OnClick="DosyaOlusturBtn_Click">Teşekkür Belgelerini Oluştur</asp:LinkButton>
                    <asp:LinkButton ID="AdresOlusturBtn" runat="server" CssClass="btn btn-outline-info" OnClick="AdresOlusturBtn_Click">Adres Oluştur</asp:LinkButton>
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
