using System;

namespace Console_TelegramBot
{
    internal class Program
    {
        private static void Main()
        {
            new TelegramBotAPI();
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                if (long.TryParse(Console.ReadLine(), out long TG_ID))
                {
                    MyRepository.SetAuthor(TG_ID);
                }
            }
        }
    }
}