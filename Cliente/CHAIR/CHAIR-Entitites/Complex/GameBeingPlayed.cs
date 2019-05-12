using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entidades.Complex
{
    public class GameBeingPlayed
    {
        public string game { get; set; }
        public int numberOfPlayers { get; set; }
        public int numberOfPlayersPlaying { get; set; }

        public GameBeingPlayed(string game, int numberOfPlayers, int numberOfPlayersPlaying)
        {
            this.game = game;
            this.numberOfPlayers = numberOfPlayers;
            this.numberOfPlayersPlaying = numberOfPlayersPlaying;
        }

        public GameBeingPlayed()
        {
        }
    }
}
