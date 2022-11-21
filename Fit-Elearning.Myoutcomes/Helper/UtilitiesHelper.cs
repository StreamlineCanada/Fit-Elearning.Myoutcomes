using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Fit_Elearning.MyOutcomes.Models;

namespace Fit_Elearning.MyOutcomes.Helper
{
    public static class UtilitiesHelper
    {
        static bool IsAdmin;
        public static UserModel GetUserInfo(string serverId)
        {            
            UserModel userModel = new UserModel();
            userModel.userId = HttpContext.Current.User.Identity.Name;

            userModel.userStatusId = Convert.ToInt32(serverId);

            return userModel;

        }

        public static void SetIsAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }

        public static bool GetIsAdmin()
        {
            return IsAdmin;
        }

    }
}