using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private string TestKey;

        //TODO попробовать заменить словарь обычными запросами в бд используя ключ и возвращать массив
        private readonly Dictionary<int, string[]> _dic_Variants = new();

        private readonly static Random _rand = new();

        public int Rand { get; private set; }

        public bool StartTest(string key)
        {
            TestKey = key;
            var _variants = MyDapper.GetQuestions(MyDapper.GetTestKey(key).Test_ID);
            List<string> strings = new(_variants.Count);
            int count = 0;
            for (int i = 0; i < _variants.Count; i++)
            {
                if (_variants[i].Question_Group == count)
                {
                    strings.Add(_variants[i].Question_Text);
                    continue;
                }
                i--;
                _dic_Variants.Add(count++, strings.ToArray());
            }
            return count > 0;
        }

        public string GetQuestion(int i) => _dic_Variants[i][0];

        public IEnumerable<string> GetVariant(int i) => _dic_Variants[i][1..];

        public int GetRandom() => Rand = _dic_Variants.Count == 0 ? -1 : _rand.Next(0, _dic_Variants.Count);

        public static bool GetAuthor(long TG_ID) => MyDapper.GetUser(TG_ID).Author;

        public void SetResponse(int IDResponse, long TG_ID, int variant)
        {
            MyDapper.SaveAnswer(TestKey, TG_ID, _dic_Variants[variant][++IDResponse]);
            // удаляем вопрос из списка вопросов (чтобы больше не показывался)
            _dic_Variants.Remove(variant);
        }

        public static int SaveTest(List<Models.Questions> questions, string testName, short TimeToTake, long TG_ID) => MyDapper.SaveTest(questions, testName, TimeToTake, TG_ID);

        public static Dictionary<int, string> ImportResults(long TG_ID) => MyDapper.GetCompletedTests(TG_ID);

        public static bool CheckAddUser(long TG_ID) => MyDapper.GetUser(TG_ID).TG_ID != 0;

        public static void AddUser(long TG_ID, string Firstname, string Lastname) => MyDapper.AddUser(TG_ID, Firstname, Lastname);

        public static string RandomString(int length = 64) => new(
            Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                      .Select(s => s[_rand.Next(s.Length)])
                      .ToArray());
        public static void SetAuthor(long TG_ID) => MyDapper.SetAuthor(TG_ID);
    }
}