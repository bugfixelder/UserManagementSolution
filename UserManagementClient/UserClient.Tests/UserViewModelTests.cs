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
        private readonly UserViewModel _viewModel;
        private readonly Mock<IUserServiceProxy> _userServiceProxyMock;
        private readonly Mock<IDispatcherwWrapper> _dispatcherMock;
        private Mock<IUserServiceCallback> _callbackMock;
        private readonly Mock<ITimerWrapper> _timerWrapper;

        public UserViewModelTests()
        {
            // Mock dependencies
            _userServiceProxyMock = new Mock<IUserServiceProxy>();
            _dispatcherMock = new Mock<IDispatcherwWrapper>();
            _callbackMock = new Mock<IUserServiceCallback>();
            _timerWrapper = new Mock<ITimerWrapper>();

            // Setup timer để không thực sự chạy (mock behavior)
            _timerWrapper.Setup(t => t.Dispose()).Verifiable();
            _timerWrapper.Setup(t => t.Start(It.IsAny<TimerCallback>(), 
                    It.IsAny<object>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<TimerCallback, object, int, int>((callback, state, dueTime, period) => callback(state));

            // Khởi tạo UserViewModel với mocks
            _viewModel = new UserViewModel(_timerWrapper.Object, _userServiceProxyMock.Object, _dispatcherMock.Object);
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

            _userServiceProxyMock.Setup(s => s.SubscribeAsync()).Returns(Task.CompletedTask);
            _userServiceProxyMock.Setup(s => s.GetAllUsersAsync()).Returns(Task.FromResult(users.ToArray()));
            
            // Act
            await _viewModel.InitializeAsync();

            // Assert
            _userServiceProxyMock.Verify(s => s.SubscribeAsync(), Times.Once());
            _userServiceProxyMock.Verify(s => s.GetAllUsersAsync(), Times.Once());
            Assert.NotNull(_viewModel.Users);
            Assert.Equal(2, _viewModel.Users.Count);
            Assert.Contains(_viewModel.Users, x =>x.Id == 1 && x.Name == "Nam");
            Assert.Contains(_viewModel.Users, x =>x.Id == 2 && x.Name == "Lan");
            Assert.True(_viewModel.IsSubscribedForTestOnly);
         }

        [Fact]
        public async Task UserServiceProxyOnUserStatusChanged_ShouldUpdateLogText()
        {
            // Arrange
            _userServiceProxyMock.SetupAdd(s => 
                s.UserStatusChanged += It.IsAny<EventHandler<UserStatus>>())
                .Callback<EventHandler<UserStatus>>(handler => handler(this, UserStatus.Active))
                .Verifiable();

            _dispatcherMock.Setup(d => 
                d.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
                .Callback<Action, DispatcherPriority>((action, priority) => action());

            // Act
            _userServiceProxyMock.Raise(p => p.UserStatusChanged += null, this, UserStatus.Active);

            // Assert
            Assert.Contains("new status: Active", _viewModel.LogText);
            _dispatcherMock.Verify(d =>
                d.InvokeAsync(It.Is<Action>(a => a != null), It.IsAny<DispatcherPriority>()), Times.Once);
        }
    }
}
