using CHAIRAPI_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entities.Complex
{
    /// <summary>
    /// Class used to send information about an user's game with all the information about the game
    /// </summary>
    public class UserGamesWithGame
    {
        public UserGames relationship { get; set; }
        public Game game { get; set; }

        public UserGamesWithGame(UserGames relationship, Game game)
        {
            this.relationship = relationship;
            this.game = game;
        }

        public UserGamesWithGame()
        {
            this.relationship = new UserGames();
            this.game = new Game();
        }
    }
}
