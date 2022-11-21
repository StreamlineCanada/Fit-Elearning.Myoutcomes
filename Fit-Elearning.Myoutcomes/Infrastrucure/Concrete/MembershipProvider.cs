using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using System.Data.Entity;

using Ninject;
using System.Web.Mvc;
using Fit_Elearning.MyOutcomes.Domain;
using Fit_Elearning.MyOutcomes.Domain.Entities;
using Fit_Elearning.MyOutcomes.Domain.Abstract;

using Fit_Elearning.MyOutcomes.Infrastructure.Abstract;




namespace Fit_Elearning.MyOutcomes.Infrastructure.Concrete
{
    public class MyOutcomesMembershipProvider : IMembershipProvider
    {

        private IUserRepository repository { get; set; }


        public MyOutcomesMembershipProvider(IUserRepository r)
        {
            if (r != null)
            {
                repository = r;
            }
            else
            {
                repository = DependencyResolver.Current.GetService<IUserRepository>();
            }
        }


        public bool ValidateUser(string userId, string password)
        {
            if (string.IsNullOrEmpty(password.Trim()) || string.IsNullOrEmpty(userId.Trim()))
                return false;
            var result = this.repository.ValidateUser(userId, password);
            
            return result;
        }

     


    }
}