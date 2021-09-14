using System;
using System.Collections.Generic;

namespace Xavor.SD.Model
{
    public partial class Bom
    {
        public int Id { get; set; }
        public string BatchId { get; set; }
        public string CustomerId { get; set; }
        public string BomData { get; set; }
        public string BomType { get; set; }
        public string FileFormat { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
