﻿using CHAIR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Complex
{
    public class UserForFriendList
    {
        public UserFriends relationship { get; set; }
        public string nickname { get; set; }
        public bool online { get; set; }
        public bool admin { get; set; }
        public string gamePlaying { get; set; } //Variable used to know whether the user is playing a game or not. If not, it's null, otherwise, it's the game's name

        public UserForFriendList(UserFriends relationship, string nickname, bool online, bool admin, string gamePlaying)
        {
            this.relationship = relationship;
            this.nickname = nickname;
            this.online = online;
            this.admin = admin;
            this.gamePlaying = gamePlaying;
        }

        public UserForFriendList()
        {
            this.relationship = new UserFriends();
            this.nickname = "";
            this.online = false;
            this.admin = false;
            this.gamePlaying = null;
        }
    }
}
