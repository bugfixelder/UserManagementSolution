using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly Timer _timer;
        private static List<User> _users = new List<User>()
        {
            new User{Id = 1, Name = "Nam"},
            new User{Id = 2, Name = "Lan" }
        };

        internal static List<User> Users
        {
            get
            {
                return _users;
            }
        }

        private static readonly object _lock = new object();

        internal static object Lock
        {

            get
            {
                return _lock;
            }
        }

        private readonly ICallbackChannelProvider _callbackChannelProvider;
        private readonly ICallbackManager _callbackManager;
        public UserService() : this(new CallbackChannelProvider(), new CallbackManager())
        {
            
        }

        public UserService(ICallbackChannelProvider callbackChannelProvider, ICallbackManager callbackManager)
        {
            _callbackChannelProvider = callbackChannelProvider;
            _callbackManager = callbackManager;
            _timer = new Timer(TimerCallback, null, 0, 10000);
        }

        private async void TimerCallback(object state)
        {
            var randomStatus = (UserStatus)new Random().Next(0, 2);
            Console.WriteLine($"Callback invoking: new status = {randomStatus}");

            var service = state as UserService;
            await _callbackManager.NotifyAllCallbacksAsync(randomStatus).ConfigureAwait(false);

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

        public async Task SubscribeAsync()
        {
            var callback = _callbackChannelProvider.GetCallbackChannel();
            _callbackManager.SubscribeAsync(callback);
        }

        public async Task UnsubscribeAsync()
        {
            var callback = _callbackChannelProvider.GetCallbackChannel();
            _callbackManager.UnsubscribeAsync(callback);
        }
    }
}
