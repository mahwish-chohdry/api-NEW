using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IEmailBL
    {
        Email InsertEmail(Email Email);
        Email UpdateEmail(Email Email);
        IEnumerable<Email> GetEmail();
        bool DeleteEmail(int EmailId);
        Email GetEmail(int Id);
        IQueryable<Email> QueryEmail();
    }
}
