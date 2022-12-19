<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MtsOlayListesiWP.ascx.cs" Inherits="MTS_WebParts.MtsOlayListesiWP.MtsOlayListesiWP" %>


<div class="container">
    <div class="card shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-primary font-weight-bold mb-1" ID="TitleLbl" runat="server" Text="MTS Gerçekleşen İşlemler"></asp:Label>
                <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="form-group">
                        <table id="CustomDataTable" class="table table-striped table-bordered" width="100%">
                            <thead>
                                <tr>
                                    <th>Tarih</th>
                                    <th>İşlem Konusu</th>
                                    <th>İşlem Tipi</th>
                                    <th>Açıklama</th>
                                    <th>İşlem Yapan</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <%--<asp:Timer ID="RefreshTimer" runat="server" OnTick="RefreshTimer_Tick">
                    </asp:Timer>--%>
                </ContentTemplate>
<%--                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RefreshTimer" EventName="tick" />
                </Triggers>--%>
            </asp:UpdatePanel>
           
        </div>
        <div class="card-footer">
           
        </div>
    </div>

    
</div>
