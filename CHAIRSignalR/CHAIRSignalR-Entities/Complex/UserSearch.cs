using CHAIRSignalR_Entities.Complex;
using CHAIRSignalR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entities.Complex
{
    /// <summary>
    /// Class used to display each user in the search page. The friendsSince can be null, meaning that the user searching friends
    /// and the User user aren't friends. If it isn't null, it means they are friends and have been since that date
    /// </summary>
    public class UserSearch
    {
        public User user { get; set; }
        public DateTime? friendsSince { get; set; }
        public bool relationshipExists { get; set; }

        public UserSearch(User user, DateTime? friendsSince, bool relationshipExists)
        {
            this.user = user;
            this.friendsSince = friendsSince;
            this.relationshipExists = relationshipExists;
        }

        public UserSearch()
        {
            this.user = new User();
            this.friendsSince = null;
            this.relationshipExists = false;
        }

    }
}
