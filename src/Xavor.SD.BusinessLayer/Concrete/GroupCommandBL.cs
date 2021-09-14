using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;

namespace Xavor.SD.BusinessLayer
{
    public class GroupcommandBL : IGroupcommandBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Groupcommand> repo;
        public GroupcommandBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Groupcommand>();
        }

        public void AddOrUpdateGroupcommand(Groupcommand groupCommand)
        {
            var existingGroupcommand = QueryGroupcommand().Where(x => x.GroupId == groupCommand.GroupId && x.CustomerId == groupCommand.CustomerId).FirstOrDefault();
            if (existingGroupcommand != null) 
            {

                existingGroupcommand.GroupId = groupCommand.GroupId;
                existingGroupcommand.CommandHistoryId = groupCommand.CommandHistoryId;
                existingGroupcommand.ModifiedDate = DateTime.UtcNow;
                UpdateGroupcommand(existingGroupcommand);
            }
            else 
            {
                InsertGroupcommand(groupCommand);
            }
        }

        //public void AddOrUpdateGroupcommand(Groupcommand groupCommand)
        //{
        //    var existingGroupcommand = QueryGroupcommand().Where(x => x.GroupId == groupCommand.GroupId && x.CustomerId == groupCommand.CustomerId).FirstOrDefault();
        //    if (existingGroupcommand != null) // Update case
        //    {
        //        existingGroupcommand.PowerStatus = groupCommand.PowerStatus;
        //        existingGroupcommand.Temp = groupCommand.Temp;
        //        existingGroupcommand.Speed = groupCommand.Speed;
        //        existingGroupcommand.Humidity = groupCommand.Humidity;
        //        existingGroupcommand.AutoTemp = groupCommand.AutoTemp;
        //        existingGroupcommand.AutoTimer = groupCommand.AutoTimer;
        //        existingGroupcommand.AutoStartTime = groupCommand.AutoStartTime;
        //        existingGroupcommand.AutoEndTime = groupCommand.AutoEndTime;
        //        existingGroupcommand.HasPreviousSetting = groupCommand.HasPreviousSetting;
        //        existingGroupcommand.IdealTemp = groupCommand.IdealTemp;
        //        existingGroupcommand.MaintenanceHours = groupCommand.MaintenanceHours;
        //        existingGroupcommand.MaxTemp = groupCommand.MaxTemp;
        //        existingGroupcommand.MinTemp = groupCommand.MinTemp;
        //        existingGroupcommand.OverrideSettings = groupCommand.OverrideSettings;
        //        existingGroupcommand.TimeZone = groupCommand.TimeZone;
        //        existingGroupcommand.UsageHours = groupCommand.UsageHours;
        //        existingGroupcommand.IsExecuted = groupCommand.IsExecuted;
        //        existingGroupcommand.CommandType = groupCommand.CommandType;
        //        existingGroupcommand.CreatedDate = groupCommand.CreatedDate;
        //        existingGroupcommand.ModifiedDate = groupCommand.ModifiedDate;
        //        existingGroupcommand.GroupId = groupCommand.GroupId;

        //        UpdateGroupcommand(existingGroupcommand);
        //    }
        //    else // Insert case
        //    {
        //        InsertGroupcommand(groupCommand);
        //    }
        //}

        public bool DeleteGroupcommand(int GroupcommandId)
        {
            try
            {
                repo.Delete(GroupcommandId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Groupcommand> GetGroupcommand()
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

        public Groupcommand GetGroupcommand(int Id)
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

        public Groupcommand InsertGroupcommand(Groupcommand Groupcommand)
        {
            try
            {
                repo.Add(Groupcommand);
                uow.SaveChanges();

                return Groupcommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Groupcommand> QueryGroupcommand()
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

        public Groupcommand UpdateGroupcommand(Groupcommand Groupcommand)
        {
            try
            {
                repo.Update(Groupcommand);
                uow.SaveChanges();
                return Groupcommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteGroupcommandByGroupId(int group_id)
        {
            var groupCommands = QueryGroupcommand().Where(dg => dg.GroupId == group_id).ToList<Groupcommand>();
            if (groupCommands.Count != 0)
            {
                foreach (var dg in groupCommands)
                {
                    DeleteGroupcommand(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
