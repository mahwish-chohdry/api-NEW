using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer
{
    public interface IFormBL
    {
        IEnumerable<Form> GetForms();
        bool DeleteForm(int formId);
        Form GetForm(int Id);
        IQueryable<Form> QueryForm();
        Form InsertForm(Form form);
        Form UpdateForm(Form form);
    }
}
