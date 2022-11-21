using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Web.Routing;

using Fit_Elearning.MyOutcomes.Models;
using Fit_Elearning.MyOutcomes.Infrastructure.Abstract;
using Fit_Elearning.MyOutcomes.Infrastructure.Concrete;
using Fit_Elearning.MyOutcomes.Domain.Entities;
using Fit_Elearning.MyOutcomes.Domain.Abstract;
using Fit_Elearning.MyOutcomes.Domain.Concrete;
using Fit_Elearning.MyOutcomes.Domain.Helpers;

using Fit_Elearning.MyOutcomes.Helper;

namespace Fit_Elearning.MyOutcomes.Controllers
{
    public class HomeController : Controller
    {
        private MyOutcomesMembershipProvider MyOutcomesMembershipProvider { get; set; }
        public IUserRepository Repository;

        public HomeController()
        {
            Repository = new EfUserRepository();
        }
        
   

        protected override void Initialize(RequestContext requestContext)
        {
            if (MyOutcomesMembershipProvider == null)
                MyOutcomesMembershipProvider = new MyOutcomesMembershipProvider(Repository);


            base.Initialize(requestContext);
        }

        public ActionResult ExtLogin(string data)
        {
            if (data == "" || data == null)
            {
                return new RedirectResult("/Home/ExtError");
            }

            try
            {

                string decodedUrl = Decrypt(data);
                NameValueCollection queryVals = HttpUtility.ParseQueryString(decodedUrl);

                // check link is not old
                DateTime timeStamp = new DateTime(Convert.ToInt64(queryVals["timestamp"]));
                if (DateTime.Compare(DateTime.Now, timeStamp.AddSeconds(18000)) > 0)
                {
                    ErrorModel em = new ErrorModel { errorMessage = "The link has timed out. Press the back button and refresh the screen and try again." };
                    return View("ExtError",em);
                }

                EmailProcessor ep = new EmailProcessor();
                

                int sourceServerId = Convert.ToInt32(queryVals["sourceServerId"]);
                int? lessonAccessId = Convert.ToInt32(queryVals["lessonAccessId"]);

                string userId = queryVals["uid"];
                string email = queryVals["email"];

                //ep.SendEmail("chrisstocker@myoutcomes.com", "FIT-ELEARN", "email:" + email + "|" + queryVals.ToString());

                USER u = Repository.GetUser(userId, sourceServerId);
                if (u == null)
                {
                    Repository.CreateUser(userId, sourceServerId, lessonAccessId, email);
                    u = Repository.GetUser(userId, sourceServerId);
                }
                else
                {
                    Repository.UpdateUser(u.LOGIN_USER_ID, u.NAME, email, "", u.ID, lessonAccessId);
                }

                FormsAuthentication.SetAuthCookie(userId, false);

                HttpCookie cookie = new HttpCookie("ExtServerId");
                cookie.Value = sourceServerId.ToString();
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                Repository.SaveUserAudit(u.ID, "Login", Request.ServerVariables["REMOTE_ADDR"]);
            }
            catch (Exception e)
            {
                ErrorModel em = new ErrorModel { errorMessage = "An error has occurred. Press the back button and refresh the screen and try again. <br/>" + e.Message };
                return View("ExtError", em);
            }
            

            return new RedirectResult("/Home/Index");
        }

        
        
        public ActionResult Index()
        {
            int moduleId = 1;

            var ui = new UserModel();
            try
            {
                ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            }
            catch
                {
                return new RedirectResult("/home/Login");
            }

            if (ui == null)
            {
                return new RedirectResult("/home/Login");
            }
            USER u = Repository.GetUser(ui.userId, ui.userStatusId);           
            UtilitiesHelper.SetIsAdmin(Convert.ToBoolean(u.isAdmin));

            if ( u == null)            {
                return new RedirectResult("/home/Login");
            }

            var model = this.GetTrainingRoom(moduleId, u.ID);
            
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Contact_Login()
        {
            return View();
        }
        public ActionResult Forgot_PW()
        {
            return View();
        }
        public ActionResult ICCE_Support()
        {
            return View();
        }
        public ActionResult Awareness_Profile()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Request_Certificate()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            var certificate = new Certificate();

            certificate.certificateImageUrl = "cert_full.jpg";
            
            certificate.displayName = user.NAME != "" ? user.NAME : user.LOGIN_USER_ID;
            try
            {
                certificate.certificateDateStr = Repository.GetModuleStartEndDate(userId, Convert.ToInt32(Request.Cookies["ExtServerId"].Value));
            }
            catch (Exception e)
            
            { return new RedirectResult("/Home/Index?e="+e.Message); }


            return View(certificate);
        }

        public ActionResult Request_Certificate_Advanced()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            var certificate = new Certificate();

            certificate.certificateImageUrl = "cert_full_deliberate_practice.png";
           
            certificate.displayName = user.NAME != "" ? user.NAME : user.LOGIN_USER_ID;
            try
            {
                certificate.certificateDateStr = Repository.GetModuleStartEndDate(userId, Convert.ToInt32(Request.Cookies["ExtServerId"].Value));
            }
            catch { return new RedirectResult("/Home/Index"); }


            return View("Request_Certificate",certificate);
        }

        public ActionResult Congratulations()
        {
            return View();
        }

        public ActionResult ProfileEdit()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            var model = new UserModel { id = user.ID, username = user.LOGIN_USER_ID, email = user.EMAIL, name = user.NAME };
            
