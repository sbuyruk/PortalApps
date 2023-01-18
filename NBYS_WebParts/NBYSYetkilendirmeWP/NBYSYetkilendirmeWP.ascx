<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NBYSYetkilendirmeWP.ascx.cs" Inherits="NBYS_WebParts.NBYSYetkilendirmeWP.NBYSYetkilendirmeWP" %>

<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-info font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="NBYS Yetkilendirme"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <div class="card">
                <div class="card-header">
                    <h2>Bölge Temsilcilikleri</h2>
                </div>
                <div class="card-body">
                     <div class="form-group">
                        <asp:Label ID="Label1" runat="server" Text="Label">Nakit Bağış Listesi</asp:Label>
                        <div class="border border-1">
                            <div class="checkbox pt-3">
                                <label>
                                    <asp:CheckBox ID="BelgeIstemiyorChk" runat="server" Checked="false" ToolTip="Seçildiğinde belge istemeyen bağışçıları Bölge Temsilcilikleri göremez" />
                                    Belge İstemeyen Bağışçıları Bölge Temsilciliklerine Gösterme 
                                </label>
                            </div>
                            <div class="checkbox pt-3">
                                <label>
                                    <asp:CheckBox ID="UlasilamiyorChk" runat="server" Checked="false" ToolTip="Seçildiğinde kendisine ulaşılamayan bağışçıları Bölge Temsilcilikleri göremez" />
                                    Ulaşılamayan Bağışçıları Bölge Temsilciliklerine Gösterme 
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                 <div class="card-footer">

                 </div>
            </div>
            <div class="card">
                <div class="card-header">
                    <h2>Hukuk Müşavirliği</h2>
                </div>
                <div class="card-body">
                   
                </div>
                <div class="card-footer">
                </div>
            </div>
            <div class="card">
                <div class="card-header">
                    <h2>Muh.Fin.Dir.lüğü</h2>
                </div>
                <div class="card-body">
                </div>
                <div class="card-footer">
                </div>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
        </div>
    </div>
</div>