using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoginValues
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}