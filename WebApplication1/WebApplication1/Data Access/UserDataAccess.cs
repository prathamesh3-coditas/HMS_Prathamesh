﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;


namespace WebApplication1.Data_Access
{
    public class UserDataAccess
    {
        HMS_Project_newEntities2 entities = new HMS_Project_newEntities2();


        public User validateUser(LoginValues values)
        {
            var data = (entities.Users)
            .Where(a => a.userName == values.UserName && a.password_ == values.Password)
            .FirstOrDefault();

            return data;
        }


        public IEnumerable<User> GetAllUsers()
        {
            var data = entities.Users.ToList();
            return data;
        }

        public User GetUserById(int id)
        {
            var userFound = entities.Users.Find(id);
            return userFound;
        }
        public string getFullNameById(LoginValues values)
        {
            var fullName = entities.Users.Where(a => a.userName == values.UserName)
            .Select(a => a.full_name).FirstOrDefault();

            return fullName;
        }

        public string onSuccess(LoginValues values)
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

            return record.roleOfUser;
        }

        public User CreateUser(User user)
        {
            entities.Users.Add(user);
            entities.SaveChanges();

            return user;
        }


        public User UpdateUserInfo(User user, int id)
        {
            var userFound = entities.Users.Find(id);


            userFound.full_name = user.full_name;
            userFound.email = user.email;
            userFound.password_ = user.password_;
            userFound.userName = user.userName;
            userFound.age = user.age;
            userFound.gender = user.gender;
            userFound.contact_number = user.contact_number;
            userFound.specialization = user.specialization;

            entities.SaveChanges();

            return userFound;
        }


        public bool DeleteUser(int id)
        {
            try
            {
                User u = entities.Users.Find(Convert.ToInt32(id));



                Patient patientByUserId = entities.Patients.Where(x => x.userId == id).First();
                Patient p = entities.Patients.Find(patientByUserId.patient_id);





                Appointment appointmentByUserId = entities.Appointments.Where(x => x.userId == id).First();
                Appointment a = entities.Appointments.Find(appointmentByUserId.appointment_id);





                Billing billByUserId = entities.Billings.Where(x => x.userId == id).First();
                Billing b = entities.Billings.Find(billByUserId.bill_number);


                Console.WriteLine();
                if (p != null)
                {
                    var deleteFromPatient = entities.Patients.Remove(p);
                    entities.SaveChanges();
                }
                if (a != null)
                {
                    var deleteFromAppointment = entities.Appointments.Remove(a);
                    entities.SaveChanges();
                }
                if (b != null)
                {
                    var deleteFromBilling = entities.Billings.Remove(b);
                    entities.SaveChanges();
                }
                var res = entities.Users.Remove(u);

                entities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;

            }

        }
    }
}