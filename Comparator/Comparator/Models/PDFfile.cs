using System;
using System.ComponentModel.DataAnnotations;

namespace Comparator.Models
{
    public class PDFfile
    {
        [Key]
        public int PDFID { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}