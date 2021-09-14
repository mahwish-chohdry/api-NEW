using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer
{
    public interface IEmailService
    {
        void SaveEmail(Email email);
    }
}
