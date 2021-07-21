using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager.SDK.Facades
{
    public static class Log
    {
        public static void Trace(string message, string logPrefix = null)
        {
            InternalLog(message, LogMessageType.Trace, logPrefix);
        }

        public static void Debug(string message, string logPrefix = null)
        {
            InternalLog(message, LogMessageType.Debug, logPrefix);
        }

        public static void Info(string message, string logPrefix = null)
        {
            InternalLog(message, LogMessageType.Info, logPrefix);
        }

        public static void Warning(string message, string logPrefix = null)
        {
            InternalLog(message, LogMessageType.Warning, logPrefix);
        }

        public static void Error(string message, string logPrefix = null)
        {
            InternalLog(message, LogMessageType.Error, logPrefix);
        }

        private static void InternalLog(string message, LogMessageType logMessageType, string logPrefix = null)
        {
            string outputMessage = ParseMessage(message, logMessageType, logPrefix);

            Console.WriteLine(outputMessage);
        }

        private static string ParseMessage(string message, LogMessageType logMessageType, string logPrefix = null)
        {
            string returnMessage = $"[{logMessageType.ToString().ToUpper()}] <{DateTime.UtcNow:dd.MM.yyyy HH:mm:ss}> ";

            if (!string.IsNullOrWhiteSpace(logPrefix))
                returnMessage += $"{logPrefix} ";

            returnMessage += message;

            return returnMessage;
        }
    }

    public enum LogMessageType{
        Trace,
        Debug,
        Info,
        Warning,
        Error
    }
}
