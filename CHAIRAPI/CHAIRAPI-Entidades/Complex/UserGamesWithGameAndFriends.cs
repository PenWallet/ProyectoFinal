using CHAIRAPI_Entidades.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entidades.Complex
{
    public class UserGamesWithGameAndFriends
    {
        public UserGames relationship { get; set; } 
        public Game game { get; set; }
        public List<UserFriends> friends { get; set; }

        public UserGamesWithGameAndFriends(UserGames relationship, Game game, List<UserFriends> friends)
        {
            this.relationship = relationship;
            this.game = game;
            this.friends = friends;
        }

        public UserGamesWithGameAndFriends()
        {
        }
    }
}
