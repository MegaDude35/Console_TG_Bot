using System;

namespace Console_TelegramBot.Models
{
    public struct TestKeys
    {
        public int ID { set; get; }
        public string Test_Key { set; get; }
        public int Test_ID { get; set; }
        public long User_ID { set; get; }
        public DateTime Date_Enable_Key { set; get; }
        public DateTime Date_Disable_Key { set; get; }
        public byte Tryes { set; get; }
        public bool Enabled { set; get; }
    }
}