using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scripts.Threading
{
    internal class SimpleWaitPulse
    {
        public void Wait()
        {
            lock (locker)
                Monitor.Wait(locker);
        }

        public void Pulse()
        {
            lock (locker)
                Monitor.Pulse(locker);
        }

        private readonly object locker = new object();
    }
}
