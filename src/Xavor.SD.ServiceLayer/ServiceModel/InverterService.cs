using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Service;

namespace Xavor.SD.ServiceLayer.ServiceModel
{
    public class InverterService : IInverterService
    {
        public readonly IInverterBL _inverterBL ;

        public InverterService(IInverterBL inverterBL)
        {
            _inverterBL = inverterBL;
        }


        public Inverter GetInverter(string inverterId)
        {
            return _inverterBL.QueryInverter().Where(x=>x.InverterId == inverterId).FirstOrDefault();
        }

        public Inverter GetInverter(int inverterId)
        {
            return _inverterBL.QueryInverter().Where(x => x.Id == inverterId).FirstOrDefault();
        }

        public List<InverterDTO> GetInverterList(string lang)
        {
            var inverterDtoList = new List<InverterDTO>();
            var inverterList = _inverterBL.GetInverter();

            foreach (var inverterObj in inverterList)
            {
                var inverter = new InverterDTO();
                inverter.InverterId = inverterObj.InverterId;
                if(lang == "en")
                {
                    inverter.InverterName = inverterObj.InverterName;
                }
                else if(lang == "zh")
                {
                    inverter.InverterName = inverterObj.ZhInverterName;
                }
                
                inverter.IsActive = inverterObj.IsActive;
                inverterDtoList.Add(inverter);
            }
            return inverterDtoList;
        }
    }
}
