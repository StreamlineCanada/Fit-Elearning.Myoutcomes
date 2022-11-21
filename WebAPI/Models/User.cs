using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    //public class FitUser
    //{
    //    public int id { get; set; }
    //    public string contactName { get; set; }
    //    public string loginUserId { get; set; }
    //    public DateTime createDate { get; set; }
    //    public string companyName { get; set; }
    //    public string fitNumber { get; set; }
    //    public string email { get; set; }
    //    public int statusId { get; set; }
    //}
    public class MostRecentDetails
    {
        public string mostRecentLesson { get; set; }
        public string mostRecentScore { get; set; }
    }

    public class FitElearnUserDetail
    {
        public int id { get; set; }

        public string loginUserId { get; set; }

        public string mostRecentLessonDate { get; set; }
        public string lessonAccess { get; set; }
        public string mostRecentLesson { get; set; }
        public double? mostRecentScore { get; set; }
    }

}