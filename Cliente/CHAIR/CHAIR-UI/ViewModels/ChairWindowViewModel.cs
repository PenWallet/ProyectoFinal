using CHAIR_Entities.Persistent;
using CHAIR_Entitites.Models;
using CHAIR_UI.Interfaces;
using CHAIR_UI.SignalR;
using CHAIR_UI.Utils;
using CHAIRSignalR_Entities.Complex;
using Microsoft.AspNet.SignalR.Client;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class ChairWindowViewModel : VMBase
    {
        #region Constructors
        public ChairWindowViewModel(IBasicActionsChair view)
        {
            _view = view;
            _drawerOpen = false;
            _optionsList = new List<OptionItem>()
            {
                //First the item, then the visible text
                new OptionItem("Account", SharedInfo.loggedUser.nickname),
                new OptionItem("GamepadVariant", "Library"),
                new OptionItem("ShoppingBasket", "Store"),
                new OptionItem("Settings", "Settings"),
                new OptionItem("ExitToApp", "Log out")
            };

            //If the user is an admin, add the admin tab
            if (SharedInfo.loggedUser.admin)
                _optionsList.Add(new OptionItem("Cat", "Admin"));

            _signalR = SignalRHubsConnection.chairHub;

            _signalR.proxy.On<List<Game>>("getAllStoreGames", getAllStoreGames);
            _signalR.proxy.On("unexpectedError", unexpectedError);

            _user = SharedInfo.loggedUser;
            _user.online = true;
            selectedOption = _optionsList[0];
        }

        #endregion


        #region Private properties
        private UserWithToken _user { get; set; }
        private User _profileUser { get; set; } //This is the user to be displayed in the Profile UserControl
        private IBasicActionsChair _view { get; set; }
        private OptionItem _selectedOption { get; set; }
        private List<OptionItem> _optionsList { get; set; }
        private bool _drawerOpen { get; set; }
        private SignalRConnection _signalR { get; set; }
        #endregion


        #region Public properties
        public List<Game> storeGames { get; set; } //List of all the games available in the store minus the frontpage game
        public Game frontPageGame { get; set; } //Game with frontPage set to true
        public User profileUser
        {
            get
            {
                return _profileUser;
            }

            set
            {
                _profileUser = value;
                NotifyPropertyChanged("profileUser");
            }
        }
        public UserWithToken user
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
                NotifyPropertyChanged("user");
            }
        }
        public bool drawerOpen
        {
            get
            {
                return _drawerOpen;
            }

            set
            {
                _drawerOpen = value;
                NotifyPropertyChanged("drawerOpen");
            }
        }
        public List<OptionItem> optionsList
        {
            get
            {
                return _optionsList;
            }

            set
            {
                _optionsList = value;
                NotifyPropertyChanged("optionsList");
            }
        }
        public OptionItem selectedOption
        {
            get
            {
                return _selectedOption;
            }

            set
            {
                _selectedOption = value;
                NotifyPropertyChanged("selectedOption");
                //If the user selected Profile, we must give profileUser our own user's value.
                if (_selectedOption.name == _user.nickname)
                    _profileUser = Utilities.userWithTokenToUser(_user);
                else if (_selectedOption.name == "Store")
                    _signalR.proxy.Invoke("getAllStoreGames", _user.token);
                _view.ChangePage(_selectedOption.name, this);
                drawerOpen = false;
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
        #endregion


        #region Commands
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
        private void getAllStoreGames(List<Game> games)
        {
            if(games.Count != 0)
            {
                try
                {
                    frontPageGame = games.Single(x => x.frontPage);
                    games.Remove(frontPageGame);
                }
                catch (Exception e) { frontPageGame = null; }

                storeGames = games;
            }
        }

        private void unexpectedError()
        {
            int i = 0;
            i++;
        }
        #endregion
    }
}
