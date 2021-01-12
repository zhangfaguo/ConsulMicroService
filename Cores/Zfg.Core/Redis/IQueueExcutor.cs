using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Common.Redis
{
    public interface IQueueExcutor
    {
        /// <summary>
        /// 生命周期管理
        /// </summary>
        IScope Scope { get; set; }


        /// <summary>
        /// 队列执行
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        string Excutor(string msg);
    }
}
