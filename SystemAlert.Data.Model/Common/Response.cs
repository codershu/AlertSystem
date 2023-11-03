using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Model.Common
{
    public class Response<T> : Common.Model.Response<T>
    {
        public Response()
        {
        }

        public Response(Common.Model.Response<T> response) : base(response)
        {
        }

        private Response(Common.Model.Response response) : base(response)
        {
        }

        public static Response<T> Ok(ResponseCode.CodeNumbers codeNumber, Stopwatch stopwatch = null)
        {
            var code = new ResponseCode(codeNumber);
            return new Response<T>(Common.Model.Response.Ok<T>(code, default, stopwatch));
        }

        public static Response<T> Ok(ResponseCode.CodeNumbers codeNumber, T result, Stopwatch stopwatch = null)
        {
            var code = new ResponseCode(codeNumber);
            return new Response<T>(Common.Model.Response.Ok<T>(code, result, stopwatch));
        }

        public static Response<T> Error(ResponseCode.CodeNumbers codeNumber, string errorMessage = "", Stopwatch stopwatch = null)
        {
            var code = new ResponseCode(codeNumber);
            return new Response<T>(Common.Model.Response.Error(code, errorMessage, stopwatch));
        }


        public static new Response<T> Error(Exception ex, Stopwatch stopwatch = null)
        {
            return new Response<T>(Common.Model.Response.Error(ex, stopwatch));
        }
    }
}
