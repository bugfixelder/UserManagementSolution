using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using UserService.Data;

namespace UserService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UserService : IUserService
    {
        private static IUserCallback _callback;
        private readonly Timer _timer;
        private static List<User> _users = new List<User>()
        {
            new User{Id = 1, Name = "Nam"},
            new User{Id = 2, Name = "Lan" }
        };

        public UserService()
        {
            _timer = new Timer(TimerCallback, null, 0, 10000);
        }

        private static async void TimerCallback(object state)
        {
            var randomStatus = (UserStatus)new Random().Next(0, 2);
            Console.WriteLine($"Callback invoking: new status = {randomStatus}");

            if (_callback != null)
            {
                var communicationObject = _callback as ICommunicationObject;
                if (communicationObject != null && communicationObject.State == CommunicationState.Opened)
                {
                    try
                    {
                        await _callback.OnUserStatusChanged(randomStatus);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in callback: {ex.Message}");
                        _callback = null; // Delete callback to prevent call next time
                    }
                }
                else
                {
                    Console.WriteLine("Callback channel is not open or has faulted");
                    _callback = null;
                }
            }
            else
            {
                Console.WriteLine("Callback is null, no active connection");
            }

            Console.WriteLine($"TimerCallback END");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await Task.Delay(100); // Giả lập công việc bất đồng bộ
            return _users;
        }

        public async Task<User> GetUserAsync(int id)
        {
            await Task.Delay(100);
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public async Task AddUserAsync(User user)
        {
            await Task.Delay(100);
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await Task.Delay(100);
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null) existing.Name = user.Name;
        }

        public async Task DeleteUserAsync(int id)
        {
            await Task.Delay(100);
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null) _users.Remove(user);
        }

        public async Task SubscribeAsync()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IUserCallback>();
        }
    }
}
