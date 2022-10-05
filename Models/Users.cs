namespace Console_TelegramBot.Models
{
    public struct Users
    {
        public int Id { set; get; }
        public string Fisrtname { set; get; }
        public string Lastname { get; set; }
        public long TG_Id { set; get; }
        public bool Author { set; get; }
    }
}