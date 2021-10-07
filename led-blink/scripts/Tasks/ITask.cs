using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Scripts.Tasks
{
    internal interface ITask : IDisposable
    {
        public Task<bool> ExecuteAsync(ILogger logger);
    }
}
