namespace Comparator.Data
{
    using System.Data.Entity;
    using Comparator.Models;
    public class PDFContext : DbContext
    {
        public PDFContext() : base("DefaultConnection"){ }
        public DbSet<PDFfile> Files { get; set; }        
    }
}