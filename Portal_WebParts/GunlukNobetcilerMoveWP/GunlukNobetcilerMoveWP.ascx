<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GunlukNobetcilerMoveWP.ascx.cs" Inherits="Portal_WebParts.GunlukNobetcilerMoveWP.GunlukNobetcilerMoveWP" %>
<script type="text/javascript">
    function OpenNobetciPopup(clicked) {
        $("#NobetciPopupDiv").modal({ backdrop: true });
    }

</script>

<div class="part-item staff-member" onclick=" OpenNobetciPopup(true);">
    <div class="title">
        <h5>
            <asp:Label ID="TitelLbl" CssClass="col-form-label" runat="server" Text="Bugün Görevli Personel"></asp:Label>
        </h5>
    </div>
    <div class="part-inner">
        <ul class="staff-list mCustomScrollbar" runat="server" id="ulUsers">

            <asp:Repeater ID="NobetciRepeater" runat="server">
                <ItemTemplate>
                    <li class="cf">
                        <div class="user-photo">
                            <img src="<%#Eval("PersonelResimleri") %><%#Eval("KullaniciAdi") %>.jpg?RenditionID=1" alt="" onerror="this.src='/PersonelResimleri/personel.jpg';">
                        </div>
                        <div class="user-info">
                            <div class="user-name">
                                <p><%#Eval("Isim") %></p>
                            </div>
                            <div class="user-passenger">
                                <div class="passenger-name">
                                    <p><%#Eval("Bolum") %></p>
                                </div>
                            </div>
                    </li>

                </ItemTemplate>

            </asp:Repeater>

        </ul>

        <div runat="server" id="divEmptyUsers" visible="false">Günlük görevli personel bulunamamıştır.</div>
    </div>
</div>
<div class="modal " id="NobetciPopupDiv" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body ">
                <div class="card">
                    <div class="card-header">
                        <h5>
                            <asp:Label ID="Label1" CssClass="font-weight-bold" runat="server" Text="Bugün Görevli Personel" Font-Size="Large"></asp:Label>
                        </h5>
                    </div>
                    <div class="card-body">
                        <div class="part-inner">
                            <ul class="staff-list mCustomScrollbar" runat="server" id="ul1">
                                <asp:Repeater ID="PopupRepeater" runat="server">
                                    <ItemTemplate>
                                        <li class="row form-group border border-dark alert-primary pt-2">
                                            <div class="col-3">
                                                <%--<img height="100" class="rounded-circle border" src="/PersonelResimleri/<%#Eval("KullaniciAdi") %>.jpg?RenditionID=5" alt="" onerror="this.src='/PersonelResimleri/personel.jpg';">--%>
                                                <img height="100" class="rounded-circle border" src="<%#Eval("PersonelResimleri") %><%#Eval("KullaniciAdi") %>.jpg?RenditionID=5" alt="" onerror="this.src='/PersonelResimleri/personel.jpg';">
                                            </div>
                                            <div class="col">
                                                <div class="font-weight-bold">
                                                    <p><%#Eval("Isim") %></p>
                                                </div>
                                                <div class="font-weight-bold">
                                                    <p><%#Eval("Bolum") %></p>
                                                </div>
                                            </div>
                                        </li>

                                    </ItemTemplate>

                                </asp:Repeater>

                            </ul>
                            <div runat="server" id="div1" visible="false">Günlük görevli personel bulunamamıştır.</div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-dismiss="modal">Kapat</button>
            </div>
        </div>

    </div>
</div>
