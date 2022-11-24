using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace WebApplication1.Controllers
{
    public class PatientController : Controller
    {

        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult TakeAppointment(DateTime? date, string time)
        {
            TempData["AppointmentDate"] = date;
            TempData["AppointmentTime"] = time;
            return RedirectToAction("PatientDetails");
        }

        public ActionResult PatientDetails()
        {
            //If method is accessed with pasting URL then Session will be null,in that case 
            //we'll be redirected to LoginPage
            var v = ((User)Session["User"]);
            if (v==null)
            {
                return RedirectToAction("LoginPage", "Home");
            }

            //Get all patient data
            var patientData = entities.Patients.ToList();
            //Check if the same userId already present in patient table
            var isExisting = patientData.Where(a => a.userId == v.userId).FirstOrDefault();


            if (isExisting == null)
            {
                //if same userId not present then return create view to enter details
                return View();
            }
            else
            {
                //If userId alredy exists in patient table then show the view having existing data
                var patient = entities.Patients.Where(a => a.userId == v.userId).FirstOrDefault();

                var history = patient.prev_history;
                var repo = patient.reports;

                return View(patient);
            }
        }

        [HttpPost]
        public ActionResult PatientDetails(Patient details)
        {

            var loggedUser = (User)Session["User"];

            //create unique patient id by combining userId and mobileNumber
            details.patient_id = ((loggedUser).userId).ToString()
                + "_"+(loggedUser).contact_number.ToString();

            //set userId same as userId in User table
            details.userId = (loggedUser).userId;

            //Get the data of existing patient from patient table
            var data = entities.Patients.Where(a=>a.patient_id==details.patient_id).FirstOrDefault();

            if (data!=null)
            {
                //New data will be concatinated with old data 
                data.prev_history = details.prev_history;
                data.reports = details.reports;
            }
            else
            {
                //If patient is new then entire data will be added
                entities.Patients.Add(details);
            }

            {

                //Created an appointment request in appointment 
                TimeSpan time = TimeSpan.Parse(TempData["AppointmentTime"].ToString());
                var appointmentRequest = new Appointment()
                {
                    patient_id = details.patient_id,
                    userId = loggedUser.userId,
                    appointmentDate = (DateTime)TempData["AppointmentDate"],
                    appointmentTime = time,
                    isApproved = false
                };
                var appoint = entities.Appointments.Add(appointmentRequest);
            }

            entities.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}