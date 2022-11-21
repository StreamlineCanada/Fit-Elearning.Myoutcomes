using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fit_Elearning.MyOutcomes.Domain.Entities;
using Fit_Elearning.MyOutcomes.Domain.Concrete;
using Fit_Elearning.MyOutcomes.Models;

namespace Fit_Elearning.MyOutcomes.Domain.Abstract
{
    public interface IUserRepository
    {
        bool ValidateUser(string userId, string password);

        bool CreateUser(string userId, int userStatusId, int? lessonAccessId, string email);

        bool UpdateUser(string username, string name, string email, string password, int? userId, int? lessonAccessId);

        USER GetUser(string userId, int userStatusId);

        USER GetUserByEmail(string email);

        spGetLessonFlow_Result GetLessonFlow(int moduleId, int lessonId, int sortId);

        VIDEO GetVideo(int videoId);

        QUESTION_GROUP GetQuestionGroup(int questionGroupId);

        List<spGetQuestions_Result> GetQuestions(int questionGroupId);

        List<QUESTION_OPTION> GetQuestionOptions(int questionId);

        spSaveUserResponse_Result SaveUserResponse(UserResponse ur);

        spSaveUserActivity_Result SaveUserActivity(UserActivityResponse uar);

        string GetUserResponse(int moduleId, int lessonId, int questionId, int userId);

        string GetUserActivity(int moduleId, int lessonId, int userId, int activityInputId);

        spSaveLessonComplete_Result SaveLessonComplete(int moduleId, int lessonId, int userId);

        spSaveQuizComplete_Result SaveQuizComplete(int moduleId, int lessonId, int userId, float score);

        spGetLesson_Result GetLesson(int moduleId, int lessonId);

        spGetQuizScore_Result GetQuizScore(int moduleId, int lessonId, int userId);

        spGetQuizComplete_Result GetQuizComplete(int modeuleId, int lessonId, int userId);

        bool GetLessonComplete(int moduleId, int lessonId, int userId);

        double GetCoursePassedScore(int userId, bool isAdvancedCourse);

        List<spGetLessons_Result> GetLessons(int moduleId);

        spGetLastCompletedLesson_Result GetLastCompletedLesson(int moduleId, int userId);

        int? GetLessonId(int moduleId, int sortNumber);

        spGetLessonActivity_Result GetLessonActivity(int lessonId);

        List<spGetTextQuestions_Result> GetTextQuestions(int moduleId, int lessonId);

        List<spGetRadioNonQuizQuestions_Result> spGetRadioNonQuizQuestions(int moduleId, int lessonId);

        bool RegisterUser(USER u);

        bool UserExists(string username);

        bool UserExists(USER u);

        bool SaveUserAudit(int userId, string auditOperation, string ip);

        DateTime? GetFirstLoginDateTime(int userId);

        bool VerifyAndSendNewPassword(string email, string newPw);

        string GetModuleStartEndDate(int userId, int extServerId);
   
        bool EmailUsernameExistElsewhere(string username, string email, int userId);

        bool CreateUser(UserModel user);

        bool CreateUser2(CreateUserModel user);

        List<USER> GetAllUsers(int showInactives);

        bool UpdateUser2(CreateUserModel user);

        bool DeactivateUser(int id,string loginUserId);

        bool ReactivateUser(int id, string loginUserId);

    }
}
