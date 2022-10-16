<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankaDosyasiYuklemeWP.ascx.cs" Inherits="NBYS_WebParts.BankaDosyasiYuklemeWP.BankaDosyasiYuklemeWP" %>
<div class="container">
    <div class="card text-left shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="col-form-label text-info" runat="server" Text="Banka Dosyası Yükleme"></asp:Label>
            </h3>
        </div>
        <div id="BankalarCard" class="card-body text-center">
            <div class="form-group row">
                <label class="col-2 col-form-label font-weight-bold" for="IslemTarihiTxt">İşlem Tarihi</label>
                <asp:TextBox ID="IslemTarihiTxt" AutoPostBack="true" runat="server" class="form-control DateTimePickerV1 col-2" OnTextChanged="islemTarihiTxt_TextChanged"></asp:TextBox>
            </div>
            <h3 class="text-center text-white bg-info">Günlük Dosyalar</h3>
            <div class="form-group row border border-info m-0">
                <div class="col">
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="AkbankLbl" runat="server" Text="AKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
                            <asp:Label ID="AkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="AkbankFU" runat="server" CssClass="form-control" />
                        </div>

                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="GarantiLbl" runat="server" Text="GARANTİ BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="#006600"></asp:Label>
                            <asp:Label ID="GarantiOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="GarantiFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="HalkbankLbl" runat="server" Text="HALKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#3399FF"></asp:Label>
                            <asp:Label ID="HalkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="HalkbankFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                </div>
                <div class="col">
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="IsbankLbl" runat="server" Text="İŞBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#3333FF"></asp:Label>
                            <asp:Label ID="IsbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="IsbankFU" runat="server" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="TebLbl" runat="server" Text="TEB" Font-Size="X-Large" Font-Bold="True" ForeColor="#006600"></asp:Label>
                            <asp:Label ID="TebOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="TebFU" runat="server" />
                        </div>

                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="VakifbankGunlukLbl" runat="server" Text="Vakıfbank(Günlük)" Font-Size="X-Large" Font-Bold="True" ForeColor="#FFCC00"></asp:Label>
                            <asp:Label ID="VakifbankGunlukOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifbankGunlukFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="ZiraatLbl" runat="server" Text="ZİRAAT BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                            <asp:Label ID="ZiraatOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="ZiraatFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <h3 class="text-center text-white bg-danger">Dönemlik Dosyalar</h3>
            <div class="form-group row border border-info m-0">
                <div class="col">
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="AkbankEkstreLbl" runat="server" Text="Akbank (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF3300"></asp:Label>
                            <asp:Label ID="AkbankEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="AkbankEkstreFU" runat="server" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="GarantiEkstreLbl" runat="server" Text="Garanti (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#006600"></asp:Label>
                            <asp:Label ID="GarantiEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="GarantiEkstreFU" runat="server" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="Halkbank2Lbl" runat="server" Text="HALKBANK 2" Font-Size="X-Large" Font-Bold="True" ForeColor="#3399FF"></asp:Label>
                            <asp:Label ID="Halkbank2OkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="Halkbank2FU" runat="server" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="IsbankEkstreLbl" runat="server" Text="İşbank (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#3333FF"></asp:Label>
                            <asp:Label ID="IsbankEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="IsbankEkstrebankFU" runat="server" CssClass="form-control"  />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="KartIleLbl" runat="server" Text="Kart ile Bağış" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF6600"></asp:Label>
                            <asp:Label ID="KartIleOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <div  class="row">
                                <asp:Label CssClass="col-form-label col" ID="Label1" runat="server" Text="Label">Bağış Tarihi:</asp:Label>
                                <input runat="server" type="text" id="BagisTarihiTxt" class="form-control DateTimePickerV1 col-4" readonly="readonly" />
                            </div>
                            <asp:FileUpload ID="KartIleFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                </div>
                <div class="col">

                    <div class="card  m-4" style="display: none">
                        <div class="card-header">
                            <asp:Label ID="VakifbankLbl" runat="server" Text="VakıfBank" Font-Size="X-Large" Font-Bold="True" ForeColor="#FFCC00"></asp:Label>
                            <asp:Label ID="VakifbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifbankFU" runat="server" CssClass="form-control" ToolTip="Vakıfbank dosya seçiniz" Enabled="false"/>
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="Vakifbank2Lbl" runat="server" Text="VakıfBank 2" Font-Size="X-Large" Font-Bold="True" ForeColor="#FF5900"></asp:Label>
                            <asp:Label ID="Vakifbank2OkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">

                            <asp:FileUpload ID="Vakifbank2FU" runat="server" CssClass="form-control" ToolTip="Vakıfbank 2 dosya seçiniz" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="ZiraatEkstreLbl" runat="server" Text="ZİRAAT BANKASI (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
                            <asp:Label ID="ZiraatEkstreOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="ZiraatEkstreFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="VakifKatilimLbl" runat="server" Text="VAKIF KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Fuchsia"></asp:Label>
                            <asp:Label ID="VakifKatilimOk" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifKatilimFU" runat="server" CssClass="form-control" ToolTip="Vakıfbank dosya yükleme aktif değil" />
                        </div>
                    </div>
                    <div class="card  m-4">
                        <div class="card-header">
                            <asp:Label ID="ZiraatKatilimLbl" runat="server" Text="ZİRAAT KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Black"></asp:Label>
                            <asp:Label ID="ZiraatKatilimOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="ZiraatKatilimFU" runat="server" CssClass="form-control" />
                        </div>
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