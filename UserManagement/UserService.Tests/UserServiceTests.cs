using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UserService.Data;
using Xunit;

namespace UserService.Tests
{
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IUserCallback> _callbackMock;
        private Guid _callbackKey;
        private Mock<ICallbackChannelProvider> _channelProviderMock;
        private Mock<ICallbackManager> _callbackManagerMock;

        public UserServiceTests()
        {
            _channelProviderMock = new Mock<ICallbackChannelProvider>();
            _callbackManagerMock = new Mock<ICallbackManager>();
            _userService = new UserService(_channelProviderMock.Object, _callbackManagerMock.Object);
            _callbackMock = new Mock<IUserCallback>();
            _callbackKey = Guid.NewGuid();

            ResetState();
        }

        private void ResetState()
        {
            lock (UserService.Lock)
            {
                UserService.Users.Clear();
                UserService.Users.Add(new Data.User { Id = 1, Name = "Nam" });
                UserService.Users.Add(new Data.User { Id = 2, Name = "Lan" });
            }
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Act
            var users = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Contains(users, u => u.Id == 1 && u.Name == "Nam");
            Assert.Contains(users, u => u.Id == 2 && u.Name == "Lan");
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUserById()
        {
            // Act
            var user = await _userService.GetUserAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("Nam", user.Name);

            // Test khi không tìm thấy user
            var nonExistentUser = await _userService.GetUserAsync(999);
            Assert.Null(nonExistentUser);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddNewUser()
        {
            // Arrange
            var newUser = new User { Name = "NewUser" };

            // Act
            await _userService.AddUserAsync(newUser);

            // Assert
            var users = await _userService.GetAllUsersAsync();
            Assert.NotNull(users);
            Assert.Equal(3, users.Count); // Kiểm tra có 3 user sau khi thêm
            var addedUser = users.FirstOrDefault(u => u.Name == "NewUser");
            Assert.NotNull(addedUser);
            Assert.Equal(3, addedUser.Id); // Kiểm tra ID tự tăng
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateExistingUser()
        {
            // Arrange
            var userToUpdate = new User { Id = 1, Name = "UpdatedNam" };

            // Act
            await _userService.UpdateUserAsync(userToUpdate);

            // Assert
            var updatedUser = await _userService.GetUserAsync(1);
            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedNam", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserById()
        {
            // Act
            await _userService.DeleteUserAsync(1);

            // Assert
            var users = await _userService.GetAllUsersAsync();
            Assert.NotNull(users);
            Assert.Single(users); // Kiểm tra chỉ còn 1 user
            Assert.DoesNotContain(users, u => u.Id == 1); // Kiểm tra user Id 1 đã bị xóa
        }

        [Fact]
        public async Task SubscribeAsync_ShouldCallCallbackManagerWithCorrectCallback()
        {
            // Act
            await _userService.SubscribeAsync();

            // Assert
            _callbackManagerMock.Verify(
                m => m.SubscribeAsync(_callbackMock.Object).GetAwaiter(), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeAsync_ShouldCallCallbackManagerWithCorrectCallback()
        {
            // Act
            await _userService.UnsubscribeAsync();

            // Assert
            _callbackManagerMock.Verify(m => m.UnsubscribeAsync(_callbackMock.Object), Times.Once);

        }
    }
}
