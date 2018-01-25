using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Mailzory
{
    public class Email:IDisposable
    {
        #region Fields

        protected readonly string _razorTemplate;
        private readonly SmtpClient _smtpClient;

        #endregion

        #region Properties

        public dynamic ViewBag { get; set; }

        public List<Attachment> Attachments { get; } = new List<Attachment>();
        public List<LinkedResource> LinkedResources { get; } = new List<LinkedResource>();

        private string FromMailAddress { get; set; }
        private string FromDisplayName { get; set; }

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

        public Task SendAsync(string[] toMailAddresses, string subject, string[] ccMailAddresses=null, string[] bccMailAddresses = null)
        {
            var to = toMailAddresses.Select(item => new MailAddress(item)).ToArray();

            var cc = ccMailAddresses == null
                ? null
                : ccMailAddresses.Select(item => new MailAddress(item)).ToArray();

            var bcc = bccMailAddresses == null
                ? null
                : bccMailAddresses.Select(item => new MailAddress(item)).ToArray();

            return SendAsync(to, subject, cc, bcc);
        }

        public async Task SendAsync(MailAddress[] toMailAddresses, string subject, MailAddress[] ccMailAddresses = null, MailAddress[] bccMailAddresses = null)
        {
            var email = ConfigureEmail(toMailAddresses, subject, ccMailAddresses, bccMailAddresses);
            await _smtpClient.SendMailAsync(email).ConfigureAwait(false);
        }

        public void Send(string[] toMailAddresses, string subject, string[] ccMailAddresses = null, string[] bccMailAddresses = null)
        {
            var to = toMailAddresses.Select(item => new MailAddress(item)).ToArray();

            var cc = ccMailAddresses ==null
                ?null
                : ccMailAddresses.Select(item => new MailAddress(item)).ToArray();

            var bcc = bccMailAddresses == null
                ? null
                : bccMailAddresses.Select(item => new MailAddress(item)).ToArray();

            Send(to, subject, cc, bcc);
        }

        public void Send(MailAddress[] toMailAddresses, string subject, MailAddress[] ccMailAddresses = null, MailAddress[] bccMailAddresses = null)
        {
            var email = ConfigureEmail(toMailAddresses, subject, ccMailAddresses, bccMailAddresses);
            _smtpClient.Send(email);
        }

        public void Send(string toMailAddresses, string subject, string[] ccMailAddresses = null, string[] bccMailAddresses = null)
        {
            Send(new[] { toMailAddresses }, subject,ccMailAddresses,bccMailAddresses);
        }

        public Task SendAsync(string toMailAddresses, string subject, string[] ccMailAddresses = null, string[] bccMailAddresses = null)
        {
            return SendAsync(new[] { toMailAddresses }, subject, ccMailAddresses, bccMailAddresses);
        }

        public void SetFrom(string mailAddress, string displayName = null)
        {
            FromMailAddress = mailAddress;
            FromDisplayName = displayName;
        }

        public LinkedResource AddImageResource(Stream stream, string contentId, string contentType)
        {
            return AddImageResource(stream, contentId, new ContentType(contentType));
        }

        public LinkedResource AddImageResource(Stream stream, string contentId, ContentType contentType)
        {
            var imageResource = new LinkedResource(stream, MediaTypeNames.Image.Jpeg)
            {
                ContentId = contentId,
                ContentType = contentType
            };

            LinkedResources.Add(imageResource);

            return imageResource;
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        #endregion

        #region Privates

        private MailMessage ConfigureEmail(MailAddress[] toMails, string subject, MailAddress[] ccMails, MailAddress[] bccMails)
        {
            var emailHtmlBody = GenerateMailBody();

            var email = new MailMessage
            {
                Body = emailHtmlBody,
                IsBodyHtml = true,
                Subject = subject,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            foreach (var attachment in Attachments)
            {
                email.Attachments.Add(attachment);
            }

            var view = AlternateView.CreateAlternateViewFromString(emailHtmlBody, null, MediaTypeNames.Text.Html);
            LinkedResources?.ForEach(res => view.LinkedResources.Add(res));
            email.AlternateViews.Add(view);

            foreach (var toMail in toMails ?? Enumerable.Empty<MailAddress>())
            {
                email.To.Add(toMail);
            }

            foreach (var ccMail in ccMails ?? Enumerable.Empty<MailAddress>())
            {
                email.CC.Add(ccMail);
            }

            foreach (var bccMail in bccMails ?? Enumerable.Empty<MailAddress>())
            {
                email.Bcc.Add(bccMail);
            }

            if (FromMailAddress != null)
            {
                email.From = new MailAddress(FromMailAddress, FromDisplayName);
            }

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

        private readonly T _model;

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
