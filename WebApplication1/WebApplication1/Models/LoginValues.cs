using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoginValues
    {
        [Required(ErrorMessage =("Username is required for Login"))]
        public string UserName { get; set; }

        [Required(ErrorMessage = ("Password is required for Login"))]
        public string Password { get; set; } = string.Empty;
    }
}