using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
   public interface IBomBL
    {
        Bom InsertBom(Bom bom);
        Bom UpdateBom(Bom bom);
        IEnumerable<Bom> GetBom();
        bool DeleteBom(int bomId);
        Bom GetBom(int Id);
        IQueryable<Bom> QueryBom();
    }
}
