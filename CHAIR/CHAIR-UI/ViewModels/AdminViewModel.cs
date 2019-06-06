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
using CHAIR_Entities.Persistent;
using GalaSoft.MvvmLight.Command;
using CHAIR_Entities.Enums;
using System.Collections.ObjectModel;

namespace CHAIR_UI.ViewModels
{
    public class AdminViewModel : VMBase
    {
        #region Default constructor
        public AdminViewModel()
        {
            _signalR = SignalRHubsConnection.chairHub;
            _processingNewGame = false;
            _initializingStoreGamesNames = true;

            _signalR.proxy.On<string, AdminNotificationType>("adminNotification", adminNotification);
            _signalR.proxy.On<List<string>>("adminGetAllUsers", adminGetAllUsers);
            _signalR.proxy.On<List<string>>("adminGetBannedUsers", adminGetBannedUsers);
            _signalR.proxy.On<ObservableCollection<GameBeingPlayed>>("adminUpdateGamesBeingPlayed", adminUpdateGamesBeingPlayed);
            _signalR.proxy.On("adminUpdateGamesBeingPlayedAlert", adminUpdateGamesBeingPlayedAlert);

            //Prepare variables from the add game functionality
            _gameToAdd = new Game();
            _processingNewGame = false;
            _updatingFrontPageGame = false;
            _gameToAdd.releaseDate = DateTime.Now;

            //Prepare variables from the ban user functionality
            _selectedUserToBanIp = false;
            _banButtonText = "Ban User";
            _processingBan = false;
            _processingNewGame = false;
            _processingPardon = false;
            _pardonButtonText = "Pardon User";

            //Get the information from the server
            _signalR.proxy.Invoke("adminGetAllUsers", SharedInfo.loggedUser.token);
            _signalR.proxy.Invoke("adminUpdateGamesBeingPlayed", SharedInfo.loggedUser.token);
            _signalR.proxy.Invoke("adminGetBannedUsers", SharedInfo.loggedUser.token);
        }
        #endregion

        #region Private properties
        private SignalRConnection _signalR;
        private List<string> _allUsers;
        private List<string> _bannedUsers;
        private List<string> _storeGames;
        private ObservableCollection<GameBeingPlayed> _gamesBeingPlayed;
        private DelegateCommand _addNewGameCommand;
        private DelegateCommand _banUserAndIpCommand;
        private DelegateCommand _pardonUserAndIpCommand;
        private DelegateCommand _changeFrontPageGameCommand;
        private Game _gameToAdd;
        private bool _processingNewGame;
        private bool _updatingFrontPageGame;
        private bool _selectedUserToBanIp;
        private bool _processingBan;
        private bool _processingPardon;
        private bool _selectedUserToPardonIp;
        private bool _initializingStoreGamesNames;
        private string _selectedGameToFrontPage;
        private string _selectedUserToBan;
        private string _selectedUserToBanDuration;
        private string _selectedUserToBanReason;
        private string _selectedUserToPardon;
        private string _banButtonText;
        private string _pardonButtonText;
        #endregion

