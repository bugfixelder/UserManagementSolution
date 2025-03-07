﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using UserClient.Infrastructures;
using UserClient.UserServiceProxy;

namespace UserClient
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private readonly ITimerWrapper _timerWrapper;
        private readonly IUserServiceProxy _userServiceProxy;
        public IDispatcherwWrapper Dispatcher { get; }

        public UserViewModel(ITimerWrapper timerWrapper, IUserServiceProxy userService, IDispatcherwWrapper dispatcherWrapper)
        {
            _userServiceProxy = userService ?? throw new ArgumentNullException(nameof(userService));
            Dispatcher = dispatcherWrapper;

            _userServiceProxy.UserStatusChanged -= UserServiceProxyOnUserStatusChanged;
            _userServiceProxy.UserStatusChanged += UserServiceProxyOnUserStatusChanged;

            AddCommand = new RelayCommand(AddUser);
            UpdateCommand = new RelayCommand(UpdateUser);
            DeleteCommand = new RelayCommand(DeleteUser);
            _timerWrapper = timerWrapper ?? throw new ArgumentNullException(nameof(timerWrapper));
            _timerWrapper.Start(RefreshUsersInterval, null, 0, 30000);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private User _selectedUser;
        public User SelectedUser 
        { 
            get => _selectedUser; 
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private string _logText;

        public string LogText
        {
            get => _logText;
            set 
            { 
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }


        public ObservableCollection<User> Users { get; set; }

        private bool _isSubscribed;

        internal bool IsSubscribedForTestOnly
        {
            get => _isSubscribed; 
            set => _isSubscribed = value;
        }

        private void UserServiceProxyOnUserStatusChanged(object sender, UserStatus e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                LogText += $"new status: {e}\n";
            });
        }

        private async void RefreshUsersInterval(object state)
        {
            try
            {
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                LogText += ex.Message + "\n";
            }
        }

        public ICommand AddCommand   { get; }
        public ICommand UpdateCommand   { get; }
        public ICommand DeleteCommand   { get; }        

        // we cannot call async Task in constructor, so we have to modify the window app form to call it in the Loaded event
        // other solution is use the async void and call it in the constructor but we will not manage the time when the function is finished.
        // it can raise error or UI updating not correctly
        // another is using a third party framework like Prism or MVVM Light which support async constructor
        /// <summary>
        /// Initializes the view model asynchronously by loading user data
        /// This method is called from the WPF windows's Loaded event to ensure proper async/await handling
        /// </summary>
        /// <remarks>
        /// <para>
        /// We cannot directly call asynchronous methods (returning <see cref="Task"/>) in a constructor because constructors
        /// must return <c>void</c> and cannot use <c>await</c>. Therefore, this method is designed to be invoked from 
        /// the <c>Loaded</c> event of the MainWindow to handle asynchronous operations correctly.
        /// </para>
        /// <para>
        /// Alternative approaches include:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// Using <c>async void</c> in the constructor to call <c>LoadUserAsync</c> directly. However, this approach
        /// has drawbacks: it does not allow tracking when the asynchronous operation completes, can swallow exceptions
        /// and may lead to UI updates occurring incorrectly or unpredictably.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Leveraging third-party MVVM frameworks like Prism or MVVM Light, which provide built-in support for asynchronous initialization or constructor-like
        /// behavior. This require additional dependencies but simplifies async handling.
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            if (!_isSubscribed)
            {
                _isSubscribed = true;
                await _userServiceProxy.SubscribeAsync();
            }
            await LoadUsersAsync();
        }
        public async Task UnsubscribeAsync()
        {
            if (_isSubscribed)
            {
                await _userServiceProxy.UnsubscribeAsync();
                _isSubscribed = false;
                _timerWrapper.Dispose();
                _userServiceProxy.UserStatusChanged -= UserServiceProxyOnUserStatusChanged;
            }
        }

        private void AddUser()
        {
            var newUser = new User() { Name = "New User"};
            _userServiceProxy.AddUser(newUser);
            Users.Add(newUser);
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userServiceProxy.GetAllUsersAsync();
            Users = new ObservableCollection<User>(users);
            OnPropertyChanged(nameof(Users));
        }

        private async void UpdateUser()
        {
            if (SelectedUser != null)
            {
                await _userServiceProxy.UpdateUserAsync(SelectedUser);
                Users = new ObservableCollection<User>(await _userServiceProxy.GetAllUsersAsync());
                OnPropertyChanged(nameof(Users));
            }
        }

        private async void DeleteUser()
        {
            if (SelectedUser != null)
            {
                await _userServiceProxy.DeleteUserAsync(SelectedUser.Id);
                Users.Remove(SelectedUser);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute)  => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
    
}