            return View(model);
        }

        public ActionResult Progress_Report()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;

            bool basicLessonUser = Convert.ToBoolean(user.MODULE1_BASIC);
            bool advancedLessonUser = Convert.ToBoolean(user.MODULE1_ADVANCED);

            var model = this.GetTrainingRoom(1, userId);

            if (basicLessonUser && !advancedLessonUser)
            {
                model.lessons.RemoveRange(13, 2);
            }
            else if (advancedLessonUser && !basicLessonUser)
            {
                model.lessons.RemoveRange(1, 12);
            }



            return View(model);
        }

        public ActionResult About_FIT_Tutorial()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                if (MyOutcomesMembershipProvider.ValidateUser(model.UserId, model.Password))
                {

                    FormsAuthentication.SetAuthCookie(model.UserId, false);

                    HttpCookie cookie = new HttpCookie("ExtServerId");
                    cookie.Value = ConstantVars.USERSTATUS_ORIGN_LOCAL.ToString();
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                    USER u = Repository.GetUser(model.UserId, ConstantVars.USERSTATUS_ORIGN_LOCAL);

                    if (u == null)
                    {
                        ModelState.AddModelError("", "User ID or password is incorrect.");
                    }
                    else
                    {

                        Repository.SaveUserAudit(u.ID, "Login", Request.ServerVariables["REMOTE_ADDR"]);


                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                }

                else
                {
                    ModelState.AddModelError("", "User ID or password is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }

        public ActionResult UpdateProfile(UserModel um)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            
            if (!um.lessonAccessId.HasValue)
            {
                if (user.MODULE1_BASIC == 1 && user.MODULE1_ADVANCED == 0)
                {
                    um.lessonAccessId = 0;
                }
                else if (user.MODULE1_ADVANCED == 1 && user.MODULE1_BASIC == 0)
                {
                    um.lessonAccessId = 1;
                }
                else if (user.MODULE1_BASIC == 1 && user.MODULE1_ADVANCED == 1)
                {
                    um.lessonAccessId = 2;
                }
            }


            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            if (user.ID != um.id)
            {
                um.success = false;
                um.errorMsg = "Your are not authorized to perform this action.";
            }
            else {

                bool exists = Repository.EmailUsernameExistElsewhere(um.username, um.email, um.id);

                if (exists)
                {
                    um.success = false;
                    um.errorMsg = "The username or email address is already in use by another user. Please try another.";
                }
                else
                {
                    um.success = Repository.UpdateUser(um.username, um.name, um.email, um.password, um.id, um.lessonAccessId);
                    if (!um.success)
                    {
                        um.errorMsg = "An error occurred while updating your profile. Please try again";
                    }
                }
            }

            return Json(um, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenBetaList()
        {
            List<UserModel> betaUsers = new List<UserModel>();

            betaUsers.Add(new UserModel { email = "scottdmiller@talkingcure.com", name = "Scott D Miller ", username = "Scott.D.Miller", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "rpmaes@shaw.ca", name = "CYNTHIA MAESCHALCK",username="CYNTHIA.MAESCHALCK", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "daryl@darylchow.com", name = "Daryl Chow PhD", username = "Daryl.Chow", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "trousmaniere@gmail.com", name = "Tony Rousmaniere", username = "Tony.Rousmaniere", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "von.borg@outlook.com", name = "Von Borg", username = "Von.Borg", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "rene.goedhart@kempler-instituut.n", name = "Rene Goedhart", username = "Rene.Goedhart", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "daxuan.ng@nus.edu.sg", name = "Ng Da Xuan", username = "Ng.Da.Xuan", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "Anne.Koerber@sa.gov.au", name = "Koerber, Anne", username = "Koerber.Anne", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "mwaters@ubalt.edu", name = "Myra Waters",username="Myra.Waters", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "sespears1@aol.com", name = "Sydney Spears", username = "Sydney.Spears", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "dddayton@gmail.com", name = "Dave Dayton", username = "Dave.Dayton", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "chinltao@aol.com", name = "Chin Tao", username = "Chin.Tao", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "colleenlucas@shaw.ca", name = "Colleen Lucas", username = "Colleen.Lucas", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "socialarbetaren.se@gmail.com",name = "Antony Peiris",username="Antony.Peiris", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "maria_c_nistor@yahoo.com", name = "Maria Nistor", username = "Maria.Nistor", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "chr_cazauvieilh@yahoo.fr", name = "Christophe Cazauvielh", username = "Christophe.Cazauvielh", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "freedomplusheart@yahoo.com", name = "Kathleen Conrad", username = "Kathleen.Conrad", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "Sonja.Skocic@healthscope.com.au", name = "Sonja Skocic", username = "Sonja.Skocic", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "ps@psychoterapie-cbrod.cz", name = "Český Brod",username="Český.Brod", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "lhorton1@gmail.com", name = "Lee Horton", username = "Lee.Horton", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "ginger@heartfocustherapy.com", name = "Ginger Bahardar", username = "Ginger.Bahardar", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "rikkepap@gmail.com",name = "ikke Papsøe", username = "Rikke.Papsøe", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "ajrprent@hotmail.com",name = "Alastair Prentice", username = "Alastair.Prentice", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "psychologenpraktijkbreukers@hetnet.nl",name = "P. Breukers", username = "P.Breukers", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "puia@gratiaplenacounseling.org",name = "Pui Au", username = "Pui.Au", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "Jennifer.Ostlinger@csc-scc.gc.ca",name = "Jennifer Ostlinger", username = "Jennifer.Ostlinger", password = System.Web.Security.Membership.GeneratePassword(8, 1) });
            betaUsers.Add(new UserModel { email = "ellie4e@gmail.com",name = "Ellie Fourie", username = "Ellie.Fourie", password = System.Web.Security.Membership.GeneratePassword(8, 1) });

            foreach(var user in betaUsers)
            {
                Repository.CreateUser(user);
                Repository.VerifyAndSendNewPassword(user.email, user.password);
            }


            return View(betaUsers);
        }

        public ActionResult VerifyAndSendNewPassword(ForgotModel fm)
        {
            string newPw = System.Web.Security.Membership.GeneratePassword(8,1);
            bool r = Repository.VerifyAndSendNewPassword(fm.email, newPw);

            var lm = new ForgotModel();
            
            try
            {
                var mail = new System.Net.Mail.MailAddress(fm.email);
            }
            catch
            {
                lm = new ForgotModel {message = "Please enter a valid email address.", success = false};
                return Json(lm, JsonRequestBehavior.AllowGet);
            }


            if (r)
            {
                USER u = Repository.GetUserByEmail(fm.email);
                
                Fit_Elearning.MyOutcomes.Domain.Helpers.EmailProcessor ep = new EmailProcessor();
                string er = ep.SendPasswordEmail(u.LOGIN_USER_ID,fm.email,newPw);
                if (er == "ok")
                {
                    lm = new ForgotModel { message = "An email has been sent to you with a new password.", success = true };
                }
                else
                {
                    lm = new ForgotModel { message = er, success = false };
                }
            }
            else
            {
                lm = new ForgotModel { message = "We are unable to find any account with that email address.<br/>Are you a MyOutcomes Application user? If so please <a href='http://www.myoutcomes.com'>login to MyOutcomes</a> and then click the Fit E-Learn button.<br/> For more help <a href='mailto:support@myoutcomes.com'>contact technical support</a>.", success = false };
            }

            return Json(lm, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LogOff()
        {
            int serverId = Convert.ToInt32(Request.Cookies["ExtServerId"].Value);

            if (serverId == ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
                System.Web.HttpContext.Current.Session.Remove(ui.userId);

                FormsAuthentication.SignOut();


                return RedirectToAction("Login", "Home");
            }
            else
            {
                string serverUrl = "";
                switch (serverId)
                {
                    case ConstantVars.USERSTATUS_ORGIN_US:
                        serverUrl = "https://www.myoutcomesapp.com/User/IndexUser";
                        break;
                    case ConstantVars.USERSTATUS_ORIGN_CAN:
                        serverUrl = "https://can.myoutcomesapp.com/User/IndexUser";
                        break;
                    case ConstantVars.USERSTATUS_ORIGIN_BETA:
                        serverUrl = "https://beta.myoutcomesapp.com/User/IndexUser";
                        break;
                }

                return Redirect(serverUrl);
            }
        }

        public ActionResult Intro()
        {
            return View();
        }
        public ActionResult Intro_Activities()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult SaveUserResponse(UserResponse ur)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            ur.userId = user.ID;

            try
            {
                var result = Repository.SaveUserResponse(ur);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new spSaveUserResponse_Result() { error = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult SaveUserActivity(UserActivityResponse uar)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            uar.userId = user.ID;

            var result = Repository.SaveUserActivity(uar);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetUserActivity(int moduleId, int lessonId, int activityInputId)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            int userId = user.ID;
            
            var result = new UserActivityResponse{ userId = userId, moduleId = moduleId, lessonId = lessonId,  activityInputId = activityInputId};

            string activityText = Repository.GetUserActivity(moduleId, lessonId, userId, activityInputId);
            result.activityText = activityText;

            return Json(result, JsonRequestBehavior.AllowGet);



        }


        [Authorize]
        [HttpGet]
        public ActionResult SaveLessonComplete(LessonComplete lc)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            lc.userId = user.ID;

            var result = Repository.SaveLessonComplete(lc.moduleId, lc.lessonId, lc.userId);


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult SaveQuizComplete(QuizComplete qc)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            qc.userId = user.ID;

            float score;
            try
            {
                score = float.Parse(qc.score);
            }
            catch {
                score = 0;
            }            

            var result = Repository.SaveQuizComplete(qc.moduleId, qc.lessonId, qc.userId, score);


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        public ActionResult LessonFlow(int moduleId, int lessonId, int sortId)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            
            var flow = Repository.GetLessonFlow(moduleId, lessonId, sortId);

            if (flow == null || flow.LESSON_FLOW_ID == 0)
            {
                    return RedirectToAction("Index", "Home");
            }

            if (flow.LESSON_SORT_ORDER == 0)
            {
                Repository.SaveLessonComplete(moduleId, lessonId, userId);
            }

           bool hasQuestions = false;
           bool hasRadioQuestions = false;
           bool hasCheckboxes = false;
           bool hasSelects = false;
            

            
            var model = new LessonFlow();
            model.lesson = new Lesson();
            model.moduleId = moduleId;
            model.userId = userId;
            model.lesson.lessonId = lessonId;
            model.lessonFlowId = flow.LESSON_FLOW_ID;
            model.lessonFlowSortOrderNumber = (int)flow.LESSON_FLOW_SORT_ORDER;
            model.lessonFlowTypeId = flow.LESSON_FLOW_TYPE_ID;
            model.lessonFlowText = flow.LESSON_FLOW_TEXT;
            model.lessonFlowContent = flow.LESSON_FLOW_CONTENT;

            
            model.lesson.lessonName = flow.LESSON_NAME;
            model.lesson.lessonSortOrderNumber = (int)flow.LESSON_SORT_ORDER;
            model.lesson.lessonImageFile = flow.LESSON_IMAGE_FILE;

            if (model.lessonFlowTypeId == (int)LessonFlowContentTypes.Video)
            {

                var video = Repository.GetVideo((int)flow.VIDEO_ID);
                model.youTubeVideoID = video.VIDEO_YOUTUBE_ID;

            }
            else if (model.lessonFlowTypeId == (int)LessonFlowContentTypes.QuestionGroup)
            {
                hasQuestions = true;
                
                var questionGroup = Repository.GetQuestionGroup((int)flow.QUESTION_GROUP_ID);
                var questions = Repository.GetQuestions(questionGroup.QUESTION_GROUP_ID);

                QuestionGroup qg = new QuestionGroup();
                qg.questionGroupId = questionGroup.QUESTION_GROUP_ID;
                qg.questionGroupTitle = questionGroup.QUESTION_GROUP_TITLE;
                qg.questionList = new List<Question>();

                foreach (spGetQuestions_Result question in questions)
                {
                    
                    
                    
                    Question q = new Question { questionId = question.QUESTION_ID, questionNumber = question.QUESTION_SORT_ORDER, questionText = question.QUESTION_TEXT, questionType = (int)question.QUESTION_TYPE_ID, answeredCorrectly = false };
                    
                    List<QuestionOption> qos = new List<QuestionOption>();

                    string response = Repository.GetUserResponse(moduleId, lessonId, question.QUESTION_ID, userId);
                    q.questionResponse = response;


                    if (question.QUESTION_TYPE_ID == (int)QuestionTypes.Radio || question.QUESTION_TYPE_ID == (int)QuestionTypes.RadioNonQiuz)
                    {
                        hasRadioQuestions = question.QUESTION_TYPE_ID == (int)QuestionTypes.Radio;
                        
                        var questionOptions = Repository.GetQuestionOptions(question.QUESTION_ID);
                        foreach (QUESTION_OPTION questionOption in questionOptions)
                        {
                            string questionOptionChecked = "";
                            string answerIcon = "";

                            if (questionOption.QUESTION_OPTION_ID.ToString() == response)
                            {
                                questionOptionChecked = "checked";

                                if (questionOption.QUESTION_OPTION_CORRECT)
                                {
                                    q.answeredCorrectly = true;
                                    answerIcon = "<img src='../Images/icons/correct_icon.png' alt='Correct' style='vertical-align:bottom' >";
                                }
                                else
                                {
                                    answerIcon = "<img src='../Images/icons/incorrect_icon.png' alt='Incorrect' style='vertical-align:bottom' >";
                                }
                            }

                            qos.Add(new QuestionOption { questionOptionId = questionOption.QUESTION_OPTION_ID, questionOptionText = questionOption.QUESTION_OPTION_TEXT, questionOptionChecked = questionOptionChecked, questionOptionImage = answerIcon });
                        }

                    }
                    else if (question.QUESTION_TYPE_ID == (int)QuestionTypes.Select)
                    {
                        hasSelects = true;

                        var questionOptions = Repository.GetQuestionOptions(question.QUESTION_ID);
                        foreach (QUESTION_OPTION questionOption in questionOptions)
                        {
                            string questionOptionSelected = "";

                            if (questionOption.QUESTION_OPTION_ID.ToString() == response)
                            {
                                questionOptionSelected = "selected='selected'";
                            }

                            qos.Add(new QuestionOption { questionOptionId = questionOption.QUESTION_OPTION_ID, questionOptionText = questionOption.QUESTION_OPTION_TEXT, questionOptionSelected = questionOptionSelected });
                        }

                    }
                    else if (question.QUESTION_TYPE_ID == (int)QuestionTypes.CheckBox)
                    {
                        q.questionResponse = (q.questionResponse == "1" ? "checked" : "");
                        hasCheckboxes = true;
                    }

                    q.questionOptions = qos;
                    qg.questionList.Add(q);

                    

                }

                model.questionGroup = qg;

            }
            else if (model.lessonFlowTypeId == (int)LessonFlowContentTypes.ConfirmLessonComplete)
            {
                bool result = Repository.GetLessonComplete(moduleId, lessonId, userId);
                if (!result)
                {
                    model.lessonNextBtnStyle = "display:none;";
                }
                else
                {
                    model.lessonCompleteCheck = "checked";
                    model.lessonCompleteDisabled = "disabled";
                }
                
            }

            if (hasRadioQuestions)
            {
                model.lessonNextURL = "javascript:FitElearn.Question.allAnswered();";
            }
            else if (hasQuestions && !hasCheckboxes && !hasSelects)
            {
                model.lessonNextURL = "javascript:FitElearn.Question.checkAllResponses();";
                model.lessonNextPageUrl = "LessonFlow?moduleId=" + moduleId + "&lessonId=" + lessonId + "&sortId=" + (sortId + 1);
            }
            else
            {
                model.lessonNextURL = "LessonFlow?moduleId=" + moduleId + "&lessonId=" + lessonId + "&sortId=" + (sortId + 1);
            }

            if (sortId == 1)
            {
                model.lessonBackBtnStyle = "display:none;";
            }
            else
            {
                model.lessonBackURL = "LessonFlow?moduleId=" + moduleId + "&lessonId=" + lessonId + "&sortId=" + (sortId - 1);
            }
            return View(model);
        }


        [Authorize]
        public ActionResult LessonFlowComplete(int moduleId, int lessonId)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            
            var lesson = Repository.GetLesson(moduleId,lessonId);

            if (lesson == null || lesson.LESSON_ID == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            bool isAdvancedUser = Convert.ToBoolean(user.MODULE1_ADVANCED);
            bool isBasicUser = Convert.ToBoolean(user.MODULE1_BASIC);

            
            var model = new LessonFlow();
            model.lesson = new Lesson();
            

            model.userId = userId;
            model.moduleId = moduleId;
            model.lesson.lessonId = lessonId;
            model.lesson.lessonImageFile = lesson.LESSON_IMAGE_FILE;
            model.lesson.lessonSortOrderNumber = (int)lesson.LESSON_SORT_ORDER;
            model.lesson.lessonName = lesson.LESSON_NAME;
            model.lesson.lessonContent = lesson.LESSON_CONTENT;

            double score = 0;
            var lessonScoreResult = Repository.GetQuizScore(moduleId, lessonId, userId);
            if (lessonScoreResult != null && lessonScoreResult.num_questions > 0)
            {
                score = Convert.ToDouble(lessonScoreResult.score) / Convert.ToDouble(lessonScoreResult.num_questions);
            }


            score = score * 100;
            model.lesson.lessonQuizPass = (score >= 80);
            model.lesson.lessonQuizScore = Math.Round(score, 1);

            model.hideCompleteNextButton = false;
            if (isBasicUser && model.lesson.lessonSortOrderNumber == 12 && model.lesson.lessonQuizPass)
            {
                model.hideCompleteNextButton = true;
            }
            

            if (model.lesson.lessonSortOrderNumber == 14 && model.lesson.lessonQuizPass)
            {
                model.hideCompleteNextButton = true;
            }

            return View(model);
        }
        

        [Authorize]
        public ActionResult Training_Room()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            
            int moduleId = 1;

            var model = this.GetTrainingRoom(moduleId, userId);
            
            return View(model);

        }


        [Authorize]
        public ActionResult AwarenessProfile()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            int userId = user.ID;
            
            int moduleId = 1;
            

            var model = this.GetTrainingRoom(moduleId, userId);

            return View(model);
        }


        [Authorize]
        private TrainingRoom GetTrainingRoom(int moduleId, int userId)
        {

            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            
            
            bool introComplete = false;
            bool lessonAvailable;
            string lessonStartUrl = "";

            int introLessonId = (int)Repository.GetLessonId(moduleId, 0);
            introComplete = Repository.GetLessonComplete(moduleId, introLessonId, userId);

            spGetLastCompletedLesson_Result lastLesson = Repository.GetLastCompletedLesson(moduleId, userId);

            bool basicLessonUser = Convert.ToBoolean(user.MODULE1_BASIC);
            bool advancedLessonUser = Convert.ToBoolean(user.MODULE1_ADVANCED);


            

            var model = new TrainingRoom();

            try
            {
                model.lessons = new List<Lesson>();

                model.moduleId = moduleId;
                model.userId = userId;
                model.userName = user.LOGIN_USER_ID;

                model.introComplete = introComplete;

                model.isBasicUser = basicLessonUser;
                model.isAdvancedUser = advancedLessonUser;

                var lessons = Repository.GetLessons(moduleId);

                foreach (spGetLessons_Result lesson in lessons)
                {

                    var l = new Lesson
                    {
                        lessonId = (int)lesson.LESSON_ID,
                        lessonName = lesson.LESSON_NAME,
                        lessonImageFile = lesson.LESSON_IMAGE_FILE,
                        lessonSortOrderNumber = lesson.LESSON_SORT_ORDER,
                        lessonDescription = lesson.LESSON_DESCRIPTION,
                        lessonStartUrl = lessonStartUrl
                    };


                    lessonAvailable = true;



                    if (!introComplete && lesson.LESSON_SORT_ORDER > 0)
                    {
                        lessonAvailable = false;
                    }

                    if (introComplete && lesson.LESSON_SORT_ORDER == 1 && !basicLessonUser)
                    {
                        lessonAvailable = false;
                    }



                    if (introComplete && lesson.LESSON_SORT_ORDER >= 2)
                    {
                        if (!basicLessonUser && lesson.LESSON_SORT_ORDER < ConstantVars.ADVANCED_LESSSON_NUM_START)
                        {
                            lessonAvailable = false;
                        }
                        else if (!advancedLessonUser && lesson.LESSON_SORT_ORDER >= ConstantVars.ADVANCED_LESSSON_NUM_START)
                        {
                            lessonAvailable = false;
                        }
                        else
                        {
                            if (advancedLessonUser && lesson.LESSON_SORT_ORDER == 13)
                            {

                            }
                            else
                            {
                                int previousLessonId = (int)Repository.GetLessonId(moduleId, lesson.LESSON_SORT_ORDER - 1);


                                spGetQuizComplete_Result previousQuiz = Repository.GetQuizComplete(moduleId, previousLessonId, userId);
                                if (previousQuiz == null || previousQuiz.USER_ID == 0)
                                {
                                    lessonAvailable = false;
                                }
                            }
                        }

                    }



                    if (lessonAvailable)
                    {
                        lessonStartUrl = "LessonFlow?moduleId=" + moduleId + "&lessonId=" + lesson.LESSON_ID + "&sortId=1";
                    }
                    else
                    {
                        if (!basicLessonUser && lesson.LESSON_SORT_ORDER < ConstantVars.ADVANCED_LESSSON_NUM_START)
                        {
                            lessonStartUrl = "#lesson_" + lesson.LESSON_ID;
                            l.lessonStartUrlClass = "basicPrompt";
                        }
                        else if (!advancedLessonUser && lesson.LESSON_SORT_ORDER >= ConstantVars.ADVANCED_LESSSON_NUM_START)
                        {
                            lessonStartUrl = "#lesson_" + lesson.LESSON_ID;
                            l.lessonStartUrlClass = "advancedPrompt";
                        }
                        else
                        {
                            if ((basicLessonUser && lesson.LESSON_SORT_ORDER == 1) || (!basicLessonUser && advancedLessonUser && lesson.LESSON_SORT_ORDER == 13))
                            {
                                lessonStartUrl = "javascript:alert('Please click and watch the Introduction to start');";
                            }
                            else
                            {

                                lessonStartUrl = "javascript:alert('You must successfully complete the previous lesson before you can begin this one.');";

                            }
                        }
                    }

                    l.lessonStartUrl = lessonStartUrl;
                    l.isAvailable = lessonAvailable;


                    spGetLessonActivity_Result activity = Repository.GetLessonActivity((int)lesson.LESSON_ID);
                    if (activity != null)
                    {
                        l.lessonActivity = new LessonActivity { activityTitle = activity.ACTIVITY_TITLE, activityText = activity.ACTIVITY_CONTENT };
                    }
                    else
                    {
                        l.lessonActivity = new LessonActivity { activityTitle = "No activities for this lesson", activityText = "" };
                    }

                    double score = 0;
                    l.lessonQuizPass = false;
                    l.lessonQuizComplete = false;

                    var lessonScoreResult = Repository.GetQuizScore(moduleId, (int)lesson.LESSON_ID, userId);
                    if (lessonScoreResult != null && lessonScoreResult.num_questions > 0)
                    {
                        l.lessonQuizComplete = true;

                        score = Convert.ToDouble(lessonScoreResult.score) / Convert.ToDouble(lessonScoreResult.num_questions);
                        score = score * 100;

                        l.lessonQuizPass = (score >= 80);
                        l.lessonQuizScore = Math.Round(score, 1);
                    }

                    var textQuestionGroup = new QuestionGroup { questionList = new List<Question>() };
                    var questions = new List<Question>();

                    var textQuestions = Repository.GetTextQuestions(moduleId, (int)lesson.LESSON_ID);
                    if (textQuestions != null)
                    {
                        foreach (spGetTextQuestions_Result textQuestion in textQuestions)
                        {
                            string response = Repository.GetUserResponse(moduleId, (int)lesson.LESSON_ID, textQuestion.QUESTION_ID, userId);

                            questions.Add(new Question
                            {
                                questionId = textQuestion.QUESTION_ID,
                                questionText = textQuestion.QUESTION_TEXT,
                                questionResponse = response,
                                questionType = (int)QuestionTypes.TextArea
                            });

                        }
                    }


                    var radioNonQuizQuestions = Repository.spGetRadioNonQuizQuestions(moduleId, (int)lesson.LESSON_ID);
                    if (radioNonQuizQuestions != null)
                    {
                        foreach (spGetRadioNonQuizQuestions_Result radioQuestion in radioNonQuizQuestions)
                        {
                            var res = Repository.GetUserResponse(moduleId, (int)lesson.LESSON_ID, radioQuestion.QUESTION_ID, userId);

                            if (res != "")
                            {

                                int respnseOptionId = Convert.ToInt32(res);
                                var options = Repository.GetQuestionOptions(radioQuestion.QUESTION_ID);
                                var responseOption = options.First(o => o.QUESTION_OPTION_ID == respnseOptionId);
                                var responseString = responseOption.QUESTION_OPTION_TEXT;


                                questions.Add(new Question
                                {
                                    questionId = radioQuestion.QUESTION_ID,
                                    questionText = radioQuestion.QUESTION_TEXT,
                                    questionResponse = responseString,
                                    questionType = (int)QuestionTypes.RadioNonQiuz
                                });
                            }

                        }
                    }






                    textQuestionGroup.questionList = questions;




                    l.questionGroup = textQuestionGroup;


                    // on pace

                    l.onPace = true;
                    if (lesson.LESSON_SORT_ORDER > 0)
                    {
                        l.onPace = this.isUserOnPace(l.lessonQuizComplete, l.lessonQuizPass, lesson.LESSON_SORT_ORDER, userId);
                    }
                    else
                    {
                        if (introComplete)
                        {
                            l.onPace = false;
                        }
                    }

                    model.lessons.Add(l);

                }


                model.showProgress = true;
                model.showBasicCertificateLink = false;
                model.showAdvancedCertificateLink = false;
                model.basicOverallCourseScore = -1;
                model.advancedOverallCourseScore = -1;

                if (!introComplete)
                {
                    model.nextLessonSortNum = 0;
                    model.nextLessonId = introLessonId;
                    model.nextLessonLabel = "Introduction";
                }
                else
                {
                    spGetLesson_Result lastLessonComplete = Repository.GetLesson(moduleId, lastLesson.LESSON_ID);
                    if (lastLessonComplete == null)
                    {
                        model.nextLessonSortNum = basicLessonUser ? 1 : 13;
                        model.nextLessonId = (int)Repository.GetLessonId(moduleId, (int)model.nextLessonSortNum);
                        model.nextLessonLabel = "Lesson " + model.nextLessonSortNum;
                    }
                    else if ((basicLessonUser && lastLessonComplete.LESSON_SORT_ORDER == 12) || (advancedLessonUser && lastLessonComplete.LESSON_SORT_ORDER == 14))
                    {
                        model.nextLessonSortNum = null;
                        model.showProgress = false;
                        model.nextLessonLabel = "";
                    }
                    else
                    {
                        model.nextLessonSortNum = (int)lastLessonComplete.LESSON_SORT_ORDER + 1;
                        model.nextLessonId = (int)Repository.GetLessonId(moduleId, (int)model.nextLessonSortNum);
                        model.nextLessonLabel = "Lesson " + model.nextLessonSortNum;
                    }



                    if (basicLessonUser && lastLessonComplete.LESSON_SORT_ORDER == 12)
                    {
                        model.basicOverallCourseScore = Repository.GetCoursePassedScore(userId, false);

                        if (model.basicOverallCourseScore >= 80)
                        {
                            model.showBasicCertificateLink = true;
                            model.ceUnitUrl = "";
                            if (user.EMAIL != null && user.EMAIL != "")
                            {
                                DateTime now = DateTime.Now;
                                //string dateStr = "";
                                string dateStr = (now.Month < 10 ? "0" + now.Month.ToString() : now.Month.ToString()) +
                                                 (now.Day < 10 ? "0" + now.Day.ToString() : now.Day.ToString()) +
                                                 (now.Year - 2000).ToString();

                                model.ceUnitUrl = "https://www.ceunits.com/myoutcomes/?uid=" + user.ID.ToString() + "&email=" + user.EMAIL + "&date=" + dateStr;
                            }

                        }

                    }

                    if (advancedLessonUser && lastLessonComplete.LESSON_SORT_ORDER == 14)
                    {
                        model.advancedOverallCourseScore = Repository.GetCoursePassedScore(userId, true);

                        if (model.advancedOverallCourseScore >= 80)
                        {
                            model.showAdvancedCertificateLink = true;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                int x = 1;
            }

            return model;
        }

        private bool isUserOnPace(bool lessonQuizComplete, bool lessonQuizPass, int sortNumber, int userId)
        {


            if (lessonQuizPass)
            {
                return false;
            }
            

            if (lessonQuizComplete)
            {
                return true;
            }

            DateTime? firstLogin = Repository.GetFirstLoginDateTime(userId);
            if (firstLogin == null)
            {
                return false;
            }

            DateTime today = DateTime.Now;

            int daysOffset = sortNumber * ConstantVars.ONPACEPERIODDAYS; // each lesson to be complete within 14 days

            DateTime expectedCompletionDate = firstLogin.Value.AddDays(daysOffset);
            if (DateTime.Compare(today, expectedCompletionDate) <= 0)
            {
                return true;
            }

            return false;



        }


        [Authorize]
        public ActionResult ProgressBar()
        {
            return PartialView("~/Views/Shared/_Progress.cshtml");
        }


        [Authorize]
        public ActionResult LessonActivity(int moduleId, int lessonId)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }
            
            var lessonActivity = new LessonActivity();
            lessonActivity.moduleId = moduleId;
            lessonActivity.lessonId = lessonId;
            lessonActivity.userId = user.ID;

            spGetLesson_Result lesson = Repository.GetLesson(moduleId, lessonId);
            if (lesson == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activity = Repository.GetLessonActivity(lessonId);
            if (activity != null)
            {
                lessonActivity.activityTitle = activity.ACTIVITY_TITLE;
                lessonActivity.activityText = activity.ACTIVITY_CONTENT;
                lessonActivity.lessonName = activity.LESSON_NAME;
                lessonActivity.lessonImageFile = activity.LESSON_IMAGE_FILE;
                lessonActivity.lessonSortNum = (int)lesson.LESSON_SORT_ORDER;
            }
            else
            {
                lessonActivity.activityTitle = "No Activity for this Lesson";
                lessonActivity.activityText = "";
            }

            return View(lessonActivity);
        }

        

        private string Decrypt(string encryptedText)
        {
            string key = "89!ufAxkdfj#";
            byte[] DecryptKey = { };
            byte[] IV = { 56, 44, 77, 94, 82, 125, 40, 12 };
            byte[] inputByte = new byte[encryptedText.Length];

            DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByte = Convert.FromBase64String(encryptedText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        public ActionResult ManageUsers(int showInactives = 0)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            var model = new UserModel { id = user.ID, username = user.LOGIN_USER_ID, email = user.EMAIL, name = user.NAME };

            var res = Repository.GetAllUsers(showInactives);

            List<CreateUserModel> Users = new List<CreateUserModel>();      

            if (res.Any())
            {
                foreach (USER u in res)
                {
                    CreateUserModel um = new CreateUserModel();
                    um.Id = u.ID;
                    um.LoginUserId = u.LOGIN_USER_ID;
                    um.Name = u.NAME;
                    if(u.EMAIL != null)
                    {
                        um.Email = u.EMAIL;
                    }
                    else
                    {
                        um.Email = "";
                    }
                    if (u.COMPANY_NAME != null)
                    {
                        um.CompanyName = u.COMPANY_NAME;
                    }
                    else
                    {
                        um.CompanyName = "";
                    }
                    if (u.FIT_NUMBER != null)
                    {
                        um.FitNumber = u.FIT_NUMBER;
                    }
                    else
                    {
                        um.FitNumber = "";
                    }                    
                    um.CreateDate = u.CREATE_DATETIME;

                    um.BasicModule = u.MODULE1_BASIC;
                    um.AdvancedModule = u.MODULE1_ADVANCED;// ? u.MODULE1_ADVANCED : (byte)0;
                    um.UserStatusId = u.USER_STATUS_CODE.HasValue ? u.USER_STATUS_CODE.Value : (short)-1;

                    Users.Add(um);
                }
            }

            model.Users = Users;

            return View(model);
        }

        public ActionResult CreateUserView()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER user = Repository.GetUser(ui.userId, ui.userStatusId);
            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            //var model = new UserModel { id = user.ID, username = user.LOGIN_USER_ID, email = user.EMAIL, name = user.NAME };

            return View();
        }

        public ActionResult CreateUser(CreateUserModel um)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }

            try
            {
                var mail = new System.Net.Mail.MailAddress(um.Email);
            }
            catch
            {
                um.Success = false;
                um.ErrorMsg = "Please enter a valid email address.";
                return Json(um, JsonRequestBehavior.AllowGet);
            }

            bool exists = Repository.EmailUsernameExistElsewhere(um.LoginUserId, um.Email, 0);

            if (exists)
            {
                um.Success = false;
                um.ErrorMsg = "The Login User ID or Email address is already in use by another user. Please try another.";
            }
            else
            {
                um.CreateIP = this.Request.ServerVariables["REMOTE_ADDR"];
                um.CreateUserID = user.ID;
                um.CreateDate = System.DateTime.Now;
                um.Password = System.Web.Security.Membership.GeneratePassword(8, 1);
                um.UserStatusId = 0;

                um.Success = Repository.CreateUser2(um);
                if (!um.Success)
                {
                    um.ErrorMsg = "An error occurred while creating a new user. Please try again";
                }
                else
                {
                    Repository.VerifyAndSendNewPassword(um.Email, um.Password);
                    
                    Fit_Elearning.MyOutcomes.Domain.Helpers.EmailProcessor ep = new EmailProcessor();
                    string er = ep.SendNewUserEmail(um.LoginUserId, um.Email, um.Password, um.BasicModule == (byte)1, um.AdvancedModule == (byte)1);
                    if (er != "ok")
                    {
                        um.ErrorMsg = er;                        
                    }
                }
            }            

            return Json(um, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUserView()
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER currentUser = Repository.GetUser(ui.userId, ui.userStatusId);
            if (currentUser == null)
            {
                return new RedirectResult("/Home/Login");
            }
            var userId = Request.QueryString["userid"].ToString();
            USER user = Repository.GetUser(userId, 0);

            var model = new CreateUserModel();
            model.Id = user.ID;
            model.LoginUserId = user.LOGIN_USER_ID;
            model.Name = user.NAME;
            model.Email = user.EMAIL;
            model.CompanyName = user.COMPANY_NAME;
            model.FitNumber = user.FIT_NUMBER;
            model.BasicModule = user.MODULE1_BASIC;
            model.AdvancedModule = user.MODULE1_ADVANCED; //.HasValue ? user.MODULE1_ADVANCED.Value : (byte)0;

            return View(model);
        }

        public ActionResult UpdateUser(CreateUserModel um)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);
            USER user = Repository.GetUser(ui.userId, ui.userStatusId);

            if (user == null)
            {
                return new RedirectResult("/Home/Login");
            }
            
            bool exists = Repository.EmailUsernameExistElsewhere(um.LoginUserId, um.Email, um.Id);

            if (exists)
            {
                um.Success = false;
                um.ErrorMsg = "The Login User ID or Email address is already in use by another user. Please try another.";
            }
            else
            {
                um.UpdateIP = this.Request.ServerVariables["REMOTE_ADDR"];
                um.UpdateUserID = user.ID;
                um.UpdateDate = System.DateTime.Now;                

                um.Success = Repository.UpdateUser2(um);
                if (!um.Success)
                {
                    um.ErrorMsg = "An error occurred while updating a user. Please try again";
                }
            }

            return Json(um, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeactivateUser(CreateUserModel um)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER currentUser = Repository.GetUser(ui.userId, ui.userStatusId);
            if (currentUser == null)
            {
                return new RedirectResult("/Home/Login");
            }

            bool res = Repository.DeactivateUser(um.Id, um.LoginUserId);
            if (res)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }            
        }

        public ActionResult ReactivateUser(CreateUserModel um)
        {
            var ui = UtilitiesHelper.GetUserInfo(Request.Cookies["ExtServerId"].Value);

            if (ui.userStatusId != ConstantVars.USERSTATUS_ORIGN_LOCAL)
            {
                return new RedirectResult("/Home/Index");
            }

            USER currentUser = Repository.GetUser(ui.userId, ui.userStatusId);
            if (currentUser == null)
            {
                return new RedirectResult("/Home/Login");
            }

            bool res = Repository.ReactivateUser(um.Id, um.LoginUserId);
            if (res)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
    }
           
}
