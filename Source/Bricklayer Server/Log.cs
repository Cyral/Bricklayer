using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.Server
{
    /// <summary>
    /// Logs messages to the console with many options, such as colors, prefixes, and in-built string.Format arguments
    /// </summary>
    public static class Log
    {
        #region Console Helpers
        /// <summary>
        /// Writes text to the console on a new line
        /// </summary>
        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        /// Writes text to the console on a new line, with string.Format args
        /// </summary>
        public static void WriteLine(string text, params object[] objs)
        {
            WriteLine(string.Format(text, objs));
        }

        /// <summary>
        /// Writes text to the console on a new line, with a specified color
        /// </summary>
        public static void WriteLine(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Writes text to the console on a new line, with a specified color, and string.Format args
        /// </summary>
        public static void WriteLine(ConsoleColor color, string text, params object[] objs)
        {
            WriteLine(color, string.Format(text, objs));
        }

        /// <summary>
        /// Writes text to the console on a new line, using the specified log prefix
        /// </summary>
        public static void WriteLine(LogType type, string text)
        {
            type.WriteText(text);
        }


        /// <summary>
        /// Writes text to the console on a new line, using the specified log prefix, and a color for the rest of the text
        /// </summary>
        public static void WriteLine(LogType type, ConsoleColor color, string text)
        {
            type.WriteText(text, color);
        }

        /// <summary>
        /// Writes text to the console on a new line, using the specified log prefix, and string.Format args
        /// </summary>
        public static void WriteLine(LogType type, string text, params object[] objs)
        {
            WriteLine(type, string.Format(text, objs));
        }

        /// <summary>
        /// Writes text to the console on a new line, using the specified log prefix, and a color for the rest of the text
        /// </summary>
        public static void WriteLine(LogType type, ConsoleColor color, string text, params object[] objs)
        {
            WriteLine(type, color, string.Format(text, objs));
        }

        /// <summary>
        /// Writes text to the console
        /// </summary>
        public static void Write(string text)
        {
            Console.Write(text);
        }

        /// <summary>
        /// Writes text to the console with specified color
        /// </summary>
        public static void Write(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Writes text to the console
        /// </summary>
        public static void Write(string text, params string[] objs)
        {
            Write(string.Format(text, objs));
        }

        /// <summary>
        /// Writes text to the console
        /// </summary>
        public static void Write(ConsoleColor color, string text, params string[] objs)
        {
            Write(color, string.Format(text, objs));
        }

        /// <summary>
        /// Writes a line break/new line to the console
        /// </summary>
        public static void WriteBreak()
        {
            Console.Write(Environment.NewLine);
        }
        #endregion
    }

    /// <summary>
    /// Messages shown in the console with a specified color and prefix
    /// </summary>
    public class LogType
    {
        public static LogType Standard, Error, Chat, Server, Room, Status;
        private static StringBuilder sb;
        private const string spacer = ": ";

        static LogType()
        {
            sb = new StringBuilder();

            //Types
            Standard = new LogType("", ConsoleColor.White);
            Error = new LogType("Error", ConsoleColor.Red);
            Chat = new LogType("Chat", ConsoleColor.Yellow);
            Server = new LogType("Server", ConsoleColor.Green);
            Room = new LogType("Room", ConsoleColor.Magenta);
            Status = new LogType("Status", ConsoleColor.Cyan);
        }

        #region Instance
        public string Prefix { get; private set; }
        public ConsoleColor Color { get; private set; }

        public LogType (string prefix, ConsoleColor color)
        {
            Prefix = prefix;
            Color = color;
        }

        /// <summary>
        /// Write text to the console using the prefix
        /// </summary>
        public void WriteText(string text)
        {
            Console.ForegroundColor = Color;
            Console.Write(sb.Clear().Append(Prefix).Append(spacer).ToString());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }

        /// <summary>
        /// Write text to the console using the prefix and a color for the rest of the text
        /// </summary>
        public void WriteText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = Color;
            Console.Write(sb.Clear().Append(Prefix).Append(spacer).ToString());
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        #endregion
    }
}
