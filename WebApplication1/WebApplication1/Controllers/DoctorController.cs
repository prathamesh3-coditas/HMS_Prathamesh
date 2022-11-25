using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            TempData.Keep("name");
            return View();
        }

        public ActionResult viewAllAppointment()
        {
            return View();
        }
    }
}