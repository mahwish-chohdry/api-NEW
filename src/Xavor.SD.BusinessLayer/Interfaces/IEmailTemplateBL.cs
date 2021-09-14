using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IEmailTemplateBL
    {
        IEnumerable<EmailTemplate> GetEmailTemplate();
        EmailTemplate GetEmailTemplateByCode(string code, string lang);
        EmailTemplate GetEmailTemplate(int Id);
        IQueryable<EmailTemplate> QueryEmailTemplate();
    }
}
