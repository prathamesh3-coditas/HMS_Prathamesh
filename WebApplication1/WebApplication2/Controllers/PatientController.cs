using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class PatientController : Controller
    {

        HMS_Project_newEntities entities = new HMS_Project_newEntities();
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult TakeAppointment(DateTime? date,string time)
        {
            return RedirectToAction("DeseaseDetails");
        }

        public ActionResult DeseaseDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeseaseDetails(Desease_Details details)
        {
            entities.Desease_Details.Add(details);
            entities.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}