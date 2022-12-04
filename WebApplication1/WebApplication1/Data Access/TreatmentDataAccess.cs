using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Data_Access
{
    public class TreatmentDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();

        public void AddDetails(Treatment treatment)
        {
            var res = entities.Treatments.Add(treatment);
            entities.SaveChanges();
        }

        public IEnumerable<Treatment> GetTreatmentDetails(string patId)
        {
            var data = entities.Treatments.Where(a => a.patient_id == patId).ToList();

            //var data = (entities.Treatments).ToList().Join((entities.Consumables).ToList(),
            //    t => t.consumable_id,
            //    c => c.consumable_id,
            //    (t, c) => new
            //    {
            //        conId = t.consumable_id,
            //        conName = c.consumable_name,
            //        price = c.price,
            //        quantity = t.quantity,
            //        isActive = t.IsActive
            //    }).ToList();

            if (data != null)
            {
                return data;
            }
            else
            {
                return null;
            }
        }
        public void EditDetails(IEnumerable<Treatment> treatments)
        {
            foreach (var treatment in treatments)
            {
                var singleEntry = entities.Treatments.Where(a => a.treatment_id == treatment.treatment_id).First();
                singleEntry.quantity = treatment.quantity;
                //singleEntry.is_admitted = treatment.is_admitted;
                singleEntry.patient_id = treatment.patient_id;
                singleEntry.consumable_id = treatment.consumable_id;
                singleEntry.IsActive = false;

                entities.SaveChanges();

            }
        }
    }
}