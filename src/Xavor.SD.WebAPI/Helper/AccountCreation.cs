using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer;

namespace Xavor.SD.WebAPI.Helper
{
    public class AccountCreation : IAccountCreation
    {
        private static Random random = new Random();
        private readonly IConfiguration configuration;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        public string ActivationLink;
        public string ConnectionString;
        readonly string ApiKey;
        readonly string FromEmailAddress;
        readonly string FromEmailDisplayName;
        SendGridClient client;
        public AccountCreation(IConfiguration confg, IEmailService emailService, IEmailTemplateService emailTemplateService)
        {
            configuration = confg;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            ApiKey = configuration.GetSection("Email").GetSection("ApiKey").Value;
            client = new SendGridClient(ApiKey);
            FromEmailAddress = configuration.GetSection("Email").GetSection("FromEmailAddress").Value;
            FromEmailDisplayName = configuration.GetSection("Email").GetSection("FromEmailDisplayName").Value;
            ConnectionString = configuration.GetConnectionString("AzureStorage");
        }
        public string GetRandomPassword(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<string> SendActivationEmail(string email, string password, string firstName, string lastName, string lang)
        {
            var response = "OK 200";
            try
            {
                var subject = "Activation Email";
                var body = "<h1>Welcome, Your account has been Created</h1><p>Your Account default Password is " + password + " </p><br/><p>Get Started</p>";
                var template = _emailTemplateService.GetEmailTemplateByCode("AccountActivation", lang);

                if (template != null)
                {
                    if (!string.IsNullOrEmpty(template.Subject))
                    {
                        subject = template.Subject;
                    }
                    if (!string.IsNullOrEmpty(template.Body))
                    {
                        body = template.Body.Replace("{firstname}", firstName).Replace("{lastname}", lastName).Replace("{username}", email).Replace("{password}", password);
                    }
                }
                Email emailToSend = new Email
                {
                    From = FromEmailAddress,
                    To = email,
                    Body = body,
                    Subject = subject,
                    CreatedDate = DateTime.UtcNow
                };
                _emailService.SaveEmail(emailToSend);

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(FromEmailAddress, FromEmailDisplayName),
                    Subject = subject,
                    HtmlContent = body
                };
                msg.AddTo(new EmailAddress(email, "User"));
                var result = await client.SendEmailAsync(msg);
                response = response + " & Status Code is " + result.StatusCode;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return response;
        }

        public async Task<string> SendForgottenPasswordEmail(string username, string email, string password, string lang)
        {
            var response = "OK 200";
            try
            {
                var subject = "Activation Email";
                var body = "<p>Hi " + username + ",<br/><br/> Can't remember your password? Don't worry about it. <br/><br/> <b> Your Account Password is : " + password + "</b> <br/><br/>  Didn't ask to recover your password? <br/> If you didn't ask for your password, it's likely that another user entered your username or email address by mistake while trying to reset their password. If that's the case, you don't need to take any further action and can safely disregard this email.</p>  <br/>";
                var template = _emailTemplateService.GetEmailTemplateByCode("ForgotPassword", lang);

                if (template != null)
                {
                    if (!string.IsNullOrEmpty(template.Subject))
                    {
                        subject = template.Subject;
                    }
                    if (!string.IsNullOrEmpty(template.Body))
                    {
                        body = template.Body.Replace("{username}", email).Replace("{password}", password);
                    }
                }
                Email emailToSend = new Email
                {
                    From = FromEmailAddress,
                    To = email,
                    Body = body,
                    Subject = subject,
                    CreatedDate = DateTime.UtcNow
                };
                _emailService.SaveEmail(emailToSend);

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(FromEmailAddress, FromEmailDisplayName),
                    Subject = subject,
                    HtmlContent = body
                };
                msg.AddTo(new EmailAddress(email, "User"));
                var result = await client.SendEmailAsync(msg);
                response = response + " & Status Code is " + result.StatusCode;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return response;
        }

        public async Task<string> SendFeedbackEmails(EmailDTO emailDTO, List<string> emailRecipients)
        {
            var response = new Response(System.Net.HttpStatusCode.OK, null, null);
            try
            {
                foreach (var recipient in emailRecipients)
                {
                    var msg = new SendGridMessage()
                    {
                        From = new EmailAddress(FromEmailAddress, FromEmailDisplayName),
                        Subject = emailDTO.subject,
                        HtmlContent = "<strong>" + emailDTO.body + "</strong>"
                    };
                    msg.AddTo(new EmailAddress(recipient, "User"));
                    response = await client.SendEmailAsync(msg);
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return "OK 200" + " & Status Code is " + response.StatusCode;
        }
    }
}
