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
    public class CreateUserModel
    {
        public int Id { get; set; }
        public string LoginUserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public short UserStatusId { get; set; }

        public System.DateTime CreateDate { get; set; }
        public string CreateIP { get; set; }
        public int CreateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateIP { get; set; }
        public Nullable<int> UpdateUserID { get; set; }

        public byte BasicModule { get; set; }
        public byte AdvancedModule { get; set; }

        public string CompanyName { get; set; }        
        public string FitNumber { get; set; }

        public bool Success { get; set; }
        public string ErrorMsg { get; set; }        
    }
}