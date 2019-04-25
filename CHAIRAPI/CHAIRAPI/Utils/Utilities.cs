using CHAIRAPI.Responses;
using CHAIRAPI_DAL.Handlers;
using CHAIRAPI_Entidades.Persistent;
using IniParser;
using IniParser.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CHAIRAPI.Utils
{
    public static class Utilities
    {
        public static bool checkUserFriendsHasRequiredFieldsToPostOrPut(UserFriends rel)
        {
            if (isNullOrEmpty(rel.user1, rel.user2))
                return false;

            return true;
        }

        public static bool checkUserGamesHasRequiredFieldsToPostOrPut(UserGames rel)
        {
            if (isNullOrEmpty(rel.user, rel.game))
                return false;

            return true;
        }

        public static bool checkUserHasRequiredFieldsToBan(User user)
        {
            if (isNullOrEmpty(user.nickname, user.banReason) || user.bannedUntil == null || user.bannedUntil == new DateTime())
                return false;

            return true;
        }

        public static bool checkUserHasRequiredFieldsToRegister(User user)
        {
            if (isNullOrEmpty(user.nickname, user.password, user.lastIP) || user.birthDate == new DateTime())
                return false;

            return true;
        }

        public static bool checkUserHasRequiredFieldsToLoginOrUpdate(User user)
        {
            if (isNullOrEmpty(user.nickname, user.password, user.lastIP))
                return false;

            return true;
        }

        /// <summary>
        /// Small method that returns if a string or a set of strings are null or empty
        /// </summary>
        /// <param name="strings"></param>
        /// <returns>If one of the strings is null or is empty, it returns true. If none of them are null or empty, it returns false</returns>
        public static bool isNullOrEmpty(string algo, params string[] strings)
        {
            foreach(string str in strings)
            {
                if (str == null || str == string.Empty)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Method to check whether a user is banned or not
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <returns>Null if the user isn't banned, or a BanResponse object if he is</returns>
        public static BanResponse isUserBanned(User user)
        {
            if (user.bannedUntil != null && user.bannedUntil > DateTime.Now)
                return new BanResponse(BannedByEnum.USER, user.banReason, (DateTime)user.bannedUntil);
            else
            {
                IPBan ipBan = IPBansHandler.searchIPBanByIP(user.lastIP);

                if (ipBan != null && ipBan.untilDate > DateTime.Now)
                    return new BanResponse(BannedByEnum.IP, ipBan.banReason, ipBan.untilDate);
                else
                    return null;
            }
        }

        /// <summary>
        /// Method to generate a token based on an user
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static string generateToken(User user)
        {
            //Added a Nuget Package to be able to read config files. I stored there the key
            var parser = new FileIniDataParser();
            IniData config = parser.ReadFile("Config/config.ini");
            string signingKeyString = config["JWTSettings"]["JWTSigningKey"];
            string issuer = config["JWTSettings"]["Issuer"];

            //Symmetric Security Key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));

            //Add claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, user.admin ? "Administrator" : "User"));
            claims.Add(new Claim("usr", user.nickname));

            //Sign Credentials
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //Create the actual token
            var token = new JwtSecurityToken(
                    issuer: issuer,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials,
                    claims: claims
                );

            //Return Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Method to generate a token based on an user
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public static string generateToken(UserWithSalt user)
        {
            //Added a Nuget Package to be able to read config files. I stored there the key
            var parser = new FileIniDataParser();
            IniData config = parser.ReadFile("Config/config.ini");
            string signingKeyString = config["JWTSettings"]["JWTSigningKey"];
            string issuer = config["JWTSettings"]["Issuer"];

            //Symmetric Security Key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyString));

            //Add claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, user.admin ? "Administrator" : "User"));
            claims.Add(new Claim("usr", user.nickname));

            //Sign Credentials
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //Create the actual token
            var token = new JwtSecurityToken(
                    issuer: issuer,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials,
                    claims: claims
                );

            //Return Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static User convertUserWithSaltToUser(UserWithSalt user)
        {
            User userToSend = new User();
            userToSend.nickname = user.nickname;
            userToSend.password = "";
            userToSend.profileDescription = user.profileDescription;
            userToSend.profileLocation = user.profileLocation;
            userToSend.birthDate = user.birthDate;
            userToSend.privateProfile = user.privateProfile;
            userToSend.accountCreationDate = user.accountCreationDate;
            userToSend.online = user.online;
            userToSend.admin = user.admin;
            userToSend.lastIP = user.lastIP;
            userToSend.bannedUntil = user.bannedUntil;
            userToSend.banReason = user.banReason;

            return userToSend;
        }

        public static UserWithSalt convertUserToUserWithSalt(User user)
        {
            UserWithSalt userWithSalt = new UserWithSalt();
            userWithSalt.nickname = user.nickname;
            userWithSalt.password = user.password;
            userWithSalt.profileDescription = user.profileDescription;
            userWithSalt.profileLocation = user.profileLocation;
            userWithSalt.birthDate = user.birthDate;
            userWithSalt.privateProfile = user.privateProfile;
            userWithSalt.accountCreationDate = user.accountCreationDate;
            userWithSalt.online = user.online;
            userWithSalt.admin = user.admin;
            userWithSalt.lastIP = user.lastIP;
            userWithSalt.bannedUntil = user.bannedUntil;
            userWithSalt.banReason = user.banReason;

            return userWithSalt;
        }

        /// <summary>
        /// Small private method to check if the nickname in the URL is the same as the nickname
        /// stored in the "usr" claim of the token (basically, check for validity)
        /// </summary>
        /// <param name="nickname">Send as many nicknames as you want (useful for double relationships like UserFriends)</param>
        /// <returns></returns>
        public static bool checkUsrClaimValidity(ClaimsPrincipal user, params string[] nickname)
        {
            //First check, just in case (it can blow up)
            if (user != null && user.Claims != null && user.Claims.Count() != 0)
            {
                //Get the user to whom the token was provided
                Claim usrClaim = user.Claims.ToList().Single(x => x.Type == "usr");
                if (usrClaim != null)
                {
                    foreach (string nick in nickname)
                    {
                        if (nick == usrClaim.Value)
                            return true;
                    }
                }
                    
            }

            return false;
        }

    }
}
