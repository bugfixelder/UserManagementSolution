using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UserService.Data;

namespace UserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class UserService : IUserService
    {
        private static List<User> _users = new List<User>()
        {
            new User{Id = 1, Name = "Nam"},
            new User{Id = 2, Name = "Lan" }
        };

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
    }
}
