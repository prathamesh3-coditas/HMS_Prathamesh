using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace WebApplication1.Data_Access
{
    public class AppointmentDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        public Appointment CreateAppointment(Appointment app)
        {
            entities.Appointments.Add(app);
            entities.SaveChanges();

            return app;
        }



        public /*Appointment*/ bool ApproveOrReject(Appointment AppointmentPending, bool approveOrReject)
        {

            if (approveOrReject == true)
            {

                //If approved by receptionist then set value for column as true
                var appointmentFound = entities.Appointments.Find(AppointmentPending.appointment_id);

                AppointmentPending.isApproved = true;

                appointmentFound.appointment_id = AppointmentPending.appointment_id;
                appointmentFound.AdmitDate = AppointmentPending.AdmitDate;
                appointmentFound.patient_id = AppointmentPending.patient_id;
                appointmentFound.treatment_id = AppointmentPending.treatment_id;
                appointmentFound.userId = AppointmentPending.userId;
                appointmentFound.appointmentDate = AppointmentPending.appointmentDate;
                appointmentFound.appointmentTime = AppointmentPending.appointmentTime;
                appointmentFound.isApproved = AppointmentPending.isApproved;


                var res = entities.SaveChanges();
                //return null;

                return true;
            }
            else
            {
                var deletedRequest = entities.Appointments.Remove(AppointmentPending);
                entities.SaveChanges();
                //return deletedRequest;
                return false;
            }
        }

        public IEnumerable<Appointment> GetPendingAppointments()
        {
            var appointData = entities.Appointments.Where(a => a.isApproved == false).ToList();
            return appointData;
        }

        public IEnumerable<Appointment> GetUpcomingAppointments(User user)
        {
            if (user!=null)
            {
                try
                {
                    var patientId = entities.Patients.Where(a => a.userId == user.userId).Select(a => a.patient_id).First();
                    var patientRecord = entities.Patients.Where(a => a.patient_id == patientId).First();
                    DateTime d = DateTime.Now.Date;
                    //var d = DateTime.Now.Date.ToString("dd:MM:yyyy");

                    var appointData = entities.Appointments.Where(a => a.patient_id == patientRecord.patient_id && a.appointmentDate >= d).ToList();
                    return appointData;
                }
                catch (Exception)
                {

                    return new List<Appointment>();
                }
            }

            return new List<Appointment>();
        }


        public IEnumerable<Appointment> GetAllAppointments()
        {
            try
            {
                var allAppointments = entities.Appointments.ToList();
                return allAppointments;
            }
            catch (Exception)
            {

                return null;
            }
        }


        //public IEnumerable<Appointment> GetAllAppointments(int? userId)
        public IEnumerable<sp_GetAllAppointments_Result> GetAllAppointments(int? userId)
        {
            try
            {
                //var allAppointments = entities.Appointments.ToList();
                var allAppointments = entities.sp_GetAllAppointments(userId);
                var appoint = allAppointments.Where(a => a.userId == userId).ToList();
                return appoint;

            }
            catch (Exception)
            {

                return null;
            }
        }

    }
}