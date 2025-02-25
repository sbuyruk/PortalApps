<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FTKYazilariWP.ascx.cs" Inherits="NBYS_WebParts.FTKYazilariWP.FTKYazilariWP" %>

<div class="container ">
    <asp:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <div class="card shadow">
                <div class="card-header">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <a class=" btn btn-outline-primary float-end mr-4" runat="server" id="YonergeLnk"
                            data-fancybox
                            data-type="pdf"
                            data-width="960"
                            data-height="720"
                            href="">
                            <i class="fa fa-book" aria-hidden="true"></i>
                        </a>
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="FTK Yazıları"></asp:Label>
                        <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="card-body">
                    <div class="form-group row p-2">
                        <div class="form-group col-2 border">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" for="IliDDL">İli</asp:Label>
                                <asp:DropDownList ID="IliDDL" runat="server" CssClass="form-control" OnSelectedIndexChanged="IliDDL_SelectedIndexChanged" AutoPostBack="true" Style="height: auto"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server" for="IlcesiDDL">İlçesi</asp:Label>
                                <asp:DropDownList ID="IlcesiDDL" runat="server" CssClass="form-control" AutoPostBack="true" Style="height: auto" OnSelectedIndexChanged="IlcesiDDL_SelectedIndexChanged"></asp:DropDownList>
                            </div>
<%--                            <div class="form-group">
                                <asp:Label CssClass="col-from-label" runat="server" Text="Sor.Bölge" Enabled="False"></asp:Label>
                                <asp:TextBox ID="SorumluBolgeTxt" CssClass="form-control" runat="server" Text="" ReadOnly="true"></asp:TextBox>
                            </div>--%>
                        </div>
                        
                        <div class="form-group col-3 border">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İlgi Tarihi</asp:Label>
                                <asp:TextBox ID="IlgiTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İlgi Sayısı</asp:Label>
                                <asp:TextBox ID="IlgiSayisiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                                                        <div class="form-group">
                                <label for="EvrakTarihiTxt" class="col-form-label">Evrak Tarihi</label>
                                <asp:TextBox ID="EvrakTarihiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="EvrakTarihiTxt" class="col-form-label">Evrak Sayısı</label>
                                <asp:TextBox ID="EvrakSayisiTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col border">

                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">Parafe (Grup Başkanı)</asp:Label>
                                <asp:TextBox ID="Parafe1Txt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">Parafe (Şb.Md.)</asp:Label>
                                <asp:TextBox ID="Parafe2Txt" runat="server" CssClass="form-control">…./01/2024 B.H.Dir. M.DİRİCAN</asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İrtibat Noktası</asp:Label>
                                <asp:TextBox ID="IrtibatNoktasiTxt" runat="server" CssClass="form-control">Dorukhan GÜNDÜR (Dâhili Tel:261)</asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group col border">
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İmza (Adi Soyadı)</asp:Label>
                                <asp:TextBox ID="ImzalayanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İmza (Ünvan)</asp:Label>
                                <asp:TextBox ID="ImzalayanUnvanTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label CssClass="col-form-label" runat="server">İmza (Makam)</asp:Label>
                                <asp:TextBox ID="ImzalayanMakamTxt" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="form-group col checkbox" id="EPostaDiv" runat="server" style="display: block;">
                        <label>
                            <asp:CheckBox ID="EPostaGonderChk" runat="server" Checked="true" ToolTip="Kayıt ve güncelleme yapıldığında sorumlu bölgeye E-Posta göndermek için seçin." />
                            Kayıt veya güncelleme yapıldığında sorumlu bölgeye E-Posta gönderilsin.
                        </label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:HyperLink ID="DosyaLnk" runat="server" CssClass="btn-link m-3" Visible="false">Yazı</asp:HyperLink>
                </div>

                <div class="card-footer">
                    <asp:LinkButton ID="KurulusYazisiOlusturBtn" runat="server" CssClass="btn btn-outline-success" OnClick="KurulusYazisiOlusturBtn_Click">Kuruluş Yazısı Oluştur</asp:LinkButton>
                    <asp:LinkButton ID="GuncellemeYazisiOlusturBtn" runat="server" CssClass="btn btn-outline-primary" OnClick="GuncellemeYazisiOlusturBtn_Click">Güncelleme Yazısı Oluştur</asp:LinkButton>
                    <asp:LinkButton ID="FTKListesiBtn" runat="server" CssClass="btn btn-outline-secondary float-end" Text="FTK Listesi" OnClick="FTKListesiBtn_Click"></asp:LinkButton>
                    <asp:LinkButton ID="FTKIslemleriBtn" CssClass="btn btn-outline-secondary float-end mr-3" runat="server" Text="FTK İşlemleri" OnClick="FTKIslemleriBtn_Click" CausesValidation="False"></asp:LinkButton>
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