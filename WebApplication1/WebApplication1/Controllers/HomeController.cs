﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.DynamicData;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(LoginValues values)
        {

            if (ModelState.IsValid)
            {
                var data = (entities.Users)
                    .Where(a => a.userName == values.UserName && a.password_ == values.Password)
                    .FirstOrDefault();

                Session["User"] = data;
                Console.WriteLine();
                if (data == null)
                {
                    ViewBag.ErrorMsg = "Invalid username and password";
                    return View();
                }
                else
                {
                    TempData["name"] = entities.Users.Where(a => a.userName == values.UserName).Select(a => a.full_name).FirstOrDefault();
                    return RedirectToAction("Success",values);
                }
            }

            return View();
        }

        public ActionResult Success(LoginValues values)
        {
            var role = (entities.Users).ToList().Join((entities.Roles).ToList(),
                u => u.role_id,
                r => r.role_id,
                (u, r) => new
                {
                    userName = u.userName,
                    nameOfUser = u.full_name,
                    roleOfUser = r.roleName
                });

            var record = role.Where(a => a.userName == values.UserName).FirstOrDefault();
            

            Console.WriteLine();
            if (record.roleOfUser == "Patient")
            {
                return RedirectToAction("Index", "Patient");
            }
            else if (record.roleOfUser == "Receptionist")
            {
                return RedirectToAction("Index", "Receptionist");
            }
            else if (record.roleOfUser == "Doctor")
            {
                return RedirectToAction("Index","Doctor");
            }
            else if (record.roleOfUser == "Admin")
            {
                return RedirectToAction("Index", "Admin");

            }
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(User user)
        {
            user.role_id = 4;

            var allUsers = entities.Users.ToList();
            Session["SignUpUser"] = user;

            var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
            var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
            var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();
            Console.WriteLine();
            if (isUserNameValid != null)
            {
                TempData["InvalidUserName"] = "UserName is not available";
            }

            if (isEMailValid != null)
            {
                TempData["InvalidEMail"] = "E-mail already registered..!!!";
            }

            if (isContactValid != null)
            {
                TempData["InvalidContact"] = "Contact already registered..!!!";
            }


            if (TempData.Count > 0) //initially no user has logged in so TempData.Count==0
            {
                return View(((User)Session["SignUpUser"]));
            }
            else
            {
                entities.Users.Add(user);
                entities.SaveChangesAsync();
            }

            return RedirectToAction("LoginPage");
        }


        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}