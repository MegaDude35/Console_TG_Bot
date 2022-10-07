using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private readonly Dictionary<int, string[]> _dic_variants = new();

        private readonly Dictionary<int, string> _dic_answerPerson = new();

        private readonly static Random _rand = new();

        public bool StartTest(string key)
        {
            var _variants = MyDapper.GetQuestions(MyDapper.GetTestKey(key).Test_ID);
            List<string> strings = new(_variants.Count);
            string rightAnswer = string.Empty;
            int count = 0;
            for (int i = 0; i < _variants.Count; i++)
            {
                if (_variants[i].Question_Group == count)
                {
                    strings.Add(_variants[i].Question_Text);
                    if (_variants[i].Question_Ball != 0)
                    {
                        rightAnswer = _variants[i].Question_Text;
                    }
                    continue;
                }
                else
                {
                    i--;
                }
                _dic_answerPerson.TryAdd(count, rightAnswer);
                _dic_variants.TryAdd(count++, strings.ToArray());
            }
            return count > 0;
        }

        public int GetIndexCorrectOption(int r)
        {

            if (_dic_variants.TryGetValue(r, out string[] value))
            {
                // теперь ищем индекс правильного ответа
                for (int i = 1; i < value.Length; i++)
                {
                    if (value[i] == _dic_answerPerson[r])
                    {
                        return i - 1;
                    }
                }
            }
            return -1;
        }

        public string GetQuestion(int i) => _dic_variants[i][0];

        public IEnumerable<string> GetVariant(int i) => _dic_variants[i][1..];

        public int GetRandom() => _dic_variants.Count == 0 ? -1 : _rand.Next(0, _dic_variants.Count);

        public static bool GetAuthor(long TG_ID) => MyDapper.GetUser(TG_ID).Author;

        public void SetResponse(int IDResponse)
        {
            // если вариант ответа есть и он правильный
            if (GetIndexCorrectOption(IDResponse) != -1)
            {
                // удаляем вопрос из списка вопросов (чтобы больше не показывался)
                _dic_variants.Remove(IDResponse);
            }
        }

        public static int SaveTest(List<Models.Questions> questions, string testName, short TimeToTake, long TG_ID) => MyDapper.SaveTest(questions, testName, TimeToTake, TG_ID);

        public static Dictionary<int, string> ImportResults(long TG_ID) => MyDapper.GetCompletedTests(TG_ID);

        public static bool CheckAddUser(long TG_ID) => MyDapper.GetUser(TG_ID).ID != 0;

        public static void AddUser(long TG_ID, string Firstname, string Lastname) => MyDapper.AddUser(TG_ID, Firstname, Lastname);

        public static string RandomString(int length = 64) => new(
            Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                      .Select(s => s[_rand.Next(s.Length)])
                      .ToArray());
    }
}