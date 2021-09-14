using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;

namespace Xavor.SD.BusinessLayer.Interfaces
{
    public interface IDeviceBatchNumberBL
    {
        Devicebatchnumber InsertDeviceBatchNumber(Devicebatchnumber deviceBatchNumber);
        Devicebatchnumber UpdateDeviceBatchNumber(Devicebatchnumber deviceBatchNumber);
        IEnumerable<Devicebatchnumber> GetDeviceBatchNumber();
        bool DeleteDeviceBatchNumber(int batchId);
        Devicebatchnumber GetDeviceBatchNumber(int Id);
        IQueryable<Devicebatchnumber> QueryDeviceBatchNumber();
    }
}
