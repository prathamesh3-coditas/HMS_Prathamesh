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

        PatientDataAccess patientAccess = new PatientDataAccess();
        AppointmentDataAccess appointmentAccess = new AppointmentDataAccess();
        DeseaseDataAccess deseaseAccess = new DeseaseDataAccess();
        UserDataAccess userAccess = new UserDataAccess();
        TreatmentDataAccess treatmentAccess = new TreatmentDataAccess();
        ConsumableDataAccess consumableAccess = new ConsumableDataAccess();
        BillingDataAccess billingAccess = new BillingDataAccess();
        PaymentDataAccess paymentAccess = new PaymentDataAccess();


        [Authorize(Roles = "Patient")]
        public ActionResult Index()
        {
            TempData.Keep("name");

            var allAppointments = appointmentAccess.GetAllAppointments();
            return View(allAppointments);
        }


        
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public ActionResult TakeAppointment(DateTime? date, string time)
        {
            try
            {
                if (date != null)
                {
                    Session["AppointmentDate"] = date;
                }
                else
                {
                    if (/*time!="null" && */time != null)
                    {
                        Session["AppointmentTime"] = time;
                    }
                    else
                    {
                        ViewBag.AppointmentDateRequired = "Please Select Appointment Date";
                        return View("Index");
                    }

                }


                if (Session["AppointmentDate"] != null && Session["AppointmentTime"] != null)
                {
                    return RedirectToAction("PatientDetails");
                }
                var allAppointments = appointmentAccess.GetAllAppointments();

                return View("Index", allAppointments);

            }
            catch (Exception)
            {
                return View("PatientError");
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult PatientDetails()
        {
            try
            {
                //If method is accessed with pasting URL then Session will be null,in that case 
                //we'll be redirected to LoginPage
                var user = ((User)Session["User"]);
                if (user == null)
                {
                    return RedirectToAction("LoginPage", "Home");
                }

                //Get all patient data
                var patientData = patientAccess.GetAllPatients();
                //Check if the same userId already present in patient table
                var isExisting = patientData.Where(a => a.userId == user.userId).FirstOrDefault();


                if (isExisting == null)
                {
                    //if same userId not present then return create view to enter details
                    return View();
                }
                else
                {
                    //If userId alredy exists in patient table then show the view having existing data
                    //var patient = entities.Patients.Where(a => a.userId == v.userId).FirstOrDefault();

                    var patient = patientData.Where(a => a.userId == user.userId).FirstOrDefault();
                    return View(patient);
                }
            }
            catch (Exception)
            {
                return View("PatientError");
            }
        }

        
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public ActionResult PatientDetails(Patient details, string deseaseCategory)
        {

            try
            {
                var loggedUser = (User)Session["User"];

                //create unique patient id by combining userId and mobileNumber
                details.patient_id = ((loggedUser).userId).ToString()
                    + "_" + (loggedUser).contact_number.ToString();

                //set userId same as userId in User table
                //var userId = userAccess.GetAllUsers().Where(a => a.userId == loggedUser.userId).Select(a => a.userId).FirstOrDefault();
                details.userId = loggedUser.userId;

                //Get the data of existing patient from patient table
                var existingDataOfPatient = patientAccess.GetAllPatients()
                    .Where(a => a.patient_id == details.patient_id)
                    .FirstOrDefault();

                Console.WriteLine();
                if (ModelState.IsValid)
                {
                    if (existingDataOfPatient != null)
                    {
                        //New data will be concatinated with old data 
                        existingDataOfPatient.userId = details.userId;
                        existingDataOfPatient.prev_history = details.prev_history;
                        existingDataOfPatient.reports = details.reports;

                        patientAccess.EditPatientDetails(details.patient_id, existingDataOfPatient);
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
                        TimeSpan time = TimeSpan.Parse(Session["AppointmentTime"].ToString());
                        var appointmentRequest = new Appointment()
                        {
                            patient_id = details.patient_id,
                            appointmentDate = (DateTime)Session["AppointmentDate"],
                            appointmentTime = time,
                            isApproved = false
                        };

                        if (foundInDeseaseDetail == null)
                        {
                            //if desease details of patient are not found means patient is coming first time
                            //so by default redirect it to general physician so add userId of general physician
                            //in Appointment table
                            if (deseaseCategory == "")
                            {
                                //desease category="" means patient doesn't know desease category so redirect him to General
                                var doctorId = userAccess.GetAllUsers().Where(a => a.specialization == "General")
                                .Select(a => a.userId)
                                .First();

                                appointmentRequest.userId = doctorId;
                            }
                            else
                            {
                                //else if desease category is provided by user then create appointment for doctor of particular specialization
                                var doctorId = userAccess.GetAllUsers().Where(a => a.specialization == deseaseCategory)
                                    .Select(a => a.userId)
                                    .First();
                                appointmentRequest.userId = doctorId;
                            }

                        }
                        else
                        {
                            //get details from desease details table
                            var deseaseDetailOfPatient = deseaseAccess.GetDeseaseDetails(details.patient_id);
                            string category = deseaseDetailOfPatient.desease_catagory;


                            //Assign Doctor Id according to desease category
                            var doctorId = userAccess.GetAllUsers().Where(a => a.specialization == $"{category}")
                                .Select(a => a.userId)
                                .First();

                            appointmentRequest.userId = doctorId;

                            //Deactivating the previous treatment of patient when patient comes 2nd time
                            var treatmentDetails = treatmentAccess.GetTreatmentDetails(details.patient_id);
                            treatmentAccess.EditDetails(treatmentDetails);

                        }

                        var appoint = appointmentAccess.CreateAppointment(appointmentRequest);
                        //Current Appointment Details will be removed from session
                        Session.Remove("AppointmentDate");
                        Session.Remove("AppointmentTime");
                    }
                }
                else
                {
                    return View();
                }



                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View("PatientError");
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult viewAppointments()
        {
            try
            {
                var user = ((User)Session["User"]);
                var list = appointmentAccess.GetUpcomingAppointments(user);
                return View(list);
            }
            catch (Exception)
            {
                return View("PatientError");
            }

        }

        [Authorize(Roles = "Patient")]
        public ActionResult CurrentTreatment()
        {
            try
            {
                var user = (User)Session["User"];
                var allPatients = patientAccess.GetAllPatients();
                Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>> tuple = null;
                try
                {
                    var patDetails = allPatients.Where(a => a.userId == user.userId).First();
                    var treatmemtTable = treatmentAccess.GetTreatmentDetails(patDetails.patient_id).Where(a => a.IsActive == true);
                    var consumableTable = consumableAccess.GetAllConsumables();

                    tuple = new Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>>(treatmemtTable, consumableTable);
                }
                catch (Exception)
                {

                    return View();
                }


                return View(tuple);
            }
            catch (Exception)
            {
                return View("PatientError");
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult ShowBilling()
        {
            try
            {
                var user = (User)Session["User"];
                var patient = patientAccess.GetAllPatients().Where(a => a.userId == user.userId).First();
                var bills = billingAccess.ShowBillsByPatientId(patient.patient_id);
                var consumables = consumableAccess.GetAllConsumables();

                Tuple<IEnumerable<Billing>, IEnumerable<Consumable>> tuple =
                    new Tuple<IEnumerable<Billing>, IEnumerable<Consumable>>(bills, consumables);
                return View(tuple);
            }
            catch (Exception)
            {
                return View("PatientError");
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult GetReports()
        {
            try
            {
                var user = (User)Session["User"];
                var reportData = patientAccess.GetReports(user.userId);
                return View(reportData);
            }
            catch (Exception)
            {

                return View();
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult PayBill()
        {
            try
            {
                var user = (User)Session["User"];
                var allPatients = patientAccess.GetAllPatients();
                Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>> tuple = null;
                try
                {
                    var patDetails = allPatients.Where(a => a.userId == user.userId).First();
                    var treatmemtTable = treatmentAccess.GetTreatmentDetails(patDetails.patient_id).Where(a => a.IsActive == true);
                    var consumableTable = consumableAccess.GetAllConsumables();

                    tuple = new Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>>(treatmemtTable, consumableTable);
                }
                catch (Exception)
                {

                    return View();
                }


                return View(tuple);
            }
            catch (Exception)
            {

                return View("PatientError");
            }
        }

        
        [HttpPost] //We are making Payment details entry in DB but yet to be approved by receptionist
        [Authorize(Roles = "Patient")]
        public ActionResult PayBill(string patientId, int? amount)
        {

            if (Session["isPaid"]==null)
            {
                Session["isPaid"] = true;
            }
            //var totalAmount = billingAccess.GetTotalBillByPatient(patientId);
            Payment payment = new Payment()
            {
                patientId = patientId,
                total_bill = amount,
                isRequested = false
            };

            try
            {
                var res = paymentAccess.GetPaymentDetails().Where(a => a.patientId == patientId).ToList();

                if (res.Count() != 0)
                {
                    if ((bool)Session["isPaid"] == true)
                    {
                        paymentAccess.AddPaymentDetails(payment);
                        Session["isPaid"] = false;
                        return View("Successful");
                    }
                    else if ((bool)Session["isPaid"] == false)
                    {
                        return View("AlreadyResponded");
                    }

                }
                else
                {
                    paymentAccess.AddPaymentDetails(payment);
                    Session["isPaid"] = false;
                    return View("Successful");
                }
            }
            catch (Exception)
            {

                paymentAccess.AddPaymentDetails(payment);

                return View("Successful");
            }

            return View("AlreadyResponded");
        }

    }
}