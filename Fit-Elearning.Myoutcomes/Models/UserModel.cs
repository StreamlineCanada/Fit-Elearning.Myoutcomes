using Fit_Elearning.MyOutcomes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class UserModel
    {
        public string userId { get; set; }
        public int userStatusId { get; set; }

        public int id { get; set; }
        public string username { get; set; }        
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }

        public bool success { get; set; }
        public string errorMsg { get; set; }

        public int? lessonAccessId { get; set; }

        public List<CreateUserModel> Users { get; set; }
        public bool isDeactivated { get; set; }
    }
}