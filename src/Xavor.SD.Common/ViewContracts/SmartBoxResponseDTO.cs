using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.Common.ViewContracts
{
    public class SmartBoxResponseDTO
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public object Data { get; set; }
        public string Timezone { get; set; }
        public string CustomerId { get; set; }
        public string PostStatusFrequency { get; set; }

        public Ruleengine Ruleengine { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
