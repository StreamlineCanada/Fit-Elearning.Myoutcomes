using System;
using System.Linq;
using System.Data.Entity;
using Fit_Elearning.MyOutcomes.Domain;
using Fit_Elearning.MyOutcomes.Domain.Abstract;
using Fit_Elearning.MyOutcomes.Domain.Entities;
using Fit_Elearning.MyOutcomes.Models;
using Fit_Elearning.MyOutcomes.Domain.Helpers;



using System.Data.Objects;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Fit_Elearning.MyOutcomes.Domain.Concrete
{
    public class EfUserRepository : IUserRepository
    {

        #region Variables

        private fit_elearningEntities entities = new fit_elearningEntities();


        
        #endregion

        #region Properties

        #endregion

        #region Constructors

        public EfUserRepository()
        {
            this.entities = new fit_elearningEntities();
            this.entities.CommandTimeout = 42000; // this should be somewhere else (config file preferably)
        }

        #endregion

        #region Query Methods

        public IQueryable<Entities.USER> Users
        {
            get { return entities.USER; }
        }
        public bool ValidateUser(string userId, string password)
        {

            bool? v = entities.SpValidateUser(userId, password).First<bool?>();
            if (v == null) return false;
            else if (v == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CreateUser(string userId, int userStatusId, int? lessonAccessId, string email)
        {

            string newPw = System.Web.Security.Membership.GeneratePassword(8,1);
            try
            {
                bool basicLesson = true;
                bool advancedLesson = false;
                
                if (lessonAccessId.HasValue)
                {
                    switch(lessonAccessId.Value)
                    {
                        case 1:
                            basicLesson = false;
                            advancedLesson = true;
                            break;
                        case 2:
                            advancedLesson = true;
                            break;
                    }
                }

                entities.spCreateUser(userId, newPw, userStatusId, DateTime.Now, "::1", 1, basicLesson, advancedLesson, email);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(string username, string name, string email, string password, int? userId, int? lessonAccessId)
        {
            try
            {
                bool basicLesson = true;
                bool advancedLesson = false;
                
                if (lessonAccessId.HasValue)
                {
                    switch(lessonAccessId.Value)
                    {
                        case 1:
                            basicLesson = false;
                            advancedLesson = true;
                            break;
                        case 2:
                            advancedLesson = true;
                            break;
                    }
                }
                
                entities.spUpdateUser(username, name, email, password, userId, basicLesson, advancedLesson);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public USER GetUser(string userId, int userStatusId)
        {
            return entities.USER.SingleOrDefault(user => user.LOGIN_USER_ID == userId && user.USER_STATUS_CODE == userStatusId);
        }

        public USER GetUserByEmail(string email)
        {
            return entities.USER.SingleOrDefault(u => u.EMAIL == email);
        }

        public spGetLessonFlow_Result GetLessonFlow(int moduleId, int lessonId, int sortId)
        {
            return entities.spGetLessonFlow(moduleId, lessonId, sortId).FirstOrDefault<spGetLessonFlow_Result>();
        }
        
        public VIDEO GetVideo(int videoId)
        {
            return entities.VIDEO.SingleOrDefault(video => video.VIDEO_ID == videoId);
        }
        
        public QUESTION_GROUP GetQuestionGroup(int questionGroupId)
        {
            return entities.QUESTION_GROUP.SingleOrDefault(questionGroup => questionGroup.QUESTION_GROUP_ID == questionGroupId);
        }

       
        public List<spGetQuestions_Result> GetQuestions(int questionGroupId)
        {
            return entities.spGetQuestions(questionGroupId).ToList<spGetQuestions_Result>();
        }
 
        public List<QUESTION_OPTION> GetQuestionOptions(int questionId)
        {
            return entities.QUESTION_OPTION.Where(qro => qro.QUESTION_ID == questionId).ToList<QUESTION_OPTION>();
        }

        public spSaveUserResponse_Result SaveUserResponse(UserResponse ur)
        {
            return entities.spSaveUserResponse(ur.moduleId, ur.lessonId, ur.questionId, ur.questionTypeId, ur.userId, ur.response).FirstOrDefault();
        }

        public spSaveUserActivity_Result SaveUserActivity(UserActivityResponse uar)
        {
            return entities.spSaveUserActivity(uar.userId, uar.moduleId, uar.lessonId, uar.activityInputId, uar.activityText).FirstOrDefault();
        }

        public spSaveLessonComplete_Result SaveLessonComplete(int moduleId, int lessonId, int userId)
        {
            return entities.spSaveLessonComplete(moduleId, lessonId, userId).FirstOrDefault();
        }

        public string GetUserResponse(int moduleId, int lessonId, int questionId, int userId)
        {
            try
            {
                return entities.spGetUserResponse(moduleId, lessonId, questionId, userId).First<string>();
            }
            catch
            {
                return "";
            }
        }

        public string GetUserActivity(int moduleId, int lessonId, int userId, int activityInputId)
        {
            try
            {
                return entities.spGetUserActivity(moduleId, lessonId, userId, activityInputId).First<string>();
            }
            catch
            {
                return "";
            }
        }


        public spSaveQuizComplete_Result SaveQuizComplete(int moduleId, int lessonId, int userId, float score)
        {
            return entities.spSaveQuizComplete(moduleId, lessonId, userId, score).FirstOrDefault();
        }

        public spGetLesson_Result GetLesson(int moduleId,int lessonId)
        {

            return entities.spGetLesson(moduleId, lessonId).FirstOrDefault();
        }

        public spGetQuizScore_Result GetQuizScore(int moduleId, int lessonId, int userId)
        {
            return entities.spGetQuizScore(moduleId, lessonId, userId).FirstOrDefault();
        }

        public spGetQuizComplete_Result GetQuizComplete(int modeuleId, int lessonId, int userId)
        {
            return entities.spGetQuizComplete(modeuleId, lessonId, userId).FirstOrDefault();
        }

        public bool GetLessonComplete(int moduleId, int lessonId, int userId)
        {
            try
            {
                int? resultId = entities.spGetLessonComplete(moduleId, lessonId, userId).FirstOrDefault();

                return (resultId != null);
            }
            catch
            {
                return false;
            }
        }

        public double GetCoursePassedScore(int userId, bool isAdvancedCourse)
        {
            int moduleId = 1;
            int startLessonSortNum = isAdvancedCourse ? 13 : 1;
            int lastLessonSortNum = isAdvancedCourse ? 14: 12;

            int? lastLessonId = entities.spGetLessonId(moduleId, lastLessonSortNum).FirstOrDefault();
            if (lastLessonId == null)
            {
                return -1;
            }

            
            if (!this.GetLessonComplete(moduleId,lastLessonId.Value,userId))
            {
                return -1;
            }

            double sumTotalLessonScorePercent = 0;

            for (int i = startLessonSortNum; i <= lastLessonSortNum; i++)
            {
                int? thisLessonId = entities.spGetLessonId(moduleId, i).FirstOrDefault();
                if (thisLessonId == null)
                {
                    return -1;
                }

                var lessonScoreResult = this.GetQuizScore(moduleId, thisLessonId.Value, userId);
                if (lessonScoreResult == null || !lessonScoreResult.score.HasValue)
                {
                    return -1;
                }

                int numQuestions = lessonScoreResult.num_questions.Value;

                var scoresRes = entities.USER_QUIZ_COMPLETE.Where(c => c.MODULE_ID == moduleId && c.LESSON_ID == thisLessonId.Value && c.USER_ID == userId).ToList();
                if (scoresRes == null)
                {
                    return -1;
                }
                double avgLessonScorePercent = (scoresRes.Average(c => c.USER_QUIZ_SCORE) / numQuestions) * 100;
                sumTotalLessonScorePercent += avgLessonScorePercent;

            }

            return Math.Round((sumTotalLessonScorePercent / (isAdvancedCourse ? 2 : 12)), 1, MidpointRounding.AwayFromZero);


        }

        public List<spGetLessons_Result> GetLessons(int moduleId)
        {
            return entities.spGetLessons(moduleId).ToList<spGetLessons_Result>();
        }

        public spGetLastCompletedLesson_Result GetLastCompletedLesson(int moduleId, int userId)
        {
            try
            {
                return entities.spGetLastCompletedLesson(moduleId, userId).First();
            }
            catch
            {
                return new spGetLastCompletedLesson_Result { LESSON_ID = 0, USER_QUIZ_COMPLETE_DATETIME = DateTime.MinValue };
            }
        }

        public int? GetLessonId(int moduleId, int sortNumber)
        {
            return entities.spGetLessonId(moduleId, sortNumber).FirstOrDefault();
        }

        public spGetLessonActivity_Result GetLessonActivity(int lessonId)
        {
            return entities.spGetLessonActivity(lessonId).FirstOrDefault();
        }

        public List<spGetTextQuestions_Result> GetTextQuestions(int moduleId, int lessonId)
        {
            return entities.spGetTextQuestions(moduleId, lessonId).ToList<spGetTextQuestions_Result>();
        }

       public List<spGetRadioNonQuizQuestions_Result> spGetRadioNonQuizQuestions(int moduleId, int lessonId)
       {
           return entities.spGetRadioNonQuizQuestions(moduleId,lessonId).ToList<spGetRadioNonQuizQuestions_Result>();
       }

        public bool RegisterUser(USER u)
        {
            //entities.USER.Attach(u);
            entities.USER.AddObject(u);
            entities.SaveChanges();
            return u.ID>0;
        }

        public bool UserExists(string username)
        {

            var a = Users.Count(u => u.LOGIN_USER_ID == username);
            return a > 0;

        }

        public bool UserExists(USER u)
        {
            return UserExists(u.LOGIN_USER_ID);
        }

        public bool SaveUserAudit(int userId, string auditOperation, string ip)
        {
            try
            {
                USER_AUDIT ua = new USER_AUDIT();
                ua.USER_ID = userId;
                ua.AUDIT_OPERATION = auditOperation;
                ua.AUDIT_IP = ip;
                ua.AUDIT_DATETIME = DateTime.Now;

                entities.AddToUSER_AUDIT(ua);
                entities.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public DateTime? GetFirstLoginDateTime(int userId)
        {
            try
            {
                USER_AUDIT userAudit = entities.USER_AUDIT.Where(ua => ua.USER_ID == userId).OrderBy(ua => ua.AUDIT_DATETIME).FirstOrDefault();
                if (userAudit == null)
                {
                    return null;
                }
                else
                {
                    return userAudit.AUDIT_DATETIME;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool VerifyAndSendNewPassword(string email, string newPw)
        {
            try
            {
                var r = entities.SpVerifyAndSendNewPassword(email, newPw);
                int? val = r.First();
                if (val == null)
                {
                    return false;
                }

                return (val == 1);

            }
            catch
            {
                return false;
            }
        }

        public string GetModuleStartEndDate(int userId, int extServerId)
        {
            int moduleId = 1;

            USER user = entities.USER.Where(u => u.ID == userId).FirstOrDefault();
            bool basicLessonUser = Convert.ToBoolean(user.MODULE1_BASIC);
            bool advancedLessonUser = Convert.ToBoolean(user.MODULE1_ADVANCED);

            int startLessonNum = 1;
            int endLessonNum = 12;

            /*
            if (advancedLessonUser)
            {
                endLessonNum = 14;
                if (!basicLessonUser)
                {
                    startLessonNum = 13;
                }
            }*/

            int startLessonId = this.GetLessonId(1,startLessonNum).Value;
            int endLessonId = this.GetLessonId(1, endLessonNum).Value;

            var sq = entities.USER_QUIZ_COMPLETE.First(qc => qc.LESSON_ID == startLessonId && qc.USER_ID == userId && qc.MODULE_ID == moduleId);
            var eq = entities.USER_QUIZ_COMPLETE.First(qc => qc.LESSON_ID == endLessonId && qc.USER_ID == userId && qc.MODULE_ID == moduleId);

            return sq.USER_QUIZ_COMPLETE_DATETIME.ToString("d MMM yyyy") + " - " + eq.USER_QUIZ_COMPLETE_DATETIME.ToString("d MMM yyyy");


        }

        public bool EmailUsernameExistElsewhere(string username, string email, int userId)
        {
            int other = entities.USER.Count(u => (u.LOGIN_USER_ID == username || u.EMAIL == email) && u.ID != userId);

            return other > 0;
        }
        
        public List<spGetUsersBehind_Result> GetUsersBehind(int moduleId=1)
        {
            var users = entities.spGetUsersBehind(moduleId);
            return users.ToList();
        }

        public bool CreateUser(UserModel user)
        {
            try
            {
                USER u = new USER();
                u.LOGIN_USER_ID = user.username;
                u.NAME = user.name;
                u.EMAIL = user.email;
                u.PASSWORD = user.password;
                u.CREATE_USER_ID = 1;
                u.CREATE_IP = "0.0.0.0";
                u.CREATE_DATETIME = DateTime.Now;
                u.MODULE1_BASIC = 0;
                u.MODULE1_ADVANCED = 1;
                u.USER_STATUS_CODE = 0;

                entities.USER.AddObject(u);
                entities.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CreateUser2(CreateUserModel user)
        {
            try
            {
                entities.spCreateUser2(user.LoginUserId, user.Name, user.Password, user.Email, user.UserStatusId, user.CreateDate, user.CreateUserID, user.CreateIP, user.BasicModule, user.AdvancedModule, user.CompanyName, user.FitNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<USER> GetAllUsers(int showInactives)
        {
            if (showInactives == 1)
            {
                return entities.USER.Where(u => u.USER_STATUS_CODE == -1 && u.isAdmin != 1 && u.LOGIN_USER_ID != "").OrderBy(u => u.LOGIN_USER_ID).ToList<USER>();
            }

            return entities.USER.Where(u => u.USER_STATUS_CODE == 0 && u.isAdmin != 1 && u.LOGIN_USER_ID != "").OrderBy(u => u.LOGIN_USER_ID).ToList<USER>();
        }

        public bool UpdateUser2(CreateUserModel user)
        {
            try
            {
                entities.spUpdateUser2(user.Id, user.LoginUserId, user.Name, user.Email, user.UpdateDate, user.UpdateUserID, user.UpdateIP, user.BasicModule, user.AdvancedModule, user.CompanyName, user.FitNumber);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeactivateUser(int id, string loginUserId)
        {
            try
            {
                entities.spDeactivateUser(id, loginUserId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ReactivateUser(int id, string loginUserId)
        {
            try
            {
                entities.spReactivateUser(id, loginUserId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


    }
}
