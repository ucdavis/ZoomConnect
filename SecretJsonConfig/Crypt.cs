using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace SecretJsonConfig
{
    public class Crypt
    {
        private readonly IDataProtector _protector;

        public Crypt(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public string Encrypt(string plaintext)
        {
            return _protector.Protect(plaintext);
        }

        public bool TryDecrypt(string encryptedText, out string decryptedText)
        {
            try
            {
                decryptedText = _protector.Unprotect(encryptedText);

                return true;
            }
            catch (CryptographicException)
            {
                decryptedText = null;

                return false;
            }
        }
    }
}
