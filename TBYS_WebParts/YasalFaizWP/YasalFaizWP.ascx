<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YasalFaizWP.ascx.cs" Inherits="TBYS_WebParts.YasalFaizWP.YasalFaizWP" %>
<div class="container shadow w-50">

    <div class="card">
        <div class="card-header" id="CardHeader" runat="server">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-2">
                <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Aylık Yasal Faiz Oranları"></asp:Label>
                <asp:Label CssClass="col-form-label text-white" ID="IdLbl" runat="server"></asp:Label>
                <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
            </h3>
        </div>
        <div class="card-body " id="MainCardDiv" runat="server">
            <div id="YilDiv" class="border border-dark alert-secondary row pt-2">

                <div class="form-group col-1">
                    <label class="col-form-label" for="YilDDL">Yıl </label>

                </div>
                <div class="form-group col">
                    <asp:DropDownList ID="YilDDL" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="YilDDL_SelectedIndexChanged" />
                </div>
<%--                <div class="form-group col">
                    <label class="col-form-label float-end">Varsayılan Faiz Oranı: </label>

                </div>
                <div class="form-group col-2">
                    <asp:TextBox ID="FaizOraniTxt" runat="server" CssClass="form-control input-money text-end" />
                </div>
                <div class="form-group col">
                    <asp:LinkButton ID="FaizOraniBtn" runat="server" CssClass="btn btn-outline-primary" Text="Faiz Oranlarını Doldur" OnClick="FaizOraniBtn_Click" />
                </div>--%>
            </div>

            <div id="AylikFaizOranlariDiv" class="border border-dark row p-2">
                <asp:Table ID="AyrintiTable" runat="server" class="table table-striped table-hover table-sm table-striped">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Sıra</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Ay </asp:TableHeaderCell>
                        <asp:TableHeaderCell>Yıl</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Faiz Oranı</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">TÜFE(%)</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">ÜFE(%)</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="200">Açıklama</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Kaydet</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira1Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay1Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil1Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani1Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani1Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe1Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe1Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe1Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe1Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama1Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet1Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet1Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira2Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay2Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil2Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani2Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani2Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe2Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe2Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe2Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe2Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama2Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet2Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet2Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira3Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay3Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil3Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani3Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani3Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe3Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe3Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe3Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe3Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama3Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet3Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet3Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira4Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay4Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil4Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani4Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani4Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe4Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe4Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe4Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe4Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama4Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet4Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet4Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira5Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay5Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil5Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani5Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani5Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe5Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe5Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe5Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe5Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama5Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet5Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet5Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira6Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay6Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil6Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani6Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani6Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe6Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe6Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe6Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe6Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama6Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet6Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet6Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira7Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay7Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil7Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani7Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani7Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe7Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe7Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe7Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe7Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama7Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet7Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet7Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira8Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay8Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil8Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani8Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani8Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe8Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe8Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe8Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe8Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama8Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet8Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet8Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira9Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay9Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil9Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani9Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani9Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe9Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe9Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe9Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe9Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama9Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet9Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet9Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira10Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay10Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil10Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani10Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani10Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe10Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe10Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe10Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe10Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama10Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet10Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet10Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira11Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay11Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil11Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani11Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani11Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe11Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe11Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe11Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe11Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama11Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet11Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet11Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ID="Sira12Cell"></asp:TableCell>
                        <asp:TableCell ID="Ay12Cell"></asp:TableCell>
                        <asp:TableCell ID="Yil12Cell"></asp:TableCell>
                        <asp:TableCell ID="FaizOrani12Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="FaizOrani12Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Tufe12Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Tufe12Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Ufe12Cell">
                            <asp:TextBox CssClass="form-control input-money text-end" ID="Ufe12Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell>
                            <asp:TextBox CssClass="form-control" ID="Aciklama12Txt" runat="server"></asp:TextBox>
                        </asp:TableCell>
                        <asp:TableCell ID="Kaydet12Cell">
                            <asp:LinkButton CssClass="form-control btn btn-outline-success" ID="Kaydet12Btn" runat="server" OnClick="Kaydet1Btn_Click">Kaydet</asp:LinkButton>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
        </div>
        <div class="card-footer">
            <asp:LinkButton ID="HepsiniKaydetBtn" runat="server" CssClass="btn btn-outline-success" Text="Hepsini Kaydet" OnClick="HepsiniKaydetBtn_Click" />
            <asp:LinkButton ID="OdemePlaniListBtn" CssClass="btn btn-outline-secondary float-end" runat="server" Text="Ödeme Planları" OnClick="OdemePlaniListBtn_Click" />
        </div>
    </div>
</div>
