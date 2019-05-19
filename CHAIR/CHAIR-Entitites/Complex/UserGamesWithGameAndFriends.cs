using CHAIR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Complex
{
    public class UserGamesWithGameAndFriends
    {
        public UserGames relationship { get; set; } 
        public Game game { get; set; }
        public List<User> friends { get; set; }

        public UserGamesWithGameAndFriends(UserGames relationship, Game game, List<User> friends)
        {
            this.relationship = relationship;
            this.game = game;
            this.friends = friends;
        }

        public UserGamesWithGameAndFriends()
        {
            this.relationship = new UserGames();
            this.game = new Game();
            this.friends = new List<User>();
        }
    }
}
