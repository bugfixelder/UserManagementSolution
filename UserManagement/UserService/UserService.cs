using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using UserService.Data;

namespace UserService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class UserService : IUserService
    {
        private static readonly ConcurrentDictionary<Guid, IUserCallback> _callbacks = new ConcurrentDictionary<Guid, IUserCallback>();
        private static readonly Timer _timer;
        private static List<User> _users = new List<User>()
        {
            new User{Id = 1, Name = "Nam"},
            new User{Id = 2, Name = "Lan" }
        };

        private static readonly object _lock = new object();

        static UserService()
        {
            _timer = new Timer(TimerCallback, null, 0, 10000);
        }

        public UserService()
        {
            
        }

        private static async void TimerCallback(object state)
        {
            var randomStatus = (UserStatus)new Random().Next(0, 2);
            Console.WriteLine($"Callback invoking: new status = {randomStatus}");

            foreach (var callback in _callbacks.Values)
            {
                if (callback != null)
                {
                    var communicationObject = callback as ICommunicationObject;
                    if (communicationObject != null && communicationObject.State == CommunicationState.Opened)
                    {
                        try
                        {
                            await callback.OnUserStatusChanged(randomStatus);
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
                else
                {
                    Console.WriteLine("Callback is null, no active connection");
                    _callbacks.TryRemove(GetKeyForCallback(callback), out _);
                }
            }
            

            Console.WriteLine($"TimerCallback END");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await Task.Delay(100); // Giả lập công việc bất đồng bộ
            lock (_lock)
            {
                return _users; 
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            await Task.Delay(100);
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Id == id); 
            }
        }

        public async Task AddUserAsync(User user)
        {
            await Task.Delay(100);
            lock (_lock)
            {
                user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(user); 
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            await Task.Delay(100);
            lock (_lock)
            {
                var existing = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null) existing.Name = user.Name; 
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            await Task.Delay(100);
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user != null) _users.Remove(user); 
            }
        }

        private static Guid GetKeyForCallback(IUserCallback callback)
        {
            return _callbacks.FirstOrDefault(kvp => kvp.Value == callback).Key;
        }
        public async Task SubscribeAsync()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IUserCallback>();
            _callbacks.TryAdd(Guid.NewGuid(), callback);
        }

        public async Task UnsubscribeAsync()
        {
            var callback = OperationContext.Current.GetCallbackChannel<IUserCallback>();
            var key = GetKeyForCallback(callback);
            _callbacks.TryRemove(key, out _);            
        }
    }
}
