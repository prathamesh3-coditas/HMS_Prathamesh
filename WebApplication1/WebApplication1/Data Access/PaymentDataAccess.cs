using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Data_Access
{
    public class PaymentDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        public int AddPaymentDetails(Payment payment)
        {
            entities.Payments.Add(payment);
            var res = entities.SaveChanges();
            return res;
        }

        public IEnumerable<Payment> GetPaymentDetails()
        {
            return entities.Payments.ToList();
        }

        public void DeletePaymentDetails(string patId)
        {
            var detail = entities.Payments.Where(a=>a.patientId == patId && a.isRequested==false).First();
            entities.Payments.Remove(detail);

            entities.SaveChanges();
        }


        public void EditPaymentDetails(string patId)
        {
            var paymentDetails = entities.Payments.Where(a=>a.patientId== patId && a.isRequested==false).First();

            paymentDetails.isRequested=true;
            entities.SaveChanges();
        }

    }
}