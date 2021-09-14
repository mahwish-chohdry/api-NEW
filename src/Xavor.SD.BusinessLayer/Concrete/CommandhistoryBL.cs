using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.BusinessLayer.Interfaces;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;

namespace Xavor.SD.BusinessLayer
{
    public class CommandhistoryBL : ICommandhistoryBL
    {
        private readonly IUnitOfWork _uow;

        private IRepository<Commandhistory> repo;

        public CommandhistoryBL(IUnitOfWork uow)
        {

            _uow = uow;
            repo = uow.GetRepository<Commandhistory>();
        }

        public bool DeleteCommandhistory(int CommandhistoryId)
        {
            try
            {
                repo.Delete(CommandhistoryId);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Commandhistory GetCommandhistory(int Id)
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

        public IEnumerable<Commandhistory> GetCommandhistorys()
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

        public IQueryable<Commandhistory> QueryCommandhistory()
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

        public Commandhistory InsertCommandhistory(Commandhistory Commandhistory)
        {
            try
            {
                repo.Add(Commandhistory);
                _uow.SaveChanges();

                return Commandhistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Commandhistory UpdateCommandhistory(Commandhistory Commandhistory)
        {
            try
            {
                repo.Update(Commandhistory);
                _uow.SaveChanges();

                return Commandhistory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Commandhistory GetUnExceutedCommand(int deviceid)
        {
            try
            {
                Commandhistory command = repo.Queryable().Where(x => x.IsExecuted == 0 && x.DeviceId== deviceid).OrderBy(x => x.CreatedDate)
                    .FirstOrDefault();
                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Commandhistory GetLastExceutedCommand(int deviceid)
        {
            try
            {
                Commandhistory command = repo.Queryable().Where(x=>x.DeviceId==deviceid && x.IsExecuted==1).OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
                return command;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteCommandhistoryByGroupId(int group_id)
        {
            var commandHistories = QueryCommandhistory().Where(dg => dg.GroupId == group_id).ToList<Commandhistory>();
            if (commandHistories.Count != 0)
            {
                foreach (var dg in commandHistories)
                {
                    DeleteCommandhistory(dg.Id);
                }

                return true;
            }
            return false;

        }

        public bool DeleteCommandhistoryByDeviceId(int deviceId)
        {
            var deviceCommandhistory = QueryCommandhistory().Where(dg => dg.DeviceId == deviceId).ToList<Commandhistory>();
            if (deviceCommandhistory.Count != 0)
            {
                foreach (var dg in deviceCommandhistory)
                {
                    DeleteCommandhistory(dg.Id);
                }

                return true;
            }
            return false;

        }

    }
}
