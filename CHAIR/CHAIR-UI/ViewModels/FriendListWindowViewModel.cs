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
    public class FriendListWindowViewModel : VMBase
    {
        #region Constructors
        public FriendListWindowViewModel()
        {
            _signalR = SignalRHubsConnection.chairHub;

            _signalR.proxy.On<List<UserForFriendList>>("getFriends", getFriends);

            _signalR.proxy.Invoke("getFriends", SharedInfo.loggedUser.nickname, SharedInfo.loggedUser.token);
        }

        #endregion


        #region Private properties
        private SignalRConnection _signalR;
        private List<UserForFriendList> _friendsList;
        #endregion

        #region Public properties
        public List<UserForFriendList> onlineFriends
        {
            get
            {
                return _friendsList.Where(x => x.online && x.relationship.acceptedRequestDate != null).ToList();
            }
        }
        public List<UserForFriendList> offlineFriends
        {
            get
            {
                return _friendsList.Where(x => !x.online && x.relationship.acceptedRequestDate != null).ToList();
            }
        }
        public List<UserForFriendList> pendingRequestFriends
        {
            get
            {
                //Return all the friends from whom we haven't accepted the request this user sent them (where the user1 (the friend request sender) is not us)
                return _friendsList.Where(x => x.relationship.acceptedRequestDate == null && x.relationship.user1 != SharedInfo.loggedUser.nickname).ToList();
            }
        }
        public List<UserForFriendList> friendsList
        {
            set
            {
                //We set the new value and notify changes to all lists which depend on friendList
                _friendsList = value;
                NotifyPropertyChanged("onlineFriends");
                NotifyPropertyChanged("offlineFriends");
                NotifyPropertyChanged("pendingRequestFriends");
            }
        }
        #endregion


        #region Commands
        #endregion


        #region SignalR Methods
        private void getFriends(List<UserForFriendList> obj)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                friendsList = obj;
            });
        }

        private void unexpectedError(string error)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                MessageBox.Show(error);
            });
        }
        #endregion
    }
}
