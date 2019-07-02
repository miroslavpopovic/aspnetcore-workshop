using System;
using Microsoft.Extensions.Logging;

namespace TimeTracker.Tests.UnitTests
{
    public class FakeLogger<T> : ILogger<T>, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this; // This is why FakeLogger implements IDisposable
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            // Do nothing
        }

        public void Dispose()
        {
            // Do nothing
        }
    }
}
