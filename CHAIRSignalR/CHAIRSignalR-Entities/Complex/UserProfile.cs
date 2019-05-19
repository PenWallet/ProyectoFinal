using CHAIRSignalR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entities.Complex
{
    /// <summary>
    /// Class used to display all the information necessary to show an user's profile
    /// </summary>
    public class UserProfile
    {
        public User user { get; set; }
        public List<UserGamesWithGame> games { get; set; }

        public UserProfile(User user, List<UserGamesWithGame> games)
        {
            this.user = user;
            this.games = games;
        }

        public UserProfile()
        {
            this.user = new User();
            this.games = new List<UserGamesWithGame>();
        }

    }
}
