using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;

namespace Xavor.SD.ServiceLayer.Service
{
    public interface IEnvironmentsensorsService
    {

      

            List<EnvironmentsensorsDTO> GetAllSensors();
        
    }
}
