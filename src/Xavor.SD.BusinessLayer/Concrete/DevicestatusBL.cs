using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;

namespace Xavor.SD.BusinessLayer
{
    public class DevicestatusBL : IDevicestatusBL
    {
        private readonly IUnitOfWork _uow;
        private readonly IDeviceBL _deviceBL;
        private readonly IDevicestatushistoryBL _deviceHistoryBL;
        private readonly IDevicealarmsBL _deviceAlarmsBL;
        private IRepository<Devicestatus> repo;

        public DevicestatusBL(IUnitOfWork uow, IDeviceBL deviceBL, IDevicestatushistoryBL deviceStatusHistoryBL, IDevicealarmsBL deviceAlarmsBL)
        {

            _uow = uow;
            _deviceBL = deviceBL;
            _deviceHistoryBL = deviceStatusHistoryBL;
            _deviceAlarmsBL = deviceAlarmsBL;
            repo = uow.GetRepository<Devicestatus>();
        }

        public bool DeleteDevicestatus(int DevicestatusId)
        {
            try
            {
                repo.Delete(DevicestatusId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Devicestatus GetDevicestatus(int Id)
        {
            try
            {
                if (Id <= default(int))
                    throw new ArgumentException("Invalid id");

                return repo.Find(Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public IEnumerable<Devicestatus> GetDevicestatuss()
        {
            try
            {
                return repo.GetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IQueryable<Devicestatus> QueryDevicestatus()
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

        public Devicestatus InsertDevicestatus(Devicestatus Devicestatus)
        {
            try
            {
                repo.Add(Devicestatus);
                _uow.SaveChanges();

                return Devicestatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Devicestatus UpdateDevicestatus(Devicestatus Devicestatus)
        {
            try
            {
                repo.Update(Devicestatus);
                _uow.SaveChanges();

                return Devicestatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Devicestatus UpdateDevicestatus(SmartDeviceContainer SmartDevice)
        {
            var existingDevice = _deviceBL.QueryDevice().Where(x => x.DeviceId == SmartDevice.DeviceId).FirstOrDefault();
            if (existingDevice != null)
            {
                var existingDevicestatus = QueryDevicestatus().Where(x => x.DeviceId == existingDevice.Id).FirstOrDefault();
                if (existingDevicestatus != null) // Update Devicestatus case
                {
                    if (SmartDevice.PowerStatus == 1) //&& SmartDevice.PLC == 1) direction commented by embedded team
                    {

                        var datediff = DateTime.UtcNow - existingDevicestatus.ModifiedDate;
                        var totalSeconds = Convert.ToInt32(datediff.Value.TotalSeconds);

                        if (totalSeconds > 0)
                        {
                            existingDevicestatus.ModifiedDate = DateTime.UtcNow;
                            existingDevicestatus.TotalRunningTime += totalSeconds;
                            existingDevicestatus.RunningTime += totalSeconds;
                            // addOrUpdate to status history
                            _deviceHistoryBL.AddOrUpdateDevicestatushistory(existingDevice.Id, SmartDevice, totalSeconds);
                        }

                        existingDevicestatus.Temp = Convert.ToDouble(SmartDevice.Temperature);
                        existingDevicestatus.Humidity = Convert.ToDouble(SmartDevice.Humidity);
                        existingDevicestatus.Pressure = Convert.ToInt32(SmartDevice.Pressure);
                        existingDevicestatus.Iaq = Convert.ToDouble(SmartDevice.Iaq);
                        existingDevicestatus.IaqAccuracy = Convert.ToInt32(SmartDevice.IaqAccuracy);
                        existingDevicestatus.StaticIaq = Convert.ToDouble(SmartDevice.StaticIaq);
                        existingDevicestatus.StaticIaqAccuracy = Convert.ToInt32(SmartDevice.StaticIaqAccuracy);
                        existingDevicestatus.Co2Concentration = Convert.ToDouble(SmartDevice.Co2Concentration);
                        existingDevicestatus.Co2ConcentrationAccuracy = Convert.ToInt32(SmartDevice.Co2ConcentrationAccuracy);
                        existingDevicestatus.VocConcentration = Convert.ToDouble(SmartDevice.VocConcentration);
                        existingDevicestatus.VocConcentrationAccuracy = Convert.ToInt32(SmartDevice.VocConcentrationAccuracy);
                        existingDevicestatus.GasPercentage = Convert.ToDouble(SmartDevice.GasPercentage); // Math.Round(Convert.ToDouble(SmartDevice.GasPercentage), 3, MidpointRounding.AwayFromZero);

                        existingDevicestatus.Warnings = SmartDevice.Warning;
                        existingDevicestatus.Alarm = SmartDevice.Alarm;
                        return UpdateDevicestatus(existingDevicestatus);
                    }
                    else if (SmartDevice.PowerStatus == 0 || SmartDevice.PLC == 0)
                    {
                        var datediff = DateTime.UtcNow - existingDevicestatus.ModifiedDate;
                        var totalSeconds = Convert.ToInt32(datediff.Value.TotalSeconds);
                        if (totalSeconds > 0)
                        {
                            existingDevicestatus.ModifiedDate = DateTime.UtcNow;
                        }

                        // addOrUpdate to status history
                        _deviceHistoryBL.AddOrUpdateDevicestatushistory(existingDevice.Id, SmartDevice, 0);

                        existingDevicestatus.Temp = Convert.ToDouble(SmartDevice.Temperature);
                        existingDevicestatus.Humidity = Convert.ToDouble(SmartDevice.Humidity);
                        existingDevicestatus.Pressure = Convert.ToInt32(SmartDevice.Pressure);
                        existingDevicestatus.Iaq = Convert.ToDouble(SmartDevice.Iaq);
                        existingDevicestatus.IaqAccuracy = Convert.ToInt32(SmartDevice.IaqAccuracy);
                        existingDevicestatus.StaticIaq = Convert.ToDouble(SmartDevice.StaticIaq);
                        existingDevicestatus.StaticIaqAccuracy = Convert.ToInt32(SmartDevice.StaticIaqAccuracy);
                        existingDevicestatus.Co2Concentration = Convert.ToDouble(SmartDevice.Co2Concentration);
                        existingDevicestatus.Co2ConcentrationAccuracy = Convert.ToInt32(SmartDevice.Co2ConcentrationAccuracy);
                        existingDevicestatus.VocConcentration = Convert.ToDouble(SmartDevice.VocConcentration);
                        existingDevicestatus.VocConcentrationAccuracy = Convert.ToInt32(SmartDevice.VocConcentrationAccuracy);
                        existingDevicestatus.GasPercentage = Convert.ToDouble(SmartDevice.GasPercentage); // Math.Round(Convert.ToDouble(SmartDevice.GasPercentage), 3, MidpointRounding.AwayFromZero);

                        existingDevicestatus.Warnings = SmartDevice.Warning;
                        existingDevicestatus.Alarm = SmartDevice.Alarm;

                        return UpdateDevicestatus(existingDevicestatus);
                    }
                    return existingDevicestatus;
                }
                else // Add new Devicestatus case
                {
                    Devicestatus newDevicestatus = new Devicestatus();
                    newDevicestatus.DeviceId = existingDevice.Id;
                    newDevicestatus.Speed = Convert.ToInt32(SmartDevice.Speed);

                    newDevicestatus.Temp = Convert.ToDouble(SmartDevice.Temperature);
                    newDevicestatus.Humidity = Convert.ToDouble(SmartDevice.Humidity);
                    newDevicestatus.Pressure = Convert.ToInt32(SmartDevice.Pressure);
                    newDevicestatus.Iaq = Convert.ToDouble(SmartDevice.Iaq);
                    newDevicestatus.IaqAccuracy = Convert.ToInt32(SmartDevice.IaqAccuracy);
                    newDevicestatus.StaticIaq = Convert.ToDouble(SmartDevice.StaticIaq);
                    newDevicestatus.StaticIaqAccuracy = Convert.ToInt32(SmartDevice.StaticIaqAccuracy);
                    newDevicestatus.Co2Concentration = Convert.ToDouble(SmartDevice.Co2Concentration);
                    newDevicestatus.Co2ConcentrationAccuracy = Convert.ToInt32(SmartDevice.Co2ConcentrationAccuracy);
                    newDevicestatus.VocConcentration = Convert.ToDouble(SmartDevice.VocConcentration);
                    newDevicestatus.VocConcentrationAccuracy = Convert.ToInt32(SmartDevice.VocConcentrationAccuracy);
                    newDevicestatus.GasPercentage = Convert.ToDouble(SmartDevice.GasPercentage); // Math.Round(Convert.ToDouble(SmartDevice.GasPercentage), 3, MidpointRounding.AwayFromZero);

                    newDevicestatus.AutoTemp = 0;
                    newDevicestatus.AutoTimer = 0;
                    newDevicestatus.AutoStartTime = SmartDevice.AutoStartTime;
                    newDevicestatus.AutoEndTime = SmartDevice.AutoEndTime;
                    newDevicestatus.IdealTemp = SmartDevice.IdealTemp;
                    newDevicestatus.MaintenanceHours = SmartDevice.MaintenanceHours;
                    newDevicestatus.MaxTemp = SmartDevice.MaxTemp;
                    newDevicestatus.MinTemp = SmartDevice.MinTemp;
                    newDevicestatus.ModifiedDate = DateTime.UtcNow;
                    newDevicestatus.Warnings = SmartDevice.Warning;
                    newDevicestatus.Alarm = SmartDevice.Alarm;
                    return InsertDevicestatus(newDevicestatus);
                }

            }
            return null;
        }

        public Devicestatus GetDevicestatusByDeviceId(int deviceId)
        {
            var storedDevicestatus = QueryDevicestatus().Where(x => x.DeviceId == deviceId).FirstOrDefault();
            return storedDevicestatus;
        }
        public Devicestatus TransformStatus(Devicestatus deviceStatus, StatusDTO SmartDevice)
        {
            deviceStatus.MaintenanceHours = SmartDevice.MaintenanceHours;
            //deviceStatus.Speed = SmartDevice.Speed;
            if (SmartDevice.AutoTemp == true)
            {
                deviceStatus.AutoTemp = 1;
            }
            else
            {
                deviceStatus.AutoTemp = 0;
            }
            if (SmartDevice.AutoTimer == true)
            {
                deviceStatus.AutoTimer = 1;
            }
            else
            {
                deviceStatus.AutoTimer = 0;
            }


            //if (SmartDevice.PowerStatus == true)
            //{
            //    deviceStatus.PowerStatus = 1;
            //}
            //else
            //{
            //    deviceStatus.PowerStatus = 0;
            //}
            deviceStatus.AutoEndTime = SmartDevice.AutoEndTime;
            deviceStatus.AutoStartTime = SmartDevice.AutoStartTime;
            deviceStatus.MaxTemp = SmartDevice.IdealTemp + 2;
            deviceStatus.MinTemp = SmartDevice.IdealTemp - 2;
            deviceStatus.IdealTemp = SmartDevice.IdealTemp;

            deviceStatus.Temp = SmartDevice.Temp;
            deviceStatus.Humidity = SmartDevice.Humidity;
            deviceStatus.Pressure = SmartDevice.Pressure;
            deviceStatus.Iaq = SmartDevice.Iaq;
            deviceStatus.IaqAccuracy = SmartDevice.IaqAccuracy;
            deviceStatus.StaticIaq = SmartDevice.StaticIaq;
            deviceStatus.StaticIaqAccuracy = SmartDevice.StaticIaqAccuracy;
            deviceStatus.Co2Concentration = SmartDevice.Co2Concentration;
            deviceStatus.Co2ConcentrationAccuracy = SmartDevice.Co2ConcentrationAccuracy;
            deviceStatus.VocConcentration = SmartDevice.VocConcentration;
            deviceStatus.VocConcentrationAccuracy = SmartDevice.VocConcentrationAccuracy;
            deviceStatus.GasPercentage = SmartDevice.GasPercentage;
            //if (SmartDevice.GasPercentage != null) deviceStatus.GasPercentage = Math.Round(Convert.ToDouble(SmartDevice.GasPercentage), 3, MidpointRounding.AwayFromZero);

            return deviceStatus;
        }

        public Devicestatus UpdateDevicestatus(StatusDTO SmartDevice, string StatusFieldToModify = "All")
        {
            var existingDevice = _deviceBL.QueryDevice().Where(x => x.DeviceId == SmartDevice.DeviceId).FirstOrDefault();
            var existingDevicestatus = QueryDevicestatus().Where(x => x.DeviceId == existingDevice.Id).FirstOrDefault();
            if (existingDevicestatus != null) // Update Devicestatus case
            {
                switch (StatusFieldToModify.ToLower())
                {
                    case "all":
                        existingDevicestatus = TransformStatus(existingDevicestatus, SmartDevice);
                        break;
                    case "maintance":
                        existingDevicestatus.MaintenanceHours = SmartDevice.MaintenanceHours;
                        break;
                    case "speed":
                        existingDevicestatus.Speed = SmartDevice.Speed;
                        break;
                    case "autotemp":
                        if (SmartDevice.AutoTemp == true)
                        {
                            existingDevicestatus.AutoTemp = 1;
                        }
                        else
                        {
                            existingDevicestatus.AutoTemp = 0;
                        }

                        existingDevicestatus.MaxTemp = SmartDevice.IdealTemp + 2;
                        existingDevicestatus.MinTemp = SmartDevice.IdealTemp - 2;
                        existingDevicestatus.IdealTemp = SmartDevice.IdealTemp;

                        break;
                    case "autotimer":

                        if (SmartDevice.AutoTimer == true)
                        {
                            existingDevicestatus.AutoTimer = 1;
                        }
                        else
                        {
                            existingDevicestatus.AutoTimer = 0;
                        }
                        existingDevicestatus.AutoEndTime = SmartDevice.AutoEndTime;
                        existingDevicestatus.AutoStartTime = SmartDevice.AutoStartTime;

                        break;
                    case "switch":

                        if (SmartDevice.PowerStatus == true)
                        {
                            existingDevicestatus.PowerStatus = 1;
                        }
                        else
                        {
                            existingDevicestatus.PowerStatus = 0;
                        }

                        break;
                    case "grouppowerstatus":
                        if (SmartDevice.PowerStatus == true)
                        {
                            existingDevicestatus.PowerStatus = 1;
                        }
                        else
                        {
                            existingDevicestatus.PowerStatus = 0;
                        }
                        SmartDevice.Speed = existingDevicestatus.Speed;
                        break;


                }
                return UpdateDevicestatus(existingDevicestatus);
            }
            return null;
        }

        public bool DeleteDevicestatusByDeviceId(int deviceId)
        {
            var Devicestatuses = QueryDevicestatus().Where(dg => dg.DeviceId == deviceId).ToList<Devicestatus>();
            if (Devicestatuses.Count != 0)
            {
                foreach (var dg in Devicestatuses)
                {
                    DeleteDevicestatus(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
