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
    public class RefreshtokensBL : IRefreshtokensBL
    {
        private readonly IUnitOfWork uow;
        private SmartFanDbContext context;
        private IRepository<Refreshtokens> repo;
        public RefreshtokensBL()
        {
            context = new SmartFanDbContext();
            uow = new UnitOfWork<SmartFanDbContext>(context);
            repo = uow.GetRepository<Refreshtokens>();
        }

        public void AddOrUpdateRefreshToken(Refreshtokens refreshTokens)
        {
            var existingEntry = QueryRefreshtokens().Where(x => x.UserId == refreshTokens.UserId).FirstOrDefault();
            if(existingEntry == null)
            {
                InsertRefreshtokens(refreshTokens);
            }
            else
            {
                existingEntry.RefreshToken = refreshTokens.RefreshToken;
                UpdateRefreshtokens(existingEntry);
            }
        }

        public bool DeleteRefreshtokens(int RefreshtokensId)
        {
            try
            {
                repo.Delete(RefreshtokensId);
                uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public IEnumerable<Refreshtokens> GetRefreshtokens()
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

        public Refreshtokens GetRefreshtokens(string refreshTokenFromUser)
        {
            var existingRefreshToken = QueryRefreshtokens().Where(x => x.RefreshToken == refreshTokenFromUser).FirstOrDefault();
            return existingRefreshToken;
        }

        public Refreshtokens InsertRefreshtokens(Refreshtokens Refreshtokens)
        {
            try
            {
                repo.Add(Refreshtokens);
                uow.SaveChanges();

                return Refreshtokens;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Refreshtokens> QueryRefreshtokens()
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

        public Refreshtokens UpdateRefreshtokens(Refreshtokens Refreshtokens)
        {
            try
            {
                repo.Update(Refreshtokens);
                uow.SaveChanges();
                return Refreshtokens;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteUserRefreshTokensByUserId(int userId)
        {
            var refreshtokensList = QueryRefreshtokens().Where(dg => dg.UserId == userId).ToList<Refreshtokens>();
            if (refreshtokensList.Count != 0)
            {
                foreach (var dg in refreshtokensList)
                {
                    DeleteRefreshtokens(dg.Id);
                }

                return true;
            }
            return false;

        }
    }
}
