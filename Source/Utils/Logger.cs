using System.Diagnostics;
using System.Runtime.CompilerServices;
using ASTRedux.Enums;

namespace ASTRedux.Utils
{
    internal static class Logger
    {
        public static bool IsVerbose { get; set; }

        public static LogLevel Level { get; set; }

        /// <summary>
        /// Performance analysis for Logger
        /// </summary>
        public static Stopwatch SW = new();

        private static TimeSpan TotalTime = new();

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
        /// <param name="type">A LogType enum describing the severity of the message</param>
        /// <param name="memberName">A string representing the method in which the Message occurred.</param>
        /// <param name="srcPath">A string representing the source file in which the Message occurred.</param>
        /// <param name="srcLine">A string representing the line of code in which the Message occurred.</param>
        public static void Message(string message, 
            LogType type, 
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

            switch(Level)
            {
                case LogLevel.NONE: 
                    break;
                case LogLevel.LOW:
                    SetColour(type);
                    Console.WriteLine($"[{type}]: {message}");
                    Console.ForegroundColor = ConsoleColor.White;

                    break;
                case LogLevel.MEDIUM:
                    SetColour(type);
                    Console.WriteLine($"[{type}] @ Line {srcLine} in {memberName}: {message}");
                    Console.ForegroundColor = ConsoleColor.White;

                    break;
                case LogLevel.HIGH:
                    SetColour(type);
                    Console.WriteLine($"[{type}] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}(): {message}");
                    Console.ForegroundColor = ConsoleColor.White;

                    break;
                case LogLevel.EXTREME:
                    TotalTime += SW.Elapsed;

                    SetColour(type);
                    Console.WriteLine($"\n{SW.Elapsed.TotalMilliseconds}ms elapsed/{TotalTime.TotalMilliseconds}ms total\n[{type}] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}():\n{message}");
                    Console.ForegroundColor = ConsoleColor.White;

                    SW.Restart();

                    break;
            }
        }

        /// <summary>
        /// Logs an error irregardless of logging level.
        /// </summary>
        /// <param name="message">A string description of an event to log</param>
        /// <param name="type">A LogType enum describing the severity of the message</param>
        /// <param name="memberName">A string representing the method in which the CriticalMessage occurred.</param>
        /// <param name="srcPath">A string representing the source file in which the CriticalMessage occurred.</param>
        /// <param name="srcLine">A string representing the line of code in which the CriticalMessage occurred.</param>
        public static void CriticalMessage(string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            SetColour(LogType.ERROR);
            Console.WriteLine($"[ERROR] @ Line {srcLine} in file {Path.GetFileName(srcPath)} at method {memberName}(): {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
