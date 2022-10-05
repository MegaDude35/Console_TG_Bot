using System.Collections.Concurrent;

namespace Console_TelegramBot
{
    public class Personal
    {
        public readonly static ConcurrentDictionary<long, string> lastCommand = new();

        public static void SetCommand(long id_personal, string textCommand)
        {
            lastCommand.TryAdd(id_personal, textCommand);
        }
    }
}