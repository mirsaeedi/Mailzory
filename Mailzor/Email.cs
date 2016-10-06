using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Mailzor
{
    public class Email:IDisposable
    {
        #region Fields

        protected string _razorTemplate;
        private SmtpClient _smtpClient;

        #endregion

        #region Properties

        public dynamic ViewBag { get; set; }

        #endregion

        #region Constructors

        public Email(string razorTemplate,SmtpClient smtpClient=null)
        {
            _razorTemplate = razorTemplate;
            ViewBag = new ExpandoObject();
            _smtpClient = smtpClient ?? new SmtpClient();
        }

        #endregion

        #region Interface

        public void SendAsync(string[] toMails, string subject,object userToken)
        {
            var email = ConfigureEmail(toMails, subject);
            _smtpClient.SendAsync(email,userToken);
        }

        public void Send(string[] toMails, string subject)
        {
            var email = ConfigureEmail(toMails,subject);
            _smtpClient.Send(email);
        }

        public void Send(string toMail, string subject)
        {
            Send(new string[] { toMail }, subject);
        }

        public void SendAsync(string toMail, string subject, object userToken)
        {
            SendAsync(new string[] { toMail }, subject, userToken);
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        #endregion

        #region Privates

        private MailMessage ConfigureEmail(string[] toMails, string subject)
        {
            var emailHtmlBody = GenerateMailBody();

            var email = new MailMessage()
            {
                Body = emailHtmlBody,
                IsBodyHtml = true,
                Subject = subject
            };

            foreach (var toMail in toMails)
                email.To.Add(new MailAddress(toMail));

            return email;
        }

        protected virtual string GenerateMailBody()
        {
            var razorEngineService = RazorEngineService.Create();

            var dynamicViewBag = new DynamicViewBag((IDictionary<string, object>)ViewBag);
            var emailHtmlBody = razorEngineService
                .RunCompile(_razorTemplate, "template", viewBag: dynamicViewBag);

            return emailHtmlBody;
        }

        #endregion

    }

    public class Email<T>: Email where T :class
    {
        #region Properties

        private T _model = null;

        #endregion

        #region Constructors

        public Email(string razorTemplate, T Model, SmtpClient smtpClient=null)
            : base(razorTemplate, smtpClient)
        {
            _model = Model;
        }

        #endregion

        #region Privates

        protected override string GenerateMailBody()
        {
            var razorEngineService = RazorEngineService.Create();

            var dynamicViewBag = new DynamicViewBag((IDictionary<string, object>)ViewBag);
            var emailHtmlBody = razorEngineService
                .RunCompile(_razorTemplate, "template", typeof(T), _model, dynamicViewBag);

            return emailHtmlBody;
        }

        #endregion

    }
}
