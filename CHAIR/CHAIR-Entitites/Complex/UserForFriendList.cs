﻿using CHAIR_Entities.Models;
using CHAIR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CHAIR_Entities.Complex
{
    public class UserForFriendList : VMBase
    {
        private ObservableCollection<Message> _messages;
        public bool _online { get; set; }
        public string _gamePlaying { get; set; }

        public UserFriends relationship { get; set; }
        public bool admin { get; set; }
        public string nickname { get; set; }
        public ObservableCollection<Message> messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                NotifyPropertyChanged("messages");
            }
        }
        public bool online
        {
            get
            {
                return _online;
            }
            set
            {
                _online = value;
                NotifyPropertyChanged("online");
            }
        }
        public string gamePlaying //Variable used to know whether the user is playing a game or not. If not, it's null, otherwise, it's the game's name
        {
            get
            {
                return _gamePlaying;
            }
            set
            {
                _gamePlaying = value;
                NotifyPropertyChanged("gamePlaying");
            }
        }

        public UserForFriendList(UserFriends relationship, ObservableCollection<Message> messages, string nickname, bool online, bool admin, string gamePlaying)
        {
            this.relationship = relationship;
            this.messages = messages;
            this.nickname = nickname;
            this.online = online;
            this.admin = admin;
            this.gamePlaying = gamePlaying;
        }

        public UserForFriendList()
        {
            this.relationship = new UserFriends();
            this.messages = new ObservableCollection<Message>();
            this.nickname = "";
            this.online = false;
            this.admin = false;
            this.gamePlaying = null;
        }
    }
}
