using CHAIR_Entities.Responses;
using CHAIR_UI.Interfaces;
using CHAIR_UI.SignalR;
using CHAIR_UI.Views;
using CHAIR_Entities.Complex;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class LoginWindowViewModel : VMBase
    {
        #region Default constructor
        public LoginWindowViewModel(IBasicActions view)
        {
            _username = "Penny"; //TODO: Eliminar esto jeje
            _password = "1234";
            _view = view;

            //SignalR Connection
            _signalR = SignalRHubsConnection.loginHub;

            _signalR.proxy.On<UserWithToken>("loginSuccessful", loginSuccessful);
            _signalR.proxy.On<BanResponse>("loginUnauthorized", loginUnauthorized);
        }
        #endregion

        #region Private properties
        private DelegateCommand _loginCommand;
        private string _username;
        private string _password;
        private bool? _closeWindow;
        private string _errors;
        private SignalRConnection _signalR;
        private IBasicActions _view; //This allows me to close and minimize the view without breaking MVVM patterns
        private bool _loadingLogin;
        #endregion

        #region Public properties
        public bool rememberMe { get; set; }
        public bool loadingLogin
        {
            get
            {
                return _loadingLogin;
            }

            set
            {
                _loadingLogin = value;
                NotifyPropertyChanged("loadingLogin");
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
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
                _loginCommand = new DelegateCommand(LoginCommand_Executed, LoginCommand_CanExecute);
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
        public string errors
        {
            get
            {
                return _errors;
            }

            set
            {
                _errors = value;
                NotifyPropertyChanged("errors");
            }
        }
        #endregion

        #region Commands
        private void RegisterCommand_Executed()
        {
            //Show the registration window
            _view.OpenWindow("RegisterWindow");

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
            loadingLogin = true;
            _signalR.proxy.Invoke("login", _username, _password);
        }

        private bool LoginCommand_CanExecute()
        {
            //Can't click login if there's nothing written on username or password fields or if it's already trying to log in
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || _loadingLogin)
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
        private void loginSuccessful(UserWithToken user)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Save the information in the SharedInfo for the ChairWindowViewModel to have
                SharedInfo.loggedUser = user;

                //Open the application window
                _view.OpenWindow("ChairWindow");

                //Close this one
                _view.Close();

                loadingLogin = false;
            });
        }

        private void loginUnauthorized(BanResponse ban)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //If the object is null, it means there is no ban, and the Unauthorized comes from an incorrect login (username or password)
                if(ban == null)
                    _view.ShowPopUp("The username or password you introduced is incorrect! Please try again.");
                else
                {
                    string str = "";

                    str += $"You are banned until {ban.bannedUntil}.\n";
                    str += $"Reason: {ban.banReason}";

                    _view.ShowPopUp(str);
                }

                loadingLogin = false;
            });
        }
        #endregion
    }
}
