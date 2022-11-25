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
        // GET: Admin
        //HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        UserDataAccess userAccess = new UserDataAccess();
        
        public ActionResult Index()
        {
            TempData.Keep("name");
            return View();
        }

        public ActionResult Register()
        {
            User user = new User();
            return View(user);
        }


        [HttpPost]
        public ActionResult Register(User user)
        {
            Session["UserToRegister"] = user;


            if (ModelState.IsValid)
            {
                if (user.role_id == null)
                {
                    return View();
                }

                //var allUsers = entities.Users.ToList();

                var allUsers = userAccess.GetAllUsers();


                var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
                var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
                var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();

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
                    //entities.Users.Add(user);
                    userAccess.CreateUser(user);
                    //entities.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            else
            {
                if (user.role_id==null)
                {
                    ViewBag.roleId = "Oops..You haven't selected a role id";
                }

                if (user.gender=="null")
                {
                    ViewBag.gender = "Oops..You haven't selected a gender";
                }
                return View((User)Session["UserToRegister"]);
            }
        }

        public ActionResult ShowAll()
        {
            //var data  = entities.Users.ToList();
            var allUsers = userAccess.GetAllUsers();
            return View(allUsers);
        }

        
        public ActionResult Edit(int id)
        {
            TempData["IdToUpdate"] = id;
            //var data = entities.Users.Find(id);

            var searchedUser = userAccess.GetUserById(id);
            return View(searchedUser);
        }

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

        public ActionResult Delete(int id)
        {
            TempData["UserIdToDelete"] = id;
            //var data = entities.Users.Find(id);

            var userFound = userAccess.GetUserById(id);
            return View(userFound);
        }

        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm()
        {
            int id = Convert.ToInt32(TempData["UserIdToDelete"]);
            //User u = entities.Users.Find(Convert.ToInt32(TempData["UserIdToDelete"]));
            //entities.Users.Remove(u);

            //entities.SaveChangesAsync();

            var res = userAccess.DeleteUser(id);
            
            return RedirectToAction("ShowAll");


        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }

}