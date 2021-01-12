using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExtensions
    {

        public static int ToInt(this string str, int rst = 0)
        {
            var v = rst;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    v = Convert.ToInt32(str);

                }
                catch (Exception)
                {

                }
            }
            return v;
        }
    }
}
