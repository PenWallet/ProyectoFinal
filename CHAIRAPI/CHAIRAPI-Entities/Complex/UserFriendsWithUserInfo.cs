using CHAIRAPI_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entities.Complex
{
    public class UserWhoPlaysMyGame
    {
        public string game { get; set; }
        public User user { get; set; }

        public UserWhoPlaysMyGame(string game, User user)
        {
            this.game = game;
            this.user = user;
        }

        public UserWhoPlaysMyGame()
        {
            this.game = "";
            this.user = new User();
        }
    }
}
