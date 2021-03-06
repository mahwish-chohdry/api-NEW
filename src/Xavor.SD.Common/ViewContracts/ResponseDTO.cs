using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.ViewContracts
{
    public class ResponseDTO
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public object Data { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
