using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using UserClient.UserServiceProxy;

namespace UserClient.Infrastructures
{
    public interface IUserServiceProxy
    {
        event EventHandler<UserStatus> UserStatusChanged;
        Task<User[]> GetAllUsersAsync();
        Task<User> GetUserAsync(int id);
        void AddUser(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task SubscribeAsync();
        Task UnsubscribeAsync();
        //void SetCallbackHandler(IUserServiceCallback callbackHandler);
    }

    public class UserServiceProxy : IUserServiceProxy, IUserServiceCallback, IDisposable
    {
        public event EventHandler<UserStatus> UserStatusChanged;
        private readonly UserServiceClient _userServiceClient;

        public UserServiceProxy()
        {
            _userServiceClient = new UserServiceClient(new InstanceContext(this));
        }
        public Task<User[]> GetAllUsersAsync()
        {
            return _userServiceClient.GetAllUsersAsync();
        }

        public Task<User> GetUserAsync(int id)
        {
            return _userServiceClient.GetUserAsync(id);
        }

        public void AddUser(User user)
        {
            _userServiceClient.AddUser(user);
        }

        public Task UpdateUserAsync(User user)
        {
            return _userServiceClient.UpdateUserAsync(user);
        }

        public Task DeleteUserAsync(int id)
        {
            return _userServiceClient.DeleteUserAsync(id);
        }

        public Task SubscribeAsync()
        {
            return _userServiceClient.SubscribeAsync();
        }

        public Task UnsubscribeAsync()
        {
            return _userServiceClient.UnsubscribeAsync();
        }

        public void OnUserStatusChanged(UserStatus status)
        {
            UserStatusChanged?.Invoke(this, status);
        }

        public void Dispose()
        {
            _userServiceClient?.Close();
        }
    }
}
