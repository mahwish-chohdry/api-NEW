using System;

namespace Xavor.SD.Common
{
    public static class Constants
    {
        public static string IOT_HUB_CONNECTION_STRING = "IOTHubConnectionString";
        public static string SAS_TOKEN_EXPIRY = "SASTokenExpiryInHours";
        public static double SAS_TOKEN_EXPIRY_DEFAULT = 24;
        public static bool statusFromCloud = false;
        public  enum STATUSCODES{
            INVALID_DEVICE=701,
            Create_Device_Failed=700,
        }

    }
    public enum Alarms
    {
        NoAlarm=-1,
        MainPhaseLoss = 1,
        DriveOverload = 2,
        OverCurrent = 3,
        InternalFault = 4,
        EarthFault = 5,
        ShortCircuit = 6,
        DriveOverTemperature = 7,
        MotorPhaseMissing = 8,
    }

    public enum Warnings
    {
        NoWarning = -1,
        MainPhaseLoss = 1,
        OverVoltage = 2,
        UnderVoltage = 3,
        DriveOverload = 4,
        OverCurrent = 5,
        FanFault = 6,
        CurrentLimit = 7,
    }
    public enum Command {
        auto,
        manual,
        //Speed, 
        //Switch,
        //Autotemp,
        //Autotimer, 
        //Override

    }
}
