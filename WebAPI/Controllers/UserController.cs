using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using WebAPI.DBModels;
using WebAPI.Controllers;
using System.Diagnostics;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        FIT_ElearnDBDataContext dbObject = new FIT_ElearnDBDataContext();
        
        public UserController()
        {

        }

        //public List<FitUser> GetStandaloneUsers()
        //{
        //    List<FitUser> fitUserList = new List<FitUser>();
        //    try {
        //        List<USER> userRes = dbObject.USERs.Where(u => u.USER_STATUS_CODE == 0 && !u.EMAIL.Contains("myoutcomes") && !u.LOGIN_USER_ID.Contains("demo")).OrderBy(u => u.COMPANY_NAME).ToList();

        //        fitUserList = userRes.Select(u => new FitUser { id = u.ID, contactName = u.NAME, loginUserId = u.LOGIN_USER_ID, createDate = u.CREATE_DATETIME, fitNumber = u.FIT_NUMBER, companyName = u.COMPANY_NAME, email = u.EMAIL, statusId = (int)u.USER_STATUS_CODE.Value }).ToList();
        //    }catch(Exception e)
        //    {
                
        //    }
           

        //    return fitUserList;
        //}

        [HttpGet]
        public List<FitElearnUserDetail> GetUserMostRecentDetails(string userID, int serverId)
        {
            List<FitElearnUserDetail> mostRecentDetailsList = new List<FitElearnUserDetail>();
            try
            {
                List<string> usernameList = userID.Split(',').ToList();

                foreach (string username in usernameList)
                {


                    var x = dbObject.SpGetMostRecentDetails(username,serverId).FirstOrDefault();
                    if (x != null)
                    {
                        FitElearnUserDetail userDetails = new FitElearnUserDetail();
                        userDetails.mostRecentLesson = x.LESSON_SORT_ORDER.ToString();
                        userDetails.mostRecentScore = x.USER_QUIZ_SCORE;
                        userDetails.mostRecentLessonDate = x.USER_QUIZ_COMPLETE_DATETIME.ToShortDateString();
                        userDetails.loginUserId = username;
                        mostRecentDetailsList.Add(userDetails);

                    }
                }
            }
            catch (Exception e)
            {

            }


            return mostRecentDetailsList;
        }
    }
}