using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Models
{

    public class Response
    {
        public int userId { get; set; }
        public int moduleId { get; set; }
        public int lessonId { get; set; }

    }
    
    public class UserResponse : Response
    {
        public int questionId { get; set; }
        public int questionTypeId { get; set; }
        public string response { get; set; }
    }

    public class LessonComplete : Response
    {
        public bool lessonComplete { get; set; }
    }

    public class QuizComplete : Response
    {
        public string score { get; set; }
    }

    public class UserActivityResponse : Response
    {
        public int activityInputId { get; set; }
        public string activityText { get; set; }
    }
}