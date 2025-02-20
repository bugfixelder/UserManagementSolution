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

        public void DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            { 
                _users.Remove(user); 
            }
        }

        void IUserService.AddUser(User user)
        {
            user.Id = _users.Any() ? _users.Max(x => x.Id) + 1 : 1;
        }

        List<User> IUserService.GetAllUsers()
        {
            return _users;
        }

        Task<List<User>> IUserService.GetAllUsersAsync()
        {
            return Task.FromResult(_users);
        }

        User IUserService.GetUser(int id)
        {
            return _users.FirstOrDefault(_x => _x.Id == id);
        }

        void IUserService.UpdateUser(User user)
        {
            var existingUser = _users.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
            }
        }
    }
}
