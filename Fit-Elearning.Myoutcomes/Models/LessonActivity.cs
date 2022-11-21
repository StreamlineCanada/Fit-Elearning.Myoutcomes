using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class LessonActivity
    {
        public int moduleId { get; set; }
        public int lessonId { get; set; }
        public int userId { get; set; }

        public string lessonName { get; set; }
        public string lessonImageFile { get; set; }
        public int lessonSortNum { get; set; }

        public string activityTitle { get; set; }
        public string activityText { get; set; }
    }
}