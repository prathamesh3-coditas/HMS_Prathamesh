using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace WebApplication1.Data_Access
{
    
    public class DeseaseDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();

        public Desease_Details GetDeseaseDetails(string patientId)
        {
            try
            {
                var res = entities.Desease_Details.Where(a => a.patient_id == patientId).First();
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void AddDeseaseDetails(Desease_Details details)
        {
            entities.Desease_Details.Add(details);
            entities.SaveChanges();
        }


        public void IncrementNumberOfVisits(Desease_Details details)
        {
            var newDetail = entities.Desease_Details.Where(a => a.patient_id == details.patient_id).First();

            newDetail.patient_id = details.patient_id;
            newDetail.desease_catagory = details.desease_catagory;
            newDetail.no_of_visits = ++details.no_of_visits;
            Console.WriteLine();
            entities.SaveChanges();
        }

  
        public void EditDeseaseCategory(Desease_Details details)
        {
            var oldDetails = entities.Desease_Details.Where(a=>a.patient_id==details.patient_id).First();

            oldDetails.patient_id=details.patient_id;
            oldDetails.no_of_visits = details.no_of_visits;
            oldDetails.desease_catagory = details.desease_catagory;

            entities.SaveChanges();
        }
    }
}