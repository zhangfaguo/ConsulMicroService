using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{

    public class IocAttribute : Attribute
    {
        public LiftTime LiftTime { get; set; } = LiftTime.MicroTime;

        public string Name { get; set; }


        public Type InterfaceType { get; set; }
    }


    public enum LiftTime
    {
        /// <summary>
        /// 瞬时
        /// </summary>
        MicroTime,
        /// <summary>
        /// 单例
        /// </summary>
        Single,
        /// <summary>
        /// 请求单例
        /// </summary>
        RequestSingle
    }
}
