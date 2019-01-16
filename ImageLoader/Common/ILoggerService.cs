using System;
using System.Runtime.CompilerServices;

namespace ImageLoader.Common
{
    public interface ILoggerService
    {
        LogLevel CurrentLogLevel { get; set; }

        void LogError(Exception ex, [CallerMemberName] string sender = null);
        void LogError(string message, [CallerMemberName] string sender = null);
        void LogWarning(string message, [CallerMemberName] string sender = null);
        void Trace(string message, [CallerMemberName] string sender = null);
    }

    public enum LogLevel
    {
        Information,
        Warning,
        Error
    }
}