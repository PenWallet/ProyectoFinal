using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entities.Persistent
{
    public class User
    {
        public string nickname { get; set; }
        public string password { get; set; }
        public string profileDescription { get; set; }
        public string profileLocation { get; set; }
        public DateTime birthDate { get; set; }
        public bool privateProfile { get; set; }
        public DateTime accountCreationDate { get; set; }
        public bool online { get; set; }
        public bool admin { get; set; }
        public string lastIP { get; set; }
        public DateTime? bannedUntil { get; set; }
        public string banReason { get; set; }

        public User()
        {
        }

        public User(string nickname, string password, string profileDescription, string profileLocation, DateTime birthDate, bool privateProfile, DateTime accountCreationDate, bool online, bool admin, string lastIP, DateTime bannedUntil, string banReason)
        {
            this.nickname = nickname;
            this.password = password;
            this.profileDescription = profileDescription;
            this.profileLocation = profileLocation;
            this.birthDate = birthDate;
            this.privateProfile = privateProfile;
            this.accountCreationDate = accountCreationDate;
            this.online = online;
            this.admin = admin;
            this.lastIP = lastIP;
            this.bannedUntil = bannedUntil;
            this.banReason = banReason;
        }

    }
}
