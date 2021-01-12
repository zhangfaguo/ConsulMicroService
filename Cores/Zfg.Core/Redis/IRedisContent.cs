using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zfg.Core.Common.Redis
{
    public interface IRedisContent
    {

        #region  Keys

        bool HasKey(string key, int dbIndex = -1);

        int TTL(string key, int dbIndex = -1);


        void SetTTL(string key, int seconds, int db = -1);


        void KeyDelete(string key, int dbIndex = -1);


        bool KeyExsit(string key, int dbIndex = -1);


        #endregion


        #region String
        string StringGet(string key, int dbIndex = -1);
        string StringGet(string key, Func<string> load, int expireSecond = 600, int dbIndex = -1);
        void StringSet(string key, string value, int expireSecond = 600, int dbIndex = -1);
        T Lock<T>(string key, Func<T> executor, int dbIndex = 2);
        /// <summary>
        /// 分布式锁运行
        /// </summary>
        /// <param name="excutor"></param>
        void Lock(string key, Action excutor, int dbIndex = 2);


        int Increment(string key, int step, int dbIndex = -1);

        int Decrement(string key, int step, int dbIndex = -1);


        #endregion

        #region Object

        T ObjectGet<T>(string key, int dbIndex = -1) where T : class;
        T ObjectGet<T>(string key, Func<T> load, int expireSecond = 600, int dbIndex = -1) where T : class;
        void ObjectSet<T>(string key, T value, int expireSecond = 600, int dbIndex = -1) where T : class;
        #endregion

        #region Hash
        void HashSet(string key, string field, string value, int dbIndex = -1);
        Dictionary<string, string> HashGet(string key, string field = "", int dbIndex = -1);
        #endregion

        #region Quote


        /// <summary>
        /// 异步执行并返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task<ServiceResult<string>> PushRun(string key, string msg, string cmd, bool isBack = false);


        /// <summary>
        /// 异步执行并返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        void PushDelayRun(string key, string msg, string cmd, int delayMinute);

        void PullExcutor(string key, Func<string, string, string> excutor, CancellationToken token);

        #endregion

        #region List

        void Push(string key, string value, int dbIndex = 4);

        void Push(string key, string value, int maxLength, int dbIndex = 4);
        List<T> Pops<T>(string key, int takeCount = 100, int dbIndex = 4);

        List<string> Pops(string key, int takeCount = 100, int dbIndex = 4);


        List<string> Ranges(string key, int start = 0, int end = -1, int dbIndex = 4);

        long ListLength(string key, int dbIndex = 4);

        #endregion
    }
}
