using System;
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
using WebApplication1.Data_Access;
using WebApplication1.Encryption;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {


        UserDataAccess userData = new UserDataAccess();

        public ActionResult Index()
        {

            if (Session["User"] != null)
            {
                var roleId = ((User)Session["User"]).role_id;
                if (roleId == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roleId == 2)
                {
                    return RedirectToAction("Index", "Doctor");
                }
                else if (roleId == 3)
                {
                    return RedirectToAction("Index", "Receptionist");
                }
                else if (roleId == 4)
                {
                    return RedirectToAction("Index", "Patient");
                }

                return View();
            }
            else
            
            {

                return View();

            }


        }

        public ActionResult LoginPage()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(string userName, string pass)
        {
            try
            {
                Session.Clear();
                LoginValues values = new LoginValues();
                values.UserName = userName;

                values.Password = Encryption.Encryption.EncodePassword(pass);

                FormsAuthentication.SetAuthCookie(userName, false);
                if (ModelState.IsValid)
                {

                    var data = userData.validateUser(values);


                    Console.WriteLine();
                    if (data == null)
                    {
                        ViewBag.ErrorMsg = "Invalid username and password";
                        return View();
                    }
                    else
                    {
                        Session["User"] = data;
                        TempData["name"] = userData.getFullNameById(values);
                        return RedirectToAction("Success", values);
                    }
                }
                return View();
            }
            catch (Exception)
            {

                return View("HomeError");
            }
        }

        public ActionResult Success(LoginValues values)
        {


            try
            {
                if (User.IsInRole("Patient"))
                {
                    return RedirectToAction("Index", "Patient");
                }
                else if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("Index", "Doctor");
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("Receptionist"))
                {
                    return RedirectToAction("Index", "Receptionist");
                }

                return View();
            }
            catch (Exception)
            {

                return View("HomeError");
            }
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.role_id = 4;


                    var allUsers = userData.GetAllUsers().ToList();
                    Session["SignUpUser"] = user;



                    var isUserNameValid = allUsers.Where(a => a.userName == user.userName).FirstOrDefault();
                    var isEMailValid = allUsers.Where(a => a.email == user.email).FirstOrDefault();
                    var isContactValid = allUsers.Where(a => a.contact_number == user.contact_number).FirstOrDefault();

                    var contactValidation = userData.ValidateContact(user.contact_number);
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


                    if (TempData.Count > 1)
                    {
                        return View(((User)Session["SignUpUser"]));
                    }
                    else
                    {

                        user.password_ = Encryption.Encryption.EncodePassword(user.password_);

                        user.confirmPassword_ = Encryption.Encryption.EncodePassword(user.confirmPassword_);
                        userData.CreateUser(user);
                        Session.Remove("SignUpUser");
                    }

                    return View("Successful");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {

                return View("HomeError");
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult About()
        {

            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "Patient")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}