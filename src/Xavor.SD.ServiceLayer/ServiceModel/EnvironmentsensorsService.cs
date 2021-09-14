using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.BusinessLayer;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.ServiceLayer.Service;
using Xavor.SD.ServiceLayer.Transformations;

namespace Xavor.SD.ServiceLayer.ServiceModel
{
    public class EnvironmentsensorsService : IEnvironmentsensorsService
    {


        private IEnvironmentsensorsBL _environmentBL;
        private IDeviceAlarmsHistoryBL _deviceAlarmsHistoryBL;
        private IDeviceBL _deviceBL;
        private ITransformations _transformation;


        public EnvironmentsensorsService(IEnvironmentsensorsBL environmentBL, IDeviceAlarmsHistoryBL deviceAlarmsHistoryBL, IDeviceBL deviceBL, ITransformations transformation)
        {
            _environmentBL = environmentBL;
            _deviceAlarmsHistoryBL = deviceAlarmsHistoryBL;
            _deviceBL = deviceBL;
            _transformation = transformation;
        }

        public List<EnvironmentsensorsDTO> GetAllSensors()
        {
            IEnumerable<Environmentsensors> environment = _environmentBL.GetEnvironmentsensors();
            List<EnvironmentsensorsDTO> environDtos = new List<EnvironmentsensorsDTO>();

            foreach (var environ in environment)
            {
                EnvironmentsensorsDTO dto = new EnvironmentsensorsDTO();
                dto.Id = environ.Id;
                dto.Type = environ.Type;
                dto.Min = environ.Min;
                dto.Max = environ.Max;

                dto.Unit = environ.Unit;
                environDtos.Add(dto);
            }
            return environDtos;
        }





    }
}
