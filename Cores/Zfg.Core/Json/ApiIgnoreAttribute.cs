using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Common
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class ApiIgnoreAttribute : Attribute { }
}
