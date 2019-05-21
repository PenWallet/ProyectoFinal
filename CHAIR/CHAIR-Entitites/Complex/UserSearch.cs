using CHAIR_Entities.Complex;
using CHAIR_Entities.Models;
using CHAIR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIR_Entities.Complex
{
    /// <summary>
    /// Class used to display each user in the search page. The friendsSince can be null, meaning that the user searching friends
    /// and the User user aren't friends. If it isn't null, it means they are friends and have been since that date
    /// </summary>
    public class UserSearch : VMBase
    {
        private bool _relationshipExists { get; set; }

        public User user { get; set; }
        public DateTime? friendsSince { get; set; }
        public bool relationshipExists
        {
            get
            {
                return _relationshipExists;
            }

            set
            {
                _relationshipExists = value;
                NotifyPropertyChanged("relationshipExists");
            }
        }

        public UserSearch(User user, DateTime? friendsSince, bool relationshipExists)
        {
            this.user = user;
            this.friendsSince = friendsSince;
            this._relationshipExists = relationshipExists;
        }

        public UserSearch()
        {
            this.user = new User();
            this.friendsSince = null;
            this._relationshipExists = false;
        }

    }
}
