using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer.Concrete
{
    public class DeviceBatchNumberBL : IDeviceBatchNumberBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Devicebatchnumber> repo;

        public DeviceBatchNumberBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Devicebatchnumber>();
        }
        public bool DeleteDeviceBatchNumber(int batchId)
        {
            try
            {
                repo.Delete(batchId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Devicebatchnumber> GetDeviceBatchNumber()
        {
            try
            {
                return repo.GetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Devicebatchnumber GetDeviceBatchNumber(int Id)
        {
            try
            {
                if (Id <= default(int))
                    throw new ArgumentException("Invalid id");
                return repo.Find(Id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Devicebatchnumber InsertDeviceBatchNumber(Devicebatchnumber deviceBatchNumber)
        {
            try
            {
                repo.Add(deviceBatchNumber);
                uow.SaveChanges();

                return deviceBatchNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Devicebatchnumber> QueryDeviceBatchNumber()
        {
            try
            {
                return repo.Queryable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Devicebatchnumber UpdateDeviceBatchNumber(Devicebatchnumber deviceBatchNumber)
        {
            try
            {
                repo.Update(deviceBatchNumber);
                uow.SaveChanges();
                return deviceBatchNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
