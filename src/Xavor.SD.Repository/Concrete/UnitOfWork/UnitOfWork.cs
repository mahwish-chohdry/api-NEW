using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xavor.SD.Common.Utilities;
using Xavor.SD.Common.ViewContracts;
using Xavor.SD.Model;
using Xavor.SD.Repository.Concrete;
using Xavor.SD.Repository.Contracts;
using Xavor.SD.Repository.Contracts.UnitOfWork;
using Xavor.SD.Repository.Interfaces;

namespace Xavor.SD.Repository.UnitOfWork
{


    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IUnitOfWork
        where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(Context);
            return (IRepository<TEntity>)_repositories[type];
        }

        public TContext Context { get; }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch(DbUpdateConcurrencyException ex) 
            {
               throw new ResponseException(new ResponseDTO
                {
                    StatusCode = "Warning",
                    Message = "The action could not be performed please try again.",
                    Data = null
                });
            }
         
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
