using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;


namespace Fit_Elearning.MyOutcomes.Domain.Helpers
{
    

    public class EmailProcessor
    {


        public EmailProcessor()
        {
        }

        public void SendEmail(string to, string subject, string message)
        {
            var mimeMessage = new MimeMessage();


            using (var smtpClient = new SmtpClient())
            {
                string from = "noreply@myoutcomes.com";

                mimeMessage.From.Add(new MailboxAddress("MyOutcomes Fit-eLearning", from));

                mimeMessage.To.Add(new MailboxAddress(to, to));


                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(TextFormat.Plain)
                {
                    Text = message
                };


                smtpClient.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);

                //Remove any OAuth functionality as we won't be using it. 
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

                smtpClient.Authenticate("noreply@myoutcomes.com", "notarealpwd");

                smtpClient.Send(mimeMessage);

                smtpClient.Disconnect(true);



            }
        }

        public string SendPasswordEmail(string userId, string email, string pw)
        {
            string result = "ok";

            try
            {
                string subject = "Your MyOutcomes Fit E-Learning Account";

                if (email == string.Empty) email = "notarealuser@myoutcomes.com";

                string body = "You are receiving this email in response to a MyOutcomes FIT eLearning account request.\r\n\r\n";

                body += "Your User ID is: " + userId + "\r\n";
                body += "Your Password is: " + pw + "\r\n";

                body += "\r\nPlease login here: " + System.Configuration.ConfigurationManager.AppSettings["ApplicationAddress"] + " ";

                body += "\r\n\r\nIf you did not initiate this request please contact support@myoutcomes.com.\r\n\r\n";

                body += "Thank you for using MyOutcomes FIT e-Learning.";


                this.SendEmail(email, subject, body);

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }

           
        }

        public string SendNewUserEmail(string userId, string email, string pw, bool basicAccess, bool advancedAccess)
        {
            string result = "ok";

            try
            {
                string subject = "Welcome to MyOutcomes FIT e-Learning";

                if (!basicAccess && advancedAccess)
                {
                    subject = "Welcome to MyOutcomes Deliberate Practice";
                }
                else if (basicAccess && advancedAccess)
                {
                    subject += " and Deliberate Practice";
                }

                if (email == string.Empty) email = "notarealuser@myoutcomes.com";

                string body = subject + ".\r\n\r\n";

                body += "Your User ID is: " + userId + "\r\n";
                body += "Your Password is: " + pw + "\r\n";

                body += "\r\nPlease login here: " + System.Configuration.ConfigurationManager.AppSettings["ApplicationAddress"] + " ";

                body += "\r\n\r\nIf you did not initiate this request or have any questions please contact support@myoutcomes.com.\r\n\r\n";

                body += "Thank you.";


                this.SendEmail(email, subject, body);

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }


        }

       


    }
}
