using System;
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
        UserDataAccess userAccess = new UserDataAccess();
        ConsumableDataAccess consumableAccess = new ConsumableDataAccess();

        [Authorize(Roles = "Admin")]
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


        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Register(User user)
        {
            Session["UserToRegister"] = user;

            try
            {

                if (ModelState.IsValid)
                {

                    var allUsers = userAccess.GetAllUsers();


                    var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
                    var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
                    var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();

                    var contactValidation = userAccess.ValidateContact(user.contact_number);
                    Console.WriteLine();
                    if (contactValidation == 0)
                    {
                        TempData["contactValidator"] = "Please provide 10 digit valid number";
                    }
                    else if (contactValidation == 1)
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

                        user.password_ = Encryption.Encryption.EncodePassword(user.password_);

                        user.confirmPassword_ = Encryption.Encryption.EncodePassword(user.confirmPassword_);
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

                    return View((User)Session["UserToRegister"]);
                }
            }
            catch (Exception)
            {

                return View("ErrorPage");
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
            try
            {
                TempData["IdToUpdate"] = id;

                var searchedUser = userAccess.GetUserById(id);
                return View(searchedUser);
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(User user)
        {
            try
            {
                int id = Convert.ToInt32(TempData["IdToUpdate"]);

                var res = userAccess.UpdateUserInfo(user, id);

                return RedirectToAction("ShowAll", "Admin");
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                TempData["UserIdToDelete"] = id;

                var userFound = userAccess.GetUserById(id);
                return View(userFound);
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm()
        {
            try
            {
                int id = Convert.ToInt32(TempData["UserIdToDelete"]);


                var res = userAccess.DeleteUser(id);
                if (res == true)
                {
                    return RedirectToAction("ShowAll");

                }
                else
                {
                    return View("ErrorPage");
                }
            }
            catch (Exception)
            {

                return View("ErrorPage");
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult ConsumableDetails()
        {
            try
            {
                var all = consumableAccess.GetAllConsumables();
                return View(all);

            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddConsumables()
        {
            try
            {
                var consumable = new Consumable();
                return View(consumable);
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddConsumables(Consumable consumable)
        {
            try
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
            catch (Exception)
            {

                return View("ErrorPage");
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("LoginPage","Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditConsumables(int? id)
        {
            try
            {
                TempData["ConsumableId"] = id;
                var data = consumableAccess.GetConsumableById((int)id);
                return View(data);
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditConsumables(Consumable newData)
        {
            try
            {
                var id = (int)TempData["ConsumableId"];
                var consumable = consumableAccess.GetConsumableById(id);

                consumableAccess.EditConsumable(newData, id);

                return RedirectToAction("ConsumableDetails");
            }
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult SearchUser(string name)
        {
            try
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
            catch (Exception)
            {

                return View("ErrorPage");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult SearchMedicine(string name)
        {
            try
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
            catch (Exception)
            {
                return View("ErrorPage");
            }
        }
    }

}