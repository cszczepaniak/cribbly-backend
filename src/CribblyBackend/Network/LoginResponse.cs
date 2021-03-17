
using CribblyBackend.DataAccess.Models;

namespace CribblyBackend.Network
{
    public class LoginResponse
    {
        public Player Player { get; set; }
        public bool IsReturning { get; set; }
    }
}