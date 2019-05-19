using CHAIRSignalR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entities.Complex
{
    /// <summary>
    /// Class used to show the games in the stores
    /// It contains all the information about a game, as well as the relationship information about the requested user
    /// If the user doesn't play that game, relationship will be null, meaning he doesn't play it
    /// </summary>
    public class GameStore
    {
        public Game game { get; set; }
        public UserGames relationship { get; set; }

        public GameStore(Game game, UserGames relationship)
        {
            this.game = game;
            this.relationship = relationship;
        }

        public GameStore()
        {
            this.game = new Game();
            this.relationship = new UserGames();
        }
    }
}
