using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Data_Access;

namespace WebApplication1.Controllers
{
    public class DoctorController : Controller
    {
        AppointmentDataAccess appointmentAccess = new AppointmentDataAccess();
        ConsumableDataAccess consumableAccess = new ConsumableDataAccess();
        TreatmentDataAccess treatmentAccess = new TreatmentDataAccess();
        DeseaseDataAccess deseaseAccess = new DeseaseDataAccess();
        BillingDataAccess billingAccess = new BillingDataAccess();
        PatientDataAccess patientAccess = new PatientDataAccess();
        // GET: Doctor

        [Authorize(Roles = "Doctor")]
        public ActionResult Index()
        {
            TempData.Keep("name");
            return View();
        }


        [Authorize(Roles = "Doctor")]
        public ActionResult viewAppointment(DateTime? date)

        {
            var drDetails = (User)Session["User"];


            var allAppointments = appointmentAccess.GetAllAppointments(drDetails.userId);

            if (date != null)
            {
                var myAppointments = allAppointments.Where(a => a.userId == drDetails.userId && a.isApproved == true && a.appointmentDate == date);
                return View(myAppointments);
            }
            else
            {
                var myAppointments = allAppointments.Where(a => a.userId == drDetails.userId && a.isApproved == true);
                return View(myAppointments);
            }
        }

        

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public ActionResult PrescribeMedicine(string patId, Treatment treatment, int? consumableID, string Finish)
        {

            if (consumableID != null && treatment.quantity!=0)
            {
                
                Treatment addDetails = new Treatment();
                addDetails.patient_id = (string)Session["PatientForTreatment"];
                TempData.Keep();
                addDetails.consumable_id = consumableID;
                addDetails.IsActive = true;
                addDetails.quantity = treatment.quantity;

                treatmentAccess.AddDetails(addDetails);
                var allConsumables = consumableAccess.GetAllConsumables();

                return View(allConsumables);
            }
            else
            {
                if (Finish == "Finish")
                {
                    //When Doctor Finish then desease category will be changed by doctor
                    return RedirectToAction("DeseaseCategory");
                }
                var allConsumables = consumableAccess.GetAllConsumables();
                Session["PatientForTreatment"] = patId;

                return View(allConsumables);
            }
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult DeseaseCategory()
        {
            var patientId = (string)Session["PatientForTreatment"];
           var patientDetails=  patientAccess.GetReports(patientId);
            return View(patientDetails);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public ActionResult DeseaseCategory(string category)
        {
            var patientId = (string)Session["PatientForTreatment"];
            Desease_Details details = new Desease_Details();
            
            details.patient_id = patientId;
            details.desease_catagory = category;

            var deseaseObj = deseaseAccess.GetDeseaseDetails(patientId);
            details.no_of_visits = ++deseaseObj.no_of_visits;

            deseaseAccess.EditDeseaseCategory(details);

            //=========================================

            return View();
        }


        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public ActionResult Billing()
        {
            var patientId = (string)Session["PatientForTreatment"];
            var user = (User)Session["User"];
            var DrId = user.userId;
            var activeTreatments = treatmentAccess.GetTreatmentDetails(patientId).Where(a=>a.IsActive==true);

            billingAccess.GenerateBill(activeTreatments,patientId,DrId);
            //var data = billingAccess.BillingDetails(activeTreatments);
            //return View(data);

            return RedirectToAction("DeseaseCategory");
        }



        //public ActionResult GenerateReport()
        //{
        //    string patientId = (string)Session["PatientForTreatment"];
        //    var patientData = 
        //    patientAccess.GetReports(patientId);
        //    return View();
        //}

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public ActionResult GenerateReport(string report)
        {
            string patientId = (string)Session["PatientForTreatment"];
            patientAccess.EditReports(report,patientId);

            Billing();
            return RedirectToAction("viewAppointment", "Doctor");
            
        }


        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public ActionResult SearchMedicine(string name)
        {
            name = name.ToLower();
            if (name==String.Empty)
            {
                var consumableDetails = consumableAccess.GetAllConsumables();
                return View("PrescribeMedicine",consumableDetails);
            }
            else
            {
                var consumableDetails = consumableAccess.GetAllConsumables().Where(a => a.consumable_name.ToLower().Contains(name) || name.Contains(a.consumable_name.ToLower()));
                return View("PrescribeMedicine", consumableDetails);

            }
        }
    }
}