using System;
using System.Windows.Threading;

namespace UserClient.Infrastructures
{
    public interface IDispatcherwWrapper
    {
        DispatcherOperation InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal);
    }

    public class DispatcherWrapper : IDispatcherwWrapper
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherWrapper(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public DispatcherOperation InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return _dispatcher.InvokeAsync(action, priority);
        }
    }
}
