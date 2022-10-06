using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private readonly static Dictionary<int, string[]> _dic_variants = new();

        private readonly static Dictionary<int, string> _dic_answerPerson = new();

        private readonly static Random _rand = new();

        public static bool StartTest(string key)
        {
            var _variants = MyDapper.GetQuestions(MyDapper.GetTestKey(key).IdTest);
            List<string> strings = new(_variants.Count);
            string rightAnswer = string.Empty;
            int count = 0;
            foreach (var variant in _variants)
            {
                if (variant.Question_Group == count)
                {
                    strings.Add(variant.Question_Text);
                    if (variant.Question_Ball != 0)
                    {
                        rightAnswer = variant.Question_Text;
                    }
                    continue;
                }
                _dic_answerPerson.TryAdd(count, rightAnswer);
                _dic_variants.TryAdd(count++, strings.ToArray());
            }
            return count > 0;
        }

        public static int GetIndexCorrectOption(int r)
        {

            if (_dic_variants.TryGetValue(r, out string[] value))
            {
                // теперь ищем индекс правильного ответа
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == _dic_answerPerson[r])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string GetQuestion(int i) => _dic_variants[i][0];

        public static IEnumerable<string> GetVariant(int i) => _dic_variants[i][1..];

        public static int GetRandom() => _rand.Next(0, _dic_variants.Count + 1);

        public static bool GetAuthor(long TG_ID) => MyDapper.GetUser(TG_ID).Author;

        public static void SetResponse(int IDResponse)
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

        public static bool CheckAddUser(long TG_ID) => MyDapper.GetUser(TG_ID).Id != 0;

        public static void AddUser(long TG_ID, string Firstname, string Lastname) => MyDapper.AddUser(TG_ID, Firstname, Lastname);

        public static string RandomString(int length = 64) => new(
            Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                      .Select(s => s[_rand.Next(s.Length)])
                      .ToArray());
    }
}