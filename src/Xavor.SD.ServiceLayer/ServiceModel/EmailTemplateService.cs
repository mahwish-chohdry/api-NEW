using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEmailTemplateBL _EmailTemplateBL;

        public EmailTemplateService(IEmailTemplateBL EmailTemplateBL)
        {
            _EmailTemplateBL = EmailTemplateBL;
        }
        public IEnumerable<EmailTemplate> GetEmailTemplate()
        {
            return _EmailTemplateBL.GetEmailTemplate();
        }

        public EmailTemplate GetEmailTemplateByCode(string code, string lang)
        {
            return _EmailTemplateBL.GetEmailTemplateByCode( code,  lang);
        }

        public EmailTemplate GetEmailTemplate(int Id)
        {
            return _EmailTemplateBL.GetEmailTemplate(Id);
        }
    }
}
