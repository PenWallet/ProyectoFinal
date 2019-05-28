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

            _signalR.proxy.On<string, AdminNotificationType>("adminNotification", adminNotification);
            _signalR.proxy.On<List<Game>>("getAllStoreGames", getAllStoreGames);
            _signalR.proxy.On<List<string>>("getAllUsers", getAllUsers);
            _signalR.proxy.On<ObservableCollection<GameBeingPlayed>>("adminUpdateGamesBeingPlayed", adminUpdateGamesBeingPlayed);

            _gameToAdd = new Game();
            _processingNewGame = false;
            _updatingFrontPageGame = false;
            _gameToAdd.releaseDate = DateTime.Now;

            //Get the information from the server
            _signalR.proxy.Invoke("getAllUsers", SharedInfo.loggedUser.token);
            _signalR.proxy.Invoke("adminUpdateGamesBeingPlayed", SharedInfo.loggedUser.token);
        }
        #endregion

        #region Private properties
        private SignalRConnection _signalR;
        private List<string> _onlineUsers;
        private List<Game> _storeGames;
        private ObservableCollection<GameBeingPlayed> _gamesBeingPlayed;
        private DelegateCommand _addNewGameCommand;
        private DelegateCommand _banPlayerCommand;
        private DelegateCommand _banIpCommand;
        private DelegateCommand _changeFrontPageGameCommand;
        private Game _gameToAdd;
        private bool _processingNewGame;
        private bool _updatingFrontPageGame;
        private string _gameToUpdateToFrontPage;
        #endregion

        #region Public properties
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
        public List<string> onlineUsers
        {
            get
            {
                return _onlineUsers;
            }
            set
            {
                _onlineUsers = value;
                NotifyPropertyChanged("onlineUsers");
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
        public DelegateCommand banPlayerCommand
        {
            get
            {
                _banPlayerCommand = new DelegateCommand(BanPlayerCommand_Executed, BanPlayerCommand_CanExecute);
                return _banPlayerCommand;
            }
        }
        public DelegateCommand banIpCommand
        {
            get
            {
                _banIpCommand = new DelegateCommand(BanIpCommand_Executed, BanIpCommand_CanExecute);
                return _banIpCommand;
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

        private bool BanPlayerCommand_CanExecute()
        {
            return !_processingNewGame && !string.IsNullOrWhiteSpace(_gameToAdd.name) && !string.IsNullOrWhiteSpace(_gameToAdd.description) && !string.IsNullOrWhiteSpace(_gameToAdd.developer) && _gameToAdd.minimumAge >= 0 && !string.IsNullOrWhiteSpace(_gameToAdd.instructions) && !string.IsNullOrWhiteSpace(_gameToAdd.downloadUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.storeImageUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.libraryImageUrl);
        }

        private void BanPlayerCommand_Executed()
        {
            _processingNewGame = true;
            _addNewGameCommand.RaiseCanExecuteChanged();
            _signalR.proxy.Invoke("adminAddGameToStore", _gameToAdd, SharedInfo.loggedUser.token);
        }

        private bool BanIpCommand_CanExecute()
        {
            return !_processingNewGame && !string.IsNullOrWhiteSpace(_gameToAdd.name) && !string.IsNullOrWhiteSpace(_gameToAdd.description) && !string.IsNullOrWhiteSpace(_gameToAdd.developer) && _gameToAdd.minimumAge >= 0 && !string.IsNullOrWhiteSpace(_gameToAdd.instructions) && !string.IsNullOrWhiteSpace(_gameToAdd.downloadUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.storeImageUrl) && !string.IsNullOrWhiteSpace(_gameToAdd.libraryImageUrl);
        }

        private void BanIpCommand_Executed()
        {
            _processingNewGame = true;
            _addNewGameCommand.RaiseCanExecuteChanged();
            _signalR.proxy.Invoke("adminAddGameToStore", _gameToAdd, SharedInfo.loggedUser.token);
        }

        private void ChangeFrontPageGameCommand_Executed()
        {
            _updatingFrontPageGame = true;
            _changeFrontPageGameCommand.RaiseCanExecuteChanged();
            _signalR.proxy.Invoke("adminChangeFrontPageGame", _gameToUpdateToFrontPage, SharedInfo.loggedUser.token);
        }

        private bool ChangeFrontPageGameCommand_CanExecute()
        {
            return !_updatingFrontPageGame && !string.IsNullOrWhiteSpace(_gameToUpdateToFrontPage);
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
                        _changeFrontPageGameCommand.RaiseCanExecuteChanged();
                        break;
                }
            });
        }

        private void getAllStoreGames(List<Game> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                storeGames = obj;
            });
        }

        private void getAllUsers(List<string> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                onlineUsers = obj;
            });
        }

        private void adminUpdateGamesBeingPlayed(ObservableCollection<GameBeingPlayed> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                gamesBeingPlayed = obj;
            });
        }
        #endregion

        #region Methods

        #endregion
    }
}
