using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public class EmailService : IEmailService
    {
        private readonly IEmailBL _emailBL;

        public EmailService(IEmailBL emailBL)
        {
            _emailBL = emailBL;
        }
        public void SaveEmail(Email email)
        {

            _emailBL.InsertEmail(email);
        }

    }
}
