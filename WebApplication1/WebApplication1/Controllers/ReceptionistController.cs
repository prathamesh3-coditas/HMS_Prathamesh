using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class ReceptionistController : Controller
    {

        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        // GET: Receptionist
        public ActionResult Index()
        {
            return View();
        }


        
        
        [HttpPost]
        public ActionResult ShowAppointments(bool? isApproved,string patientId)
        {
            //Checking if Receptionist selects Approve OR Reject
            //When Receptionist clicks on Show all appointments then isApproved==null
            //so it will get all appointments waiting for confirmation
            //By default when an appointment is created,We've put isApproved==false
            if (isApproved != null)
            {
                //getting details of appointment based on patient id
                var appointmentCreated = entities.Appointments.Where(a => a.patient_id == patientId).FirstOrDefault();
                if (isApproved==true)
                {
                    //If approved by receptionist then set value for column as true
                    appointmentCreated.isApproved = true;
                    entities.SaveChanges();
                }
                else
                {
                    //If appointment is rejected then it'll be removed from table
                    appointmentCreated.isApproved = false;
                    var res = entities.Appointments.Remove(appointmentCreated);
                    entities.SaveChanges();
                }
            }

            //Here we're taking appointments having isApproved==false but when an appointment is
            //approved then isApproved will be set to true so this will by default selects 
            //appointments which are yet to be responded by receptionist
            var appointData = entities.Appointments.Where(a=>a.isApproved==false).ToList();

            return View(appointData);
        }
    }
}