
using System;

namespace UTI
{
    public class Log
    {
        internal static void Error(string v)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(v);
            Console.ForegroundColor = ConsoleColor.White;
        }
        internal static void Error(string v, Exception ex)
        {
            Error(v + $"\n{ex.Message}\n{ex.StackTrace}");
        }
        internal static void Loading(string v)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(v);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Warning(string v)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(v);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Server(string v)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(v);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}


