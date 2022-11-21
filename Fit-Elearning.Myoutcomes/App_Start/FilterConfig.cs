using System.Web;
using System.Web.Mvc;

namespace Fit_Elearning.MyOutcomes
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}