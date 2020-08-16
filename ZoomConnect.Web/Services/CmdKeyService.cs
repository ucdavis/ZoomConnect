using System;
using System.Linq;
using System.Security.Cryptography;

namespace ZoomConnect.Web.Services
{
    /// <summary>
    /// Generates keys for use in API calls.
    /// </summary>
    /// <remarks>
    /// https://www.devtrends.co.uk/blog/hashing-encryption-and-random-in-asp.net-core
    /// </remarks>
    public class CmdKeyService
    {
        private const string AllowableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public string GenerateKey()
        {
            var bytes = new byte[50];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
        }
    }
}
