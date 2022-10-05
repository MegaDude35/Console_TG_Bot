using System;

namespace Console_TelegramBot.Entities
{
    struct Inspection
    {
        private string Id { set; get; }
        private string IdPerson { set; get; }
        private string IdQuestion { set; get; }
        private string Result { set; get; }
        private DateTime Date { set; get; }
    }
}