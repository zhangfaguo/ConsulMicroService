using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core
{
    public interface ILogger : IDisposable
    {
        void Write(string message);


        void Write(Exception exp, string msg = "");

        void Persistence();
    }
}
