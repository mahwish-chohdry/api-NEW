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
    public class FirmwareBL : IFirmwareBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Firmware> repo;
        public FirmwareBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Firmware>();
        }

        public bool DeleteFirmware(int firmwareId)
        {
            try
            {
                repo.Delete(firmwareId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Firmware> GetFirmware()
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

        public Firmware GetFirmware(int Id)
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

        public Firmware InsertFirmware(Firmware firmware)
        {
            try
            {
                repo.Add(firmware);
                uow.SaveChanges();

                return firmware;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Firmware> QueryFirmware()
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

        public Firmware UpdateFirmware(Firmware firmware)
        {
            try
            {
                repo.Update(firmware);
                uow.SaveChanges();
                return firmware;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
