namespace Comparator.Controllers
{
    using Comparator.Data;
    using Comparator.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    public class PDFController : Controller
    {
        private PDFContext db = new PDFContext();
        // GET: PDF
        public ActionResult Index()
        {
            return View(db.Files.ToList());
        }
        // GET: PDF/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: PDF/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase upload)
        {

            String FileExt = Path.GetExtension(upload.FileName).ToUpper();
            if(FileExt == ".PDF")
            {
                Stream str = upload.InputStream;
                BinaryReader Breader = new BinaryReader(str);
                Byte[] Context = Breader.ReadBytes((Int32)str.Length);
                PDFfile file = new PDFfile
                {
                    FileName = upload.FileName,
                    Content = Context
                };
                db.Files.Add(file);
            }
            else
            {
                return View();
            }
            db.SaveChanges();

            return RedirectToAction("Create");
        }
        public ActionResult Delete()
        {
            return View(db.Files.ToList());
        }
        [HttpPost]
        public ActionResult Delete(IEnumerable<int> PDFID)
        {
            foreach (var id in PDFID)
            {
                var file = db.Files.Single(p => p.PDFID == id);
                db.Files.Remove(file);
            }

            db.SaveChanges();
            return RedirectToAction("Delete");
        }
    }
}