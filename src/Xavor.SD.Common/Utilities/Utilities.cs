using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;

namespace Xavor.SD.Common.Utilities
{
    public static class Utility
    {
        public static bool CompareDeviceStatus(SmartDeviceContainer deviceStatus,DevicestatusDTO deviceDbStatus) 
        {
            var power = deviceStatus.PowerStatus == 1 ? true : false;
            var autotimer = deviceStatus.AutoTimer == 1 ? true : false;
            var autotemp = deviceStatus.AutoTemp == 1 ? true : false;
            if(deviceStatus.Speed == deviceDbStatus.Speed && power == deviceDbStatus.PowerStatus 
                && autotemp == deviceDbStatus.AutoTemp && autotimer == deviceDbStatus.AutoTimer &&
                deviceDbStatus.IdealTemp == deviceStatus.IdealTemp
                )
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public static string GetStatusMessage(DateTime lastRecord)
        {
            var statusMessage = "Offline";
            try
            {
                var year = DateTime.UtcNow.Year - lastRecord.Year;
                var month = DateTime.UtcNow.Month - lastRecord.Month;
                var day = DateTime.UtcNow.Day - lastRecord.Day;
                var hour = DateTime.UtcNow.Subtract(lastRecord).Hours;
                var minutes = DateTime.UtcNow.Subtract(lastRecord).Minutes;

                if (year == 0 && month == 0 && day == 00 && day == 00 && hour == 0)
                {
                    if (minutes < 2)
                        statusMessage = "Online";
                    else if (minutes >= 2 && minutes <= 10)
                        statusMessage = "Idle for " + minutes + " minutes";
                    else
                        statusMessage = "Offline";
                }
                else
                    statusMessage = "Offline";
            }
            catch (Exception ex)
            {

            }

            return statusMessage;
        }

        public static string GetCommaSeparatedDevices(List<string> devices)
        {
            var devicenames = "";
            foreach (var device in devices)
            {
                if (string.IsNullOrEmpty(devicenames))
                    devicenames = "'" + device + "'";
                else
                    devicenames = devicenames + ",'" + device + "'";
            }
            return devicenames;
        }

        public static string InsertSpaceBeforeUpperCase(string str)
        {
            var sb = new StringBuilder();
            char previousChar = char.MinValue;
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length != 0 && previousChar != ' ') { sb.Append(' '); }
                }
                sb.Append(c);
                previousChar = c;
            }
            return sb.ToString();
        }

        public static string IntToBinary(int value)
        {
            string binary = Convert.ToString(value, 2);
            return binary;
        }

        public static string BinaryToAlarm(string value)
        {
            var alarmValue = "No Alarm";
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (alarmValue != "No Alarm") break;
                var alarm = value.Substring(i, 1);
                switch (Convert.ToInt32(alarm))
                {
                    case (int)Alarms.DriveOverload: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.DriveOverload.ToString()); break; }
                    case (int)Alarms.DriveOverTemperature: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.DriveOverTemperature.ToString()); break; }
                    case (int)Alarms.EarthFault: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.EarthFault.ToString()); break; }
                    case (int)Alarms.InternalFault: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.InternalFault.ToString()); break; }
                    case (int)Alarms.MainPhaseLoss: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.MainPhaseLoss.ToString()); break; }
                    case (int)Alarms.MotorPhaseMissing: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.MotorPhaseMissing.ToString()); break; }
                    case (int)Alarms.OverCurrent: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.OverCurrent.ToString()); break; }
                    case (int)Alarms.ShortCircuit: { alarmValue = Utility.InsertSpaceBeforeUpperCase(Alarms.ShortCircuit.ToString()); break; }
                    default: { break; }
                }
            }
            return alarmValue;
        }

        public static string BinaryToWarning(string value)
        {
            var warningValue = "No Warning";
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (warningValue != "No Warning") break;
                var warning = value.Substring(i, 1);
                switch (Convert.ToInt32(warning))
                {
                    case (int)Warnings.CurrentLimit: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.CurrentLimit.ToString()); break; }
                    case (int)Warnings.DriveOverload: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.DriveOverload.ToString()); break; }
                    case (int)Warnings.FanFault: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.FanFault.ToString()); break; }
                    case (int)Warnings.MainPhaseLoss: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.MainPhaseLoss.ToString()); break; }
                    case (int)Warnings.OverCurrent: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.OverCurrent.ToString()); break; }
                    case (int)Warnings.OverVoltage: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.OverVoltage.ToString()); break; }
                    case (int)Warnings.UnderVoltage: { warningValue = Utility.InsertSpaceBeforeUpperCase(Warnings.UnderVoltage.ToString()); break; }
                }
            }
            return warningValue;
        }

        public static async Task<string> UploadFileAsync(IFormFile File, string Connectionstring, string ContainerName, string FolderName, string FileName, bool overwrite = true, bool PrivateAccess = true)
        {
            CloudStorageAccount storageAccount;
            string url = "";
            if (CloudStorageAccount.TryParse(Connectionstring, out storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                ContainerName = ContainerName.ToLower().Replace(" ", "");
                FolderName = FolderName.ToLower().Replace(" ", "");
                FileName = FileName.ToLower().Replace(" ", "");
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();
                if (PrivateAccess == true)
                {
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off

                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                }


                var directory = cloudBlobContainer.GetDirectoryReference(FolderName);
                CloudBlockBlob cloudBlockBlob = directory.GetBlockBlobReference(FileName);
                cloudBlockBlob.Properties.ContentType = File.ContentType;
                if (overwrite == false)
                {
                    if (await cloudBlockBlob.ExistsAsync())
                    {
                        throw new System.Exception("File Already Exists with same name.");
                    }
                }
                await cloudBlockBlob.UploadFromStreamAsync(File.OpenReadStream());
                url = cloudBlockBlob.SnapshotQualifiedUri.ToString();
                return url;
            }
            else
            {
                return url;
            }
        }

        static public string RestAPICall(string Data, string URL, string RequestType = "Post", string authorizationToken = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(URL);
            try
            {
                if (RequestType.ToUpper() == "POST")
                {
                    if (authorizationToken != null)
                    {
                        request.Headers.Add("Authorization", authorizationToken);
                    }
                    request.Method = "POST";
                    if (Data != "")
                    {
                        request.ContentType = "application/json";
                        request.ContentLength = Data.Length;
                    }

                    using (var webStream = request.GetRequestStream())
                    {
                        using (var requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                        {
                            requestWriter.Write(Data);
                        }
                    }
                    var webResponse = request.GetResponse();
                    using (var webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    {
                        using (var responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            Console.Out.WriteLine(response);
                            return response;
                        }
                    }
                }
                else
                {

                    if (authorizationToken != null)
                    {
                        request.Headers.Add("Authorization", authorizationToken);
                    }
                    var webResponse = request.GetResponse();
                    using (var webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    {
                        using (var responseReader = new StreamReader(webStream))
                        {
                            string response = responseReader.ReadToEnd();
                            Console.Out.WriteLine(response);
                            return response;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return "Failure";
            }
            finally
            {
                //request.Abort();
            }

        }

        public static async Task<string> UploadStreamAsync(MemoryStream stream, string Connectionstring, string ContainerName, string FolderName, string FileName, bool overwrite = true, bool PrivateAccess = true,string ContentType = "application/octet-stream",string fileExtenstion = ".bin")
        {
            CloudStorageAccount storageAccount;
            string url = "";
            if (CloudStorageAccount.TryParse(Connectionstring, out storageAccount))
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                ContainerName = ContainerName.ToLower().Replace(" ", "");
                FolderName = FolderName.ToLower().Replace(" ", "");
                FileName = FileName.ToLower().Replace(" ", "");
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();
                if (PrivateAccess == true)
                {
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off

                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                }
                else 
                {
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob

                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                }

                FileName = FileName + fileExtenstion;
                var directory = cloudBlobContainer.GetDirectoryReference(FolderName);
                CloudBlockBlob cloudBlockBlob = directory.GetBlockBlobReference(FileName);
                cloudBlockBlob.Properties.ContentType = ContentType;
                if (overwrite == false)
                {
                    if (await cloudBlockBlob.ExistsAsync())
                    {
                        throw new System.Exception("File Already Exists with same name.");
                    }
                }
                await cloudBlockBlob.UploadFromStreamAsync(stream);
                url = cloudBlockBlob.SnapshotQualifiedUri.ToString();
                return url;
            }
            else
            {
                return url;
            }
        }





    }

    public enum Alarms
    {
        MainPhaseLoss = 1,
        DriveOverload = 2,
        OverCurrent = 3,
        InternalFault = 4,
        EarthFault = 5,
        ShortCircuit = 6,
        DriveOverTemperature = 7,
        MotorPhaseMissing = 8,
    }

    public enum ScheniederAlarms
    {
        F008 = 6,
        //F008 = 7,
        F013 = 17,
        F001 = 10,
        F025 = 24,
        F020 = 9,
        F010 = 9,
        F012 = 18,
        F018 = 23,
        //F018 = 32,
        F019 = 31,
        F016 = 19,
        F030 = 22,
        F017 = 21,
        F015 = 33,
        F014 = 20,
        F009 = 35,
        F027 = 16,
        F011 = 16,
        F007 = 16,
        F006 = 13,
        F005 = 36,
        F003 = 55,
        F004 = 30,
        F002 = 26,
        F032 = 4,
        F031 = 3,
        F022 = 5,
        F024 = 27,
        //F024 = 28,
        F023 = 6,
        F028 = 25,
        F029 = 0,
        F026 = 0,













    }



    public enum Warnings
    {
        MainPhaseLoss = 1,
        OverVoltage = 2,
        UnderVoltage = 3,
        DriveOverload = 4,
        OverCurrent = 5,
        FanFault = 6,
        CurrentLimit = 7,
    }
}
