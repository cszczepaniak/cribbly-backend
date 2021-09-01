using System.Text.Json.Serialization;
using CribblyBackend.Core.Players.Models;

namespace CribblyBackend.DataAccess.Players.Models
{
    public class S3Player
    {
        public string AuthProviderId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Role { get; set; }
        public bool IsReturning { get; set; }

        public Player ToPlayer()
        {
            return new Player
            {
                AuthProviderId = AuthProviderId,
                Email = Email,
                Name = Name,
                Team = TeamId == default ? null : new() { Id = TeamId },
                Role = Role,
                IsReturning = IsReturning,
            };
        }
    }
}