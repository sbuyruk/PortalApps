using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class UtilityHelper
    {
        public UtilityHelper()
        {

        }
        #region Ortak
        //Ortak
        public static long GenerateUniqueId()
        {
            DateTime dt = DateTime.Today;
            long id = long.Parse(String.Format("{0:yyMMddHHmmssf }", DateTime.Now));
            return id;
        }
        public static string GenerateJpgId(string name)
        {
            //DateTime dt = DateTime.Today;
            name = name.Replace(" ", "");
            name = name.Replace("Ç", "C");
            name = name.Replace("ç", "c");
            name = name.Replace("Ğ", "G");
            name = name.Replace("ğ", "g");
            name = name.Replace("İ", "I");
            name = name.Replace("ı", "i");
            name = name.Replace("Ö", "O");
            name = name.Replace("ö", "o");
            name = name.Replace("Ş", "S");
            name = name.Replace("ş", "s");
            name = name.Replace("Ü", "U");
            name = name.Replace("ü", "u");
            //long id = long.Parse(String.Format("{0:ssfff }", DateTime.Now));
            return name;// +id;
        }
        public static DateTime TariheSaatEkle(DateTime tarih, string saatStr)
        {

            string[] saatLines = saatStr.Split(':');
            int saatInt = saatLines[0].ConvertToInt();
            int dakikaInt = saatLines[1].ConvertToInt();
            TimeSpan saat = new TimeSpan(saatInt, dakikaInt, 0);

            tarih = tarih.Add(saat);
            return tarih;
        }
        public static string GetCurrentUserLoginName()
        {
            string userName = string.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb currentWeb = site.OpenWeb())
                {
                    SPUser user = currentWeb.CurrentUser;
                    if (user != null)
                    {
                        userName = user.LoginName;
                    }
                }
            }
            return userName;
        }
        public static string GetCurrentUserName()
        {
            string name = string.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            {
                using (SPWeb currentWeb = site.OpenWeb())
                {
                    SPUser user = currentWeb.CurrentUser;
                    if (user != null)
                    {
                        name = user.Name;
                    }
                }
            }
            return name;
        }
        public static void SetDDLValue(DropDownList ddl, string value)
        {
            try
            {
                ListItem listItem = new ListItem();
                if (!string.IsNullOrEmpty(value))
                    listItem = ddl.Items.FindByValue(value);

                if (listItem != null)
                {
                    ddl.SelectedValue = listItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }



        }
        public static void SetListBoxValue(ListBox listBox, string value)
        {
            try
            {
                ListItem listItem = new ListItem();
                if (!string.IsNullOrEmpty(value))
                    listItem = listBox.Items.FindByValue(value);

                if (listItem != null)
                {
                    listBox.SelectedValue = listItem.Value;
                }
            }
            catch (Exception)
            {

                //TODO
            }



        }
        public static ImageCodecInfo GetImageCodeInfo(string mimeType)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo imageCodeInfo in imageEncoders)
            {
                if (imageCodeInfo.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                    return imageCodeInfo;
            }
            return null;
        }
        public static byte[] ResizeImage(Stream fileData, int maxwidth, int maxheight)
        {
            using (var image = new Bitmap(fileData))
            {
                int adjustedWidth = image.Width;
                int adjustedHieght = image.Height;

                //Check the image is less than the maxwidth. If not, resize the image dimensions.
                if (adjustedWidth > maxwidth)
                {
                    decimal ratio = Decimal.Divide(maxwidth, adjustedWidth);
                    adjustedWidth = maxwidth;
                    adjustedHieght = Decimal.ToInt32(Decimal.Multiply(adjustedHieght, ratio));//Utility.getInt(Decimal.Multiply(adjustedHieght, ratio));
                }
                //Now that we've adjusted the width, check the hieght is below the maximum hieght value
                if (adjustedHieght > maxheight)
                {
                    decimal ratio = Decimal.Divide(maxheight, adjustedHieght);
                    adjustedHieght = maxheight;
                    adjustedWidth = Convert.ToInt32(Decimal.Multiply(adjustedWidth, ratio));
                }

                var resizedImage = new Bitmap(adjustedWidth, adjustedHieght);
                var g = Graphics.FromImage(resizedImage);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.FillRectangle(Brushes.White, 0, 0, adjustedWidth, adjustedHieght);
                g.DrawImage(image, 0, 0, adjustedWidth, adjustedHieght);
                var ms = new MemoryStream();
                const int quality = 90;
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                resizedImage.Save(ms, GetImageCodeInfo("image/jpeg"), encoderParameters);
                ms.Position = 0;
                var data = new byte[ms.Length];
                ms.Read(data, 0, (int)ms.Length);
                return data;
            }
        }
        public static ExceptionHelper uploadFile2SP(FileUpload fileBrowser, string imgType, string fName, string SPImageListName, ExceptionHelper exceptionHelper, int maxWidth, int maxHeight)
        {
            try
            {
                using (SPSite spsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb web = spsite.OpenWeb())
                    {
                        if (!fileBrowser.HasFile)
                        {
                            Exception exceptionInfo = new Exception(String.Format(fileBrowser.FileName + " : Resim dosyası seçmediniz."));
                            exceptionHelper.Exceptions.Add(exceptionInfo);
                        }
                        else
                        {
                            //Check the file is less than 4MB
                            int fileSize = fileBrowser.PostedFile.ContentLength;
                            if (fileSize > 6000000)
                            {
                                Exception exceptionInfo = new Exception(String.Format(fileBrowser.FileName + " : Dosya boyutu 4Mb'tan büyük olduğu için kaydedilmedi."));
                                exceptionHelper.Exceptions.Add(exceptionInfo);

                            }
                            else
                            {
                                String imageFileExtension = System.IO.Path.GetExtension(fileBrowser.FileName);
                                //Check the user has selected a jpg image
                                if (imageFileExtension == null || imageFileExtension.ToLower() != ".jpg")
                                {
                                    Exception exceptionInfo = new Exception(String.Format(fileBrowser.FileName + " : Lütfen .jpg formatında resim seçiniz."));
                                    exceptionHelper.Exceptions.Add(exceptionInfo);
                                }
                                else
                                {
                                    var imageFileData = fileBrowser.FileBytes;
                                    using (var imageFileStream = new System.IO.MemoryStream())
                                    {
                                        imageFileStream.Write(imageFileData, 0, imageFileData.Length);
                                        //Before uploading the image to SharePoint, lets make sure resize the image if the width or height are greater than 300px. ?sb
                                        var imagePreview = ResizeImage(imageFileStream, maxWidth, maxHeight);
                                        SPList listExists = web.Lists.TryGetList(SPImageListName);
                                        SPUtility.ValidateFormDigest();

                                        if (listExists == null)
                                        {
                                            Exception exceptionInfo = new Exception(String.Format(SPImageListName + " Sharepoint listesi mevcut değil. "));
                                            exceptionHelper.Exceptions.Add(exceptionInfo);
                                        }
                                        try
                                        {
                                            var fileName = fName + imgType + imageFileExtension; //fileBrowser.FileName.Replace(" ", "-");
                                            var urlpreview = String.Format("{0}/" + SPImageListName + "/{1}", web.Url, fileName);
                                            web.AllowUnsafeUpdates = true;
                                            web.Files.Add(urlpreview, imagePreview, true);
                                            //previewImage.ImageUrl = urlpreview;
                                            //previewImage.Visible = true;
                                        }
                                        catch (Exception exception)
                                        {
                                            exceptionHelper.Exceptions.Add(exception);
                                        }
                                    }
                                }
                            }
                        }




                    }
                }
                return exceptionHelper;
            }
            catch (Exception exception)
            {
                exceptionHelper.Exceptions.Add(exception);
                return exceptionHelper;
            }

        }
        public static ExceptionHelper CopyAndCreateImageFromSPLibrary(string sourceImage, string stringNewImageName, string SPImageListName, ExceptionHelper exceptionHelper)
        {
            try
            {
                using (SPSite spsite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb web = spsite.OpenWeb())
                    {
                        //kopyalanacak dosya adı boş mu
                        if (string.IsNullOrEmpty(sourceImage))
                        {
                            Exception exceptionInfo = new Exception(String.Format("Kopyalanacak dosya adı boş."));
                            exceptionHelper.Exceptions.Add(exceptionInfo);
                        }
                        else
                        {
                            //liste var mı
                            //SPPictureLibrary pictureLibrary = (SPPictureLibrary) web.Lists.TryGetList(SPImageListName);
                            SPList splist = web.Lists.TryGetList(SPImageListName);
                            if (splist == null)
                            {
                                Exception exceptionInfo = new Exception(String.Format(SPImageListName + " Sharepoint listesi mevcut değil. "));
                                exceptionHelper.Exceptions.Add(exceptionInfo);
                            }
                            else
                            {

                                foreach (SPListItem item in splist.Items)
                                {
                                    string name = item.Name.ToString();
                                    if (string.Equals(name, "2TasinmazFoto.jpg", StringComparison.OrdinalIgnoreCase))
                                    {
                                        //SPListItem imageItem = splist.Items.Add();
                                        byte[] newFile = item.File.OpenBinary();
                                        //imageItem.File.Name
                                        //imageItem["Title"] = stringNewImageName;
                                        web.AllowUnsafeUpdates = true;
                                        //string urlpreview = "/"+ SPImageListName+"/yeniResim.jpg";
                                        var urlpreview = String.Format("{0}/" + SPImageListName + "/{1}", web.Url, "yeniResim.jpg");
                                        web.Files.Add(urlpreview, newFile, true);
                                        break;
                                    }
                                }
                                //SPQuery query = new SPQuery();
                                //query.Query = @"<Where><Eq><FieldRef Name ='Title'/><Value Type='Text'>'100TasinmazFoto1.jpg'</Value></Eq></Where>";
                                //SPListItemCollection itemCollection = splist.GetItems(query);
                                //SPListItem listItem = itemCollection[0];



                            }
                            try
                            {
                                //web.AllowUnsafeUpdates = true;
                                //web.Files.Add(urlpreview, imagePreview, true);
                                //previewImage.ImageUrl = urlpreview;
                                //previewImage.Visible = true;
                            }
                            catch (Exception exception)
                            {
                                exceptionHelper.Exceptions.Add(exception);
                            }

                        }
                    }
                }
                return exceptionHelper;
            }
            catch (Exception exception)
            {
                exceptionHelper.Exceptions.Add(exception);
                return exceptionHelper;
            }

        }
       
        public static bool ClearTableRows(Table table, bool deleteteHeaderRow)
        {
            bool isDeleted = false;

            try
            {
                int firstRow2Delete = deleteteHeaderRow ? 0 : 1;
                if (table.Rows.Count <= 1) return false;

                var rowCount = table.Rows.Count;
                for (var i = firstRow2Delete; i < rowCount; i++)
                {
                    table.Rows.RemoveAt(firstRow2Delete);
                }
                isDeleted = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isDeleted;
        }
        public static string GetImageUrl(string resimPath)
        {

            string returnUrl = string.Empty;
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string rawUrl = System.Web.HttpContext.Current.Request.RawUrl.ToString();
            int index = currentUrl.IndexOf(rawUrl);
            string rootUrl = (index < 0) ? currentUrl : currentUrl.Remove(index, rawUrl.Length);

            switch (resimPath)
            {
                case ProjeConstants.RESIMLER_BAGISCI:
                    {
                        returnUrl = rootUrl + ProjeConstants.PATH_RESIMLER_BAGISCI;
                        break;
                    }

                default:
                    break;
            }


            return returnUrl;
        }
        public static T CopyProperties<Source, T>(Source source, T target)
        {
            foreach (var sProp in source.GetType().GetProperties())
            {
                bool isMatched = target.GetType().GetProperties().Any(tProp => tProp.Name == sProp.Name && tProp.GetType() == sProp.GetType() && tProp.CanWrite);
                if (isMatched)
                {
                    var value = sProp.GetValue(source);
                    PropertyInfo propertyInfo = target.GetType().GetProperty(sProp.Name);
                    propertyInfo.SetValue(target, value);
                }
            }
            return target;
        }
        public static bool CompareLists<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var deletedItems = list1.Except(list2).Any();
            var newItems = list2.Except(list1).Any();
            return !newItems && !deletedItems;
        }
        public static void ScriptCalistir(string script)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), script, true);
        }
        public static string TelefonFormatla(string telefon)
        {
            telefon = telefon.Trim();
            string formatliTelNo = telefon;
            if (!string.IsNullOrEmpty(telefon))
            {

                // içinde "+", "(" veya ")" veya "-" veya " " var mı
                //  varsa sil
                formatliTelNo = telefon.Replace("+", string.Empty).Replace("/", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty)
                    .Replace("-", string.Empty).Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);

                if (formatliTelNo.Length > 9)// 532 XXX XXX XX en az 10 karakter olsun
                {
                    // başında 90 var mı
                    //  varsa sil
                    string ilkIkiChar = formatliTelNo.Substring(0, 2);
                    if (ilkIkiChar.Equals("90"))
                    {
                        formatliTelNo = formatliTelNo.Substring(2);
                    }
                    // başında 0 var mı
                    //  varsa sil

                    string ilkChar = formatliTelNo.Substring(0, 1);
                    if (ilkChar.Equals("0"))
                    {
                        formatliTelNo = formatliTelNo.Substring(1);
                    }
                }

            }
            return formatliTelNo == null ? string.Empty : formatliTelNo;
        }
        public static string ParametreDegeriSorgula(string parametreAdi)
        {
            string deger = null;
            try
            {
                if (!string.IsNullOrEmpty(parametreAdi))
                {
                    OrtakParametre ortakParametre = new OrtakParametre();
                    ortakParametre = ortakParametre.SelectByAnahtar(parametreAdi);
                    deger = ortakParametre?.Deger;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteTrace(ex);
            }
            return deger;
        }

        public static List<UserPrincipal> GetGroupMembers(string domain, string groupname)
        {
            //burayı application pool yetkisi ile çalıştır
            using (HostingEnvironment.Impersonate())
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupname);
                PrincipalSearchResult<Principal> members = group.GetMembers();
                List<UserPrincipal> list = new List<UserPrincipal>();
                foreach (var item in members)
                {
                    UserPrincipal user = (item as UserPrincipal);
                    list.Add(user);
                }
                return list;
            }
        }
        public static bool IsGroupMember(string groupname, string user)
        {
            bool isMember = false;
            List<UserPrincipal> members = GetGroupMembers("TSKGV", groupname);
            foreach (var item in members)
            {
                string emailaddress = item.EmailAddress;//UserPrincipal.FindByIdentity(item.Context, item.DisplayName).EmailAddress;
               if (user.Trim().Equals(emailaddress.Trim())){
                    isMember = true;
                    break;
                }

            }
            return isMember;
        }

        public static bool IsGroup(string domain, string groupname)
        {
            try
            {
                //burayı application pool yetkisi ile çalıştır
                using (HostingEnvironment.Impersonate())
                {
                    // This code runs as the application pool user
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupname);
                    return group != null;
                }

                
            }
            catch (Exception e)
            {

                MessageHelper.PublishMessage("IsGroup() hatası : "+ e.Message,ProjeConstants.MESAJ_HATA);
                return false;
            }
        }
        #endregion
        #region URL islemleri
        public static string URLGetir()
        {

            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));// < 1 ? currentUrl.LastIndexOf("/")).Substring(0, currentUrl.LastIndexOf("/")) : currentUrl.LastIndexOf("?"));
            newUrl = string.IsNullOrEmpty(newUrl) ? currentUrl.Substring(0, currentUrl.LastIndexOf("/")) : newUrl;
            return newUrl;
        }
        public static string RootURLGetir()
        {
            string sourceString = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string absoluteString = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            string query = System.Web.HttpContext.Current.Request.Url.Query;
            string removeString = absoluteString + query;

            int index = sourceString.IndexOf(removeString);
            string rootUrl = (index < 0)
                ? sourceString
                : sourceString.Remove(index, removeString.Length);

            return rootUrl;
        }
        public static string TbysURLGetir()
        {
            string url = RootURLGetir()+ProjeConstants.PATH_TBYS_URL;
            return url;
        }
        public static string HukukURLGetir()
        {
            string url = RootURLGetir()+ProjeConstants.PATH_HUKUK_URL;
            return url;
        }
        public static string TurkishTxtURLGetir()
        {
            string url = RootURLGetir() + ProjeConstants.PATH_TURKISHTXT_URL;
            return url;
        }
        public static string TbysBelgelerURLGetir()
        {
            string url = TbysURLGetir() + "/" + ProjeConstants.TBYSBELGELERI_LIB;
            return url;
        }
        public static string TbysResimlerURLGetir()
        {
            string url = TbysURLGetir() + "/" + ProjeConstants.RESIMLER_TBYS;
            return url;
        }
        public static string TbysBagisciResimleriURLGetir()
        {
            string url = TbysURLGetir() + "/" + ProjeConstants.RESIMLER_BAGISCI;
            return url;
        }
        #endregion
        #region dosya islemleri pdf
        public static bool DosyaVarMi(string url,string spLibName, string fileName)
        {
            bool isDosyaBulundu = false;

            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[spLibName];
                    SPQuery query = new SPQuery
                    {
                        ViewFields = @"<FieldRef Name='FileLeafRef' />",
                        Query = @"<Where>
                          <Eq>
                            <FieldRef Name='FileLeafRef' />
                            <Value Type='File'>" + fileName + @"</Value>
                          </Eq>
                        </Where>"
                    };
                    SPListItemCollection collection = list.GetItems(query);

                    if (collection.Count > 0)
                    {
                        isDosyaBulundu = true;
                    }
                }

            }
            
            return isDosyaBulundu;
        }
        public static bool UploadFileToSharePoint(FileUpload fileBrowser, string spLibName, string hedefDosyaAdi)
        {
            bool isOk = false;
            try
            {
                string destUrl = SPContext.Current.Web.Url;
                SPWeb site = new SPSite(destUrl).OpenWeb();
                SPList myList = site.Lists[spLibName];
                string destFileUrl = myList.RootFolder.ServerRelativeUrl +"/"+ hedefDosyaAdi;
                site.AllowUnsafeUpdates = true;

                var imageFileData = fileBrowser.FileBytes;
                using (var fileStream = new System.IO.MemoryStream())
                {
                    fileStream.Write(imageFileData, 0, imageFileData.Length);
                    site.Files.Add(destFileUrl, fileStream, true/*overwrite*/);
                }
                isOk = true;
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.Exceptions.Add(new Exception("Dosya Yüklenemedi"));
                eh.PublishException();
                isOk = false;
            }
            return isOk;
        }
        public static bool DeleteFileFromSharePointLib(string subSite, string spLibName, string dosyaAdi)
        {
            bool isOk;
            try
            {
                SPSite site = new SPSite(RootURLGetir());
                SPWeb web = site.AllWebs[subSite];
                SPFolder folder = web.Folders[spLibName];
                SPFile file = folder.Files[dosyaAdi];

                file.Delete();
                isOk = true;
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.Exceptions.Add(new Exception("Dosya Yüklenemedi"));
                eh.PublishException();
                isOk = false;
            }
            return isOk;
        }
        public static List<SPFile> GetFileListFromSharePointLib(string subSite, string spLibName, string partialFileName)
        {
            List<SPFile> list = new List<SPFile>();
            try
            {
                SPSite site = new SPSite(RootURLGetir());
                SPWeb web = site.AllWebs[subSite];
                SPFolder folder = web.Folders[spLibName];
                
                foreach (SPFile item in folder.Files)
                {
                    list.Add(item);
                }
                
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
            }
            return list;
        }
        public static List<string> GetFileNameListFromSharePointLib(string subSite, string spLibName, string partialFileName)
        {
            List<string> list = new List<string>();
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    //New SPSite object.
                    using (SPSite site = new SPSite(RootURLGetir()))
                    {
                        //Do things by assuming the permission of the "system account".
                        SPWeb web = site.AllWebs[subSite];
                        SPFolder folder = web.Folders[spLibName];
                        foreach (SPFile item in folder.Files)
                        {
                            if (item.Name.Contains(partialFileName))
                            {
                                list.Add(item.Name);
                            }

                        }
                    }
                });


                //SPSite site = new SPSite(RootURLGetir());
                //SPWeb web = site.AllWebs[subSite];
                //SPFolder folder = web.Folders[spLibName];
                
                //foreach (SPFile item in folder.Files)
                //{
                //    if (item.Name.Contains(partialFileName))
                //    {
                //        list.Add(item.Name);
                //    }
                    
                //}
                
            }
            catch (Exception e)
            {
                ExceptionHelper eh = new ExceptionHelper(e);
                eh.PublishException();
            }
            return list;
        }
        #endregion
        #region Table islemleri
        public static void SetTableHeaders(Table table,string[] headers)
        {
            table.Controls.Clear();
            TableHeaderRow row = new TableHeaderRow();  
            foreach (var item in headers)
            {
                TableHeaderCell cell = new TableHeaderCell
                {
                    Text = item
                };
                row.Controls.Add(cell);
            }
            table.Rows.Add(row);
            
        }
        #endregion
        public static string BolgeGetir(int ilId)
        {
            string bolgeAdi = string.Empty;
            Il il = new Il();
            il = il.Select<Il>(ilId);
            if (il != null)
            {
                Bolge bolge = new Bolge();
                bolge = bolge.Select(il.BolgeId);
                bolgeAdi = bolge.KisaAdi;
            }
            return (bolgeAdi);
        }
    }
}
