using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xavor.SD.Common.ViewContracts;

namespace Xavor.SD.WebAPI.Helper
{
    public interface IAccountCreation
    {
        string GetRandomPassword(int length);
        string HashPassword(string password);
        Task<string> SendActivationEmail(string email, string password, string firstName, string lastName, string lang);
        Task<string> SendForgottenPasswordEmail(string username, string email, string password, string lang);
        Task<string> SendFeedbackEmails(EmailDTO emailDTO, List<string> emailRecipients);
    }
}
