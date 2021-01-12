using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public class Condition
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 当前页面
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总数据条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页码数
        /// </summary>
        public int PageCount { get; set; }
    }
}
