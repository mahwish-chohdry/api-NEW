using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Model
{
    public class TotalDataModel<T>  where T : class
    {
        public int TotalResultsCount { get; set; }
        public List<T> Results { get; set; }
    }
}
