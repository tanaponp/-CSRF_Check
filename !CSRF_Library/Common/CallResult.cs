using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Common
{
    public class CallResult
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Data { get; set; }
        public object AdditionalData { get; set; }
    }

    public class CallResult<T>
    {
        public int Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public T Data { get; set; }
        public object AdditionalData { get; set; }
    }

    public class CallStatus
    {
        public int Status { get; set; }
        public string StatusText { get; set; }
    }

    public class CallStatusWithReturnValue
    {
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string ReturnValue { get; set; }
    }
}
