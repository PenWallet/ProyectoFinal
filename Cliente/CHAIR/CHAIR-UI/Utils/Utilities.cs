﻿using CHAIR_Entities.Persistent;
using CHAIRSignalR_Entities.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Utils
{
    public static class Utilities
    {
        public static bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsSymbol(char c)
        {
            return c > 32 && c < 127 && !IsDigit(c) && !IsLetter(c);
        }

        public static User userWithTokenToUser(UserWithToken user)
        {
            User newUser = new User();

            newUser.nickname = user.nickname;
            newUser.password = user.password;
            newUser.profileDescription = user.profileDescription;
            newUser.profileLocation = user.profileLocation;
            newUser.birthDate = user.birthDate;
            newUser.privateProfile = user.privateProfile;
            newUser.accountCreationDate = user.accountCreationDate;
            newUser.online = user.online;
            newUser.admin = user.admin;
            newUser.lastIP = user.lastIP;
            newUser.bannedUntil = user.bannedUntil;
            newUser.banReason = user.banReason;

            return newUser;
        }
    }
}
