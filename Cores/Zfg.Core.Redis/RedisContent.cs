using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zfg.Core.Common;
using Zfg.Core.Common.Redis;

namespace Zfg.Core.Redis
{
    internal class RedisContent : IRedisContent
    {
        private static ConnectionMultiplexer redisCon;

        private const string prefix = "Zfg.Test";
        public string MsgQuoteDelayCommonChannel(string time) => $"Base:MessageChannel:MsgQuoteDelayCommonChannel:{time}";

        public RedisContent(ConfigurationOptions options)
        {
            //var options = new ConfigurationOptions
            //{
            //    Password = "198822",
            //    SyncTimeout = 5000,
            //    AbortOnConnectFail = false,
            //    ConnectTimeout = 15000,
            //    ResponseTimeout = 15000,
            //    EndPoints = {
            //                    { "127.0.0.1",6379 }
            //                },
            //    AllowAdmin = true,
            //};
            redisCon = ConnectionMultiplexer.Connect(options);
        }

        public ILogger Logger { get; set; }

        private IDatabase Connect(int db)
        {
            return redisCon.GetDatabase(db);
        }

        private void Exec(Action<IDatabase> query, int db)
        {
            query.Invoke(Connect(db));
        }
        private T Exec<T>(Func<IDatabase, T> query, int db)
        {
            return query.Invoke(Connect(db));
        }


        private string FormatKey(string key)
        {
            return $"{prefix}-{key}";
        }


        #region  Keys

        public bool HasKey(string key, int dbIndex = -1)
        {
            return Exec(db =>
            {
                return db.KeyExists(key);
            }, dbIndex);
        }

        public int TTL(string key, int dbIndex = -1)
        {
            return Exec(db =>
            {
                var timespan = db.KeyTimeToLive(FormatKey(key));
                if (timespan.HasValue)
                {
                    return (int)timespan.Value.TotalSeconds;
                }
                return 0;
            }, dbIndex);
        }


        public void SetTTL(string key, int seconds, int db = -1)
        {
            Exec(h =>
            {
                h.KeyExpire(FormatKey(key), new TimeSpan(0, 0, 0, seconds));
            }, db);
        }


        public void KeyDelete(string key, int dbIndex = -1)
        {
            Exec(db =>
            {
                db.KeyDelete(FormatKey(key));
            }, dbIndex);
        }


        public bool KeyExsit(string key, int dbIndex = -1)
        {
            return Exec<bool>(db =>
            {
                return db.KeyExists(FormatKey(key));
            }, dbIndex);
        }


        #endregion


        #region String
        public string StringGet(string key, int dbIndex = -1)
        {
            return Exec(db =>
            {
                return db.StringGet(FormatKey(key));
            }, dbIndex);
        }
        public string StringGet(string key, Func<string> load, int expireSecond = 600, int dbIndex = -1)
        {
            return Exec(db =>
            {
                var val = db.StringGet(FormatKey(key));
                if (
#if DEBUG
                true ||
#endif
                string.IsNullOrEmpty(val))
                {
                    val = load.Invoke();
                    if (!string.IsNullOrEmpty(val))
                    {
                        expireSecond = expireSecond < 0 ? 600 : expireSecond;
                        if (expireSecond == 0)
                        {
                            db.StringSet(FormatKey(key), val);
                        }
                        else
                        {
                            db.StringSet(FormatKey(key), val,
                                new TimeSpan(0, expireSecond / 60, expireSecond % 60));
                        }

                    }
                }
                return val.ToString();
            }, dbIndex);
        }
        public void StringSet(string key, string value, int expireSecond = 600, int dbIndex = -1)
        {
            expireSecond = expireSecond < 0 ? 600 : expireSecond;
            Exec(db =>
            {
                if (expireSecond == 0)
                {
                    db.StringSet(FormatKey(key), value);
                }
                else
                {
                    db.StringSet(FormatKey(key), value,
                        new TimeSpan(0, expireSecond / 60, expireSecond % 60));
                }

            }, dbIndex);
        }
        public T Lock<T>(string key, Func<T> executor, int dbIndex = 2)
        {
            return Exec(db =>
            {
                var guid = Guid.NewGuid().ToString("N");
                var tick = 0;
                while (tick < 2000)
                {
                    if (db.LockTake(FormatKey(key), guid, TimeSpan.FromSeconds(8)))
                    {
                        try
                        {
                            return executor.Invoke();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            db.LockRelease(FormatKey(key), guid);
                        }
                    }
                    else
                    {
                        tick++;
                        Thread.Sleep(10);
                    }
                }
                throw new TimeoutException();
            }, dbIndex);
        }
        /// <summary>
        /// 分布式锁运行
        /// </summary>
        /// <param name="excutor"></param>
        public void Lock(string key, Action excutor, int dbIndex = 2)
        {
            Exec(db =>
            {
                var guid = Guid.NewGuid().ToString("N");
                var tick = 0;
                while (tick < 2000)
                {
                    if (db.LockTake(FormatKey(key), guid, TimeSpan.FromSeconds(8)))
                    {
                        try
                        {
                            excutor?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            db.LockRelease(FormatKey(key), guid);
                        }
                        break;
                    }
                    else
                    {
                        tick++;
                        Thread.Sleep(10);
                    }
                }
                if (tick >= 2000)
                {
                    throw new TimeoutException();
                }
            }, dbIndex);
        }


