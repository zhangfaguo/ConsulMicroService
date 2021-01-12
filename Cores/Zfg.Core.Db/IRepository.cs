using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zfg.Core.Db;

namespace Zfg.Core
{
    public interface IRepository<T> : IQueryable<T>
     where T : BaseEntity
    {
        T Insert(T entity);
        int Delete(T entity);

        List<T> InternalUpdate(Expression<Func<T, bool>> whereFunc, Action<T> editAct);

        List<T> InternalDelete(Expression<Func<T, bool>> whereFunc);
    }
}
