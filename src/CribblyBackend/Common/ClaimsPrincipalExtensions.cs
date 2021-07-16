using System.Security.Claims;

namespace CribblyBackend.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Email);
        public static string GetAuthProviderId(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}