        public int Increment(string key, int step, int dbIndex = -1)
        {
            return Exec<int>(db =>
            {
                return (int)db.StringIncrement(FormatKey(key), step);
            }, dbIndex);
        }

        public int Decrement(string key, int step, int dbIndex = -1)
        {
            return Exec<int>(db =>
            {
                return (int)db.StringDecrement(FormatKey(key), step);
            }, dbIndex);
        }


        #endregion

        #region Object

        public T ObjectGet<T>(string key, int dbIndex = -1) where T : class
        {
            var data = StringGet(key, dbIndex);
            if (!string.IsNullOrEmpty(data))
            {
                return data.ToObject<T>();
            }
            return default(T);
        }
        public T ObjectGet<T>(string key, Func<T> load, int expireSecond = 600, int dbIndex = -1) where T : class
        {
            var data = Exec(db =>
            {
                var val = db.StringGet(FormatKey(key));
                if (string.IsNullOrEmpty(val))
                {
                    val = load.Invoke()?.ToJson();
                    if (!string.IsNullOrEmpty(val))
                    {
                        expireSecond = expireSecond < 0 ? 600 : expireSecond;
                        if (expireSecond == 0)
                        {
                            db.StringSet(FormatKey(key), val);
                        }
                        else
                            db.StringSet(FormatKey(key), val,
                                new TimeSpan(0, expireSecond / 60, expireSecond % 60));
                    }
                }
                return val.ToString();
            }, dbIndex);

            if (!string.IsNullOrEmpty(data))
            {
                return data.ToObject<T>();
            }
            return default(T);
        }
        public void ObjectSet<T>(string key, T value, int expireSecond = 600, int dbIndex = -1) where T : class
        {
            StringSet(key, value.ToJson(), expireSecond, dbIndex);
        }
        #endregion

        #region Hash
        public void HashSet(string key, string field, string value, int dbIndex = -1)
        {
            Exec(db =>
            {
                db.HashSet(FormatKey(key), field, value);
            }, dbIndex);
        }
        public Dictionary<string, string> HashGet(string key, string field = "", int dbIndex = -1)
        {
            return Exec(db =>
            {
                if (!string.IsNullOrEmpty(field))
                {
                    var val = db.HashGet(FormatKey(key), field);
                    return new Dictionary<string, string>(){
                         { field, val }
                    };
                }
                return db.HashGetAll(FormatKey(key))?.ToDictionary(
                    k => k.Name.ToString(),
                    v => v.Value.ToString()
                );
            }, dbIndex);
        }
        #endregion

        #region Quote


        /// <summary>
        /// 异步执行并返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task<ServiceResult<string>> PushRun(string key, string msg, string cmd, bool isBack = false)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                cmd = key;
            }

