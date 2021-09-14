using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;

namespace Xavor.SD.BusinessLayer
{
    public class DevicestatushistoryBL : IDevicestatushistoryBL
    {
        private readonly IUnitOfWork _uow;
        private IRepository<Devicestatushistory> repo;

        public DevicestatushistoryBL(IUnitOfWork uow)
        {
            _uow = uow;
            repo = uow.GetRepository<Devicestatushistory>();
        }

        public Devicestatushistory InsertDevicestatushistory(Devicestatushistory Devicestatushistory)
        {
            try
            {
                repo.Add(Devicestatushistory);
                _uow.SaveChanges();

                return Devicestatushistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Devicestatushistory UpdateDevicestatushistory(Devicestatushistory Devicestatushistory)
        {
            try
            {
                repo.Update(Devicestatushistory);
                _uow.SaveChanges();
                return Devicestatushistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Devicestatushistory> QueryDevicestatushistory()
        {
            try
            {
                return repo.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void AddOrUpdateDevicestatushistory(int deviceId, SmartDeviceContainer SmartDevice, int totalSeconds)
        {
            var existingDevicestatusHistories = QueryDevicestatushistory().Where(x => x.DeviceId == deviceId).ToList();
            if (existingDevicestatusHistories != null && existingDevicestatusHistories.Count > 0)
            {
                var existingDevicestatushistory = existingDevicestatusHistories.OrderByDescending(x => x.ModifiedDate).FirstOrDefault();
                var modifiedDate = Convert.ToDateTime(existingDevicestatushistory.ModifiedDate);
                var currentDate = DateTime.UtcNow;

                if (currentDate.Date == modifiedDate.Date && currentDate.Date.Hour == modifiedDate.Date.Hour) //same day update case, adding hour to have some records
                {
                    existingDevicestatushistory.DeviceId = deviceId;
                    existingDevicestatushistory.PowerStatus = Convert.ToInt16(SmartDevice.PowerStatus);
                    existingDevicestatushistory.Speed = Convert.ToInt32(SmartDevice.Speed);

                    existingDevicestatushistory.Temp = Convert.ToDouble(SmartDevice.Temperature);
                    existingDevicestatushistory.Humidity = Convert.ToDouble(SmartDevice.Humidity);
                    existingDevicestatushistory.Pressure = Convert.ToInt32(SmartDevice.Pressure);
                    existingDevicestatushistory.Iaq = Convert.ToDouble(SmartDevice.Iaq);
                    existingDevicestatushistory.IaqAccuracy = Convert.ToInt32(SmartDevice.IaqAccuracy);
                    existingDevicestatushistory.StaticIaq = Convert.ToDouble(SmartDevice.StaticIaq);
                    existingDevicestatushistory.StaticIaqAccuracy = Convert.ToInt32(SmartDevice.StaticIaqAccuracy);
                    existingDevicestatushistory.Co2Concentration = Convert.ToDouble(SmartDevice.Co2Concentration);
                    existingDevicestatushistory.Co2ConcentrationAccuracy = Convert.ToInt32(SmartDevice.Co2ConcentrationAccuracy);
                    existingDevicestatushistory.VocConcentration = Convert.ToDouble(SmartDevice.VocConcentration);
                    existingDevicestatushistory.VocConcentrationAccuracy = Convert.ToInt32(SmartDevice.VocConcentrationAccuracy);
                    existingDevicestatushistory.GasPercentage = Convert.ToDouble(SmartDevice.GasPercentage);

                    existingDevicestatushistory.AutoTemp = Convert.ToInt16(SmartDevice.AutoTemp);
                    existingDevicestatushistory.AutoTimer = Convert.ToInt16(SmartDevice.AutoTimer);
                    existingDevicestatushistory.AutoStartTime = SmartDevice.AutoStartTime;
                    existingDevicestatushistory.AutoEndTime = SmartDevice.AutoEndTime;
                    existingDevicestatushistory.IdealTemp = SmartDevice.IdealTemp;
                    existingDevicestatushistory.MaintenanceHours = SmartDevice.MaintenanceHours;
                    existingDevicestatushistory.MaxTemp = SmartDevice.MaxTemp;
                    existingDevicestatushistory.MinTemp = SmartDevice.MinTemp;
                    if (totalSeconds != 0)
                    {
                        existingDevicestatushistory.RunningTime += totalSeconds;
                    }
                    existingDevicestatushistory.CommandType = "";
                    existingDevicestatushistory.ModifiedDate = DateTime.UtcNow;
                    existingDevicestatushistory = TransformDeviceStatusHistory(existingDevicestatushistory);
                    UpdateDevicestatushistory(existingDevicestatushistory);
                }
                else //different day insert case
                {
                    var deviceStatusHistory = SetDevicestatushistory(deviceId, SmartDevice, totalSeconds);
                    InsertDevicestatushistory(deviceStatusHistory);
                }
            }
            else //insert new deviceStatusHistory case
            {

                var deviceStatusHistory = SetDevicestatushistory(deviceId, SmartDevice, totalSeconds);
                InsertDevicestatushistory(deviceStatusHistory);
            }
        }

        public Devicestatushistory SetDevicestatushistory(int deviceId, SmartDeviceContainer SmartDevice, int totalSeconds)
        {
            Devicestatushistory deviceStatusHistory = new Devicestatushistory();

            deviceStatusHistory.DeviceId = deviceId;
            deviceStatusHistory.PowerStatus = Convert.ToInt16(SmartDevice.PowerStatus);
            deviceStatusHistory.Speed = Convert.ToInt32(SmartDevice.Speed);

            deviceStatusHistory.Temp = Convert.ToDouble(SmartDevice.Temperature);
            deviceStatusHistory.Humidity = Convert.ToDouble(SmartDevice.Humidity);
            deviceStatusHistory.Pressure = Convert.ToInt32(SmartDevice.Pressure);
            deviceStatusHistory.Iaq = Convert.ToDouble(SmartDevice.Iaq);
            deviceStatusHistory.IaqAccuracy = Convert.ToInt32(SmartDevice.IaqAccuracy);
            deviceStatusHistory.StaticIaq = Convert.ToDouble(SmartDevice.StaticIaq);
            deviceStatusHistory.StaticIaqAccuracy = Convert.ToInt32(SmartDevice.StaticIaqAccuracy);
            deviceStatusHistory.Co2Concentration = Convert.ToDouble(SmartDevice.Co2Concentration);
            deviceStatusHistory.Co2ConcentrationAccuracy = Convert.ToInt32(SmartDevice.Co2ConcentrationAccuracy);
            deviceStatusHistory.VocConcentration = Convert.ToDouble(SmartDevice.VocConcentration);
            deviceStatusHistory.VocConcentrationAccuracy = Convert.ToInt32(SmartDevice.VocConcentrationAccuracy);
            deviceStatusHistory.GasPercentage = Convert.ToDouble(SmartDevice.GasPercentage);

            deviceStatusHistory.AutoTemp = Convert.ToInt16(SmartDevice.AutoTemp);
            deviceStatusHistory.AutoTimer = Convert.ToInt16(SmartDevice.AutoTimer);
            deviceStatusHistory.AutoStartTime = SmartDevice.AutoStartTime;
            deviceStatusHistory.AutoEndTime = SmartDevice.AutoEndTime;
            deviceStatusHistory.IdealTemp = SmartDevice.IdealTemp;
            deviceStatusHistory.MaintenanceHours = SmartDevice.MaintenanceHours;
            deviceStatusHistory.MaxTemp = SmartDevice.MaxTemp;
            deviceStatusHistory.MinTemp = SmartDevice.MinTemp;

            if (totalSeconds != 0)
            {
                deviceStatusHistory.RunningTime = totalSeconds;
            }
            deviceStatusHistory.CommandType = "";
            deviceStatusHistory.CreatedDate = DateTime.UtcNow;
            deviceStatusHistory.ModifiedDate = DateTime.UtcNow;

            deviceStatusHistory = TransformDeviceStatusHistory(deviceStatusHistory);

            return deviceStatusHistory;
        }

        public bool DeleteDeviceStatusHistory(int devicestatushistoryId)
        {
            try
            {
                repo.Delete(devicestatushistoryId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public bool DeleteDeviceStatusHistoryByDeviceId(int deviceId)
        {
            var devicealarmshistories = QueryDevicestatushistory().Where(dg => dg.DeviceId == deviceId).ToList<Devicestatushistory>();
            if (devicealarmshistories.Count != 0)
            {
                foreach (var dg in devicealarmshistories)
                {
                    DeleteDeviceStatusHistory(dg.Id);
                }

                return true;
            }
            return false;

        }

        public Devicestatushistory TransformDeviceStatusHistory(Devicestatushistory status)
        {
            status.Temp = Math.Round(Convert.ToDouble(status.Temp), 2, MidpointRounding.AwayFromZero);
            status.Humidity = Math.Round(Convert.ToDouble(status.Humidity), 2, MidpointRounding.AwayFromZero);
            status.Iaq = Math.Round(Convert.ToDouble(status.Iaq), 2, MidpointRounding.AwayFromZero);
            status.StaticIaq = Math.Round(Convert.ToDouble(status.StaticIaq), 2, MidpointRounding.AwayFromZero);
            status.Co2Concentration = Math.Round(Convert.ToDouble(status.Co2Concentration), 2, MidpointRounding.AwayFromZero);
            status.VocConcentration = Math.Round(Convert.ToDouble(status.VocConcentration), 2, MidpointRounding.AwayFromZero);
            status.GasPercentage = Math.Round(Convert.ToDouble(status.GasPercentage), 2, MidpointRounding.AwayFromZero);
            return status;
        }
    }
}
