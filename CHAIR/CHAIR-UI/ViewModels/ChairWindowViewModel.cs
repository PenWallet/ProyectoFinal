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
using CHAIR_Entities.Responses;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using System.Media;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.Diagnostics;
using CHAIR_Entities.Enums;

namespace CHAIR_UI.ViewModels
{
    public class ChairWindowViewModel : VMBase
    {
        #region Constructors
        public ChairWindowViewModel(IBasicActionsChair view)
        {
            //Necessary for Unzipping 🤷🤷🤷
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            loggedUser = SharedInfo.loggedUser;
            _view = view;
            _drawerOpen = false;
            _libraryGameVisible = Visibility.Hidden;
            _openCommunity = true;
            _conversations = new ObservableCollection<UserForFriendList>();
            notificationsQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000)); //We create a new snackbar with a time duration for each notification of 3 seconds
            _installedGames = new List<string>();
            _goToGamePage = false;
            _gameBeingDownloaded = null;
            _gameBeingUnzipped = null;
            _sounds = new SoundsUtils();
            _friendsList = new ObservableCollection<UserForFriendList>();
            _optionsList = new List<OptionItem>()
            {
                //First the itemKind (from material design), then the visible text
                new OptionItem("Account", SharedInfo.loggedUser.nickname),
                new OptionItem("GamepadVariant", "Library"),
                new OptionItem("ShoppingBasket", "Store"),
                new OptionItem("UserGroup", "Community"),
                new OptionItem("Settings", "Settings"),
                new OptionItem("About", "About"),
                new OptionItem("ExitToApp", "Log out")
            };

            //If the user is an admin, add the admin tab
            if (SharedInfo.loggedUser.admin)
                _optionsList.Add(new OptionItem("Cat", "Admin"));

            _signalR = SignalRHubsConnection.chairHub;

            _signalR.proxy.On<List<Game>>("getAllStoreGames", getAllStoreGames);
            _signalR.proxy.On<string>("unexpectedError", unexpectedError);
            _signalR.proxy.On<UserProfile>("getUserProfile", getUserProfile);
            _signalR.proxy.On<ObservableCollection<UserGamesWithGameAndFriends>>("getAllMyGames", getAllMyGames);
            _signalR.proxy.On<GameStore>("getGameInformation", getGameInformation);
            _signalR.proxy.On<List<UserSearch>>("searchForUsers", searchForUsers);
            _signalR.proxy.On<ObservableCollection<UserForFriendList>>("getFriends", getFriends);
            _signalR.proxy.On<BanResponse>("youHaveBeenBanned", youHaveBeenBanned);
            _signalR.proxy.On("onlineSuccessful", onlineSuccessful);
            _signalR.proxy.On<string, NotificationType>("updateFriendListWithNotification", updateFriendListWithNotification);
            _signalR.proxy.On<string, ObservableCollection<Message>>("getConversation", getConversation);
            _signalR.proxy.On<string>("gameBought", gameBought);
            _signalR.proxy.On<Message>("receiveMessage", receiveMessage);
            _signalR.proxy.On("closedGameSuccessfully", closedGameSuccessfully);

            //As soon as the user opens the application, we need to retrieve all the information regarding the user 
            _initializing = true; //We set this flag to true so that when getFriends gets called from SignalR, we update our status to true
            _signalR.proxy.Invoke("getFriends", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.token);

            //We also need to pre-load all of our games
            _signalR.proxy.Invoke("getAllMyGamesAndFriends", SharedInfo.loggedUser.token);

            //Look up which games are installed (if the folder exists, I take the game is installed, I ain't searching each folder and each file, screw it)
            installedGames = SettingUtils.scanInstallingFolder();

            //Play sound indicating we connected
            _sounds.PlayOnlineSound();

