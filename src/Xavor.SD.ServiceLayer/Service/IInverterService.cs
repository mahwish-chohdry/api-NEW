using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.ServiceLayer.Service
{
    public interface IInverterService
    {
        Inverter GetInverter(string InverterId);

        Inverter GetInverter(int InverterId);

        List<InverterDTO> GetInverterList(string lang);
    }
}
