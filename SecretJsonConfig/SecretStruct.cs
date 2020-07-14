using System;

namespace SecretJsonConfig
{
    public struct SecretStruct
    {
        public SecretStruct(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public static implicit operator string(SecretStruct s) => s.Value;
        public static explicit operator SecretStruct(string s) => new SecretStruct(s);
    }
}
