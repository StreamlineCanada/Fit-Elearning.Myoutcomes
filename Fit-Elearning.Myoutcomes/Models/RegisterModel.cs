using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Fit_Elearning.MyOutcomes.Domain.Entities;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        //public string Name { get; set; }

        public USER ToEntity()
        {
            var u = new USER
            {
                 EMAIL = this.Email,
                 CREATE_DATETIME = DateTime.Now,
                 NAME = this.Name,
                 USER_STATUS_CODE = 1,
                 CREATE_USER_ID = 1,
                 CREATE_IP = "1"
            };
            return u;
        }
    }
}