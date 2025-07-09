using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ASTRedux.Utils.Logging
{
    internal static class Logger
    {
        public static LogDetail VerbosityLevel { get; set; }

        /// <summary>
        /// Performance analysis for Logger
        /// </summary>
        public static Stopwatch SW = new();

        private static TimeSpan TotalTime = new();

        private static string FormatMessage = string.Empty;

        public static StringBuilder LogOut { get; private set; } = new();

        /// <summary>
        /// Sets colour based on the type of log
        /// </summary>
        /// <param name="type">A LogType enum describing the severity of the message</param>
        private static void SetColour(LogType type)
        {
            switch (type)
            {
                case LogType.INFO:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }

        /// <summary>
        /// Logs a message in accordance with specified verbosity and logging level. Should not be used with LogType.ERROR, which will redirect to CriticalMessage().
        /// </summary>
        /// <param name="message">A string description of an event to log</param>
        /// <param name="type">A LogType enum describing the nature of the message</param>
        /// <param name="memberName">A string representing the method in which the Message occurred.</param>
        /// <param name="srcPath">A string representing the source file in which the Message occurred.</param>
        /// <param name="srcLine">A string representing the line of code in which the Message occurred.</param>
        public static void Message(string message, 
            LogType type = LogType.INFO,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            // if recieved log is an error, redirect to critical message.
            // you shouldn't do this, but should rather directly call critical message.
            if (type == LogType.ERROR)
            {
                CriticalMessage(message, memberName, srcPath, srcLine);
                return;
            }

            if (VerbosityLevel == LogDetail.NONE)
                return;

            switch (VerbosityLevel)
            {
                case LogDetail.NONE: 
                    break;
                case LogDetail.LOW:
                    FormatMessage = $"[{type}]: {message}";
                    break;
                case LogDetail.MEDIUM:
                    FormatMessage = $"[{type}] @ Line {srcLine} in {memberName}: {message}";
                    break;
                case LogDetail.HIGH:
                    FormatMessage = $"[{type}] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}(): {message}";
                    break;
                case LogDetail.EXTREME:
                    FormatMessage = $"\n{SW.Elapsed.TotalMilliseconds}ms elapsed/{TotalTime.TotalMilliseconds}ms total at {DateTime.Now:hh:mm:sstt}\n[{type}] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}(): {message}";
                    TotalTime += SW.Elapsed;
                    break;
            }

            SetColour(type);

            LogOut.Append(FormatMessage);
            Console.WriteLine(FormatMessage);

            if(VerbosityLevel == LogDetail.EXTREME)
                SW.Restart();

            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Logs an error irregardless of logging level.
        /// </summary>
        /// <param name="message">A string description of an event to log</param>
        /// <param name="type">A LogType enum describing the nature of the message</param>
        /// <param name="memberName">A string representing the method in which the CriticalMessage occurred.</param>
        /// <param name="srcPath">A string representing the source file in which the CriticalMessage occurred.</param>
        /// <param name="srcLine">A string representing the line of code in which the CriticalMessage occurred.</param>
        public static void CriticalMessage(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            string formatMessage = $"\n[ERROR] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}(): {message}";

            SetColour(LogType.ERROR);

            LogOut.Append(formatMessage);
            Console.WriteLine(formatMessage);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
