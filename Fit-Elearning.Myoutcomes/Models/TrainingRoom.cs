using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class TrainingRoom
    {
        public int moduleId { get; set; }
        public int? nextLessonId { get; set; }
        public int? nextLessonSortNum { get; set; }
        public bool showProgress { get; set; }
        
        public string nextLessonLabel { get; set; }
        public List<Lesson> lessons { get; set; }
        public bool introComplete { get; set; }

        public int userId { get; set; }
        public string userName { get; set; }

        public double basicOverallCourseScore { get; set; }
        public double advancedOverallCourseScore { get; set; }

        public bool showBasicCertificateLink { get; set; }
        public bool showAdvancedCertificateLink { get; set; }

        public string ceUnitUrl { get; set; }

        public bool isBasicUser { get; set; }

        public bool isAdvancedUser { get; set; }

    }

   
}