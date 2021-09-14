using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
   public class EnvironmentstandardsDTO
    {

        public int Id { get; set; }
        public string Type { get; set; }
        public double? GoodMin { get; set; }
        public double? GoodMax { get; set; }
        public double? ModerateMin { get; set; }
        public double? ModerateMax { get; set; }
        public double? UnhealthyMin { get; set; }
        public double? UnhealthyMax { get; set; }
        public double? VeryUnhealthyMin { get; set; }
        public double? VeryUnhealthyMax { get; set; }
        public string Unit { get; set; }
    }
}
