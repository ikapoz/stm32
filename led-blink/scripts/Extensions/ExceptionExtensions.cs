using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scripts.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToJson(this Exception ex)
        {
            var data = new ExceptionData();
            return JsonSerializer.Serialize(AddException(data, ex));
        }

        private static ExceptionData AddException(ExceptionData exceptionData, Exception ex)
        {
            exceptionData.Message = ex.Message;
            exceptionData.StackTrace = ex.StackTrace ?? String.Empty;
            var innerException = ex.InnerException;

            while (innerException != null)
            {
                var innerExData = new ExceptionData
                {
                    Message = innerException.Message,
                    StackTrace = innerException.StackTrace ?? String.Empty
                };
                exceptionData.InnerException.Add(innerExData);
                innerException = innerException.InnerException;
            }

            return exceptionData;
        }
    }

    public class ExceptionData
    {
        public string Message { get; set; } = string.Empty;

        public string StackTrace { get; set; } = string.Empty;

        public List<ExceptionData> InnerException { get; set; } = new List<ExceptionData>();
    }
}
