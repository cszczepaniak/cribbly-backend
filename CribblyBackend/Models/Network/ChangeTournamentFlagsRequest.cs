using System;

namespace CribblyBackend.Models.Network
{
    public class ChangeTournamentFlagsRequest
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOpenForRegistration { get; set; }
    }
}