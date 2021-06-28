using System;

namespace CribblyBackend.DataAccess
{
    public class DivisionNotFoundException : Exception
    {
        public DivisionNotFoundException()
        {
            
        }

        public DivisionNotFoundException(string message)
            : base("Division not found")
        {
            
        }

        public DivisionNotFoundException(string message, Exception inner)
            : base("Division not found", inner)
        {
            
        }
    }
}