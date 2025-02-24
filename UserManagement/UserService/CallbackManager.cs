using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace UserService
{
    // Interface định nghĩa các phương thức quản lý callback
    public interface ICallbackManager
    {
        Task SubscribeAsync(IUserCallback callback);
        Task UnsubscribeAsync(IUserCallback callback);
        Task NotifyAllCallbacksAsync(UserStatus status);
    }

    // Lớp triển khai quản lý callback
    public class CallbackManager : ICallbackManager
    {
        private readonly ConcurrentDictionary<Guid, IUserCallback> _callbacks = new ConcurrentDictionary<Guid, IUserCallback>();

        public async Task SubscribeAsync(IUserCallback callback)
        {
            await Task.Delay(100); // Giả lập công việc bất đồng bộ nếu cần
            _callbacks.TryAdd(Guid.NewGuid(), callback);
        }

        public async Task UnsubscribeAsync(IUserCallback callback)
        {
            await Task.Delay(100); // Giả lập công việc bất đồng bộ nếu cần
            var key = GetKeyForCallback(callback);
            _callbacks.TryRemove(key, out _);
        }

        public async Task NotifyAllCallbacksAsync(UserStatus status)
        {
            foreach (var callback in _callbacks.Values)
            {
                if (callback != null)
                {
                    var communicationObject = callback as ICommunicationObject;
                    if (communicationObject != null && communicationObject.State == CommunicationState.Opened)
                    {
                        try
                        {
                            await callback.OnUserStatusChanged(status);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error in callback: {ex.Message}");
                            _callbacks.TryRemove(GetKeyForCallback(callback), out _);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Callback channel is not open or has faulted");
                        _callbacks.TryRemove(GetKeyForCallback(callback), out _);
                    }
                }
            }
        }

        private Guid GetKeyForCallback(IUserCallback callback)
        {
            return _callbacks.FirstOrDefault(kvp => kvp.Value == callback).Key;
        }
    }
}