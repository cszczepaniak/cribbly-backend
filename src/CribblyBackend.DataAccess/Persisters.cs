using System;

namespace CribblyBackend.DataAccess
{
    public static class Persisters
    {
        public const string S3 = "s3";
        public const string MySQL = "mysql";
        public const string Memory = "memory";
    }

    public static class Persister
    {
        public static string Get() => Environment.GetEnvironmentVariable("CRIBBLY_PERSISTER").ToLower();
        public static bool IsMySQL() => Get().ToLower() == Persisters.MySQL;
    }
}