using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SimpleInjector;
using UserClient.Infrastructures;
using UserClient.UserServiceProxy;

namespace UserClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Container _container;

        public App()
        {
            _container = new Container();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            _container.Register<IDispatcherwWrapper>(() => new DispatcherWrapper(Dispatcher.CurrentDispatcher), Lifestyle.Singleton);
            _container.Register<ITimerWrapper>(() => new TimerWrapper());
            _container.Register<IUserServiceProxy>(() => new Infrastructures.UserServiceProxy());
            _container.Register<UserViewModel>(() => new UserViewModel(
                _container.GetInstance<ITimerWrapper>(),
                _container.GetInstance<IUserServiceProxy>(),
                _container.GetInstance<IDispatcherwWrapper>()
            ));
            _container.Register<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = _container.GetInstance<MainWindow>();
            mainWindow.Show();
        }
    }
}

