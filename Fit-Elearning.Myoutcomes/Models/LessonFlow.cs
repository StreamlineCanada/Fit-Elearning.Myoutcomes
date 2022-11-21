using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class LessonFlow
    {
        public int userId { get; set; }

        public int moduleId { get; set; }

        public int lessonFlowId { get; set; }
        public int lessonFlowSortOrderNumber { get; set; }
        public int lessonFlowTypeId { get; set; }

        public string lessonNextURL { get; set; }
        public string lessonFlowText { get; set; }
        public string lessonFlowContent { get; set; }

        public string lessonNextBtnStyle { get; set; }
        public string lessonNextPageUrl { get; set; }

        public string lessonBackBtnStyle { get; set; }
        public string lessonBackURL { get; set; }

        public string lessonCompleteCheck { get; set; }
        public string lessonCompleteDisabled { get; set; }

        public bool hideCompleteNextButton { get; set; }

        public string youTubeVideoID { get; set; }

        public QuestionGroup questionGroup { get; set; }

        public Lesson lesson;


    }

    public class Lesson
    {
        public int lessonId { get; set; }
        public string lessonName { get; set; }
        public int lessonSortOrderNumber { get; set; }
        public string lessonImageFile { get; set; }
        public string lessonDescription { get; set; }
        public string lessonContent { get; set; }


        public double lessonQuizScore { get; set; }
        public bool lessonQuizPass { get; set; }

        public bool lessonQuizComplete { get; set; }
        public string lessonStartUrl { get; set; }
        public string lessonStartUrlClass { get; set; }

        public bool onPace { get; set; }
        public bool isAvailable { get; set; }

        public LessonActivity lessonActivity { get; set; }

        public QuestionGroup questionGroup { get; set; }

        public LessonFlow lessonFlow { get; set; }
    }

  
}

   