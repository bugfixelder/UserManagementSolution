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
    public enum UserStatus
    {
        Active,
        Inactive,
        Pending
    }

    [ServiceContract]
    public interface IUserCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnUserStatusChanged(UserStatus status);
    }
    
    [ServiceContract(CallbackContract = typeof(IUserCallback))]
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

        [OperationContract]
        Task SubscribeAsync();
    }
}
