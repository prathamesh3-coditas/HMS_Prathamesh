using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Razor.Generator;

namespace WebApplication1.Data_Access
{
    public class ConsumableDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        public IEnumerable<Consumable> GetAllConsumables()
        {
            var allConsumables = entities.Consumables.ToList();
            if (allConsumables!=null)
            {
                return allConsumables;
            }
            else
            {
                return null;
            }
        }

        public bool AddConsumables(Consumable consumable)
        {
            var res = entities.Consumables.Add(consumable);
            entities.SaveChanges();
            if (res!=null)
            {
                return true;
            }

            return false;
        }

        public Consumable GetConsumableById(int id)
        {
            var data = entities.Consumables.ToList().Where(a => a.consumable_id == id).First();
            if (data != null)
            {
                return data;
            }

            return null;
        }
        public void EditConsumable(Consumable con,int id)
        {
            var oldData = entities.Consumables.Find(id);

            oldData.consumable_name = con.consumable_name;
            oldData.availability_detail = con.availability_detail;
            oldData.price = con.price;

            entities.SaveChanges();

        }
    }
}