using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClient.Infrastructures
{
    public interface ITimerWrapper
    {
        void Dispose();
        void Change(int dueTime, int period);
        void Start(Action<object> callback, object state, int dueTime, int period);
    }
    internal class TimerWrapper : ITimerWrapper
    {
        public void Change(int dueTime, int period)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start(Action<object> callback, object state, int dueTime, int period)
        {
            throw new NotImplementedException();
        }
    }
}
