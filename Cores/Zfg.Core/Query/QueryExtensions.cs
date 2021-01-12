using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zfg.Core.Common
{
    public static class QueryExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, Condition page)
     where T : class
        {
            var recordSize = query.Count();

            return Paging(query as IOrderedQueryable<T>, page, recordSize);
        }

        public static IQueryable<T> Paging<T>(this IOrderedQueryable<T> query, Condition page, int recordSize)
            where T : class
        {
            page.TotalCount = recordSize;

            if (page.PageSize == 0)
                page.PageSize = 10;

            page.PageCount = (page.TotalCount + page.PageSize - 1) / page.PageSize;

            if (page.PageIndex < 0)
                page.PageIndex = 0;

            if (page.PageIndex > page.PageCount && page.PageCount != 0)
                page.PageIndex = page.PageCount;

            var takeRecord = page.PageIndex * page.PageSize;

            return query.Skip(takeRecord).Take(page.PageSize);
        }



        public static IQueryable<T> Paging<T>(this IQueryable<T> query, Condition page, Func<IQueryable<T>, IOrderedQueryable<T>> orderFn)
            where T : class
        {

            var recordCount = query.Count();

            return Paging(query, page, recordCount, orderFn);
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> query, Condition page, int recordSize, Func<IQueryable<T>, IOrderedQueryable<T>> orderFn)
            where T : class
        {
            var orderQuery = orderFn(query);

            return Paging(orderQuery, page, recordSize);
        }

    }
}
