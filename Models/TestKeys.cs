using System;

namespace Console_TelegramBot.Models
{
    public struct TestKeys
    {
        public int Id { set; get; }
        public string TestKey { set; get; }
        public int IdTest { get; set; }
        public int IdUser { set; get; }
        public DateTime DateEnableKey { set; get; }
        public DateTime DateDisableKey { set; get; }
        public byte Tryes { set; get; }
        public bool Enabled { set; get; }
    }
}