using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace Model.Ortak
{
    public class OpenXMLHelper
    {
        private System.Collections.Generic.IDictionary<System.String, OpenXmlPart> UriPartDictionary = new System.Collections.Generic.Dictionary<System.String, OpenXmlPart>();
        private System.Collections.Generic.IDictionary<System.String, DataPart> UriNewDataPartDictionary = new System.Collections.Generic.Dictionary<System.String, DataPart>();
        private WordprocessingDocument document;

        public void ChangePackage(string filePath)
        {
            using (document = WordprocessingDocument.Open(filePath, true))
            {
                ChangeParts();
            }
        }

        private void ChangeParts()
        {
            //Stores the referrences to all the parts in a dictionary.
            BuildUriPartDictionary();
            //Changes the contents of the specified parts.
            ChangeMainDocumentPart1(document.MainDocumentPart);
        }

        /// <summary>
        /// Stores the references to all the parts in the package.
        /// They could be retrieved by their URIs later.
        /// </summary>
        private void BuildUriPartDictionary()
        {
            System.Collections.Generic.Queue<OpenXmlPartContainer> queue = new System.Collections.Generic.Queue<OpenXmlPartContainer>();
            queue.Enqueue(document);
            while (queue.Count > 0)
            {
                foreach (var part in queue.Dequeue().Parts)
                {
                    if (!UriPartDictionary.Keys.Contains(part.OpenXmlPart.Uri.ToString()))
                    {
                        UriPartDictionary.Add(part.OpenXmlPart.Uri.ToString(), part.OpenXmlPart);
                        queue.Enqueue(part.OpenXmlPart);
                    }
                }
            }
        }

        private void ChangeMainDocumentPart1(MainDocumentPart mainDocumentPart1)
        {
            Document document1 = mainDocumentPart1.Document;

            Body body1 = document1.GetFirstChild<Body>();

            Paragraph paragraph1 = body1.Elements<Paragraph>().ElementAt(2);
            Paragraph paragraph2 = body1.Elements<Paragraph>().ElementAt(5);
            Paragraph paragraph3 = body1.Elements<Paragraph>().ElementAt(7);
            Paragraph paragraph4 = body1.Elements<Paragraph>().ElementAt(11);

            Run run1 = paragraph1.Elements<Run>().ElementAt(1);
            Run run2 = paragraph1.Elements<Run>().ElementAt(5);

            Text text1 = run1.GetFirstChild<Text>();
            text1.Text = ": TSKGV.63/007-20/";


            Text text2 = run2.GetFirstChild<Text>();
            text2.Text = "23 Temmuz 2020";


            Run run3 = paragraph2.GetFirstChild<Run>();

            Text text3 = run3.GetFirstChild<Text>();
            text3.Text = "Kızılay Mahallesi İzmir 2 Caddesi Ersan Apartmanı No:49/4 ";


            Run run4 = paragraph3.GetFirstChild<Run>();

            Text text4 = run4.GetFirstChild<Text>();
            text4.Text = " Kızılay-Çankaya/Ankara";


            Run run5 = paragraph4.Elements<Run>().ElementAt(2);

            Text text5 = run5.GetFirstChild<Text>();
            text5.Text = "2.603,00 TL ";


            Paragraph paragraph5 = new Paragraph();

            Run run6 = new Run();
            Break break1 = new Break() { Type = BreakValues.Page };

            run6.Append(break1);

            paragraph5.Append(run6);
            body1.Append(paragraph5);

            Paragraph paragraph6 = new Paragraph();

            Run run7 = new Run();
            Break break2 = new Break() { Type = BreakValues.Page };

            run7.Append(break2);

            paragraph6.Append(run7);
            body1.Append(paragraph6);

            Paragraph paragraph7 = new Paragraph();

            Run run8 = new Run();
            Break break3 = new Break() { Type = BreakValues.Page };

            run8.Append(break3);

            paragraph7.Append(run8);
            body1.Append(paragraph7);

            Paragraph paragraph8 = new Paragraph();

            Run run9 = new Run();
            Break break4 = new Break() { Type = BreakValues.Page };

            run9.Append(break4);

            paragraph8.Append(run9);
            body1.Append(paragraph8);

            Paragraph paragraph9 = new Paragraph();

            Run run10 = new Run();
            Break break5 = new Break() { Type = BreakValues.Page };

            run10.Append(break5);

            paragraph9.Append(run10);
            body1.Append(paragraph9);

            Paragraph paragraph10 = new Paragraph();

            Run run11 = new Run();
            Break break6 = new Break() { Type = BreakValues.Page };

            run11.Append(break6);

            paragraph10.Append(run11);
            body1.Append(paragraph10);

            Paragraph paragraph11 = new Paragraph();

            Run run12 = new Run();
            Break break7 = new Break() { Type = BreakValues.Page };

            run12.Append(break7);

            paragraph11.Append(run12);
            body1.Append(paragraph11);

            Paragraph paragraph12 = new Paragraph();

            Run run13 = new Run();
            Break break8 = new Break() { Type = BreakValues.Page };

            run13.Append(break8);

            paragraph12.Append(run13);
            body1.Append(paragraph12);

            Paragraph paragraph13 = new Paragraph();

            Run run14 = new Run();
            Break break9 = new Break() { Type = BreakValues.Page };

            run14.Append(break9);

            paragraph13.Append(run14);
            body1.Append(paragraph13);

            Paragraph paragraph14 = new Paragraph();

            Run run15 = new Run();
            Break break10 = new Break() { Type = BreakValues.Page };

            run15.Append(break10);

            paragraph14.Append(run15);
            body1.Append(paragraph14);

            Paragraph paragraph15 = new Paragraph();

            Run run16 = new Run();
            Break break11 = new Break() { Type = BreakValues.Page };

            run16.Append(break11);

            paragraph15.Append(run16);
            body1.Append(paragraph15);

            Paragraph paragraph16 = new Paragraph();

            Run run17 = new Run();
            Break break12 = new Break() { Type = BreakValues.Page };

            run17.Append(break12);

            paragraph16.Append(run17);
            body1.Append(paragraph16);

            Paragraph paragraph17 = new Paragraph();

            Run run18 = new Run();
            Break break13 = new Break() { Type = BreakValues.Page };

            run18.Append(break13);

            paragraph17.Append(run18);
            body1.Append(paragraph17);

            Paragraph paragraph18 = new Paragraph();

            Run run19 = new Run();
            Break break14 = new Break() { Type = BreakValues.Page };

            run19.Append(break14);

            paragraph18.Append(run19);
            body1.Append(paragraph18);

            Paragraph paragraph19 = new Paragraph();

            Run run20 = new Run();
            Break break15 = new Break() { Type = BreakValues.Page };

            run20.Append(break15);

            paragraph19.Append(run20);
            body1.Append(paragraph19);

            Paragraph paragraph20 = new Paragraph();

            Run run21 = new Run();
            Break break16 = new Break() { Type = BreakValues.Page };

            run21.Append(break16);

            paragraph20.Append(run21);
            body1.Append(paragraph20);

            Paragraph paragraph21 = new Paragraph();

            Run run22 = new Run();
            Break break17 = new Break() { Type = BreakValues.Page };

            run22.Append(break17);

            paragraph21.Append(run22);
            body1.Append(paragraph21);
        }


    }
}