            //Initialize settings
            SettingUtils.initializeSettings();
            _installingFolder = SettingUtils.getInstallingFolder();
            _tempDownloadFolder = SettingUtils.getTempDownloadFolder();
            _messageNotifications = SettingUtils.getMessageNotificationSetting();
            _onlineNotifications = SettingUtils.getOnlineNotificationSetting();
            _offlineNotifications = SettingUtils.getOfflineNotificationSetting();
            _playingGameNotifications = SettingUtils.getPlayingGameNotificationSetting();
            _minimizeToTray = SettingUtils.getMinimizeToTraySetting();
            _canSaveSettings = false;
        }

        #endregion


        #region Private properties
        private UserProfile _profileUser { get; set; } //This is the user to be displayed in the Profile UserControl
        private IBasicActionsChair _view { get; set; }
        private OptionItem _selectedOption { get; set; }
        private List<OptionItem> _optionsList { get; set; }
        private bool _drawerOpen { get; set; }
        private SignalRConnection _signalR { get; set; }
        private List<Game> _storeGames { get; set; } //List of all the games available in the store minus the frontpage game
        private Game _frontPageGame { get; set; } //Game with frontPage set to true
        private ObservableCollection<UserGamesWithGameAndFriends> _libraryGames { get; set; }
        private UserGamesWithGameAndFriends _selectedLibraryGame { get; set; }
        private Visibility _libraryGameVisible { get; set; } //TODO: Change this for a boolean
        private bool _openCommunity { get; set; } //Variable used to know whether to open Profile to see another user's information, or to open Community, the user searcher
        private GameStore _selectedStoreGame { get; set; }
        private List<UserSearch> _searchList { get; set; }
        private bool _initializing { get; set; }
        private bool _canClickOnBuyGame { get; set; }
        private List<string> _installedGames { get; set; }
        private bool _goToGamePage { get; set; } //Variable used to know whether to travel to the game store page after receiving the SignalR call "getGameInformation"
        private RelayCommand<string> _buyStoreGameCommand { get; set; }
        private RelayCommand<string> _downloadGameCommand { get; set; }
        private RelayCommand<string> _openGameCommand { get; set; }
        private string _conversationTextToSend { get; set; }
        private string _gameBeingDownloaded { get; set; }
        private string _gameBeingUnzipped { get; set; }
        private string _gameBeingPlayed { get; set; }
        private int _downloadUnzipProgress { get; set; }
        private Process _gameProcess { get; set; }
        private SoundsUtils _sounds { get; set; }
        private bool _canSeeProfile { get; set; } //Variable used to know whether the user can see another user's profile or not (based on that user's private profile option and whether the current user is admin or not)

        //Friend list variables
        private ObservableCollection<UserForFriendList> _friendsList { get; set; }

        //ConversationWindow
        private ObservableCollection<UserForFriendList> _conversations { get; set; }
        private ObservableCollection<UserForFriendList> _friendsListOnline { get; set; }
        private ObservableCollection<UserForFriendList> _friendsListOffline { get; set; }
        private ObservableCollection<UserForFriendList> _friendsListPending { get; set; }
        private UserForFriendList _selectedConversation { get; set; }

        //Settings variables
        private string _installingFolder;
        private string _tempDownloadFolder;
        private bool _messageNotifications;
        private bool _onlineNotifications;
        private bool _offlineNotifications;
        private bool _playingGameNotifications;
        private bool _minimizeToTray;
        private bool _canSaveSettings;
        private DelegateCommand _saveSettingsCommand;
        private DelegateCommand _resetSettingsCommand;

        //ProfileEdit variables
        #endregion


        #region Public properties
        public DelegateCommand goToProfileEditCommand
        {
            get
            {
                return new DelegateCommand(goToProfileEditCommand_Executed);
            }
        }
        public bool canSeeProfile
        {
            get
            {
                return _canSeeProfile;
            }

            set
            {
                _canSeeProfile = value;
                NotifyPropertyChanged("canSeeProfile");
            }
        }
        public DelegateCommand openFolderDialogTempCommand
        {
            get
            {
                return new DelegateCommand(openFolderDialogTempCommand_Executed);
            }
        }
        public DelegateCommand openFolderDialogInstallationCommand
        {
            get
            {
                return new DelegateCommand(openFolderDialogInstallationCommand_Executed);
            }
        }
        public UserWithToken loggedUser { get; set; }
        public SnackbarMessageQueue notificationsQueue { get; set; }
        public DelegateCommand resetSettingsCommand
        {
            get
            {
                _resetSettingsCommand = new DelegateCommand(ResetSettingsCommand_Executed, SaveAndResetSettingsCommand_CanExecute);
                return _resetSettingsCommand;
            }
        }
        public DelegateCommand saveSettingsCommand
        {
            get
            {
                _saveSettingsCommand = new DelegateCommand(SaveSettingsCommand_Executed, SaveAndResetSettingsCommand_CanExecute);
                return _saveSettingsCommand;
            }
        }
        public string installingFolder
        {
            get
            {
                return _installingFolder;
            }
            set
            {
                _installingFolder = value;
                NotifyPropertyChanged("installingFolder");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public string tempDownloadFolder
        {
            get
            {
                return _tempDownloadFolder;
            }
            set
            {
                _tempDownloadFolder = value;
                NotifyPropertyChanged("tempDownloadFolder");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool messageNotifications
        {
            get
            {
                return _messageNotifications;
            }
            set
            {
                _messageNotifications = value;
                NotifyPropertyChanged("messageNotifications");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool onlineNotifications
        {
            get
            {
                return _onlineNotifications;
            }
            set
            {
                _onlineNotifications = value;
                NotifyPropertyChanged("onlineNotifications");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool offlineNotifications
        {
            get
            {
                return _offlineNotifications;
            }
            set
            {
                _offlineNotifications = value;
                NotifyPropertyChanged("offlineNotifications");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool playingGameNotifications
        {
            get
            {
                return _playingGameNotifications;
            }
            set
            {
                _playingGameNotifications = value;
                NotifyPropertyChanged("playingGameNotifications");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool minimizeToTray
        {
            get
            {
                return _minimizeToTray;
            }
            set
            {
                _minimizeToTray = value;
                NotifyPropertyChanged("minimizeToTray");
                _canSaveSettings = true;
                _saveSettingsCommand.RaiseCanExecuteChanged();
                _resetSettingsCommand.RaiseCanExecuteChanged();
            }
        }
        public bool isDownloadButtonVisible
        {
            get
            {
                if(_selectedLibraryGame != null)
                {
                    return !_installedGames.Contains(_selectedLibraryGame.game.name);
                }

                return true;
            }
        }
        public int downloadUnzipProgress
        {
            get
            {
                return _downloadUnzipProgress;
            }
            set
            {
                _downloadUnzipProgress = value;
                NotifyPropertyChanged("downloadUnzipProgress");
            }
        }
        public string gameBeingPlayed
        {
            get
            {
                return _gameBeingPlayed;
            }
            set
            {
                _gameBeingPlayed = value;
                SharedInfo.gameBeingPlayed = value;
                NotifyPropertyChanged("gameBeingPlayed");
            }
        }
        public string gameBeingUnzipped
        {
            get
            {
                return _gameBeingUnzipped;
            }
            set
            {
                _gameBeingUnzipped = value;
                NotifyPropertyChanged("gameBeingUnzipped");
            }
        }
        public string gameBeingDownloaded
        {
            get
            {
                return _gameBeingDownloaded;
            }
            set
            {
                _gameBeingDownloaded = value;
                NotifyPropertyChanged("gameBeingDownloaded");
            }
        }
        public string conversationTextToSend
        {
            get
            {
                return _conversationTextToSend;
            }
            set
            {
                _conversationTextToSend = value;
                NotifyPropertyChanged("conversationTextToSend");
            }
        }
        public List<string> installedGames
        {
            get
            {
                return _installedGames;
            }

            set
            {
                _installedGames = value;
                NotifyPropertyChanged("installedGames");
            }
        }
        public UserForFriendList selectedConversation
        {
            get
            {
                return _selectedConversation;
            }

            set
            {
                _selectedConversation = value;
                NotifyPropertyChanged("selectedConversation");
            }
        }
        public ObservableCollection<UserForFriendList> conversations
        {
            get
            {
                return _conversations;
            }

            set
            {
                _conversations = value;
                NotifyPropertyChanged("conversations");
            }
        }
        public RelayCommand<string> openGameCommand
        {
            get
            {
                _openGameCommand = new RelayCommand<string>(openGameCommand_Executed, openGameCommand_CanExecute);
                return _openGameCommand;
            }
        }
        public RelayCommand<string> downloadGameCommand
        {
            get
            {
                _downloadGameCommand = new RelayCommand<string>(downloadGameCommand_Executed, downloadGameCommand_CanExecute);
                return _downloadGameCommand;
            }
        }
        public RelayCommand<string> buyStoreGameCommand
        {
            get
            {
                _buyStoreGameCommand = new RelayCommand<string>(buyStoreGameCommand_Executed, buyStoreGameCommand_CanExecute);
                return _buyStoreGameCommand;
            }
        }
        public DelegateCommand sendMessageCommand
        {
            get
            {
                return new DelegateCommand(sendMessageCommand_Executed);
            }
        }
        public RelayCommand<string> openConversationCommand
        {
            get
            {
                return new RelayCommand<string>(openConversationCommand_Executed);
            }
        }
        public RelayCommand<UserGamesWithGameAndFriends> startPlayingGameCommand
        {
            get
            {
                return new RelayCommand<UserGamesWithGameAndFriends>(startPlayingGameCommand_Executed);
            }
        }
        public RelayCommand<string> rejectFriendshipCommand
        {
            get
            {
                return new RelayCommand<string>(rejectFriendshipCommand_Executed);
            }
        }
        public RelayCommand<string> acceptFriendshipCommand
        {
            get
            {
                return new RelayCommand<string>(acceptFriendshipCommand_Executed);
            }
        }
        public ObservableCollection<UserForFriendList> friendsListOnline
        {
            get
            {
                return _friendsListOnline;
            }
            set
            {
                _friendsListOnline = value;
                NotifyPropertyChanged("friendsListOnline");
            }
        }
        public ObservableCollection<UserForFriendList> friendsListOffline
        {
            get
            {
                return _friendsListOffline;
            }
            set
            {
                _friendsListOffline = value;
                NotifyPropertyChanged("friendsListOffline");
            }
        }
        public ObservableCollection<UserForFriendList> friendsListPending
        {
            get
            {
                return _friendsListPending;
            }
            set
            {
                _friendsListPending = value;
                NotifyPropertyChanged("friendsListPending");
            }
        }
        public ObservableCollection<UserForFriendList> friendsList
        {
            set
            {
                //We set the new value and notify changes to all lists which depend on friendList
                _friendsList = value;
                friendsListOnline = new ObservableCollection<UserForFriendList>(value?.Where(x => x.online && x.relationship.acceptedRequestDate != null).ToList());
                friendsListOffline = new ObservableCollection<UserForFriendList>(value?.Where(x => !x.online && x.relationship.acceptedRequestDate != null).ToList());
                //All the requests from other users we haven't accepted yet (where the user1 -the friend request sender- is not us)
                friendsListPending = new ObservableCollection<UserForFriendList>(value?.Where(x => x.relationship.acceptedRequestDate == null && x.relationship.user1 != SharedInfo.loggedUser.nickname).ToList());
            }
        }
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
                _canClickOnBuyGame = true;

                //We try to get the information about an user's relationship with a game and link it to the selectedStoreGame
                UserGames relationship = null;
                try
                {
                    relationship = _libraryGames.Single(x => x.game.name == _selectedStoreGame.game.name).relationship;
                    _canClickOnBuyGame = false; //If the user doesn't own the game, Single() will throw an exception and won't set _canClickOnBuyGame to false
                }catch(InvalidOperationException ex) { }

                _selectedStoreGame.relationship = relationship;
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
                NotifyPropertyChanged("isDownloadButtonVisible");

                //Because we selected a library game, we change the visibility of the list
                if(value == null)
                    libraryGameVisible = Visibility.Hidden;
                else
                    libraryGameVisible = Visibility.Visible;
            }
        }
        public ObservableCollection<UserGamesWithGameAndFriends> libraryGames
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

                //The user will be able to see the profile only if it isn't private, if the current user is an admin, or if he's trying to see his own profile
                if(!value.user.privateProfile || SharedInfo.loggedUser.admin || (value.user.nickname == SharedInfo.loggedUser.nickname))
                    canSeeProfile = true;
                else
                    canSeeProfile = false;
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
                else if (_selectedOption.name == "Log out") //If the user wants to log out, we close the CHAIR window
                {
                    //Reset the remember me values because we logged out
                    SettingUtils.setUsernameRememberMe("");
                    SettingUtils.setPasswordRememberMe("");

                    //Close this window but without  closing the entire application, we need to go out to the login window
                    _view.CloseWithParameter(false);
                    return;
                }

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
            //If the user wants to minimize the tray when X is closed, then it is minimized to the tray. Otherwise, application closes and goes back to the login window.
            if(SettingUtils.getMinimizeToTraySetting())
                _view.MinimizeToTray();
            else
                _view.CloseWithParameter(true); //Close this window
        }

        private void MinimizeCommand_Executed()
        {
            _view.Minimize();
        }

        private void goToProfileCommand_Executed(string nickname)
        {
            _signalR.proxy.Invoke("getUserProfile", nickname, SharedInfo.loggedUser.token);

            //We mark "Community" on the list, because it isn't our profile and there is no "Profile" option
            selectedOption = _optionsList.Single(x => x.name == "Community");
            //But we change the page to "Profile" to display the user's information
            _view.ChangePage("Profile", this);
        }

        private void searchUsersCommand_Executed(string search)
        {
            _signalR.proxy.Invoke("searchForUsers", search, SharedInfo.loggedUser.token);
        }

        private void goToGamePageCommand_Executed(string game)
        {
            _signalR.proxy.Invoke("getGameInformation", SharedInfo.loggedUser.nickname, game, SharedInfo.loggedUser.token);
            _goToGamePage = true;
        }

        private void addFriendCommand_Executed(string user2)
        {
            _signalR.proxy.Invoke("addFriend", SharedInfo.loggedUser.nickname, user2, SharedInfo.loggedUser.token);
            _searchList.Single(x => x.user.nickname == user2).relationshipExists = true;
        }

        private void rejectFriendshipCommand_Executed(string user2)
        {
            _signalR.proxy.Invoke("endFriendship", SharedInfo.loggedUser.nickname, user2, SharedInfo.loggedUser.token);
            _friendsList.Remove(_friendsList.Single(x => x.nickname == user2));
            NotifyPropertyChanged("friendsListPending");
        }

        private void acceptFriendshipCommand_Executed(string user2)
        {
            _signalR.proxy.Invoke("acceptFriendship", SharedInfo.loggedUser.nickname, user2, SharedInfo.loggedUser.token);
        }

        private void startPlayingGameCommand_Executed(UserGamesWithGameAndFriends game)
        {
            _signalR.proxy.Invoke("startPlayingGame", SharedInfo.loggedUser.nickname, game.game.name, SharedInfo.loggedUser.token);
        }

        private void openConversationCommand_Executed(string friend)
        {
            openNewConversation(friend);
        }

        private void sendMessageCommand_Executed()
        {
            if(!string.IsNullOrWhiteSpace(_conversationTextToSend))
            {
                Message message = new Message();
                message.sender = SharedInfo.loggedUser.nickname;
                message.receiver = _selectedConversation.nickname;
                message.text = _conversationTextToSend;
                message.date = DateTime.Now;

                _signalR.proxy.Invoke("sendMessage", message, SharedInfo.loggedUser.token);

                selectedConversation.messages.Add(message);

                _sounds.PlayMessageSound();

                conversationTextToSend = "";
            }
        }

        private void buyStoreGameCommand_Executed(string gameName)
        {
            _canClickOnBuyGame = false;
            _buyStoreGameCommand.RaiseCanExecuteChanged();

            UserGames newRelationship = new UserGames { user = SharedInfo.loggedUser.nickname, game = gameName };
            _signalR.proxy.Invoke("buyGame", newRelationship, SharedInfo.loggedUser.token);
        }

        private bool buyStoreGameCommand_CanExecute(string game)
        {
            return _canClickOnBuyGame;
        }

        private void downloadGameCommand_Executed(string gameName)
        {
            gameBeingDownloaded = gameName;
            _downloadGameCommand.RaiseCanExecuteChanged();

            string tempFilePath = SettingUtils.getTempDownloadFolder();
            tempFilePath += $"\\{gameName}.zip";
            string downloadUrl = _libraryGames.Single(x => x.game.name == gameName).game.downloadUrl;
            Uri downloadUri = new Uri(downloadUrl);

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += DownloadCompleted;
            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadFileAsync(downloadUri, tempFilePath);
        }

        private bool downloadGameCommand_CanExecute(string gameName)
        {
            return _gameBeingDownloaded == null && _gameBeingUnzipped == null;
        }

        private void openGameCommand_Executed(string gameName)
        {
            //We change the game we're playing now
            gameBeingPlayed = gameName;

            //And raise the can execute changed event
            _openGameCommand.RaiseCanExecuteChanged();

            //Variables that will be sent to the process starter
            string whatToOpen = "";
            string arguments = "";

            //We get the file that we need to open
            string instructions = _libraryGames.Single(x => x.game.name == gameName).game.instructions;
            string[] separateInstructions = instructions.Split(';');
            string key;
            string value;
            foreach(string instruction in separateInstructions)
            {
                string[] instructionsSplit = instruction.Split(':');
                key = instructionsSplit[0];
                value = instructionsSplit.Count() == 2 ? instructionsSplit[1] : "";

                switch (key)
                {
                    case "RUN":
                            whatToOpen = SettingUtils.getInstallingFolder() + $"\\{gameName}\\{value}";
                        break;

                    case "RUNJAVA":
                            whatToOpen = "java";
                        break;

                    //ARGUMENTS MUST BE AFTER THE "RUN" INSTRUCTION IN THE INSTRUCTIONS VARIABLE
                    case "ARGUMENTSJAVA":
                            arguments = "-jar " + SettingUtils.getInstallingFolder() + $"\\{gameName}\\{value}";
                        break;
                }
            }

            //We get all friends who are connected to notify them we're going offline
            List<string> usersNicknameList = new List<string>();
            foreach (UserForFriendList user in friendsListOnline)
                usersNicknameList.Add(user.nickname);

            //Call to the server to inform that we're now playing
            _signalR.proxy.Invoke("startPlayingGame", SharedInfo.loggedUser.nickname, gameName, SharedInfo.loggedUser.token, usersNicknameList);

            //Actually open the game :D
            _gameProcess = new Process();
            _gameProcess.Exited += new EventHandler(FinishedPlaying);
            _gameProcess.EnableRaisingEvents = true;
            _gameProcess.StartInfo.FileName = @whatToOpen;
            _gameProcess.StartInfo.Arguments = arguments;
            _gameProcess.Start();
        }

        private bool openGameCommand_CanExecute(string gameName)
        {
            return _gameBeingPlayed == null;
        }

        private void SaveSettingsCommand_Executed()
        {
            SettingUtils.setInstallingFolder(_installingFolder);
            SettingUtils.setTempDownloadFolder(_tempDownloadFolder);
            SettingUtils.setMessageNotificationSetting(_messageNotifications);
            SettingUtils.setOnlineNotificationSetting(_onlineNotifications);
            SettingUtils.setOfflineNotificationSetting(_offlineNotifications);
            SettingUtils.setPlayingGameNotificationSetting(_playingGameNotifications);
            SettingUtils.setMinimizeToTraySetting(_minimizeToTray);

            installedGames = SettingUtils.scanInstallingFolder();

            _canSaveSettings = false;
            _saveSettingsCommand.RaiseCanExecuteChanged();
            _resetSettingsCommand.RaiseCanExecuteChanged();

            notificationsQueue.Enqueue("Settings saved!");
        }

        private void ResetSettingsCommand_Executed()
        {

            installingFolder = SettingUtils.getInstallingFolder();
            tempDownloadFolder = SettingUtils.getTempDownloadFolder();
            messageNotifications = SettingUtils.getMessageNotificationSetting();
            onlineNotifications = SettingUtils.getOnlineNotificationSetting();
            offlineNotifications = SettingUtils.getOfflineNotificationSetting();
            playingGameNotifications = SettingUtils.getPlayingGameNotificationSetting();
            minimizeToTray = SettingUtils.getMinimizeToTraySetting();

            _canSaveSettings = false;
            _saveSettingsCommand.RaiseCanExecuteChanged();
            _resetSettingsCommand.RaiseCanExecuteChanged();
        }

        private bool SaveAndResetSettingsCommand_CanExecute()
        {
            return _canSaveSettings;
        }

        private void openFolderDialogInstallationCommand_Executed()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    installingFolder = dialog.SelectedPath;
            }
        }

        private void openFolderDialogTempCommand_Executed()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    tempDownloadFolder = dialog.SelectedPath;
            }
        }

        private void goToProfileEditCommand_Executed()
        {
            _view.ChangePage("ProfileEdit", this);
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

        private void getAllMyGames(ObservableCollection<UserGamesWithGameAndFriends> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                libraryGames = obj;
            });
        }

        private void getGameInformation(GameStore obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                selectedStoreGame = obj;
                if(_goToGamePage)
                    _view.ChangePage("Game", this);

                _goToGamePage = false;
            });
        }

        private void searchForUsers(List<UserSearch> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                searchList = obj;
            });
        }

        private void getFriends(ObservableCollection<UserForFriendList> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                friendsList = obj;

                if(_initializing)
                {
                    //Set this back to false so all of this doesn't get called when we update our friends while using the application
                    _initializing = false;

                    //We get all friends who are connected
                    List<string> usersNicknameList = new List<string>();
                    foreach(UserForFriendList user in friendsListOnline)
                        usersNicknameList.Add(user.nickname);

                    //Go online!!!
                    _signalR.proxy.Invoke("goOnline", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.admin, SharedInfo.loggedUser.token, usersNicknameList);
                }
            });
        }

        private void onlineSuccessful()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Navigate to your profile
                selectedOption = _optionsList[0];
            });
        }

        private void updateFriendListWithNotification(string notificationMessage, NotificationType notificationType)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Update our friend list
                _signalR.proxy.Invoke("getFriends", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.token);

                //Show notification
                if(!string.IsNullOrEmpty(notificationMessage))
                    showNotification(notificationMessage);

                if(notificationType != NotificationType.GENERIC)
                {
                    switch(notificationType)
                    {
                        case NotificationType.ONLINE:
                            _sounds.PlayOnlineSound();
                            break;

                        case NotificationType.OFFLINE:
                            _sounds.PlayOfflineSound();
                            break;

                        case NotificationType.PLAYING:
                            _sounds.PlayPlayingSound();
                            break;
                    }
                }
            });
        }

        private void youHaveBeenBanned(BanResponse ban)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                MessageBox.Show($"You have been banned for {ban.banReason} until {ban.bannedUntil.ToLongDateString()}. Pick up your things and GTFO.");

                //Close the application completely
                _view.CloseWithParameter(true);
            });
        }

        private void unexpectedError(string error)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                MessageBox.Show(error);
            });
        }

        private void getConversation(string friendName, ObservableCollection<Message> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Update the UserForFriendList object with the messages
                conversations.Single(x => x.nickname == friendName).messages = obj;
                
                //We change the selected conversation to the one we just opened
                selectedConversation = conversations.Single(x => x.nickname == friendName);
            });
        }

        private void gameBought(string game)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //We refresh our games
                _signalR.proxy.Invoke("getAllMyGamesAndFriends", SharedInfo.loggedUser.token);

                notificationsQueue.Enqueue($"{game} was succesfully added to your library");
            });
        }

        private void receiveMessage(Message message)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //Play sound
                _sounds.PlayMessageSound();

                string friend = message.sender;

                try
                {
                    //Basically, a Contains check
                    conversations.Single(x => x.nickname == friend).messages.Add(message);
                }
                catch (InvalidOperationException ex)
                {
                    //If it throws this exception, it means that the conversations doesn't contain our conversation with that user
                    openNewConversation(friend);
                    _view.OpenConversation();
                }
            });
        }

        private void closedGameSuccessfully()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                //We refresh our games
                _signalR.proxy.Invoke("getAllMyGamesAndFriends", SharedInfo.loggedUser.token);
            });
        }
        #endregion


        #region Functions
        public void dispose()
        {
            //TODO: Update with all variables when project is finished

            _profileUser = null;
            _view = null;
            _selectedOption = null;
            _optionsList = null;
            _signalR = null;
            _storeGames = null;
            _frontPageGame = null;
            _libraryGames = null;
            _selectedLibraryGame = null;
            _selectedStoreGame = null;
            _searchList = null;
            loggedUser = null;
        }

        private void closeOpenGame()
        {
            //We set back to null the variable where we hold the game we're playing
            gameBeingPlayed = null;
            _openGameCommand.RaiseCanExecuteChanged();

            //We get all friends who are connected to notify them we're going offline
            List<string> usersNicknameList = new List<string>();
            foreach (UserForFriendList user in friendsListOnline)
                usersNicknameList.Add(user.nickname);

            //Call to the server to inform that we're no longer playing
            _signalR.proxy.Invoke("stopPlayingGame", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.token, usersNicknameList);
        }

        public void closeOpenGameIfOpen()
        {
            if (_gameProcess != null)
            {
                _gameProcess.EnableRaisingEvents = false;
                _gameProcess.Kill();

                closeOpenGame();
            }
        }

        public void disconnectFromSignalR()
        {
            //We get all friends who are connected to notify them we're going offline
            List<string> usersNicknameList = new List<string>();
            foreach (UserForFriendList user in friendsListOnline)
                usersNicknameList.Add(user.nickname);

            _signalR.proxy.Invoke("goOffline", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.admin, SharedInfo.loggedUser.token, usersNicknameList);

            SignalRHubsConnection.closeChairHub();
        }

        private void showNotification(string message)
        {
            notificationsQueue.Enqueue(message);
        }

        private void openNewConversation(string friend)
        {
            UserForFriendList user = null;

            try
            {
                user = conversations.Single(x => x.nickname == friend);
            }
            catch (InvalidOperationException ex) { }

            //Add the reference of the friendsList object to the conversations list
            if (user == null)
                conversations.Add(_friendsList.Single(x => x.nickname == friend));
            
            _signalR.proxy.Invoke("getConversation", SharedInfo.loggedUser.nickname, friend, SharedInfo.loggedUser.token);
        }
        #endregion

        #region Download Events
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadUnzipProgress = e.ProgressPercentage;
        }

        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            gameBeingUnzipped = gameBeingDownloaded;
            downloadUnzipProgress = 0;
            gameBeingDownloaded = null;

            using (ZipFile zip = ZipFile.Read(SettingUtils.getTempDownloadFolder()+$"\\{gameBeingUnzipped}.zip"))
            {
                zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(zip_ExtractProgress);
                zip.ExtractAll(SettingUtils.getInstallingFolder(), ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer > 0)
            {
                downloadUnzipProgress = Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer);

                if (e.BytesTransferred == e.TotalBytesToTransfer && gameBeingUnzipped != null)
                {
                    notificationsQueue.Enqueue($"{gameBeingUnzipped} is now playable");
                    //File.Delete(SettingUtils.getTempDownloadFolder() + $"\\{gameBeingUnzipped}.zip");
                    gameBeingUnzipped = null;
                    downloadUnzipProgress = 0;
                    installedGames = SettingUtils.scanInstallingFolder();

                    _downloadGameCommand.RaiseCanExecuteChanged();
                    
                    NotifyPropertyChanged("isDownloadButtonVisible");
                }
            }
        }
        #endregion

        #region Playing Events
        private void FinishedPlaying(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                closeOpenGame();
                _gameProcess = null;
            });
        }
        #endregion
    }
}
