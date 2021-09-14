using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xavor.SD.Repository.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity entity);
        void AddList(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateList(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> GetList();
        bool Delete(int userId);
        TEntity Find(int Id);
        IQueryable<TEntity> Queryable();
        
    }
}
