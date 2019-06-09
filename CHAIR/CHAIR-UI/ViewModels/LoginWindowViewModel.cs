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
using CHAIR_Entities.Models;
using CHAIR_UI.Utils;

namespace CHAIR_UI.ViewModels
{
    public class LoginWindowViewModel : VMBase
    {
        #region Default constructor
        public LoginWindowViewModel(IBasicActionsLogin view)
        {
            _view = view;
            _isNotLoadingLogin = true;

            //SignalR Connection
            _signalR = SignalRHubsConnection.loginHub;

            //Initialize the settings
            SettingUtils.initializeSettings();

            //Search for a stored username and password
            _username = SettingUtils.getUsernameRememberMe();
            _password = SettingUtils.getPasswordRememberMe();
            _view.SetPassword(_password);

            _signalR.proxy.On<UserWithToken>("loginSuccessful", loginSuccessful);
            _signalR.proxy.On<BanResponse>("loginUnauthorized", loginUnauthorized);
            
            if(!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password))
            {
                rememberMe = true;
                login();
            }
        }
        #endregion

        #region Private properties
        private string _username;
        private string _password;
        private bool? _closeWindow;
        private string _errors;
        private SignalRConnection _signalR;
        private IBasicActionsLogin _view; //This allows me to close and minimize the view without breaking the MVVM pattern because it's an interface
        private bool _isNotLoadingLogin;
        private DelegateCommand _loginCommand;
        private string _dialogText;
        private bool _dialogOpened;
        #endregion

        #region Public properties
        public bool rememberMe { get; set; }
        public string dialogText
        {
            get
            {
                return _dialogText;
            }

            set
            {
                _dialogText = value;
                NotifyPropertyChanged("dialogText");
            }
        }
        public bool dialogOpened
        {
            get
            {
                return _dialogOpened;
            }

            set
            {
                _dialogOpened = value;
                NotifyPropertyChanged("dialogOpened");
            }
        }
        public bool isNotLoadingLogin
        {
            get
            {
                return _isNotLoadingLogin;
            }

            set
            {
                _isNotLoadingLogin = value;
                NotifyPropertyChanged("isNotLoadingLogin");
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
        public DelegateCommand recoverPasswordCommand
        {
            get
            {
                return new DelegateCommand(RecoverPasswordCommand_Executed);
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
                NotifyPropertyChanged("username");
                _loginCommand.RaiseCanExecuteChanged();
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
                NotifyPropertyChanged("password");
                _loginCommand.RaiseCanExecuteChanged();
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
        private void RecoverPasswordCommand_Executed()
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            _view.Close();
        }

        private void LoginCommand_Executed()
        {
            login();
        }

        private bool LoginCommand_CanExecute()
        {
            return !string.IsNullOrWhiteSpace(_username) && _username.Length >= 3 && !string.IsNullOrWhiteSpace(_password);
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

                //If remember me is checked, then we save the username and password in the registry
                if(rememberMe)
                {
                    SettingUtils.setUsernameRememberMe(_username);
                    SettingUtils.setPasswordRememberMe(_password);
                }

                //Open the application window
                _view.OpenWindow("ChairWindow");

                //Close this one
                _view.Close();
            });
        }

        private void loginUnauthorized(BanResponse ban)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //If the object is null, it means there is no ban, and the Unauthorized comes from an incorrect login (username or password)
                if(ban == null)
                {
                    //_view.ShowPopUp("The username or password you introduced is incorrect! Please try again.");

                    dialogText = "The username or password you introduced is incorrect! Please try again.";
                    dialogOpened = true;
                }
                else
                {
                    string str = "";

                    str += $"You are banned until {ban.bannedUntil}.\n";
                    str += $"Reason: {ban.banReason}";

                    //_view.ShowPopUp(str);
                    dialogText = str;
                    dialogOpened = true;
                }

                isNotLoadingLogin = true;
            });
        }
        #endregion
    
        #region Methods
        private void login()
        {
            //Login code, calls to SignalR, etc.
            isNotLoadingLogin = false;
            _signalR.proxy.Invoke("login", _username, _password);
        }
        #endregion
    }
}
