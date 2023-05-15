using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace api1.Service
{
    public class MailService
    {
        private string gmail_account = "s1410931048@gms.nutc.edu.tw";
        private string gmail_password = "gexarlffubzkdyzr";
        private string gmail_mail = "s1410931048@gms.nutc.edu.tw";

        #region 產生驗證碼
        public string GetValidateCode()
        {
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I","J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" ,
                            "a", "b", "c", "d", "e", "f", "g", "h", "i","j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" ,
                            "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            string VaildateCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < 10; i++)
            {
                VaildateCode += Code[rd.Next(Code.Count())];
            }
            return VaildateCode;
        }
        public string GetRegisterMailBody(string TempString, string UserName, string ValidateUrl)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            return TempString;
        }
        //寄驗證信的方法
        public void SendRegisterMail(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " HomeFinder註冊確認信 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion
        #region 忘記密碼
        public string ForgetMailBody(string TempString, string UserName, string NewPassword)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{NewPassword}}", NewPassword);
            return TempString;
        }
        public void ForgetSendRegisterMail(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " 忘記密碼 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion

        #region 預約看房信
        public string BookMailBody(string TempString, string UserName, DateOnly Date, string Time, string address,Guid? Id)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{Date}}", Date.ToString("yyyy/MM/dd"));
            TempString = TempString.Replace("{{Time}}", Time);
            TempString = TempString.Replace("{{Address}}", address);
            TempString = TempString.Replace("{{BookingId}}", Id.ToString());
            return TempString;
        }
        public void SentBookMail(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " 預約看房確認信 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion
        #region 審核失敗
        public string CheckBadMailBody(string TempString, string UserName, string Reason, string Title)
        {
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{Reason}}", Reason);
            TempString = TempString.Replace("{{Title}}", Title);
            return TempString;
        }
        public void SentCheckBadMail(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " 審核未通過通知 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion
        #region 取消看房
        public string CancelMailBody(string TempString, string publisher, DateOnly bookdate, string booktime, string title)
        {
            TempString = TempString.Replace("{{UserName}}", publisher);
            TempString = TempString.Replace("{{Date}}", bookdate.ToString());
            TempString = TempString.Replace("{{Time}}", booktime);
            TempString = TempString.Replace("{{Title}}", title);
            return TempString;
        }
        public string CancelMailBody(string TempString, string publisher, DateOnly bookdate, string booktime)
        {
            TempString = TempString.Replace("{{UserName}}", publisher);
            TempString = TempString.Replace("{{Date}}", bookdate.ToString());
            TempString = TempString.Replace("{{Time}}", booktime);
            return TempString;
        }
        public void SentCancelMail(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " 取消預約看房通知 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion

        #region 停權
        public string BlockMailBody(string TempString, string account)
        {
            TempString = TempString.Replace("{{UserName}}", account);
            return TempString;
        }
        public void SentBlockMailBody(string MailBody, string ToEmail)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            SmtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(gmail_mail);
            mail.To.Add(ToEmail);
            mail.Subject = " 帳戶停權通知 ";
            mail.Body = MailBody;
            mail.IsBodyHtml = true;
            SmtpServer.Send(mail);
        }
        #endregion

        #region 預約成功或失敗信
        
        #endregion
    }
}