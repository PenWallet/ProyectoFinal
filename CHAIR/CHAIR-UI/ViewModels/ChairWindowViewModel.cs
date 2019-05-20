using CHAIR_Entities.Persistent;
using CHAIR_Entities.Models;
using CHAIR_UI.Interfaces;
using CHAIR_UI.SignalR;
using CHAIR_UI.Utils;
using CHAIR_Entities.Complex;
using Microsoft.AspNet.SignalR.Client;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace CHAIR_UI.ViewModels
{
    public class ChairWindowViewModel : VMBase
    {
        #region Constructors
        public ChairWindowViewModel(IBasicActionsChair view)
        {
            _view = view;
            _drawerOpen = false;
            _libraryGameVisible = Visibility.Hidden;
            _openCommunity = true;
            _optionsList = new List<OptionItem>()
            {
                //First the itemKind (from material design), then the visible text
                new OptionItem("Account", SharedInfo.loggedUser.nickname),
                new OptionItem("GamepadVariant", "Library"),
                new OptionItem("ShoppingBasket", "Store"),
                new OptionItem("UserGroup", "Community"),
                new OptionItem("Settings", "Settings"),
                new OptionItem("ExitToApp", "Log out")
            };

            //If the user is an admin, add the admin tab
            if (SharedInfo.loggedUser.admin)
                _optionsList.Add(new OptionItem("Cat", "Admin"));

            _signalR = SignalRHubsConnection.chairHub;

            _signalR.proxy.On<List<Game>>("getAllStoreGames", getAllStoreGames);
            _signalR.proxy.On<string>("unexpectedError", unexpectedError);
            _signalR.proxy.On<UserProfile>("getUserProfile", getUserProfile);
            _signalR.proxy.On<List<UserGamesWithGameAndFriends>>("getAllMyGames", getAllMyGames);
            _signalR.proxy.On<GameStore>("getGameInformation", getGameInformation);
            _signalR.proxy.On<List<UserSearch>>("searchForUsers", searchForUsers);

            //Thread.Sleep(100);

            selectedOption = _optionsList[0]; //Navigate to your profile
        }

        #endregion


        #region Private properties
        private UserProfile _profileUser { get; set; } //This is the user to be displayed in the Profile UserControl
        private IBasicActionsChair _view { get; set; }
        private OptionItem _selectedOption { get; set; }
        private List<OptionItem> _optionsList { get; }
        private bool _drawerOpen { get; set; }
        private SignalRConnection _signalR { get; set; }
        private List<Game> _storeGames { get; set; } //List of all the games available in the store minus the frontpage game
        private Game _frontPageGame { get; set; } //Game with frontPage set to true
        private List<UserGamesWithGameAndFriends> _libraryGames { get; set; }
        private UserGamesWithGameAndFriends _selectedLibraryGame { get; set; }
        private Visibility _libraryGameVisible { get; set; }
        private bool _openCommunity { get; set; } //Variable used to know whether to open Profile to see another user's information, or to open Community, the user searcher
        private GameStore _selectedStoreGame { get; set; }
        private List<UserSearch> _searchList { get; set; }
        #endregion


        #region Public properties
        public RelayCommand<string> addFriendCommand
        {
            get
            {
                return new RelayCommand<string>(addFriendCommand_Executed);
            }
        }
        public RelayCommand<string> searchUsersCommand
        {
            get
            {
                return new RelayCommand<string>(searchUsersCommand_Executed);
            }
        }
        public List<UserSearch> searchList
        {
            get
            {
                return _searchList;
            }

            set
            {
                _searchList = value;
                NotifyPropertyChanged("searchList");
            }
        }
        public GameStore selectedStoreGame
        {
            get
            {
                return _selectedStoreGame;
            }

            set
            {
                _selectedStoreGame = value;
                NotifyPropertyChanged("selectedStoreGame");
            }
        }
        public Visibility libraryGameVisibleInverse
        {
            get
            {
                if (_libraryGameVisible == Visibility.Hidden)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        public Visibility libraryGameVisible
        {
            get
            {
                return _libraryGameVisible;
            }

            set
            {
                _libraryGameVisible = value;
                NotifyPropertyChanged("libraryGameVisible");
                NotifyPropertyChanged("libraryGameVisibleInverse");
            }
        }
        public UserGamesWithGameAndFriends selectedLibraryGame
        {
            get
            {
                return _selectedLibraryGame;
            }

            set
            {
                _selectedLibraryGame = value;
                NotifyPropertyChanged("selectedLibraryGame");
                
                //Because we selected a library game, we change the visibility of the list
                if(value == null)
                    libraryGameVisible = Visibility.Hidden;
                else
                    libraryGameVisible = Visibility.Visible;
            }
        }
        public List<UserGamesWithGameAndFriends> libraryGames
        {
            get
            {
                return _libraryGames;
            }

            set
            {
                _libraryGames = value;
                NotifyPropertyChanged("libraryGames");
            }
        }
        public RelayCommand<string> goToProfileCommand
        {
            get
            {
                return new RelayCommand<string>(goToProfileCommand_Executed);
            }
        }
        public RelayCommand<string> goToGamePageCommand
        {
            get
            {
                return new RelayCommand<string>(goToGamePageCommand_Executed);
            }
        }

        public List<Game> storeGames
        {
            get
            {
                return _storeGames;
            }

            set
            {
                _storeGames = value;
                NotifyPropertyChanged("storeGames");
            }
        }
        public Game frontPageGame
        {
            get
            {
                return _frontPageGame;
            }

            set
            {
                _frontPageGame = value;
                NotifyPropertyChanged("frontPageGame");
            }
        }
        public UserProfile profileUser
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
                
                //Calls to SignalR
                if (_selectedOption.name == SharedInfo.loggedUser.nickname) //If the user selected Profile, we must look for our own profile information
                    _signalR.proxy.Invoke("getUserProfile", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.token);
                else if (_selectedOption.name == "Store") //If the user selected the Store, we must retrieve the store games
                    _signalR.proxy.Invoke("getAllStoreGames", SharedInfo.loggedUser.token);
                else if(_selectedOption.name == "Library") //If the user selected the Library, we must retrieve all the games he plays and all their information
                    _signalR.proxy.Invoke("getAllMyGamesAndFriends", SharedInfo.loggedUser.token);
                else if(_selectedOption.name == "Log out") //If the user wants to log out, we close the connection with the server
                    SignalRHubsConnection.closeChairHub();

                //Close the drawer
                drawerOpen = false;

                //Call through the interface to the view to change to whatever view the user asked for
                _view.ChangePage(_selectedOption.name, this);
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
            SignalRHubsConnection.closeChairHub();
            _view.Close();
        }

        private void MinimizeCommand_Executed()
        {
            _view.Minimize();
        }

        private void goToProfileCommand_Executed(string nickname)
        {
            _signalR.proxy.Invoke("getUserProfile", nickname, SharedInfo.loggedUser.token);

            //We change the page to "Profile" to display the user's information
            _view.ChangePage("Profile", this);
            //But we mark "Community" on the list, because it isn't our profile and there is no "Profile" option
            selectedOption = _optionsList.Single(x => x.name == "Community");
        }

        private void searchUsersCommand_Executed(string search)
        {
            _signalR.proxy.Invoke("searchForUsers", search, SharedInfo.loggedUser.token);
        }

        private void goToGamePageCommand_Executed(string game)
        {
            _signalR.proxy.Invoke("getGameInformation", SharedInfo.loggedUser.nickname, game, SharedInfo.loggedUser.token);
            _view.ChangePage("Game", this);
        }

        private void addFriendCommand_Executed(string user2)
        {
            _signalR.proxy.Invoke("addFriend", SharedInfo.loggedUser.nickname, user2, SharedInfo.loggedUser.token);
            _searchList.Single(x => x.user.nickname == user2).relationshipExists = true;
            NotifyPropertyChanged("searchList");
        }
        #endregion

        #region SignalR Methods
        private void getAllStoreGames(List<Game> games)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                if (games.Count != 0)
                {
                    try
                    {
                        frontPageGame = games.Single(x => x.frontPage);
                        games.Remove(frontPageGame);
                    }
                    catch (Exception e) { frontPageGame = null; }

                    storeGames = games;
                }
            });

            
        }

        private void getUserProfile(UserProfile obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                profileUser = obj;
            });
        }

        private void getAllMyGames(List<UserGamesWithGameAndFriends> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                libraryGames = obj;
            });
        }

        private void getGameInformation(GameStore obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                selectedStoreGame = obj;
            });
        }

        private void searchForUsers(List<UserSearch> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                searchList = obj;
            });
        }

        private void unexpectedError(string error)
        {
            MessageBox.Show(error);
        }
        #endregion
    }
}
