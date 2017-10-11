using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCManukauTech.Models;

namespace MVCManukauTech.Controllers
{
    public class CatalogController : Controller
    {
        private XSpy2Entities db = new XSpy2Entities();

        // GET: Catalog?CategoryName=Travel
        public ActionResult Index()
        {
            //140903 JPC add CategoryName to SELECT list of fields
            String SQL = "SELECT ProductId, Products.CategoryId AS CategoryId, Name, ImageFileName, UnitCost"
                + ", SUBSTRING(Description, 1, 100) + '...' AS Description, IsDownload, DownloadFileName "
                + "FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId ";
            String CategoryName = Request.QueryString.Get("CategoryName");

            if (CategoryName != null)
            {
                //140903 JPC security check - if ProductId is dodgy then return bad request and log the fact 
                //  of a possible hacker attack.  Excessive length or containing possible control characters
                //  are cause for concern!  TODO move this into a separate reusable code method with more sophistication.
                if (CategoryName.Length > 20 || CategoryName.IndexOf("'") > -1 || CategoryName.IndexOf("#") > -1)
                {
                    //TODO Code to log this event and send alert email to admin
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //140903 JPC  Passed the above test so extend SQL
                //150807 JPC Security improvement @p0
                SQL += " WHERE CategoryName = @p0";
                //SQL += " WHERE CategoryName = '{0}'";
                //SQL = String.Format(SQL, CategoryName);
                //Send extra info to the view that this is the selected CategoryName
                ViewBag.CategoryName = CategoryName;
            }
            //150807 JPC Security improvement implementation of @p0
            var products = db.Products.SqlQuery(SQL, CategoryName);
            return View(products.ToList());
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

        // GET: Catalog/Details?ProductId=1MORE4ME
        public ActionResult Details(string ProductId)
        {
            if (ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //140903 JPC security check - if ProductId is dodgy then return bad request and log the fact 
            //  of a possible hacker attack.  Excessive length or containing possible control characters
            //  are cause for concern!  TODO move this into a separate reusable code method with more sophistication.
            if (ProductId.Length > 20 || ProductId.IndexOf("'") > -1|| ProductId.IndexOf("#") > -1)
            {
                //TODO Code to log this event and send alert email to admin
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //150807 JPC Security improvement implementation of @p0
            String SQL = "SELECT * FROM Products WHERE ProductId = @p0";
            
            //140904 JPC case of one product to look at the details.
            //  SQL gives some kind of collection where we need to clean that up with ToList() then take element [0]
            //150807 JPC Security improvement implementation of @p0 substitute ProductId
            var product = db.Products.SqlQuery(SQL, ProductId).ToList()[0];
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
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
