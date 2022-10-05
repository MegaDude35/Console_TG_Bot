namespace Console_TelegramBot.Models
{
    public struct Questions
    {
        public int Id { set; get; }
        public int Test_Id { set; get; }
        public int Question_Group { set; get; }
        public int Question_Type { set; get; }
        public string Question_Text { set; get; }
        public int Question_Ball { set; get; }
    }
}