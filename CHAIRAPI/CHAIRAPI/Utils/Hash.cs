using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CHAIRAPI.Utils
{
    public class Hash
    {
        /// <summary>
        /// Method used to create a random salt to add to the password
        /// </summary>
        /// <returns>string salt</returns>
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        /// <summary>
        /// Method used to create the hash of a password 
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt used to hash</param>
        /// <returns>string hash</returns>
        public static string Create(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: password,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        /// <summary>
        /// Method used to validate if a password and salt are valid against the given hash
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <param name="salt">The salt used to validate</param>
        /// <param name="hash">The hashed password</param>
        /// <returns>string hash</returns>
        public static bool Validate(string password, string salt, string hash)
            => Create(password, salt) == hash;
    }
}
