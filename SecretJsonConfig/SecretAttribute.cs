using System;

namespace SecretJsonConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SecretAttribute : Attribute
    {
    }
}