            var innerMsg = new PushMsg
            {
                Key = Guid.NewGuid().ToString(),
                Command = cmd,
                Body = msg,
                Time = DateTime.Now,
                IsBack = isBack
            };
            var ts = new TaskCompletionSource<ServiceResult<string>>();
            try
            {
                Exec(db =>
                {
                    db.ListLeftPush(FormatKey(key), innerMsg.ToJson());
                    if (isBack)
                    {
                        var searchLoop = 0;
                        var backKey = FormatKey($"System:AsyncExcutor_Back:{innerMsg.Key}");
                        while (searchLoop < 100)
                        {
                            if (db.KeyExists(backKey))
                            {
                                var rstMsg = (string)db.StringGet(backKey);
                                Logger?.Write($"excutor msg rst:{rstMsg}");
                                Console.WriteLine(rstMsg);
                                ts.SetResult(rstMsg.ToObject<ServiceResult<string>>());
                                db.KeyDelete(backKey);
                                break;
                            }
                            else
                            {
                                searchLoop++;
                                Thread.Sleep(100);
                            }
                        }
                        if (searchLoop >= 100)
                        {
                            Console.WriteLine("timer out");
                            ts.SetException(new TimeoutException());
                        }
                    }
                    else
                    {
                        ts.SetResult(new ServiceResult<string>(ExecutResultCode.Success));
                    }
                }, 2);
            }
            catch (Exception ex)
            {
                ts.SetException(ex);
            }
            return ts.Task;
        }


        /// <summary>
        /// 异步执行并返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void PushDelayRun(string key, string msg, string cmd, int delayMinute)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                cmd = key;
            }
            var time = DateTime.Now.AddMinutes(delayMinute);
            var sencond = time.Second / 10 * 10;
            var timeKey = time.ToString("yyyyMMddHHmm") + sencond;
            var innerMsg = new PushMsg
            {
                Key = key,
                Command = cmd,
                Body = msg,
                Time = DateTime.Now,
                IsBack = false
            };

            Exec(db =>
            {
                db.ListLeftPush(FormatKey(MsgQuoteDelayCommonChannel(timeKey)), innerMsg.ToJson());
            }, 2);

        }

        public void PullExcutor(string key, Func<string, string, string> excutor, CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (token != null && token != CancellationToken.None)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    Exec(db =>
                    {
                        var msg = db.ListRightPopLeftPush(FormatKey(key), FormatKey(key + "_back"));
                        var msgObject = ((string)msg)?.ToObject<PushMsg>();
                        if (msgObject != null)
                        {

                            try
                            {
                                var rst = excutor?.Invoke(msgObject.Body, msgObject.Command);
                                var sr = new ServiceResult<string>(ExecutResultCode.Success);
                                sr.Data = rst;
                                db.ListRemove(FormatKey(key + "_back"), msg);
                                if (msgObject.IsBack)
                                {
                                    var backKey = FormatKey($"System:AsyncExcutor_Back:{msgObject.Key}");
                                    db.StringSet(backKey, sr.ToJson(), TimeSpan.FromSeconds(21));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger?.Write(ex);
                                var sr = new ServiceResult<string>(ExecutResultCode.Fail);
                                sr.Data = ex.Message;
                                if (msgObject.IsBack)
                                {
                                    var backKey = FormatKey($"System:AsyncExcutor_Back:{msgObject.Key}");
                                    db.StringSet(backKey, sr.ToJson(), TimeSpan.FromSeconds(21));
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(200);
                        }
                    }, 2);
                }
            });
        }

        #endregion

        #region List

        public void Push(string key, string value, int dbIndex = 4)
        {
            Exec(db =>
            {
                db.ListRightPush(FormatKey(key), value);

            }, dbIndex);
        }


        public void Push(string key, string value, int maxLength, int dbIndex = 4)
        {
            Exec(db =>
            {
                var skey = FormatKey(key);
                var batch = db.CreateBatch();
                batch.ListLeftPushAsync(FormatKey(key), value);
                batch.ListTrimAsync(skey, 0, maxLength - 1);
                batch.Execute();
            }, dbIndex);
        }
        public List<T> Pops<T>(string key, int takeCount = 100, int dbIndex = 4)

        {
            var list = Pops(key, takeCount, dbIndex);
            return list.Select(t => t.ToObject<T>()).ToList();
        }

        public List<string> Pops(string key, int takeCount = 100, int dbIndex = 4)
        {
            return Exec(db =>
            {
                var skey = FormatKey(key);

                var batch = db.CreateBatch();
                var ranges = batch.ListRangeAsync(skey, 0, takeCount - 1);
                batch.ListTrimAsync(skey, takeCount, -1);
                batch.Execute();
                Task.WaitAll(ranges);
                var list = ranges.Result;
                var rList = new List<string>();
                if (list != null && list.Length > 0)
                {

                    rList = list.Select(t => (string)t).ToList();
                }
                return rList;

            }, dbIndex);
        }


        public List<string> Ranges(string key, int start = 0, int end = -1, int dbIndex = 4)
        {
            return Exec(db =>
            {
                var skey = FormatKey(key);

                var list = db.ListRange(skey);
                return list.Select(t => (string)t).ToList();
            }, dbIndex);
        }

        public long ListLength(string key, int dbIndex = 4)
        {
            return Exec(db =>
            {
                var skey = FormatKey(key);
                return db.ListLength(skey);
            }, dbIndex);
        }

        #endregion
    }
}
