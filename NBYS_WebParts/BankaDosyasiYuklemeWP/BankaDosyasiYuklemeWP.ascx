<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankaDosyasiYuklemeWP.ascx.cs" Inherits="NBYS_WebParts.BankaDosyasiYuklemeWP.BankaDosyasiYuklemeWP" %>

<div class="container-fluid">
    <div class="card text-left shadow">
        <div class="card-header">
            <asp:LinkButton ID="CloseBtn" class="close" runat="server" OnClick="CloseBtn_Click">&times;</asp:LinkButton>
            <h3 class="mb-1">
                <asp:Label CssClass="form-label text-info" runat="server" Text="Banka Dosyası Yükleme"></asp:Label>
            </h3>
        </div>
        <div class="form-group m-3">
            <div class="col-2">
                <label class="form-label fw-bold" for="IslemTarihiTxt">İşlem Tarihi</label>
                <asp:TextBox ID="IslemTarihiTxt" AutoPostBack="true" runat="server" class="form-control DateTimePickerV1" OnTextChanged="islemTarihiTxt_TextChanged"></asp:TextBox>
            </div>
        </div>
        <div id="BankalarCard" class="card-body text-center">

            <h3 class="text-center text-white bg-info">Günlük Dosyalar</h3>
            <div class="form-group row border border-info m-0">
                <div class="col">
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="AkbankLbl" runat="server" Text="AKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="red"></asp:Label>
                            <asp:Label ID="AkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="AkbankFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="FinansbankLbl" runat="server" Text="FİNANSBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="#660066"></asp:Label>
                            <asp:Label ID="FinansbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="FinansbankFU" runat="server" CssClass="form-control" />
                        </div>

                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="GarantiLbl" runat="server" Text="GARANTİ BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="green"></asp:Label>
                            <asp:Label ID="GarantiOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="GarantiFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>


                </div>
                <div class="col">
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="HalkbankLbl" runat="server" Text="HALKBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="DeepSkyBlue"></asp:Label>
                            <asp:Label ID="HalkbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="HalkbankFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="IsbankLbl" runat="server" Text="İŞBANK" Font-Size="X-Large" Font-Bold="True" ForeColor="blue"></asp:Label>
                            <asp:Label ID="IsbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="IsbankFU" runat="server" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="TebLbl" runat="server" Text="TEB" Font-Size="X-Large" Font-Bold="True" ForeColor="limegreen"></asp:Label>
                            <asp:Label ID="TebOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="TebFU" runat="server" />
                        </div>

                    </div>
                </div>
                <div class="col">
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="VakifbankGunlukLbl" runat="server" Text="Vakıfbank (Günlük)" Font-Size="X-Large" Font-Bold="True" ForeColor="orange"></asp:Label>
                            <asp:Label ID="VakifbankGunlukOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifbankGunlukFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="ZiraatLbl" runat="server" Text="ZİRAAT BANKASI" Font-Size="X-Large" Font-Bold="True" ForeColor="orangered"></asp:Label>
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
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="AkbankEkstreLbl" runat="server" Text="Akbank (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="red"></asp:Label>
                            <asp:Label ID="AkbankEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="AkbankEkstreFU" runat="server" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="EDevletLbl" runat="server" Text="E-Devlet" Font-Size="X-Large" Font-Bold="True" ForeColor="turquoise"></asp:Label>
                            <asp:Label ID="EDevletOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="EDevletFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="FinansbankEkstreLbl" runat="server" Text="Finansbank (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="#660066"></asp:Label>
                            <asp:Label ID="FinansbankEkstreOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="FinansbankEkstreFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="GarantiEkstreLbl" runat="server" Text="Garanti (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="green"></asp:Label>
                            <asp:Label ID="GarantiEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="GarantiEkstreFU" runat="server" />
                        </div>
                    </div>



                </div>
                <div class="col">
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="Halkbank2Lbl" runat="server" Text="HALKBANK 2" Font-Size="X-Large" Font-Bold="True" ForeColor="DeepSkyBlue"></asp:Label>
                            <asp:Label ID="Halkbank2OkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload CssClass="form-control" ID="Halkbank2FU" runat="server" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="IsbankEkstreLbl" runat="server" Text="İşbank (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="blue"></asp:Label>
                            <asp:Label ID="IsbankEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="IsbankEkstreFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="SMSVakifLbl" runat="server" Text="SMS Vakıfbank" Font-Size="X-Large" Font-Bold="True" ForeColor="sienna"></asp:Label>
                            <asp:Label ID="SMSVakifOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="SMSVakifFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <%--                    <div class="card  mt-3" style="display: none">
                        <div class="card-header">
                            <asp:Label ID="VakifbankLbl" runat="server" Text="VakıfBank" Font-Size="X-Large" Font-Bold="True" ForeColor="orange"></asp:Label>
                            <asp:Label ID="VakifbankOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifbankFU" runat="server" CssClass="form-control" ToolTip="Vakıfbank dosya seçiniz" Enabled="false" />
                        </div>
                    </div>--%>


                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="Vakifbank2Lbl" runat="server" Text="Vakıfbank 2" Font-Size="X-Large" Font-Bold="True" ForeColor="orange"></asp:Label>
                            <asp:Label ID="Vakifbank2OkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">

                            <asp:FileUpload ID="Vakifbank2FU" runat="server" CssClass="form-control" ToolTip="Vakıfbank 2 dosya seçiniz" />
                        </div>
                    </div>
                </div>
                <div class="col">
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="VakifKatilimLbl" runat="server" Text="VAKIF KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Fuchsia"></asp:Label>
                            <asp:Label ID="VakifKatilimOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="VakifKatilimFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="YKBEkstreLbl" runat="server" Text="Yapı Kredi Bankası" Font-Size="X-Large" Font-Bold="True" ForeColor="#9435dc"></asp:Label>
                            <asp:Label ID="YKBEkstreOkLbl" runat="server" Text="" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="YKBEkstreFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="ZiraatEkstreLbl" runat="server" Text="ZİRAAT BANKASI (Ekstre)" Font-Size="X-Large" Font-Bold="True" ForeColor="OrangeRed"></asp:Label>
                            <asp:Label ID="ZiraatEkstreOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="ZiraatEkstreFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="ZiraatKatilimLbl" runat="server" Text="ZİRAAT KATILIM" Font-Size="X-Large" Font-Bold="True" ForeColor="Black"></asp:Label>
                            <asp:Label ID="ZiraatKatilimOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="ZiraatKatilimFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card  mt-3">
                        <div class="card-header">
                            <asp:Label ID="AlbarakaLbl" runat="server" Text="ALBARAKA" Font-Size="X-Large" Font-Bold="True" ForeColor="Orange"></asp:Label>
                            <asp:Label ID="AlbarakaOkLbl" runat="server" Font-Size="X-Large" Font-Bold="True" ForeColor="#33CC33"></asp:Label>
                        </div>
                        <div class="card-body">
                            <asp:FileUpload ID="AlbarakaFU" runat="server" CssClass="form-control" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div id="FooterCard" class="card-footer">
            <asp:LinkButton CssClass="btn btn-success float-left col-2" ID="KaydetBtn" runat="server" Text="Kaydet" OnClick="KaydetBtn_Click" />
            <asp:LinkButton CssClass="btn btn-primary float-end" ID="NextBtn" runat="server" Text="İleri >>" OnClick="NextBtn_Click" />
        </div>
    </div>
</div>
