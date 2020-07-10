using System;

namespace SecretJsonConfig
{
    public struct SecretString
    {
        private readonly string _secretString;

        public SecretString(string secretString)
        {
            this._secretString = secretString;
        }

        public static implicit operator string(SecretString s) => s._secretString;
        public static explicit operator SecretString(string s) => new SecretString(s);

        public override string ToString() => $"{_secretString}";
    }
}
