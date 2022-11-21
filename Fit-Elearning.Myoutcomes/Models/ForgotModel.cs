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
    public class ForgotModel
    {
        public string email { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}