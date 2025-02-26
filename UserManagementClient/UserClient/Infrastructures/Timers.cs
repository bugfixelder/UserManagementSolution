using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UserClient.Infrastructures
{
    public interface ITimerWrapper
    {
        void Dispose();
        void Change(int dueTime, int period);
        void Start(TimerCallback callback, object state, int dueTime, int period);
    }
    internal class TimerWrapper : ITimerWrapper
    {
        private Timer _timer;

        public TimerWrapper()
        {
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            throw new NotImplementedException();
        }

        public void Change(int dueTime, int period)
        {
            _timer?.Change(dueTime, period);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start(TimerCallback callback, object state, int dueTime, int period)
        {
            _timer = new Timer(callback, state, dueTime, period);
        }
    }
}
