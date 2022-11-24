using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();
        
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
            if (user.role_id == null)
            {
                return View();
            }

            Session["UserToRegister"] = user;
            var allUsers = entities.Users.ToList();


            var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
            var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
            var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();

            if (isUserNameValid !=null)
            {
                TempData["InvalidUserName"] = "UserName is not available";
            }
            
            if (isEMailValid != null)
            {
                TempData["InvalidEMail"] = "E-mail already registered..!!!";
            }
            
            if(isContactValid != null)
            {
                TempData["InvalidContact"] = "Contact already registered..!!!";
            }



//As admin has logged in so TempData has value of TempData["name"] from Login() in Home Controller
            if (TempData.Count>1)
            {
                return View(((User)Session["UserToRegister"]));
            }
            else
            {
                entities.Users.Add(user);
                entities.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ShowAll()
        {
            var data  = entities.Users.ToList();
            return View(data);
        }

        
        public ActionResult Edit(int id)
        {
            TempData["IdToUpdate"] = id;
            var data = entities.Users.Find(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            
            var userFound = entities.Users.Find(Convert.ToInt32(TempData["IdToUpdate"]));

            userFound.full_name = user.full_name;
            userFound.email = user.email;
            userFound.password_ = user.password_;
            userFound.userName = user.userName;
            userFound.age = user.age;
            userFound.gender = user.gender;
            userFound.contact_number = user.contact_number;
            userFound.specialization = user.specialization;

            entities.SaveChanges();
            return RedirectToAction("ShowAll", "Admin");
        }

        public ActionResult Delete(int id)
        {
            TempData["IdToDelete"] = id;
            var data = entities.Users.Find(id);
            return View(data);
        }

        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm()
        {
            User u = entities.Users.Find(Convert.ToInt32(TempData["IdToDelete"]));
            entities.Users.Remove(u);

            entities.SaveChangesAsync();
            return RedirectToAction("ShowAll");
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }

}