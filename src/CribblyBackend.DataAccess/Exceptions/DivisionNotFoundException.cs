using System;

namespace CribblyBackend.DataAccess.Exceptions
{
    public class DivisionNotFoundException : Exception
    {
        public DivisionNotFoundException(string name)
            : base($"Division '{name}' not found")
        {
            
        }
        public DivisionNotFoundException(int id)
            : base($"Division {id} not found")
        {
            
        }
    }
}