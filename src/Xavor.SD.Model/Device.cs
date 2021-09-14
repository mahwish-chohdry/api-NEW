using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Device
    {
        public Device()
        {
            Commandhistory = new HashSet<Commandhistory>();
            Devicealarmshistory = new HashSet<Devicealarmshistory>();
            Devicecommand = new HashSet<Devicecommand>();
            Devicegroup = new HashSet<Devicegroup>();
            Devicestatus = new HashSet<Devicestatus>();
            Userdevice = new HashSet<Userdevice>();
        }

        public int Id { get; set; }
        public Guid? RecordId { get; set; }
        public int? CustomerId { get; set; }
        public int? InverterId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string DeviceCode { get; set; }
        public string Apssid { get; set; }
        public string Appassword { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public short? IsInstalled { get; set; }
        public short? IsDeleted { get; set; }
        public short? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string BatchId { get; set; }
        public string CurrentFirmwareVersion { get; set; }
        public string LatestFirmwareVersion { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Inverter Inverter { get; set; }
        public virtual ICollection<Commandhistory> Commandhistory { get; set; }
        public virtual ICollection<Devicealarmshistory> Devicealarmshistory { get; set; }
        public virtual ICollection<Devicecommand> Devicecommand { get; set; }
        public virtual ICollection<Devicegroup> Devicegroup { get; set; }
        public virtual ICollection<Devicestatus> Devicestatus { get; set; }
        public virtual ICollection<Userdevice> Userdevice { get; set; }
    }
}
