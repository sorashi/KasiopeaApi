using System;

namespace KasiopeaApi.Tests
{
    internal static class Credentials
    {
        public static readonly string Email = Environment.GetEnvironmentVariable("kasiopea_email");
        public static readonly string Password = Environment.GetEnvironmentVariable("kasiopea_password");
    }
}