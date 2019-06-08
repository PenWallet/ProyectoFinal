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
using MaterialDesignThemes.Wpf;
using CHAIR_Entities.Persistent;

namespace CHAIR_UI.ViewModels
{
    public class ProfileEditUserControlViewModel : VMBase
    {
        #region Default constructor
        public ProfileEditUserControlViewModel(IPasswordBox view, SnackbarMessageQueue notificationsQueue)
        {
            _notificationsQueue = notificationsQueue;
            _view = view;

            //SignalR Connection
            _signalR = SignalRHubsConnection.chairHub;
            _signalR.proxy.On("updatedUserSuccessfully", updatedUserSuccessfully);

            //Initialize profile edit
            _profileEditPrivateProfile = SharedInfo.loggedUser.privateProfile;
            _profileEditDescription = SharedInfo.loggedUser.profileDescription;
            _profileEditLocation = SharedInfo.loggedUser.profileLocation;
        }
        #endregion

        #region Private properties
        private bool _canSaveProfileEdit;
        private bool _profileEditPrivateProfile;
        private string _profileEditDescription;
        private string _profileEditLocation;
        private string _profileEditPassword;
        private DelegateCommand _saveProfileEditCommand;
        private DelegateCommand _resetProfileEditCommand;
        private SignalRConnection _signalR;
        private IPasswordBox _view;
        private SnackbarMessageQueue _notificationsQueue;
        #endregion

        #region Public properties
        public bool profileEditPrivateProfile
        {
            get
            {
                return _profileEditPrivateProfile;
            }
            set
            {
                _profileEditPrivateProfile = value;
                NotifyPropertyChanged("profileEditPrivateProfile");

                _canSaveProfileEdit = true;
                _saveProfileEditCommand.RaiseCanExecuteChanged();
                _resetProfileEditCommand.RaiseCanExecuteChanged();
            }
        }
        public string profileEditDescription
        {
            get
            {
                return _profileEditDescription;
            }
            set
            {
                _profileEditDescription = value;
                NotifyPropertyChanged("profileEditDescription");

                _canSaveProfileEdit = true;
                _saveProfileEditCommand.RaiseCanExecuteChanged();
                _resetProfileEditCommand.RaiseCanExecuteChanged();
            }
        }
        public string profileEditLocation
        {
            get
            {
                return _profileEditLocation;
            }
            set
            {
                _profileEditLocation = value;
                NotifyPropertyChanged("profileEditLocation");

                _canSaveProfileEdit = true;
                _saveProfileEditCommand.RaiseCanExecuteChanged();
                _resetProfileEditCommand.RaiseCanExecuteChanged();
            }
        }
        public string profileEditPassword
        {
            get
            {
                return _profileEditPassword;
            }
            set
            {
                _profileEditPassword = value;
                NotifyPropertyChanged("profileEditPassword");

                _canSaveProfileEdit = true;
                _saveProfileEditCommand.RaiseCanExecuteChanged();
                _resetProfileEditCommand.RaiseCanExecuteChanged();
            }
        }
        public DelegateCommand saveProfileEditCommand
        {
            get
            {
                _saveProfileEditCommand = new DelegateCommand(saveProfileEditCommand_Executed, SaveAndResetProfileEditCommand_CanExecute);
                return _saveProfileEditCommand;
            }
        }
        public DelegateCommand resetProfileEditCommand
        {
            get
            {
                _resetProfileEditCommand = new DelegateCommand(resetProfileEditCommand_Executed, SaveAndResetProfileEditCommand_CanExecute);
                return _resetProfileEditCommand;
            }
        }
        #endregion

        #region Commands
        private void resetProfileEditCommand_Executed()
        {
            //Reset edits
            profileEditPrivateProfile = SharedInfo.loggedUser.privateProfile;
            profileEditDescription = SharedInfo.loggedUser.profileDescription;
            profileEditLocation = SharedInfo.loggedUser.profileLocation;
            _view.SetPassword("");

            _canSaveProfileEdit = false;
            _saveProfileEditCommand.RaiseCanExecuteChanged();
            _resetProfileEditCommand.RaiseCanExecuteChanged();
        }

        private bool SaveAndResetProfileEditCommand_CanExecute()
        {
            return _canSaveProfileEdit;
        }

        private void saveProfileEditCommand_Executed()
        {
            User user = new User();
            user.nickname = SharedInfo.loggedUser.nickname;
            user.password = _profileEditPassword;
            user.profileLocation = _profileEditLocation;
            user.profileDescription = _profileEditDescription;
            user.privateProfile = _profileEditPrivateProfile;

            _signalR.proxy.Invoke("updateUser", user, SharedInfo.loggedUser.token);
        }
        #endregion

        #region SignalR Methods
        private void updatedUserSuccessfully()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                _notificationsQueue.Enqueue("Updated successfully!");

                //Update the SharedInfo.loggedUser with the new info
                SharedInfo.loggedUser.profileLocation = _profileEditLocation;
                SharedInfo.loggedUser.profileDescription = _profileEditDescription;
                SharedInfo.loggedUser.privateProfile = _profileEditPrivateProfile;

                //If we have the password saved in the Registry, then save it
                if (!string.IsNullOrWhiteSpace(SettingUtils.getPasswordRememberMe()))
                    SettingUtils.setPasswordRememberMe(_profileEditPassword);

                //Reset the fields
                _view.SetPassword("");
                _canSaveProfileEdit = false;
                _saveProfileEditCommand.RaiseCanExecuteChanged();
                _resetProfileEditCommand.RaiseCanExecuteChanged();
            });
        }
        #endregion
    }
}
