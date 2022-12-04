using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Web;

namespace WebApplication1.Data_Access
{
    public class PatientDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        public IEnumerable<Patient> GetAllPatients()
        {
            try
            {
                var data = entities.Patients.ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Patient CreatePatient(Patient patient)
        {
            entities.Patients.Add(patient);
            entities.SaveChanges();

            return patient;
        }

        public void EditReports(string newReport,string patientId)
        {
            var reportData = entities.Patients.Where(a => a.patient_id == patientId).First();

            reportData.reports = newReport;
            reportData.patient_id = patientId;

            var res = entities.SaveChanges();
            Console.WriteLine();
        }


        public Patient GetReports(string patientId)
        {
            try
            {
                var patientdetails = entities.Patients.Where(a => a.patient_id == patientId).First();
                return patientdetails;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public Patient GetReports(int userId)
        {
            try
            {
                string patientId = entities.Patients.Where(a => a.userId == userId).Select(a => a.patient_id).First();
                var patientdetails = entities.Patients.Where(a => a.patient_id == patientId).First();
                return patientdetails;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void EditPatientDetails(string patientId,Patient patientDetails)
        {
            var res = entities.Patients.Find(patientId);

            res.prev_history = patientDetails.prev_history;
            res.reports = patientDetails.reports;
            res.userId = patientDetails.userId;

            entities.SaveChanges();
        }
    }
}