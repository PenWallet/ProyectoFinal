using CHAIR_Entities.Models;
using CHAIR_Entities.Persistent;
using CHAIR_Entities.Responses;
using CHAIR_UI.Interfaces;
using CHAIR_UI.SignalR;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class RegisterWindowViewModel : VMBase
    {
        #region Constructors
        public RegisterWindowViewModel(IBasicActionsRegister view)
        {
            _usernameBorder = "#ABADB3";
            _passwordBorder = "#ABADB3";
            _birthdateBorder = "#ABADB3";
            _errorsVisibility = "Collapsed";
            _birthdate = new DateTime(1999, 8, 12);
            _view = view;
            _signalR = SignalRHubsConnection.loginHub;
            maximumDate = DateTime.Now;
            _goToLoginOnDialogClose = false;

            _signalR.proxy.On("registerSuccessful", registerSuccessful);
            _signalR.proxy.On("registerUserTaken", registerUserTaken);
            _signalR.proxy.On<BanResponse>("registerBanned", registerBanned);
        }


        #endregion

        #region Private properties
        private string _username;
        private string _password;
        private DateTime _birthdate;
        private string _location;
        private bool _privateProfile;
        private string _usernameBorder;
        private string _passwordBorder;
        private string _birthdateBorder;
        private string _errors;
        private string _errorsVisibility;
        private IBasicActionsRegister _view;
        private DelegateCommand _registerCommand;
        private SignalRConnection _signalR;
        private bool _loadingRegister;
        private string _dialogText;
        private bool _dialogOpened;
        private bool _goToLoginOnDialogClose;
        #endregion

        #region Public properties
        public DateTime maximumDate { get; set; }
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

                if (!value && _goToLoginOnDialogClose)
                {
                    _goToLoginOnDialogClose = false;
                    _view.ShowLogin();
                }
            }
        }
        public bool loadingRegister
        {
            get
            {
                return _loadingRegister;
            }

            set
            {
                _loadingRegister = value;
                NotifyPropertyChanged("loadingRegister");
                _registerCommand?.RaiseCanExecuteChanged();
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
        public string username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                _registerCommand.RaiseCanExecuteChanged();
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
                _registerCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("password");
            }
        }
        public DateTime birthdate
        {
            get
            {
                return _birthdate;
            }

            set
            {
                _birthdate = value;
                _registerCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("birthdate");
            }
        }

        public string location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
                _registerCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("location");
            }
        }

        public bool privateProfile
        {
            get
            {
                return _privateProfile;
            }

            set
            {
                _privateProfile = value;
                _registerCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("privateProfile");
            }
        }

        public string usernameBorder
        {
            get
            {
                return _usernameBorder;
            }

            set
            {
                _usernameBorder = value;
                NotifyPropertyChanged("usernameBorder");
            }
        }

        public string passwordBorder
        {
            get
            {
                return _passwordBorder;
            }

            set
            {
                _passwordBorder = value;
                NotifyPropertyChanged("passwordBorder");
            }
        }

        public string birthdateBorder
        {
            get
            {
                return _birthdateBorder;
            }

            set
            {
                _birthdateBorder = value;
                NotifyPropertyChanged("birthdateBorder");
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

        public string errorsVisibility
        {
            get
            {
                return _errorsVisibility;
            }

            set
            {
                _errorsVisibility = value;
                NotifyPropertyChanged("errorsVisibility");
            }
        }

        public DelegateCommand registerCommand
        {
            get
            {
                _registerCommand = new DelegateCommand(RegisterCommand_Executed, RegisterCommand_CanExecute);
                return _registerCommand;
            }
        }
        #endregion

        #region Commands
        private void CloseCommand_Executed()
        {
            //Show the login window
            _view.OpenWindow("LoginWindow");

            //Close this window
            _view.Close();

            //Dispose of everything
            dispose();

        }

        private void MinimizeCommand_Executed()
        {
            _view.Minimize();
        }

        private void RegisterCommand_Executed()
        {
            List<string> errorList = getFieldsErrors();

            if(errorList.Count == 0)
            {
                loadingRegister = true;

                errorsVisibility = "Collapsed";

                //Make the user with all the information
                User user = new User();
                user.nickname = _username;
                user.password = _password;
                user.birthDate = _birthdate;
                user.profileLocation = _location;
                user.privateProfile = _privateProfile;

                _signalR.proxy.Invoke("register", user);
            }
            else
            {
                string errorListString = "";
                foreach(string str in errorList)
                    errorListString += str + (str != errorList.Last() ? "\n" : "");

                errors = errorListString;
                errorsVisibility = "Visible";
            }
        }

        private bool RegisterCommand_CanExecute()
        {
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || _username.Any(c => c == ' ') || _password.Any(c => c == ' ') || _loadingRegister)
                return false;

            return true;
        }
        #endregion

        #region Functions
        private void dispose()
        {
            _username = null;
            _password = null;
            _location = null;
            _usernameBorder = null;
            _passwordBorder = null;
            _birthdateBorder = null;
            _errors = null;
            _errorsVisibility = null;
            _view = null;
            _registerCommand = null;
            _signalR = null;
        }

        private List<string> getFieldsErrors()
        {
            List<string> errorsList = new List<string>();
            bool wrongUsername = false, wrongPassword = false, wrongBirthdate = false;

            if(_username.Length < 3)
            {
                errorsList.Add("The username can't be shorter than 3 characters!");
                wrongUsername = true;
            }

            if(!Regex.Match(_password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#/\·~?.&])[A-Za-z\d@$!·%~/\*#.?&]{8,}$").Success)
            {
                errorsList.Add("The password must be at least 8 characters long, contain one letter, one number and one special character!");
                wrongPassword = true;
            }

            if(_birthdate > DateTime.Now)
            {
                errorsList.Add("Who you tryin to fool, baka-chan?");
                wrongBirthdate = true;
            }

            if(DateTime.Now.Subtract(_birthdate).TotalDays / 365.25 < 13)
            {
                errorsList.Add("You must be at least 13 years old! Call your mama");
                wrongBirthdate = true;
            }

            if (wrongUsername)
                usernameBorder = "#FF495C";
            else
                usernameBorder = "#ABADB3";

            if (wrongPassword)
                passwordBorder = "#FF495C";
            else
                passwordBorder = "#ABADB3";

            if (wrongBirthdate)
                birthdateBorder = "#FF495C";
            else
                birthdateBorder = "#ABADB3";

            return errorsList;
        }
        #endregion

        #region SignalR Methods
        private void registerSuccessful()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Show a pop up
                dialogText = "Registered succesfully!";
                dialogOpened = true;
                loadingRegister = false;
                _goToLoginOnDialogClose = true;
            });
        }

        private void registerUserTaken()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //_view.ShowPopUp("That username is already taken!");
                dialogText = "That username is already taken!";
                dialogOpened = true;
                loadingRegister = false;
            });
        }

        private void registerBanned(BanResponse ban)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                string str = "";

                str += $"You are banned until {ban.bannedUntil}.\n";
                str += $"Reason: {ban.banReason}";

                dialogText = str;
                dialogOpened = true;

                loadingRegister = false;
            });
        }
        #endregion
    }
}
