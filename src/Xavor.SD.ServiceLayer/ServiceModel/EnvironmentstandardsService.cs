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


    public class EnvironmentstandardsService : IEnvironmentstandardsService
    {
        private IEnvironmentstandardsBL _environmentBL;
        private IDeviceAlarmsHistoryBL _deviceAlarmsHistoryBL;
        private IDeviceBL _deviceBL;
        private ITransformations _transformation;


        public EnvironmentstandardsService(IEnvironmentstandardsBL environmentBL, IDeviceAlarmsHistoryBL deviceAlarmsHistoryBL, IDeviceBL deviceBL, ITransformations transformation)
        {
            _environmentBL = environmentBL;
            _deviceAlarmsHistoryBL = deviceAlarmsHistoryBL;
            _deviceBL = deviceBL;
            _transformation = transformation;
        }

        public List<EnvironmentstandardsDTO> GetAllStandards()
        {
            IEnumerable<Environmentstandards> environment = _environmentBL.GetEnvironmentstandards();
            List<EnvironmentstandardsDTO> environDtos = new List<EnvironmentstandardsDTO>();

            foreach (var environ in environment)
            {
                EnvironmentstandardsDTO dto = new EnvironmentstandardsDTO();
                dto.Id = environ.Id;
                dto.Type = environ.Type;
                dto.GoodMin = environ.GoodMin;
                dto.GoodMax = environ.GoodMax;
                dto.ModerateMin = environ.ModerateMin;
                dto.ModerateMax = environ.ModerateMax;
                dto.UnhealthyMin = environ.UnhealthyMin;
                dto.UnhealthyMax = environ.UnhealthyMax;
                dto.VeryUnhealthyMin = environ.VeryUnhealthyMin;
                dto.VeryUnhealthyMax = environ.VeryUnhealthyMax;
                dto.Unit = environ.Unit;
                environDtos.Add(dto);
            }
            return environDtos;
        }


      

    }
}
    
