using System;

namespace Console_TelegramBot
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "Console TG Bot";
            new TelegramBotAPI();
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                MyRepository.SetAuthor(Convert.ToInt64(Console.ReadLine()));
            }
        }
    }
}