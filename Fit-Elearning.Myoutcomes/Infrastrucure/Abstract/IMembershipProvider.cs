using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Infrastructure.Abstract
{
    public interface IMembershipProvider
    {
        bool ValidateUser(string userId, string password);
    }
}