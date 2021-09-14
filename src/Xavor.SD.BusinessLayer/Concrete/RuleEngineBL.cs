using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Xavor.SD.Model;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;
using Xavor.SD.Repository.UnitOfWork;
using Xavor.SD.BusinessLayer;

namespace Xavor.SD.BusinessLayer
{
    public class RuleEngineBL : IRuleEngineBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Ruleengine> repo;
        public RuleEngineBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Ruleengine>();
        }
        public bool DeleteRuleEngine(int ruleEngineId)
        {
            try
            {
                repo.Delete(ruleEngineId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Ruleengine> GetRuleEngine()
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

        public Ruleengine GetRuleEngine(int Id)
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

        public Ruleengine GetRuleEngineByCustomerId(int customerId)
        {
            var customer = GetRuleEngine().Where(x => x.CustomerId == customerId).FirstOrDefault();
            return customer;
        }

        public Ruleengine InsertRuleEngine(Ruleengine ruleEngine)
        {
            try
            {
                repo.Add(ruleEngine);
                uow.SaveChanges();

                return ruleEngine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Ruleengine> QueryRuleEngine()
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

        public Ruleengine UpdateRuleEngine(Ruleengine ruleEngine)
        {
            try
            {
                repo.Update(ruleEngine);
                uow.SaveChanges();
                return ruleEngine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
