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
    }
}