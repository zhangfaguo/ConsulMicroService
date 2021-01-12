using System.ComponentModel;

namespace Zfg.Core.Common
{
    public class ServiceResult
    {
        public ExecutResultCode Code
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public ExecutResultCode ResultCode
        {
            get
            {
                return Code;
            }
        }
        public ServiceResult() { }

        public ServiceResult(ExecutResultCode code)
        {
            this.Code = code;
        }

        public ServiceResult(ExecutResultCode code, string message)
        {
            this.Code = code;
            this.Message = message;
        }


        public void SetResult(ExecutResultCode code, string message = "")
        {
            this.Code = code;
            this.Message = message;
        }
    }


    public class ServiceResult<T> : ServiceResult
    {
        public ServiceResult()
           : base(ExecutResultCode.Fail)
        {

        }
        public ServiceResult(ExecutResultCode code)
            : base(code)
        {

        }

        public ServiceResult(ExecutResultCode code, string message)
          : base(code, message)
        {

        }
        public ServiceResult(ExecutResultCode code, string message, T data)
            : base(code, message)
        {
            this.Data = data;
        }


        public T Data { get; set; }
    }
    public enum ExecutResultCode
    {
        [Description("执行成功")]
        Success = 0,
        [Description("数据不存在")]
        Empty = 8,
        [Description("非法请求")]
        Invalid = 9,
        [Description("token无效")]
        InvalidToken = 401,
        [Description("未注册")]
        Unregistered = 402,
        //自定义
        Custom = 500,
        [Description("执行失败")]
        Fail = 99
    }
}
