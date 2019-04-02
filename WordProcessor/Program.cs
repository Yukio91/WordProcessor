using System;
using WordProcessor.Utils.Console;
using WordProcessor.Utils.Logger;

namespace WordProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var dictionary = new WordsDictionary(logger);

            try
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "create":
                            dictionary.Create(args[1]);
                            break;
                        case "update":
                            dictionary.Update(args[1]);
                            break;
                        case "clear":
                            dictionary.Clear();
                            break;
                    }

                    Console.WriteLine("Введите слово или его часть:");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Обработка параметров запуска", ex);
            }

            while (true)
            {
                try
                {
                    Console.Write("> ");
                    var input = ConsoleHelpers.ReadLineByOneChar();//Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(input))
                        break;

                    var result = dictionary.Find(input);
                    ConsoleHelpers.WriteList(result);
                }
                catch (Exception ex)
                {
                    logger.Error(" ", ex);
                }

            }
        }
    }
}
