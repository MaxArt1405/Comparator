namespace Comparator.Controllers
{
    using Comparator.Data;
    using Comparator.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    public class PDFController : Controller
    {
        private PDFContext db = new PDFContext();
        private List<PDFfile> files = new List<PDFfile>();
        [HttpGet]
        public ActionResult Index()
        {
            return View(db.Files.ToList());
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase upload)
        {
            String FileExt = Path.GetExtension(upload.FileName).ToUpper();

            Stream str = upload.InputStream;
            BinaryReader Breader = new BinaryReader(str);
            Byte[] Context = Breader.ReadBytes((Int32)str.Length);
            PDFfile file = new PDFfile
            {
                FileName = upload.FileName,
                Content = Context,
                PDFID = upload.ContentLength
            };
            db.Files.Add(file);
            db.SaveChanges();

            return View(file);
        }
        [HttpGet]
        public ActionResult Delete()
        {
            return View(db.Files.ToList());
        }
        [HttpPost]
        public ActionResult Delete(IEnumerable<int> PDFID)
        {
            if (PDFID != null)
            {
                foreach (var id in PDFID)
                {
                    var file = db.Files.Single(p => p.PDFID == id);
                    db.Files.Remove(file);
                }

                db.SaveChanges();
                return RedirectToAction("Delete");
            }
            return RedirectToAction("Delete");
        }
        [HttpGet]
        public ActionResult Show()
        {
            return View(db.Files.ToList());
        }
        [HttpPost]
        public ActionResult Show(IEnumerable<int> fileid)
        {
            Comparer c = new Comparer();
            if (fileid != null && fileid.Count() <= 2)
            {
                foreach (var id in fileid)
                {
                    var file = db.Files.Single(p => p.PDFID == id);
                    files.Add(file);
                }
                //c.CompareTwoPDF(files[0].PDFID, files[1].PDFID);
                return View(files);
            }
            return RedirectToAction("Show");
        }
    }
}