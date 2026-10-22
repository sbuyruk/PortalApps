using DocumentFormat.OpenXml.Packaging;
using Microsoft.SharePoint;
using Model.NBYS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Utility.ProjeGlobal;

namespace NBYS_WebParts.Logging
{
    internal sealed class NbysReferralDocumentResult
    {
        internal string FileName { get; set; }
        internal string DownloadUrl { get; set; }
    }

    internal static class NbysReferralDocument
    {
        internal static NbysReferralDocumentResult Create(BagisaVesileOlanTesekkur item, Action saved)
        {
            if (item == null || item.Id <= 0) throw new InvalidOperationException("Belge için kayıtlı teşekkür bilgisi gereklidir.");
            SPWeb web = SPContext.Current.Web;
            SPFile template = web.GetFile(web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/TemplateBagisaVesile.docx");
            if (template == null || !template.Exists) throw new InvalidOperationException("Teşekkür belgesi şablonu bulunamadı.");
            byte[] bytes = template.OpenBinary();
            if (bytes.Length == 0) throw new InvalidOperationException("Teşekkür belgesi şablonu boş.");
            string name = "BagisaVesileOlanTesekkurBelgesi-" + item.Id + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff")
                + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".docx";
            string fileUrl = web.Url + "/" + ProjeConstants.NBYSBELGELERI_LIB + "/" + name;
            string url = web.Url + "/_layouts/15/download.aspx?SourceUrl=" + System.Web.HttpUtility.UrlEncode(fileUrl);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                using (WordprocessingDocument document = WordprocessingDocument.Open(stream, true))
                {
                    if (document.MainDocumentPart == null) throw new InvalidOperationException("Şablon geçerli bir Word belge bölümü içermiyor.");
                    string xml;
                    using (StreamReader reader = new StreamReader(document.MainDocumentPart.GetStream())) xml = reader.ReadToEnd();
                    var values = new Dictionary<string, string>
                    {
                        { "imzalayanAdiSoyadiVar", item.ImzalayanAdiSoyadi }, { "imzalayanUnvanVar", item.ImzalayanUnvan },
                        { "imzalayanMakamVar", item.ImzalayanMakam }, { "belgeTarihiVar", item.BelgeTarihi.ToString("dd MMMM yyyy", new CultureInfo("tr-TR")) },
                        { "belgeNoVar", item.Id.ToString(CultureInfo.InvariantCulture) }, { "soyadiVar", item.Soyadi },
                        { "adiVar", item.Adi }, { "unvanVar", item.Unvan }, { "metin1Var", item.BelgeMetni1 }, { "metin2Var", item.BelgeMetni2 }
                    };
                    // Replace once, as literal XML text; '$', '&' and '<' must not become replacement syntax or markup.
                    xml = Regex.Replace(xml, string.Join("|", values.Keys.Select(Regex.Escape)),
                        match => SecurityElement.Escape((values[match.Value] ?? string.Empty).Trim()));
                    XmlDocument content = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
                    using (XmlReader reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
                        content.Load(reader);
                    using (XmlWriter writer = XmlWriter.Create(document.MainDocumentPart.GetStream(FileMode.Create),
                        new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, CloseOutput = true })) content.Save(writer);
                }
                stream.Position = 0;
                SPList library = web.Lists[ProjeConstants.NBYSBELGELERI_LIB];
                SPFile file = library.RootFolder.Files.Add(name, stream, false);
                if (file == null || !file.Exists) throw new InvalidOperationException("Teşekkür belgesi kitaplığa kaydedilemedi.");
                if (saved != null) saved();
            }
            return new NbysReferralDocumentResult { FileName = name, DownloadUrl = url };
        }
    }
}
