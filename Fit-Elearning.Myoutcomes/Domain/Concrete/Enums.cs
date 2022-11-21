using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fit_Elearning.MyOutcomes.Domain.Concrete
{
    public enum LessonFlowContentTypes : int
    {
        QuestionGroup = 1,
        Video = 2,
        ConfirmLessonComplete = 3,
        Content = 4
    }

    public enum QuestionTypes : int
    {
        Radio = 1,
        TextArea = 2,
        CheckBox = 3,
        TextBox = 4,
        Select = 5,
        Slider = 6,
        Empty = 7,
        RadioNonQiuz = 8,
        SlideAgree = 9
    }

}