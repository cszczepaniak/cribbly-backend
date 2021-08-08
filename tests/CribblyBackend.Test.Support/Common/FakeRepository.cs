using System.Threading;

namespace CribblyBackend.Test.Support.Common
{
    public class FakeRepository
    {
        protected int nextId = 0;
        protected int IncrementId()
        {
            return Interlocked.Increment(ref nextId);
        }
    }
}