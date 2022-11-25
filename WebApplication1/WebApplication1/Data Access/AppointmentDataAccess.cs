using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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



        public bool ApproveOrReject(Appointment AppointmentPending, bool approveOrReject)
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
                return true;
            }
            else
            {
                entities.Appointments.Remove(AppointmentPending);
                entities.SaveChanges();
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
                var patientRecord = entities.Patients.Where(a => a.userId == user.userId).First();
                DateTime d = DateTime.Now.Date;

                var appointData = entities.Appointments.Where(a => a.patient_id == patientRecord.patient_id && a.appointmentDate>=d).ToList();
                return appointData;
            }

            return new List<Appointment>();
        }


        public IEnumerable<Appointment> GetAllAppointments()
        {
            var allAppointments = entities.Appointments.ToList();
            return allAppointments;
        }

    }
}