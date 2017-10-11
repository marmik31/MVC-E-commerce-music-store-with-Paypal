using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using PagedList;
using System.Web.Mvc;
using MVCManukauTech.Models;
using Newtonsoft.Json;

namespace MVCManukauTech.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        
        private XSpy2Entities db = new XSpy2Entities();

        
        // GET: Products      
        public ActionResult Index(string Search)
        {
            //var products = db.Products.Include(p => p.Category);
            //return View(products.ToList());
            string sql = "SELECT * FROM Products";
            List<Product> Product = db.Products.SqlQuery(sql, "").ToList();

            sorting info = new sorting();
            info.SortField = "";
            info.SortDirection = "ascending";
            info.SizeOfThePage = 10;
            info.PageCount = Convert.ToInt32(Math.Ceiling((double)(Product.Count()
                           / info.SizeOfThePage)));
            info.CurrentPageIndex = 0;
            info.NewSearch = "N";
            var query = Product.OrderBy(c => c.ProductId).Take(info.SizeOfThePage);
            ViewBag.SortingPagingInfo = info;
            List<Product> model = query.ToList();
            return View(model);
        }



        [HttpPost]
        public ActionResult Index(sorting info)
        {
            var Search = "";
            Search = "%" + info.SearchText + "%";
            string sql = "SELECT * FROM Products WHERE Name LIKE @p0";
            List<Product> Product = db.Products.SqlQuery(sql, Search).ToList();
            if (info.NewSearch == "Y")
            {
                info.SortField = "";
                info.SortDirection = "ascending";
                info.SizeOfThePage = 10;
                info.PageCount = Convert.ToInt32(Math.Ceiling((double)(Product.Count()
                               / info.SizeOfThePage)));
                info.CurrentPageIndex = 0;
                info.NewSearch = "N";
            }
            var query = Product.OrderBy(c => c.ProductId).Skip(info.CurrentPageIndex * info.SizeOfThePage).Take(info.SizeOfThePage);
            ViewBag.SortingPagingInfo = info;
            List<Product> model = query.ToList();
            return View(model);
        }

        public class sorting
        {
            public string SortField { get; set; }
            public string SortDirection { get; set; }
            public int SizeOfThePage { get; set; }
            public int PageCount { get; set; }
            public int CurrentPageIndex { get; set; }
            public string SearchText { get; set; }
            public string NewSearch { get; set; }
        }

        public string IndexAJAX(string Search)
        {
            Search = "%" + Search + "%";
            string sql = "SELECT Name FROM Products WHERE Name LIKE @p0;";

            List<string> products
             = db.Database.SqlQuery<string>(sql, Search).ToList();

            string json = JsonConvert.SerializeObject(products);
            return json;
        }


        // GET: Products/Details/5

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,CategoryId,Name,ImageFileName,UnitCost,Description,IsDownload,DownloadFileName")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,CategoryId,Name,ImageFileName,UnitCost,Description,IsDownload,DownloadFileName")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        
        // GET: Products/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
