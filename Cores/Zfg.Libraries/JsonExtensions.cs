using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace System
{
    public static class JsonExtensions
    {
        public static T ToObject<T>(this string inputStr)
        {
            try
            {
                if (String.IsNullOrEmpty(inputStr))
                {
                    return default(T);
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(inputStr);
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return default(T);
            }
        }


        public static string ToJson(this object o, DefaultContractResolver resolver)
        {
            if (o == null)
            {
                return String.Empty;
            }
            if (resolver != null)
            {
                JsonSerializerSettings jsetting = new JsonSerializerSettings()
                {
                    //忽略循环引用，如果设置为Error，则遇到循环引用的时候报错
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ContractResolver = resolver,
                    //格式化日期时间
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return JsonConvert.SerializeObject(o, jsetting);
            }
            return JsonConvert.SerializeObject(o);
        }

        public static string ToJson(this object o)
        {
            return ToJson(o, new DefaultContractResolver());
        }
    }
}
