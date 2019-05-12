using CHAIRSignalR_Entities.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIRSignalR_Entities.Complex
{
    public class UserWithToken : User
    {
        public string token { get; set; }

        public UserWithToken()
        {

        }

        public UserWithToken(User user, string token)
        {
            this.nickname = user.nickname;
            this.password = user.password;
            this.profileDescription = user.profileDescription;
            this.profileLocation = user.profileLocation;
            this.birthDate = user.birthDate;
            this.privateProfile = user.privateProfile;
            this.accountCreationDate = user.accountCreationDate;
            this.online = user.online;
            this.admin = user.admin;
            this.lastIP = user.lastIP;
            this.bannedUntil = user.bannedUntil;
            this.banReason = user.banReason;
            this.token = token;
        }

    }
}
