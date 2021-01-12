using System;
using System.Collections.Generic;
using System.Text;

namespace Zfg.Core.Common.Redis
{
    public class RedisKeys
    {
        public static string MsgQuoteDelayCommonChannel(string time) => $"Base:MessageChannel:MsgQuoteDelayCommonChannel:{time}";



        #region User
        public static string UserPermission(int userId) => $"User:{userId}:Perssion";

        public static string UserInfo(int userId) => $"User:{userId}:Info";

        public static string UserLoginToken(int userId) => $"User:{userId}:LoginToken";

        public static string LoginToken(string token) => $"LoginToken:{token}";

        #endregion
    }
}
