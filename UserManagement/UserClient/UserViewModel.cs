using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UserClient.ServiceReference1;

namespace UserClient
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IUserService _userService;
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

        public ObservableCollection<User> Users { get; set; }
        public UserViewModel()
        {
            _userService = new UserServiceClient();
            Users = new ObservableCollection<User>(_userService.GetAllUsers());

            AddCommand = new ReplayCommand(AddUser);
            UpdateCommand = new ReplayCommand(UpdateUser);
            DeleteCommand = new ReplayCommand(DeleteUser);
        }


        public ICommand AddCommand   { get; }
        public ICommand UpdateCommand   { get; }
        public ICommand DeleteCommand   { get; }        

        private void AddUser()
        {
            var newUser = new User() { Name = "New User"};
            _userService.AddUser(newUser);
            Users.Add(newUser);
        }

        private void UpdateUser()
        {
            if (SelectedUser != null)
            {
                _userService.UpdateUser(SelectedUser);
                Users = new ObservableCollection<User>(_userService.GetAllUsers());
                OnPropertyChanged(nameof(Users));
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                _userService.DeleteUser(SelectedUser.Id);
                Users.Remove(SelectedUser);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ReplayCommand : ICommand
    {
        private readonly Action _execute;
        public ReplayCommand(Action execute)  => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}
