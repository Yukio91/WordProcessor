using System;
using System.Collections.Generic;

namespace WordProcessor.Utils.Console
{
    static class ConsoleHelpers
    {
        public static string ReadLineByOneChar()
        {

            string line = "";

            do
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Enter)
                {
                    if (key.Key != ConsoleKey.Backspace || String.IsNullOrEmpty(line))
                    {
                        if (key.Key != ConsoleKey.Enter)
                            continue;

                        break;
                    }

                    line = line.Substring(0, line.Length - 1);
                    System.Console.Write("\b \b");
                }else if (key.Key == ConsoleKey.Escape)
                {
                    return " ";
                }
                else
                {
                    line += key.KeyChar;
                    System.Console.Write(key.KeyChar);
                }

            } while (true);

            System.Console.WriteLine();

            return line;
        }

        public static void WriteList(IEnumerable<string> list)
        {
            foreach (var line in list)
                System.Console.WriteLine($"- {line}");
        }
    }
}