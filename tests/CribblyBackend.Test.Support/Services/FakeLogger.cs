using Serilog;
using Serilog.Events;

namespace CribblyBackend.Test.Support.Services
{
    public class FakeLogger : ILogger
    {
        public void Write(LogEvent logEvent)
        {
        }
    }
}