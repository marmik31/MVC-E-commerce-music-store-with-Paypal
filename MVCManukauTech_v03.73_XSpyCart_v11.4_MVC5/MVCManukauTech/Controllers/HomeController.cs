using MVCManukauTech.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MVCManukauTech.Controllers
{
    public class HomeController : Controller
    {
        private XSpy2Entities db = new XSpy2Entities();
        
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId() ;
            if(id != null)
            {
                string Prime = db.AspNetUsers.Find(id).IsPremierMembership.ToString();
                ViewBag.mess_Prime = Prime;
            }
          
            // Here you can add few more fields to show the things that can be changed on the home page
            ViewBag.mesage1 = db.Tables.Find(1).html;
            //ViewBag.mesage3 = db.Tables.Find(3).html;
            //ViewBag.mesage4 = db.Tables.Find(4).html;
            //ViewBag.mesage5 = db.Tables.Find(5).html;




            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message1 = db.Tables.Find(1).html;
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
