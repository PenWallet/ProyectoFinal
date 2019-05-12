using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRSignalR_Entities.Persistent
{
    /// <summary>
    /// This class inherits from User because it's the one used to access the database when registering
    /// and logging in since we need to retrieve the salt from it
    /// </summary>
    public class UserWithSalt : User
    {
        public string salt { get; set; }
        
        public UserWithSalt()
        {
        }

        public UserWithSalt(string salt, string nickname, string password, string profileDescription, string profileLocation, DateTime birthDate, bool privateProfile, DateTime accountCreationDate, bool online, bool admin, string lastIP, DateTime bannedUntil, string banReason) : base(nickname, password, profileDescription, profileLocation, birthDate, privateProfile, accountCreationDate, online, admin, lastIP, bannedUntil, banReason)
        {
            this.salt = salt;
        }

        public UserWithSalt(User user, string salt)
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
            this.salt = salt;
        }
    }
}
