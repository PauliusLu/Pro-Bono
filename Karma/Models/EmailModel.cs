using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class EmailModel
    {
        public readonly string SendingEmail = "info@karma.com";

        public readonly string EmailSubject = "Karma";

        public class EmailCharityState : EmailModel
        {
            public new readonly string EmailSubject = "Charity state changed";
            public string UserName { get; set; }
            public string CharityName { get; set; }
            public Enums.ReviewState ReviewState { get; set; }
            public DateTime TimeChanged { get; set; }

            public EmailCharityState(string userName, string charityName, Enums.ReviewState reviewState, DateTime timeChanged)
            {
                UserName = userName;
                CharityName = charityName;
                ReviewState = reviewState;
                TimeChanged = timeChanged;
            }
        }

        public async Task SendEmail(IWebHostEnvironment iWebHostEnv, User user)
        {
            // The emails are currently saved to Karma/Data for testing purposes
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = Path.Combine(iWebHostEnv.ContentRootPath, "Data")
            });

            // Templates should be in DB in the future
            StringBuilder template = new();
            template.AppendLine("Dear @Model.UserName,");
            template.AppendLine("<p>Your charity's \"@Model.CharityName\" state has changed to: @Model.ReviewState.</p>");
            template.AppendLine("- The Karma Team");

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            var email = await Email
                .From(SendingEmail)
                .To(user.Email)
                .Subject(EmailSubject)
                .UsingTemplate(template.ToString(), this)
                .SendAsync();
        }
    }
}
