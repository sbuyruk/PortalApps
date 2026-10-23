using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;
using Utility.ProjeGlobal;

namespace Portal_WebParts.Logging
{
    internal static class PortalDuyuruImage
    {
        internal static string Upload(FileUpload upload)
        {
            if (upload.PostedFile.ContentLength > 6000000)
                throw new PortalInputException("Resim en fazla 6 MB olabilir. Daha küçük bir JPG dosyası seçin.");
            if (!string.Equals(Path.GetExtension(upload.FileName), ".jpg", StringComparison.OrdinalIgnoreCase))
                throw new PortalInputException("Resim için JPG dosyası seçin.");
            byte[] jpeg;
            using (var stream = new MemoryStream(upload.FileBytes))
            {
                Image image;
                try { image = Image.FromStream(stream, true, true); }
                catch (ArgumentException) { throw new PortalInputException("Resim okunamadı. Geçerli bir JPG dosyası seçin."); }
                using (image)
                {
                    if (image.RawFormat.Guid != ImageFormat.Jpeg.Guid || (long)image.Width * image.Height > 40000000)
                        throw new PortalInputException("Geçerli ve daha küçük boyutlu bir JPG resmi seçin.");
                    double scale = Math.Min(1d, Math.Min((double)ProjeConstants.RESIM_DIGER_EN / image.Width,
                        (double)ProjeConstants.RESIM_DIGER_BOY / image.Height));
                    using (var resized = new Bitmap(Math.Max(1, (int)(image.Width * scale)), Math.Max(1, (int)(image.Height * scale))))
                    using (var graphics = Graphics.FromImage(resized))
                    using (var output = new MemoryStream())
                    {
                        graphics.Clear(Color.White);
                        graphics.DrawImage(image, 0, 0, resized.Width, resized.Height);
                        resized.Save(output, ImageFormat.Jpeg);
                        jpeg = output.ToArray();
                    }
                }
            }
            SPUtility.ValidateFormDigest();
            SPList library = SPContext.Current.Web.Lists.TryGetList(ProjeConstants.RESIMLER_DUYURU);
            if (library == null) throw new InvalidOperationException("Duyuru resim kitaplığı bulunamadı.");
            string name = "Duyuru_" + Guid.NewGuid().ToString("N");
            library.RootFolder.Files.Add(library.RootFolder.Url + "/" + name + ".jpg", jpeg, false);
            return name;
        }
    }
}
