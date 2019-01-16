using System;
using System.Runtime.CompilerServices;

namespace ImageLoader.Common
{
    public class LoggerService : ILoggerService
    {
        public LogLevel CurrentLogLevel { get; set; }

        public void LogError(Exception ex, [CallerMemberName] string sender = null)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        public void LogError(string message, [CallerMemberName] string sender = null)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void LogWarning(string message, [CallerMemberName] string sender = null)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Trace(string message, [CallerMemberName] string sender = null)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}