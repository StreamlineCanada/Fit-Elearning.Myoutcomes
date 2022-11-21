
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FitElearning.ScheduledEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchandSendEmail();
        }

        public static void SearchandSendEmail()
        {

            //rep.
            EmailProcessor ep = new EmailProcessor();
            fit_elearningEntities1 ent = new fit_elearningEntities1();
            var usersBehind = ent.spGetUsersBehind(1);
            //var usersBehind =rep.Users.ToList();
            //var usersBehind = rep.GetUsersBehind(1);
            foreach (var user in usersBehind)
            {
                try
                {
                    ep.SendEmail(user.EMAIL, GetFallBehindSubject(), String.Format(GetFallBehindMail(),user.NAME),"noreply@myoutcomes.com",true);
                    ep.SendEmail("chrisstocker@myoutcomes.com", "FIT elearn", user.NAME);
                }
                catch { };
            }
            //ep.SendEmail();
        }

        public static string GetFallBehindMail()
        {
            var s = "<img src='https://fit-elearning.myoutcomes.com/Images/red.jpg' alt='Falling behind'></img><div style='font-size:9px; font-weight:bold; color:red;'>falling<br/>behind</div>";
            s = s + "<br/>Dear {0},<br/>";
            s = s + "<br/>This is a friendly reminder that your FIT eLearning lessons are falling behind, and we want to be sure that you are successful in completing them. ";
            s = s + "If there is anything we can do to assist you in getting started again, just let us know; otherwise, your lessons are waiting for you and are just a ";
            s = s + "<a href='https://fit-elearning.myoutcomes.com'>CLICK AWAY!</a><br/>";
            s = s + "<br/>FIT eLearning Support Team";
            return s;
        }

        public static string GetFallBehindSubject()
        {
            var s = "Falling Behind";
            return s;
        }
    }

    public class EmailProcessor
    {


        public EmailProcessor()
        {
        }

        public void SendEmail(string to, string subject, string message, string from = "noreply@myoutcomes.com", bool isHtml = false)
        {
            using (var smtpClient = new SmtpClient())
            {


                MailAddress mailFrom = new MailAddress(from);


                MailMessage mailMessage = new MailMessage();                // Body
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.From = mailFrom;
                mailMessage.IsBodyHtml = isHtml;

                mailMessage.BodyEncoding = Encoding.ASCII;


                smtpClient.Send(mailMessage);

            }
        }
    }
}
