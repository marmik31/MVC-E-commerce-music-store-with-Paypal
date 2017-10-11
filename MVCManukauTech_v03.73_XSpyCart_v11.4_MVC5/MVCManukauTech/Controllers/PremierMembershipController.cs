using Microsoft.AspNet.Identity;
using MVCManukauTech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;



namespace MVCManukauTech.Controllers
{
    [Authorize]
    public class PremierMembershipController : Controller
    {
        
        private XSpy2Entities db = new XSpy2Entities();

        //public XSpy2Entities Db { get => db; set => db = value; }

        // GET: PremierMembership
        public ActionResult Index()
        {
            ViewBag.mess_Prime = "0";

            string id = User.Identity.GetUserId();
            if (id != null)
            {
                string Prime = db.AspNetUsers.Find(id).IsPremierMembership.ToString();
                if (Prime == "True")
                {
                    ViewBag.mess_Prime = "Prime";
                }

            }
            return View();


        }

        
        public ActionResult Purchase()
        {
            //var user = await UserManager.FindByIdAsync(id);
            string Id = User.Identity.GetUserId();
            AspNetUser PurchaseM = new AspNetUser();
            PurchaseM = db.AspNetUsers.Find(Id);          
           
           // PurchaseM.Id = User.Identity.GetUserId();
        

            if(PurchaseM != null)
            {
                PurchaseM.StartDate = DateTime.Now;
                PurchaseM.EndDate = DateTime.Now.AddYears(1);
                //to make end date 1 year after purchase date


                //Db.AspNetUsers.Add(PurchaseM);
                db.Entry(PurchaseM).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    PurchaseM.IsPremierMembership = true;
                    db.Entry(PurchaseM).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.MessageSuccess = "Payment is Done ";                

                }
            }
            return View();
        }
        

    }
}