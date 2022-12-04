﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Data_Access;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        UserDataAccess userAccess = new UserDataAccess();
        ConsumableDataAccess consumableAccess = new ConsumableDataAccess();

        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            TempData.Keep("name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            User user = new User();
            return View(user);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Register(User user)
        {
            Session["UserToRegister"] = user;


            if (ModelState.IsValid)
            {


                //var allUsers = entities.Users.ToList();

                var allUsers = userAccess.GetAllUsers();


                var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
                var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
                var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();

                var contactValidation = userAccess.ValidateContact(user.contact_number);

                if (contactValidation==0)
                {
                    TempData["contactValidator"] = "Please provide 10 digit valid number";
                }
                else if (contactValidation==1)
                {
                    TempData["contactValidator"] = "Mobile Number can not contain letters";
                }
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
                

                //As admin has logged in so TempData has value of TempData["name"] from Login() in Home Controller
                if (TempData.Count > 1)
                {
                    return View(((User)Session["UserToRegister"]));
                }
                else
                {
                    userAccess.CreateUser(user);
                }

                return RedirectToAction("Index");
            }
            else
            {

                if (user.role_id == null)
                {
                    ViewBag.roleId = "Oops..You haven't selected a role id";
                }
                //if (user.gender == null || user.gender=="null")
                //{
                //    ViewBag.gender = "Oops..You haven't selected a gender";
                //}

                //if (user.role_id == null)
                //{
                //    ViewBag.roleId = "Oops..You haven't selected a role id";
                //}
                return View((User)Session["UserToRegister"]);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ShowAll()
        {
            var allUsers = userAccess.GetAllUsers();
            return View(allUsers);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            TempData["IdToUpdate"] = id;

            var searchedUser = userAccess.GetUserById(id);
            return View(searchedUser);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(User user)
        {

            //var userFound = entities.Users.Find(Convert.ToInt32(TempData["IdToUpdate"]));

            int id = Convert.ToInt32(TempData["IdToUpdate"]);

            var res = userAccess.UpdateUserInfo(user, id);
            //var userFound = entities.Users.Find(id);
            //userFound.full_name = user.full_name;
            //userFound.email = user.email;
            //userFound.password_ = user.password_;
            //userFound.userName = user.userName;
            //userFound.age = user.age;
            //userFound.gender = user.gender;
            //userFound.contact_number = user.contact_number;
            //userFound.specialization = user.specialization;
            //entities.SaveChanges();
            return RedirectToAction("ShowAll", "Admin");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            TempData["UserIdToDelete"] = id;
            //var data = entities.Users.Find(id);

            var userFound = userAccess.GetUserById(id);
            return View(userFound);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm()
        {
            int id = Convert.ToInt32(TempData["UserIdToDelete"]);
            //User u = entities.Users.Find(Convert.ToInt32(TempData["UserIdToDelete"]));


            var res = userAccess.DeleteUser(id);
            
            return RedirectToAction("ShowAll");


        }


        [Authorize(Roles = "Admin")]
        public ActionResult ConsumableDetails()
        {
           var all = consumableAccess.GetAllConsumables();
            return View(all);

        }


        [Authorize(Roles = "Admin")]
        public ActionResult AddConsumables()
        {
            var consumable = new Consumable();
            return View(consumable);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddConsumables(Consumable consumable)
        {
            if (ModelState.IsValid)
            {
                var res = consumableAccess.AddConsumables(consumable);
                return RedirectToAction("ConsumableDetails");
            }
            else
            {
                return View();
            }

        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }

        public ActionResult EditConsumables(int? id)
        {
            TempData["ConsumableId"] = id;
            var data = consumableAccess.GetConsumableById((int)id);
            return View(data);
        }

        [HttpPost]
        public ActionResult EditConsumables(Consumable newData)
        {
            var id = (int)TempData["ConsumableId"];
            var consumable = consumableAccess.GetConsumableById(id);

            consumableAccess.EditConsumable(newData,id);

            return RedirectToAction("ConsumableDetails");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult SearchUser(string name)
        {
            name = name.ToLower();
            if (name == String.Empty)
            {
                var userDetails = userAccess.GetAllUsers();
                return View("ShowAll", userDetails);
            }
            else
            {
                var userDetails = userAccess.GetAllUsers().Where(a => a.full_name.ToLower().Contains(name) || name.Contains(a.full_name.ToLower()));
                return View("ShowAll", userDetails);

            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult SearchMedicine(string name)
        {
            name = name.ToLower();
            if (name == String.Empty)
            {
                var consumableDetails = consumableAccess.GetAllConsumables();
                return View("ConsumableDetails", consumableDetails);
            }
            else
            {
                var consumableDetails = consumableAccess.GetAllConsumables().Where(a => a.consumable_name.ToLower().Contains(name) || name.Contains(a.consumable_name.ToLower()));
                return View("ConsumableDetails", consumableDetails);

            }
        }
    }

}