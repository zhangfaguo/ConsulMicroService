using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Common.Redis
{
    public class PushMsg
    {
        public string Key { get; set; }

        public string Command { get; set; }
        public string Body { get; set; }

        public bool IsBack { get; set; }

        public DateTime? Time { get; set; }
    }
}
