<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasinmazOnarimGirisiWP.ascx.cs" Inherits="TBYS_WebParts.TasinmazOnarimGirisiWP.TasinmazOnarimGirisiWP" %>
<div class="container shadow w-75">
    <div class="card">
                <div class="card-header" id="CardHeader" runat="server">
                    <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
                    <h3 class="mb-2">
                        <asp:Label CssClass="col-form-label text-danger fw-bold mb-1" ID="TitleLbl" runat="server" Text="Taşınmaz Onarımları"></asp:Label>
                        <asp:Label CssClass="col-form-label" ID="IdLbl" runat="server"></asp:Label>
                        <asp:Label CssClass="col-form-label " ID="AdiLbl" runat="server"></asp:Label>
                    </h3>
                </div>
        <div class="card-body">
            <asp:UpdatePanel ID="TableUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Table ID="OnarimTable" runat="server" class="table table-hover table-sm border-bottom">
                        <asp:TableHeaderRow>
                            <asp:TableCell ID="HeaderCell0" CssClass="fw-bold">Sıra</asp:TableCell>
                            <asp:TableCell ID="HeaderCell1" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell2" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell3" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell4" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell5" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell6" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell7" CssClass="fw-bold" Visible="false"></asp:TableCell>
                            <asp:TableCell ID="HeaderCell9" ></asp:TableCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                    <div>
                        <div id="EkleDiv1" class="row">
                            <div class="form-group col-5 ">
                                <label for="YapilanIsTxt" class="col-form-label">Yapılan İş</label>
                                <div>
                                    <asp:TextBox ID="YapilanIsTxt" runat="server" class="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group col-3">
                                <label for="HarcamaUsuluDDL" class="col-form-label">Harcama Usulü</label>
                                <div>
                                    <asp:DropDownList ID="HarcamaUsuluDDL" runat="server" class="form-control "></asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="OnayTarihiTxt" class="col-form-label">Onay Tarihi</label>
                                <div>
                                    <input runat="server" type="text" id="OnayTarihiTxt" name="OnayTarihiTxt" class="form-control DateTimePickerV1 input-date" placeholder="gg.aa.yyyy" />
                                </div>
                            </div>
                            <div class="form-group col-2">
                                <label for="TutarTxt" class="col-form-label">Tutar</label>
                                <div>
                                    <asp:TextBox ID="TutarTxt" runat="server" class="form-control input-money"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div id="EkleDiv2" class="row  ">
                            <div class="form-group col-5">
                                <label for="AciklamaTxt" class="col-form-label">Açıklama</label>
                                <div>
                                     <asp:TextBox ID="AciklamaTxt" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" type="text" />
                                </div>
                            </div>
                            <div class="form-group col-3">
                                <label class="col-form-label"></label>
                                <div>
                                    <label for="OnarimEkleBtn" class="col-form-label">.</label>
                                    <asp:LinkButton ID="OnarimEkleBtn" CssClass="btn btn-outline-success" runat="server" Text="Onarım Ekle" OnClick="OnarimEkleBtn_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-secondary float-end mr-2" ID="BackBtn" runat="server" Text="Geri" CausesValidation="false" OnClick="BackBtn_Click" />
        </div>
    </div>
</div>