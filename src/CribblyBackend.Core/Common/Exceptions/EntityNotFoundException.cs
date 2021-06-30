using System;

namespace CribblyBackend.Core.Common.Exceptions
{
    public class EntityNotFoundException<T> : Exception
    {
        public EntityNotFoundException(int id)
            : base($"Could not find [{typeof(T)}] with id [{id}]")
        {
        }

    }
}