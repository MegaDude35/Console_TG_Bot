using System;

namespace Console_TelegramBot
{
    internal class Program
    {
 
        private static void Main()
        {
            //new TelegramBotAPI();
            var console = AikenParser.ParseQuestions(".\\lol.txt");
            foreach (var item in console)
            {
                Console.WriteLine(item.Question_Ball + " " + item.Question_Group + " " + item.Question_Text);
            }
            Console.ReadKey();
        }
    }
}