        #region Public properties
        public string selectedGameToFrontPage
        {
            get
            {
                return _selectedGameToFrontPage;
            }
            set
            {
                _selectedGameToFrontPage = value;
                NotifyPropertyChanged("selectedGameToFrontPage");
                _changeFrontPageGameCommand.RaiseCanExecuteChanged();
            }
        }
        public bool selectedUserToPardonIp
        {
            get
            {
                return _selectedUserToPardonIp;
            }
            set
            {
                _selectedUserToPardonIp = value;
                NotifyPropertyChanged("selectedUserToPardonIp");

                if (value)
                    pardonButtonText = "Pardon User and IP";
                else
                    pardonButtonText = "Pardon User";
            }
        }
        public string selectedUserToPardon
        {
            get
            {
                return _selectedUserToPardon;
            }
            set
            {
                _selectedUserToPardon = value;
                NotifyPropertyChanged("selectedUserToPardon");
                _pardonUserAndIpCommand.RaiseCanExecuteChanged();
            }
        }
        public string pardonButtonText
        {
            get
            {
                return _pardonButtonText;
            }
            set
            {
                _pardonButtonText = value;
                NotifyPropertyChanged("pardonButtonText");
            }
        }
        public string banButtonText
        {
            get
            {
                return _banButtonText;
            }
            set
            {
                _banButtonText = value;
                NotifyPropertyChanged("banButtonText");
            }
        }
        public bool selectedUserToBanIp
        {
            get
            {
                return _selectedUserToBanIp;
            }
            set
            {
                _selectedUserToBanIp = value;
                NotifyPropertyChanged("selectedUserToBanIp");

                banButtonText = value ? "Ban User and IP" : "Ban User";

                _banUserAndIpCommand.RaiseCanExecuteChanged();
            }
        }
        public string selectedUserToBanReason
        {
            get
            {
                return _selectedUserToBanReason;
            }
            set
            {
                _selectedUserToBanReason = value;
                NotifyPropertyChanged("selectedUserToBanReason");
                _banUserAndIpCommand.RaiseCanExecuteChanged();
            }
        }
        public string selectedUserToBanDuration
        {
            get
            {
                return _selectedUserToBanDuration;
            }
            set
            {
                _selectedUserToBanDuration = value;
                NotifyPropertyChanged("selectedUserToBanDuration");
                _banUserAndIpCommand.RaiseCanExecuteChanged();
            }
        }
        public string selectedUserToBan
        {
            get
            {
                return _selectedUserToBan;
            }
            set
            {
                _selectedUserToBan = value;
                NotifyPropertyChanged("selectedUserToBan");
                _banUserAndIpCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameName
        {
            get
            {
                return _gameToAdd.name;
            }
            set
            {
                _gameToAdd.name = value;
                NotifyPropertyChanged("gameName");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameDescription
        {
            get
            {
                return _gameToAdd.description;
            }
            set
            {
                _gameToAdd.description = value;
                NotifyPropertyChanged("gameDescription");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameDeveloper
        {
            get
            {
                return _gameToAdd.developer;
            }
            set
            {
                _gameToAdd.developer = value;
                NotifyPropertyChanged("gameDeveloper");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameMinimumAge
        {
            get
            {
                return _gameToAdd.minimumAge.ToString();
            }
            set
            {
                _gameToAdd.minimumAge = int.Parse(value);
                NotifyPropertyChanged("gameMinimumAge");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public DateTime gameReleaseDate
        {
            get
            {
                return _gameToAdd.releaseDate;
            }
            set
            {
                _gameToAdd.releaseDate = value;
                NotifyPropertyChanged("gameReleaseDate");
            }
        }
        public string gameInstructions
        {
            get
            {
                return _gameToAdd.instructions;
            }
            set
            {
                _gameToAdd.instructions = value;
                NotifyPropertyChanged("gameInstructions");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameDownloadUrl
        {
            get
            {
                return _gameToAdd.downloadUrl;
            }
            set
            {
                _gameToAdd.downloadUrl = value;
                NotifyPropertyChanged("gameDownloadUrl");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameStoreImageUrl
        {
            get
            {
                return _gameToAdd.storeImageUrl;
            }
            set
            {
                _gameToAdd.storeImageUrl = value;
                NotifyPropertyChanged("gameStoreImageUrl");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public string gameLibraryImageUrl
        {
            get
            {
                return _gameToAdd.libraryImageUrl;
            }
            set
            {
                _gameToAdd.libraryImageUrl = value;
                NotifyPropertyChanged("gameLibraryImageUrl");
                _addNewGameCommand.RaiseCanExecuteChanged();
            }
        }
        public List<string> storeGames
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
        public Game gameToAdd
        {
            get
            {
                return _gameToAdd;
            }
            set
            {
                _gameToAdd = value;
                NotifyPropertyChanged("gameToAdd");
            }
        }
        public List<string> allUsers
        {
            get
            {
                return _allUsers;
            }
            set
            {
                _allUsers = value;
                NotifyPropertyChanged("allUsers");
            }
        }
        public List<string> bannedUsers
        {
            get
            {
                return _bannedUsers;
            }
            set
            {
                _bannedUsers = value;
                NotifyPropertyChanged("bannedUsers");
            }
        }
        public ObservableCollection<GameBeingPlayed> gamesBeingPlayed
        {
            get
            {
                return _gamesBeingPlayed;
            }
            set
            {
                _gamesBeingPlayed = value;
                NotifyPropertyChanged("gamesBeingPlayed");
            }
        }
        public DelegateCommand addNewGameCommand
        {
            get
            {
                _addNewGameCommand = new DelegateCommand(AddNewGameCommand_Executed, AddNewGameCommand_CanExecute);
                return _addNewGameCommand;
            }
        }
        public DelegateCommand banUserAndIpCommand
        {
            get
            {
                _banUserAndIpCommand = new DelegateCommand(BanUserAndIpCommand_Executed, BanUserAndIpCommand_CanExecute);
                return _banUserAndIpCommand;
            }
        }
        public DelegateCommand changeFrontPageGameCommand
        {
            get
            {
                _changeFrontPageGameCommand = new DelegateCommand(ChangeFrontPageGameCommand_Executed, ChangeFrontPageGameCommand_CanExecute);
                return _changeFrontPageGameCommand;
            }
        }
        public DelegateCommand pardonUserAndIpCommand
        {
            get
            {
                _pardonUserAndIpCommand = new DelegateCommand(PardonUserAndIpCommand_Executed, PardonUserAndIpCommand_CanExecute);
                return _pardonUserAndIpCommand;
            }
        }
        #endregion

        #region Commands
        private bool AddNewGameCommand_CanExecute()
        {
            return !_processingNewGame && !string.IsNullOrWhiteSpace(_gameToAdd.name) && !string.IsNullOrWhiteSpace(_gameToAdd.description) && !string.IsNullOrWhiteSpace(_gameToAdd.developer) && _gameToAdd.minimumAge >= 0 && _gameToAdd.minimumAge <= 18 && !string.IsNullOrWhiteSpace(_gameToAdd.instructions) && !string.IsNullOrWhiteSpace(_gameToAdd.downloadUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.storeImageUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.libraryImageUrl);
        }

        private void AddNewGameCommand_Executed()
        {
            _processingNewGame = true;
            _addNewGameCommand.RaiseCanExecuteChanged();
            _signalR.proxy.Invoke("adminAddGameToStore", _gameToAdd, SharedInfo.loggedUser.token);
        }

        private bool BanUserAndIpCommand_CanExecute()
        {
            return !_processingBan && !string.IsNullOrWhiteSpace(_selectedUserToBan) && !string.IsNullOrWhiteSpace(_selectedUserToBanDuration) && !string.IsNullOrWhiteSpace(_selectedUserToBanReason);
        }

        private void BanUserAndIpCommand_Executed()
        {
            //Start processing ban
            _processingBan = true;
            _banUserAndIpCommand.RaiseCanExecuteChanged();

            DateTime bannedUntil = DateTime.Now;
            switch(_selectedUserToBanDuration)
            {
                case "24 hours":
                    bannedUntil = DateTime.Now.AddHours(24);
                    break;
                case "48 hours":
                    bannedUntil = DateTime.Now.AddHours(48);
                    break;
                case "7 days":
                    bannedUntil = DateTime.Now.AddDays(7);
                    break;
                case "30 days":
                    bannedUntil = DateTime.Now.AddDays(30);
                    break;
                case "6 months":
                    bannedUntil = DateTime.Now.AddMonths(6);
                    break;
                case "1 year":
                    bannedUntil = DateTime.Now.AddYears(1);
                    break;
                case "5 years":
                    bannedUntil = DateTime.Now.AddYears(5);
                    break;
                case "Permanent":
                    bannedUntil = DateTime.Now.AddYears(100);
                    break;
            }

            //Call to SignalR depending on whether the admin wants to ban only the user or the IP as well
            if(_selectedUserToBanIp)
                _signalR.proxy.Invoke("adminBanUserAndIp", _selectedUserToBan, _selectedUserToBanReason, bannedUntil, SharedInfo.loggedUser.token);
            else
                _signalR.proxy.Invoke("adminBanUser", _selectedUserToBan, _selectedUserToBanReason, bannedUntil, SharedInfo.loggedUser.token);
        }

        private void ChangeFrontPageGameCommand_Executed()
        {
            _updatingFrontPageGame = true;
            _changeFrontPageGameCommand.RaiseCanExecuteChanged();

            _signalR.proxy.Invoke("adminChangeFrontPageGame", _selectedGameToFrontPage, SharedInfo.loggedUser.token);
        }

        private bool ChangeFrontPageGameCommand_CanExecute()
        {
            return !_updatingFrontPageGame && !string.IsNullOrWhiteSpace(_selectedGameToFrontPage);
        }

        private void PardonUserAndIpCommand_Executed()
        {
            _processingPardon = true;
            _pardonUserAndIpCommand.RaiseCanExecuteChanged();

            if(_selectedUserToPardonIp)
                _signalR.proxy.Invoke("adminPardonUserAndIp", _selectedUserToPardon, SharedInfo.loggedUser.token);
            else
                _signalR.proxy.Invoke("adminPardonUser", _selectedUserToPardon, SharedInfo.loggedUser.token);
        }

        private bool PardonUserAndIpCommand_CanExecute()
        {
            return !_processingPardon && !string.IsNullOrWhiteSpace(_selectedUserToPardon);
        }
        #endregion

        #region SignalR Methods
        private void adminNotification(string message, AdminNotificationType notificationType)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                MessageBox.Show(message);

                //Depending on the notification type, we update on delegatecommand or the other
                switch(notificationType)
                {
                    case AdminNotificationType.GAMEADDED:
                        gameName = "";
                        gameDescription = "";
                        gameDeveloper = "";
                        gameMinimumAge = "0";
                        gameReleaseDate = DateTime.Now;
                        gameInstructions = "";
                        gameDownloadUrl = "";
                        gameStoreImageUrl = "";
                        gameLibraryImageUrl = "";
                        _processingNewGame = false;
                        _addNewGameCommand.RaiseCanExecuteChanged();
                        break;

                    case AdminNotificationType.FRONTPAGE:
                        _updatingFrontPageGame = false;
                        selectedGameToFrontPage = "";
                        _changeFrontPageGameCommand.RaiseCanExecuteChanged();
                        break;

                    case AdminNotificationType.BANPLAYER:
                        //Reset variables
                        _processingBan = false;
                        selectedUserToBan = "";
                        selectedUserToBanDuration = "";
                        selectedUserToBanIp = false;
                        selectedUserToBanReason = "";
                        _banUserAndIpCommand.RaiseCanExecuteChanged();
                        //Call to the server to refresh the banned players list
                        _signalR.proxy.Invoke("adminGetBannedUsers", SharedInfo.loggedUser.token);
                        break;

                    case AdminNotificationType.PARDONPLAYER:
                        //Reset variables
                        _processingPardon = false;
                        selectedUserToPardon = "";
                        selectedUserToPardonIp = false;
                        _pardonUserAndIpCommand.RaiseCanExecuteChanged();

                        //Call to the server to refresh the banned players list
                        _signalR.proxy.Invoke("adminGetBannedUsers", SharedInfo.loggedUser.token);
                        break;
                }
            });
        }
        
        private void adminGetAllUsers(List<string> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                allUsers = obj;
            });
        }

        private void adminGetBannedUsers(List<string> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                bannedUsers = obj;
            });
        }

        private void adminUpdateGamesBeingPlayed(ObservableCollection<GameBeingPlayed> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                if(_initializingStoreGamesNames)
                {
                    List<string> gamesNames = new List<string>();
                    foreach(GameBeingPlayed game in obj)
                        gamesNames.Add(game.game);

                    storeGames = gamesNames;

                    _initializingStoreGamesNames = false;
                }

                if (_gamesBeingPlayed == null || _gamesBeingPlayed.Count == 0)
                    gamesBeingPlayed = obj;
                else
                {
                    foreach (GameBeingPlayed game in _gamesBeingPlayed)
                    {
                        GameBeingPlayed gameUpdated = obj.Single(x => x.game == game.game);
                        game.numberOfPlayers = gameUpdated.numberOfPlayers;
                        game.numberOfPlayersPlaying = gameUpdated.numberOfPlayersPlaying;
                        game.totalRegisteredHours = gameUpdated.totalRegisteredHours;
                    }
                }
            });
        }

        private void adminGetAllStoreGamesNames(List<string> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                storeGames = obj;
            });
        }

        private void adminUpdateGamesBeingPlayedAlert()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                _signalR.proxy.Invoke("adminUpdateGamesBeingPlayed", SharedInfo.loggedUser.token);
            });
        }
        #endregion

        #region Methods

        #endregion
    }
}
