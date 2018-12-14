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
        public void CompareTwoPDF(string FirstPDF, string SecondPDF)
        {
            PdfReader FirstReader = new PdfReader(FirstPDF);
            PdfReader SecondReader = new PdfReader(SecondPDF);

            Dictionary<string, string> file1 = GetFormFieldValues(FirstReader);
            Dictionary<string, string> file2 = GetFormFieldValues(SecondReader);

            Dictionary<string, string> mergeResult = MergeDictionary(file1, file2);
            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfStamper stamp = new PdfStamper(FirstReader, pdfStream);
                foreach (var item in mergeResult)
                {
                    HighLightFields(item.Key, stamp);
                }
                stamp.FormFlattening = false;
                stamp.Close();
                pdfStream.Flush();
                pdfStream.Close();
            }

            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfStamper stamp = new PdfStamper(SecondReader, pdfStream);
                foreach (var item in mergeResult)
                {
                    HighLightFields(item.Key, stamp);
                }
                stamp.FormFlattening = false;
                stamp.Close();
                pdfStream.Flush();
                pdfStream.Close();
            }
            WriteLogToJson(file1, file2, mergeResult);
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
                int pageNum = positions[i].page;
                float left = (float)Math.Round(positions[i].position.Left);
                float right = (float)Math.Round(positions[i].position.Right);
                float top = (float)Math.Round(positions[i].position.Top);
                float bottom = (float)Math.Round(positions[i].position.Bottom);
                PdfContentByte contentByte = stamp.GetOverContent(pageNum);
                contentByte.SetColorFill(BaseColor.ORANGE);
                contentByte.Rectangle(left, top, right - left, bottom - top);
                contentByte.Fill();
            }
        }
        private void WriteLogToJson(Dictionary<string, string> file1, Dictionary<string, string> file2, Dictionary<string, string> merged)
        {
            using (var file = new StreamWriter("Merge.json", false))
            {
                foreach (var item in merged)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
            using (var file = new StreamWriter("FirstPDF.json", false))
            {
                foreach (var item in file1)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
            using (var file = new StreamWriter("SecondPDF.json", false))
            {
                foreach (var item in file2)
                {
                    file.WriteLine("\r\n" + " " + item.Value);
                }
            }
        }
    }
}