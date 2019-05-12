using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entities.Persistent
{
    public class UserGames
    {
        public string user { get; set; }
        public string game { get; set; }
        public decimal hoursPlayed { get; set; }
        public DateTime acquisitionDate { get; set; }
        public DateTime? lastPlayed { get; set; }
        public bool playing { get; set; }

        public UserGames(string user, string game, decimal hoursPlayed, DateTime acquisitionDate, DateTime? lastPlayed, bool playing)
        {
            this.user = user;
            this.game = game;
            this.hoursPlayed = hoursPlayed;
            this.acquisitionDate = acquisitionDate;
            this.lastPlayed = lastPlayed;
            this.playing = playing;
        }

        public UserGames()
        {
        }
    }
}
