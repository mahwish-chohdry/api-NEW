using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class DeviceSettingsDTO
    {
        public int DeviceId { get; set; }
        public List<Setting> Settings { get; set; }
    }

    public class GroupSettingsDTO
    {
        public int GroupId { get; set; }
        public List<Setting> Settings { get; set; }
    }

    public class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
