using Comparator.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Comparator.Models
{
    public class Comparer
    {
        private PDFContext db = new PDFContext();
        public void CompareTwoPDF(int FirstPDF, int SecondPDF)
        {
            PdfReader reader1 = new PdfReader(db.Files.Single(p => p.PDFID == FirstPDF).Content);
            PdfReader reader = new PdfReader(db.Files.Single(p => p.PDFID == SecondPDF).Content);

            Dictionary<string, string> file1 = GetFormFieldValues(reader);
            Dictionary<string, string> file2 = GetFormFieldValues(reader1);

            Dictionary<string, string> mergeResult = MergeDictionary(file1, file2);

            HighLighter(reader, mergeResult);
            HighLighter(reader1, mergeResult);

        }
        private void HighLighter(PdfReader reader, Dictionary<string, string> merge)
        {
            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfStamper stamp = new PdfStamper(reader, pdfStream);
                foreach (var item in merge)
                {
                    HighLightFields(item.Key, stamp);
                }
                stamp.FormFlattening = true;
                stamp.Close();
                pdfStream.Flush();
                pdfStream.Close();
            }
        }
        private Dictionary<string, string> GetFormFieldValues(PdfReader pdfReader)
        {
            AcroFields form = pdfReader.AcroFields;
            var dict = new Dictionary<string, string>();
            foreach (var item in form.Fields)
            {
                dict.Add(item.Key, form.GetField(item.Key));
            }
            return dict;
        }
        private Dictionary<string, string> MergeDictionary(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            var dict3 = new Dictionary<string, string>();

            foreach (var item in dict2)
            {
                if (!dict1.ContainsKey(item.Key))
                {
                    if (!dict3.ContainsKey(item.Key))
                    {
                        dict3.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    dict1.TryGetValue(item.Key, out string value);
                    if (!item.Value.Equals(value))
                    {
                        if (!dict3.ContainsKey(item.Key))
                        {
                            dict3.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            return dict3;
        }
        private void HighLightFields(string field, PdfStamper stamp)
        {
            var fieldPositions = stamp.AcroFields.GetFieldPositions(field);
            if (fieldPositions == null)
                return;

            var positions = fieldPositions.ToArray();

            for (int i = 0; i < positions.Length; i++)
            {

                PdfContentByte contentByte = stamp.GetOverContent(positions[i].page);
                contentByte.SetRGBColorFill(100, 100, 100);
                contentByte.Fill();
            }
        }
    }
}