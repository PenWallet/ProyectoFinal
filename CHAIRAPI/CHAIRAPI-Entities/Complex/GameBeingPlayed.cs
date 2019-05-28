using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entities.Complex
{
    public class GameBeingPlayed
    {
        public string game { get; set; }
        public int numberOfPlayers { get; set; }
        public int numberOfPlayersPlaying { get; set; }
        public decimal totalRegisteredHours { get; set; }

        public GameBeingPlayed(string game, int numberOfPlayers, int numberOfPlayersPlaying, decimal totalRegisteredHours)
        {
            this.game = game;
            this.numberOfPlayers = numberOfPlayers;
            this.numberOfPlayersPlaying = numberOfPlayersPlaying;
            this.totalRegisteredHours = totalRegisteredHours;
        }

        public GameBeingPlayed()
        {
        }
    }
}
