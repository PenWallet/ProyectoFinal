using CHAIR_Entities.Persistent;
using CHAIR_Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Complex
{
    /// <summary>
    /// Class used to show the games in the stores
    /// It contains all the information about a game, as well as the relationship information about the requested user
    /// If the user doesn't play that game, relationship will be null, meaning he doesn't play it
    /// </summary>
    public class GameStore : VMBase
    {
        private UserGames _relationship { get; set; }

        public Game game { get; set; }
        public UserGames relationship
        {
            get
            {
                return _relationship;
            }

            set
            {
                _relationship = value;
                NotifyPropertyChanged("relationship");
            }
        }
        public GameStore(Game game, UserGames relationship)
        {
            this.game = game;
            this._relationship = relationship;
        }

        public GameStore()
        {
            this.game = new Game();
            this._relationship = new UserGames();
        }
    }
}
