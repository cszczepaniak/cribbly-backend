using System;

namespace CribblyBackend.Common
{
    public class CribblyConfig
    {
        internal static string FirebaseAudience => Environment.GetEnvironmentVariable("FIREBASE_PROJ_ID");
    }
}