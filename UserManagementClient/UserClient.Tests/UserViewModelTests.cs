using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UserClient.Infrastructures;
using UserClient.UserServiceProxy;
using Xunit;

namespace UserClient.Tests
{
    public class UserViewModelTests
    {
        private UserViewModel _viewModel;
        private Mock<IUserService> _userServiceMock;
        private Mock<IDispatcherwWrapper> _dispatcherMock;
        private Mock<Timer> _timerMock;
        private Mock<IUserServiceCallback> _callbackMock;

        public UserViewModelTests()
        {
            // Mock dependencies
            _userServiceMock = new Mock<IUserService>();
            _dispatcherMock = new Mock<IDispatcherwWrapper>();
            _timerMock = new Mock<Timer>(MockBehavior.Loose);
            _callbackMock = new Mock<IUserServiceCallback>();

            // Khởi tạo UserViewModel với mocks
            _viewModel = new UserViewModel(_dispatcherMock.Object);

            // Setup timer để không thực sự chạy (mock behavior)
            _timerMock.Setup(t => t.Dispose()).Verifiable();

            // Inject timer vào view model (cần refactor UserViewModel để hỗ trợ injection)
            typeof(UserViewModel).GetField("_timer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(_viewModel, _timerMock.Object);

            // Setup UserService trong view model (cần refactor để inject IUserService)
            typeof(UserViewModel).GetField("_userService", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(_viewModel, _userServiceMock.Object);
        }

        [Fact]
        public async Task InitializeAsync_ShouldSubscribedAndLoadUsers()
        {
            // Arrange
            var users = new List<User>()
            {
                new User { Id = 1, Name = "Nam" },
                new User { Id = 2, Name = "Lan" }

            };

            _userServiceMock.Setup(s => s.SubscribeAsync()).Returns(Task.CompletedTask);
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).Returns(Task.FromResult(users.ToArray()));
            _dispatcherMock.Setup(d => d.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
                .Callback<Action, DispatcherPriority>((action, priority) => action());

            // Act
            await _viewModel.InitializeAsync();

            // Assert
            _userServiceMock.Verify(s => s.SubscribeAsync(), Times.Once());
            _userServiceMock.Verify(s => s.GetAllUsersAsync(), Times.Once());
            Assert.NotNull(_viewModel.Users);
            Assert.Equal(2, _viewModel.Users.Count);
            Assert.Contains(_viewModel.Users, x =>x.Id == 1 && x.Name == "Nam");
            Assert.Contains(_viewModel.Users, x =>x.Id == 2 && x.Name == "Lan");
            Assert.True(_viewModel.IsSubscribedForTestOnly);
         }
    }
}
