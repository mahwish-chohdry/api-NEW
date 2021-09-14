using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public interface IEmailTemplateService
    {
        IEnumerable<EmailTemplate> GetEmailTemplate();
        EmailTemplate GetEmailTemplateByCode(string code, string lang);
        EmailTemplate GetEmailTemplate(int Id);
    }
}
