using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;

namespace WebApplication1.Data_Access
{
    public class BillingDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();


        public void AddBillingDetails(Billing b)
        {

            entities.Billings.Add(b);
            entities.SaveChanges();
        }

        public Tuple<IEnumerable<Treatment>,IEnumerable<Consumable>> BillingDetails(IEnumerable<Treatment> treatments)
        {
            List<Consumable> consumables = new List<Consumable>();
          
            foreach (var item in treatments)
            {
               var data = entities.Consumables.Where(a => a.consumable_id == item.consumable_id).First();
                consumables.Add(data);
            }

            Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>> tuple 
                = new Tuple<IEnumerable<Treatment>, IEnumerable<Consumable>>(treatments,consumables);

            return tuple;

        }

        public void GenerateBill(IEnumerable<Treatment> treatment,string patId,int DoctorId)
        {
            List<Consumable> consumableDetails = new List<Consumable>();
            foreach (var item in treatment)
            {
                var conDetails = (from detail in entities.Consumables where
                                         detail.consumable_id == item.consumable_id select detail).First();                               

                consumableDetails.Add(conDetails);

            }
            
            Billing b = new Billing();
            
            foreach (var item in consumableDetails)
            {

                b.patient_id = patId;
                b.consumable_id = item.consumable_id;
                b.userId = DoctorId;
                b.is_paid = false;

                AddBillingDetails(b);
            }
        }

        public IEnumerable<Billing> ShowBillsByPatientId(string patId)
        {
            var data = entities.Billings.Where(a => a.patient_id == patId).ToList();
            if (data!=null)
            {
                return data;
            }

            return null;
        }

        public int GetTotalBillByPatient(string patientId)
        {
            var billingDetails = entities.Billings.Join(entities.Consumables,
                b => b.consumable_id,
                c => c.consumable_id,
                (b, c) => new
                {
                    id = c.consumable_id,
                    name = c.consumable_name,
                    patId = b.patient_id,
                    price = c.price,
                    approved = b.is_paid,
                }).Where(a=>a.approved==false).ToList();

            int totalBill = (int)billingDetails.Sum(a => a.price);

            return totalBill;
        }


        public void EditBillings(Billing bill,string patientId)
        {
            var oldBills = entities.Billings.Where(a => a.patient_id == patientId && a.is_paid==false).First();


                oldBills.is_paid = true;
                entities.SaveChanges();
                
        }
    }
}