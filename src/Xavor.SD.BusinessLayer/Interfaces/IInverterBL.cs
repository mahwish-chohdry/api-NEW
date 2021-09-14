using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
    public interface IInverterBL
    {
        Inverter InsertInverter(Inverter inverter);
        Inverter UpdateInverter(Inverter inverter);
        IEnumerable<Inverter> GetInverter();      
        bool DeleteInverter(int inverterId);
        Inverter GetInverter(int Id);
        IQueryable<Inverter> QueryInverter();
    }
}
