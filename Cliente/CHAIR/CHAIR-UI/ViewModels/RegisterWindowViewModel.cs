using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class RegisterWindowViewModel : VMBase
    {
        #region Constructors
        public RegisterWindowViewModel()
        {
            _usernameBorder = "#ABADB3";
            _passwordBorder = "#ABADB3";
            _errorsVisibility = "Hidden";
            _birthdate = DateTime.Now;
            this.closeCommand = new RelayCommand<Window>(this.CloseWindow);
            this.minimizeCommand = new RelayCommand<Window>(this.MinimizeCommand);
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
        private DelegateCommand _registerCommand;
        #endregion

        #region Public properties
        public RelayCommand<Window> closeCommand { get; private set; }
        public RelayCommand<Window> minimizeCommand { get; private set; }
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
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                //Show the login window
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                //Close this window
                window.Close();
            }

        }

        private void MinimizeCommand(Window window)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }



        private void RegisterCommand_Executed()
        {
            List<string> errorList = getFieldsErrors();

            if(errorList.Count == 0)
            {
                errorsVisibility = "Hidden";

                //Call SignalR
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
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || _username.Any(c => c == ' ') || _password.Any(c => c == ' '))
                return false;

            return true;
        }
        #endregion

        #region Functions
        private List<string> getFieldsErrors()
        {
            List<string> errorsList = new List<string>();
            bool wrongUsername = false, wrongPassword = false, wrongBirthdate = false;

            if(_username.Length < 3)
            {
                errorsList.Add("The username can't be shorter than 3 characters!");
                wrongUsername = true;
            }

            if(_password.Length < 8)
            {
                errorsList.Add("The password must be at least 8 characters long!");
                wrongPassword = true;
            } 

            if(!_password.Any(c => char.IsSymbol(c)))
            {
                errorsList.Add("The password must contain at least one special character!");
                wrongPassword = true;
            }

            if(!_password.Any(c => char.IsNumber(c)))
            {
                errorsList.Add("The password must contain at least one number!");
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
                birthdateBorder = "Transparent";

            return errorsList;
        }
        #endregion
    }
}
