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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Task<List<User>> GetAllUsersAsync();

        [OperationContract]
        Task<User> GetUserAsync(int id);

        [OperationContract]
        Task AddUserAsync(User user);

        [OperationContract]
        Task UpdateUserAsync(User user);

        [OperationContract]
        Task DeleteUserAsync(int id);
    }
}
