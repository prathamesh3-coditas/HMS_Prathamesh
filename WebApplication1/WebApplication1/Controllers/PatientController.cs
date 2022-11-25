using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using WebApplication1.Data_Access;

namespace WebApplication1.Controllers
{
    public class PatientController : Controller
    {

        //HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        PatientDataAccess patientAccess = new PatientDataAccess();
        AppointmentDataAccess appointmentAccess = new AppointmentDataAccess();
        DeseaseDataAccess deseaseAccess = new DeseaseDataAccess();
        UserDataAccess userAccess = new UserDataAccess();

        // GET: Patient
        public ActionResult Index()
        {
            TempData.Keep("name");
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
            if (v == null)
            {
                return RedirectToAction("LoginPage", "Home");
            }

            //Get all patient data
            //var patientData = entities.Patients.ToList();

            var patientData = patientAccess.GetAllPatients();
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
                //var patient = entities.Patients.Where(a => a.userId == v.userId).FirstOrDefault();

                var patient = patientData.Where(a => a.userId == v.userId).FirstOrDefault();
                return View(patient);
            }
        }

        [HttpPost]
        public ActionResult PatientDetails(Patient details)
        {

            var loggedUser = (User)Session["User"];

            //create unique patient id by combining userId and mobileNumber
            details.patient_id = ((loggedUser).userId).ToString()
                + "_" + (loggedUser).contact_number.ToString();

            //set userId same as userId in User table
            details.userId = (loggedUser).userId;

            //Get the data of existing patient from patient table
            var data = patientAccess.GetAllPatients()
                .Where(a => a.patient_id == details.patient_id)
                .FirstOrDefault();
            if (data != null)
            {
                //New data will be concatinated with old data 
                data.prev_history = details.prev_history;
                data.reports = details.reports;
            }
            else
            {

                ///-------------------------------------------------------- 
                /// Filling data in Patients table
                ///-------------------------------------------------------- 

                //If patient is new then entire data will be added
                patientAccess.CreatePatient(details);
            }


            ///-------------------------------------------------------- 
            /// Filling data in Appointment table
            ///-------------------------------------------------------- 

            {
                var foundInDeseaseDetail = deseaseAccess.GetDeseaseDetails(details.patient_id);

                //Created an appointment request in appointment 
                TimeSpan time = TimeSpan.Parse(TempData["AppointmentTime"].ToString());
                var appointmentRequest = new Appointment()
                {
                    patient_id = details.patient_id,
                    appointmentDate = (DateTime)TempData["AppointmentDate"],
                    appointmentTime = time,
                    isApproved = false
                };

                if (foundInDeseaseDetail == null)
                {
                    //if desease details of patient are not found means patient is coming first time
                    //so by default redirect it to general physician so add userId of general physician
                    //in Appointment table
                    var doctorId = userAccess.GetAllUsers().Where(a => a.specialization == "General")
                        .Select(a => a.userId)
                        .First();

                    appointmentRequest.userId = doctorId;

                }
                else
                {
                    //get details from desease details table
                    var deseaseDetailOfPatient = deseaseAccess.GetDeseaseDetails(details.patient_id);

                    //get desease category
                    var category = deseaseDetailOfPatient.desease_catagory;

                    //Assign Doctor Id according to desease category
                    var doctorId = userAccess.GetAllUsers().Where(a => a.specialization == $"{category}")
                        .Select(a => a.userId)
                        .First();

                    appointmentRequest.userId = doctorId;

                }

                var appoint = appointmentAccess.CreateAppointment(appointmentRequest);

            }



            return RedirectToAction("Index");
        }


        public ActionResult viewAppointments()
        {
            var user = ((User)Session["User"]);
            var list = appointmentAccess.GetUpcomingAppointments(user);
            return View(list);

        }
    }
}