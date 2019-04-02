using System;

namespace WordProcessor.Utils.Logger
{
    public interface ILogger
    {
        void Write(string message);
        void Error(string message, Exception exception);
    }

    public class ConsoleLogger : ILogger
    {
        public void Error(string message, Exception exception) => Write($"{message}. Error: {exception.Message} - {exception.StackTrace}");

        public void Write(string message) => System.Console.WriteLine(message);
    }
}
