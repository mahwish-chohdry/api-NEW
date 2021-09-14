using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
    public  interface IFirmwareBL
    {
        Firmware InsertFirmware(Firmware firmware);
        Firmware UpdateFirmware(Firmware firmware);
        IEnumerable<Firmware> GetFirmware();
        bool DeleteFirmware(int firmwareId);
        Firmware GetFirmware(int Id);
        IQueryable<Firmware> QueryFirmware();
    }
}
