using System.Collections.Generic;

namespace CribblyBackend.Network
{
    public class CreateDivisionRequest
    {
        public string Name { get; set; }
        public IEnumerable<int> TeamIds { get; set; }
    }
}