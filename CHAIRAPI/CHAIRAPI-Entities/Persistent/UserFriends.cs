using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entities.Persistent
{
    public class UserFriends
    {
        public string user1 { get; set; }
        public string user2 { get; set; }
        public DateTime? acceptedRequestDate { get; set; }

        public UserFriends(string user1, string user2, DateTime? acceptedRequestDate)
        {
            this.user1 = user1;
            this.user2 = user2;
            this.acceptedRequestDate = acceptedRequestDate;
        }

        public UserFriends()
        {
        }
    }
}
