using System;

namespace CribblyBackend.Core.Common.Exceptions
{
    public class UnexpectedGameDataException : Exception
    {
        public UnexpectedGameDataException(int id, string msg)
            : base($"Unexpected data for game {id}: ${msg}")
        {

        }
    }
}