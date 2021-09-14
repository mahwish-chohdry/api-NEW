using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class DeviceDTO
    {
        public string customerId { get; set; }
        public string deviceName { get; set; }
        public string deviceId { get; set; }
        public string connectivityStatus { get; set; }
        public bool hasPermission { get; set; }
        public bool IsConfigured { get; set; }
        public string apSsid { get; set; }
        public string apPassword { get; set; }
        public string currentFirmwareVersion { get; set; }
        public string latestFirmwareVersion { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public DevicestatusDTO deviceStatus { get; set; }
        public string inverterId {get;set;}

    }

    public class UserDeviceDTO
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public List<DeviceGroupDTO> deviceGroupList { get; set; }
    }

    //public class DeviceGroupDto
    //{
    //    public int customer_id { get; set; }
    //    public string device_ap_name { get; set; }
    //    public string device_display_name { get; set; }
    //    public int device_id { get; set; }
    //    public string group_name { get; set; }
    //    public int group_id { get; set; }
    //    public byte IsConfigured { get; set; }
    //    public int operator_id { get; set; }
    //    public string operator_name { get; set; }
    //    public string power_status { get; set; }
    //}


    public class DeviceGroupDTO
    {
        public string groupId { get; set; }
        public string groupName { get; set; }
        public bool fullGroupAccess { get; set; }
        public bool groupPowerStatus { get; set; }
        public List<DeviceDTO> deviceList { get; set; }

    }

    public class DevicestatusDTO
    {

        public DevicestatusDTO()
        {
            PowerStatus = false;
        }
        public string connectivityStatus { get; set; }

        public string DeviceId { get; set; }
        public bool MessageType { get; set; }

        public bool IsDeviceStatus { get; set; }
        public bool PowerStatus { get; set; }
        public int Speed { get; set; }


        public double Temp { get; set; }
        public double Humidity { get; set; }
        public int? Pressure { get; set; }
        public double? Iaq { get; set; }
        public int? IaqAccuracy { get; set; }
        public double? StaticIaq { get; set; }
        public int? StaticIaqAccuracy { get; set; }
        public double? Co2Concentration { get; set; }
        public int? Co2ConcentrationAccuracy { get; set; }
        public double? VocConcentration { get; set; }
        public int? VocConcentrationAccuracy { get; set; }
        public double? GasPercentage { get; set; }


        public bool AutoTemp { get; set; }
        public bool AutoTimer { get; set; }
        public string AutoStartTime { get; set; }
        public string AutoEndTime { get; set; }
        public bool HasPreviousSetting { get; set; }
        public int IdealTemp { get; set; }
        public int MaintenanceHours { get; set; }
        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }
        public bool OverrideSettings { get; set; }
        public string TimeZone { get; set; }
        public int UsageHours { get; set; }
        public bool IsExecuted { get; set; }
        public string CommandType { get; set; }
        public string alarm { get; set; }
        public string warning { get; set; }
        public DateTime? ModifiedDate { get; set; }



    }




    public class DeviceGroupBLDTO
    {
        public int customer_id { get; set; }
        public string device_ap_name { get; set; }
        public string device_display_name { get; set; }
        public string device_id { get; set; }
        public string group_name { get; set; }
        public string group_id { get; set; }
        public byte IsConfigured { get; set; }

        public string deviceStatus { get; set; }
        public int? gId { get; set; }
        public int dId { get; set; }

        public byte IsInstalled { get; set; }
        public int operator_id { get; set; }
        public string operator_name { get; set; }
        public string power_status { get; set; }
        public string Apssid { get; set; }
        public string Appassword { get; set; }
        public string CurrentFirmwareVersion { get; set; }
        public string LatestFirmwareVersion { get; set; }

        public string InverterId { get; set; }


    }

}