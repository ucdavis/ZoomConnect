using System;

namespace SecretJsonConfig
{
    public struct SecureStruct
    {
        public SecureStruct(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public static implicit operator string(SecureStruct s) => s.Value;
        public static explicit operator SecureStruct(string s) => new SecureStruct(s);
    }
}
