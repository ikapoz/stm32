using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Tasks
{
    internal class Result<T>
    {
        public Result(T data)
        {
            Data = data;
            Success = true;
        }

        public Result()
        {
            Data = default;
            Success = false;
        }

        public bool Success { get; set; }
        public T? Data { get; private set; }
    }
}
