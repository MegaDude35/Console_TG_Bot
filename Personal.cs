using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_TelegramBot
{
    public class Personal
    {
        public static ConcurrentDictionary<long, string> lastCommand = new ConcurrentDictionary<long, string>();

        public static void setCommand(long id_personal, string textCommand)
        {
            lastCommand.TryAdd(id_personal, textCommand);

        }

    }
}
