namespace Comparator.Controllers
{
    using Comparator.Data;
    using Comparator.Models;
    using System.Net;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Text;

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
        public ActionResult Create(Uri upload)
        {
            if (upload != null)
            {
                WebClient client = new WebClient
                {
                    Credentials = new NetworkCredential("call", "111")      
                };

                PDFfile file = new PDFfile()
                {
                    FileName = Path.GetFileName(upload.ToString()),
                    Content = client.DownloadData(upload),
                    PDFID = upload.LocalPath.Length,
                    CreateTime = DateTime.Now,
                };                         
                db.Files.Add(file);
                db.SaveChanges();
                return View(file);
            }
            db.SaveChanges();
            return View();
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
            if (fileid != null && fileid.Count() <= 2)
            {
                foreach (var id in fileid)
                {
                    var file = db.Files.Single(p => p.PDFID == id);
                    files.Add(file);
                }
                TempData["list"] = files;
                return RedirectToAction("FilePartial");
            }
            return RedirectToAction("Show");
        }
        [HttpGet]
        public ActionResult FilePartial()
        {
            var filesToShow = TempData["list"] as List<PDFfile>;
            TempData.Keep();
            return View(filesToShow);
        }
        [HttpPost]
        public ActionResult FilePartial(IEnumerable<int> fileid)
        {
            var filesToShow = TempData["list"] as List<PDFfile>;
            Comparer c = new Comparer();
            if(filesToShow.Count() == 2)
            {
                c.CompareTwoPDF(filesToShow[0].PDFID, filesToShow[1].PDFID);
            }
            
            TempData.Keep();
            return View(filesToShow);
        }
    }
}