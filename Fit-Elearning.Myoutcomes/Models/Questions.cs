using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Models
{
    public class QuestionGroup
    {
        public int questionGroupId { get; set; }
        public string questionGroupTitle { get; set; }
        public List<Question> questionList { get; set; }
    }

    public class Question
    {
        public int questionId {get; set;}
        public int questionNumber { get; set; }
        public int questionType { get; set; }
        public string questionText { get; set; }
        public string questionResponse { get; set; }
        public List<QuestionOption> questionOptions { get; set; }
        public bool answeredCorrectly { get; set; }

    }

    public class QuestionOption
    {
        public int questionOptionId { get; set; }
        public string questionOptionText { get; set; }
        public string questionOptionChecked { get; set; }
        public string questionOptionImage { get; set; }
        public string questionOptionSelected { get; set; }
    }
}