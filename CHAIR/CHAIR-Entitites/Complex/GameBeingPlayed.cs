using CHAIR_Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Complex
{
    public class GameBeingPlayed : VMBase
    {
        private int _numberOfPlayers { get; set; }
        private int _numberOfPlayersPlaying { get; set; }
        private decimal _totalRegisteredHours { get; set; }

        public string game { get; set; }
        public int numberOfPlayers
        {
            get
            {
                return _numberOfPlayers;
            }
            set
            {
                _numberOfPlayers = value;
                NotifyPropertyChanged("numberOfPlayers");
            }
        }
        public int numberOfPlayersPlaying
        {
            get
            {
                return _numberOfPlayersPlaying;
            }
            set
            {
                _numberOfPlayersPlaying = value;
                NotifyPropertyChanged("numberOfPlayersPlaying");
            }
        }
        public decimal totalRegisteredHours
        {
            get
            {
                return _totalRegisteredHours;
            }
            set
            {
                _totalRegisteredHours = value;
                NotifyPropertyChanged("totalRegisteredHours");
            }
        }

        public GameBeingPlayed(string game, int numberOfPlayers, int numberOfPlayersPlaying, decimal totalRegisteredHours)
        {
            this.game = game;
            this._numberOfPlayers = numberOfPlayers;
            this._numberOfPlayersPlaying = numberOfPlayersPlaying;
            this._totalRegisteredHours = totalRegisteredHours;
        }

        public GameBeingPlayed()
        {
        }
    }
}
