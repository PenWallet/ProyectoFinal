using CHAIR_UI.Interfaces;
using CHAIR_UI.Views;
using CHAIRSignalR_Entities.Complex;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class LoginWindowViewModel : VMBase
    {
        #region Default constructor
        public LoginWindowViewModel(ICloseableMinimizeable view)
        {
            _username = "";
            _password = "";
            _view = view;

            //SignalR
            conn = new HubConnection("http://localhost:51930/");
            proxy = conn.CreateHubProxy("LoginHub");
            conn.Start();

            proxy.On<UserWithToken>("loginSuccessful", loginSuccessful);
        }
        #endregion

        #region Private properties
        public DelegateCommand _loginCommand { get; set; }
        private string _username;
        private string _password;
        private Window _window;
        private bool? _closeWindow;
        private ICloseableMinimizeable _view;
        #endregion

        #region Public properties
        //Apparently, RelayCommand is like DelegateCommand, only with a generic type
        public DelegateCommand registerClickCommand
        {
            get
            {
                return new DelegateCommand(RegisterCommand_Executed);
            }
        }
        public DelegateCommand loginCommand
        {
            get
            {
                _loginCommand = new DelegateCommand(LoginCommand_Executed, LoginCommand_CanExecute); ;
                return _loginCommand;
            }
        }
        public DelegateCommand closeCommand
        {
            get
            {
                return new DelegateCommand(CloseCommand_Executed);
            }
        }
        public DelegateCommand minimizeCommand
        {
            get
            {
                return new DelegateCommand(MinimizeCommand_Executed);
            }
        }
        public bool rememberMe { get; set; }
        public HubConnection conn { get; set; }
        public IHubProxy proxy { get; set; }
        public bool? closeWindow
        {
            get
            {
                return _closeWindow;
            }

            set
            {
                _closeWindow = value;
                NotifyPropertyChanged("closeWindow");
            }
        }
        public DelegateCommand rickRollCommand
        {
            get
            {
                return new DelegateCommand(RickRollCommand_Executed);
            }
        }

        public string username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                _loginCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("username");
            }
        }

        public string password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                _loginCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("password");
            }
        }
        #endregion

        #region Commands
        private void RegisterCommand_Executed()
        {
            //Show the registration window
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Show();

            //Close this window
            _view.Close();
        }

        //Sorry
        private void RickRollCommand_Executed()
        {
            _view.Close();
            //System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        }

        private void LoginCommand_Executed()
        {
            //Login code, calls to SignalR, etc.
            proxy.Invoke("login", _username, _password);
        }

        private bool LoginCommand_CanExecute()
        {
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                return false;

            return true;
        }

        private void CloseCommand_Executed()
        {
            _view.Close();
        }

        private void MinimizeCommand_Executed()
        {
            _view.Minimize();
        }
        #endregion

        #region SignalR Methods
        private void loginSuccessful(UserWithToken obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                ChairWindow chairWindow = new ChairWindow();
                chairWindow.Show();

                _view.Close();
            });
        }
        #endregion
    }
}
