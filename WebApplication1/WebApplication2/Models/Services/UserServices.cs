using Antlr.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models.Services
{
    public class UserServices
    {
        HMS_Project_newEntities entities;

        public UserServices(HMS_Project_newEntities entities)
        {
            this.entities = entities;
        }
    }
}