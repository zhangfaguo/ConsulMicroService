using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Zfg.Core.Db
{
    internal class SQLRepository<T> : IRepository<T>
          where T : BaseEntity
    {
        #region Fields
        private readonly DbContext Db;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public SQLRepository(DbContext db)
        {
            Db = db;
        }


        #endregion


        public IQueryable<T> Select
        {
            get
            {
                return Db.Set<T>();
            }
        }

        public Expression Expression => Select.Expression;

        public Type ElementType => Select.ElementType;

        public IQueryProvider Provider => Select.Provider;


        #region Methods



        public virtual T Insert(T entity)
        {
            Db.Set<T>().Add(entity);
            Db.SaveChanges();
            return entity;
        }



        public virtual int Delete(T entity)
        {
            var entitys = Db.Set<T>();
            var local = Db.Set<T>().FirstOrDefault(t => t.Id == entity.Id);
            var rst = 0;
            if (local != null)
            {
                entitys.Remove(local);
                rst = Db.SaveChanges();
            }
            return rst;

        }




        public virtual List<T> InternalUpdate(Expression<Func<T, bool>> whereFunc, Action<T> editAct)
        {
            var entitys = Db.Set<T>();
            var list = entitys.Where(whereFunc).ToList();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    editAct(item);
                }

            }
            var rst = Db.SaveChanges();
            return rst > 0 ? list : null;

        }

        public virtual List<T> InternalDelete(Expression<Func<T, bool>> whereFunc)
        {
            var entitys = Db.Set<T>();
            var list = entitys.Where(whereFunc).ToList();
            if (list.Count > 0)
            {
                entitys.RemoveRange(list);
            }
            var rst = Db.SaveChanges();
            return rst > 0 ? list : null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Select.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Select.GetEnumerator();
        }

        #endregion
    }
}
