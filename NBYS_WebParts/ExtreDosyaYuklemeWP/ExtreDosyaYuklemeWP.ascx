<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExtreDosyaYuklemeWP.ascx.cs" Inherits="NBYS_WebParts.ExtreDosyaYuklemeWP.ExtreDosyaYuklemeWP" %>
<div class="container">
    <div class="card text-left shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-info" runat="server" Text="Banka Dosyası Yükleme"></asp:Label>
            </h3>
        </div>
        <div id="BankalarCard" class="card-body text-center">
<%--            <div id="HiddenDiv" class="row" style="display: none">
                <input id="IslemTarihiLbl" runat="server" text="Label" style="border-style: none;" text-align="center" />
                <asp:LinkButton ID="IslemTarihiSelectedBtn" CssClass="btn btn-danger" runat="server" CausesValidation="false" Text="" OnClientClick="{return true;};" OnClick="IslemTarihiSelectedBtn_Click" />
            </div>--%>
            <div class="form-group row">
                <label class="col-2 col-form-label font-weight-bold" for="IslemTarihiTxt">İşlem Tarihi</label>
                <asp:TextBox ID="IslemTarihiTxt" AutoPostBack="true" runat="server" class="form-control DateTimePickerV1 col-2" OnTextChanged="islemTarihiTxt_TextChanged"></asp:TextBox>

            </div>

            <div class="row card-group">
                <div class="card">
                    <asp:Label ID="AkbankLbl" runat="server" Text="AKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
                    <asp:Label ID="AkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="AkbankFU" runat="server" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="GarantiLbl" runat="server" Text="GARANTİ BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="#006600"></asp:Label>
                    <asp:Label ID="GarantiOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="GarantiFU" runat="server" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="HalkbankLbl" runat="server" Text="HALKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#3399FF"></asp:Label>
                    <asp:Label ID="HalkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="HalkbankFU" runat="server" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
            </div>
            <div class="row card-group">
                <div class="card">
                    <asp:Label ID="IsbankLbl" runat="server" Text="İŞBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#3333FF"></asp:Label>
                    <asp:Label ID="IsbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload CssClass="btn btn-outline-primary" ID="IsbankFU" runat="server" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="VakifbankLbl" runat="server" Text="VAKIFBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#FFCC00"></asp:Label>
                    <asp:Label ID="VakifbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="VakifbankFU" runat="server" CssClass="btn btn-outline-primary" ToolTip="Vakıfbank dosya yükleme aktif değil" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="ZiraatLbl" runat="server" Text="ZİRAAT BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                    <asp:Label ID="ZiraatOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="ZiraatFU" runat="server" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
            </div>
            <div class="row card-group">
                <div class="card">
                    <asp:Label ID="Halkbank2Lbl" runat="server" Text="HALK KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="#0066FF"></asp:Label>
                    <asp:Label ID="Halkbank2OkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload CssClass="btn btn-outline-primary" ID="Halkbank2FU" runat="server" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="VakifKatilimLbl" runat="server" Text="VAKIF KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Fuchsia"></asp:Label>
                    <asp:Label ID="VakifKatilimOk" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="VakifKatilimFU" runat="server" CssClass="btn btn-outline-primary" ToolTip="Vakıfbank dosya yükleme aktif değil" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="ZiraatKatilimLbl" runat="server" Text="ZİRAAT KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Black"></asp:Label>
                    <asp:Label ID="ZiraatKatilimOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="ZiraatKatilimFU" runat="server" CssClass="btn btn-outline-primary" />
                    </div>
                </div>
            </div>
            <div class="row card-group">
                <div class="card">
                    <asp:Label ID="TebLbl" runat="server" Text="TEB" Font-Size="X-Large" Font-Bold="True" ForeColor="#006600"></asp:Label>
                    <asp:Label ID="TebOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload CssClass="btn btn-outline-primary" ID="TebFU" runat="server" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="Label3" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="Fuchsia"></asp:Label>
                    <asp:Label ID="Label4" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="FileUpload2" runat="server" CssClass="btn btn-outline-primary" Visible="false" ToolTip="Dosya yükleme aktif değil" />
                    </div>
                </div>
                <div class="card">
                    <asp:Label ID="Label5" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="Black"></asp:Label>
                    <asp:Label ID="Label6" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                    <div class="card-body">
                        <asp:FileUpload ID="FileUpload3" runat="server" CssClass="btn btn-outline-primary" Visible="false" ToolTip="Dosya yükleme aktif değil" />
                    </div>
                </div>
            </div>
        </div>
        <div id="FooterCard" class="card-footer">
            <asp:LinkButton CssClass="btn btn-outline-success float-left" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton CssClass="btn btn-outline-primary float-right" ID="NextBtn" runat="server" Text="İleri >>" OnClick="NextBtn_Click" />
        </div>
    </div>
</div>
