using System;
using System.Collections.Generic;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private readonly static Dictionary<int, string[]> _dic_variants = new();

        private readonly static Dictionary<int, string> _dic_answerPerson = new();

        private readonly static Random _rand = new();

        public static bool AnswersEmpty => _dic_answerPerson.Count == 0;

        public static bool StartTest(string key)
        {
            var _variants = MyDapper.GetQuestions(MyDapper.GetTestKey(key).IdTest);
            List<string> strings = new();
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
                return true;
            }
            return false;
        }

        public static int GetIndexCorrectOption(int r)
        {
            if (r == -1)
            {
                return r;
            }

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

        public static int GetRandom() => _dic_variants.Count == 0 ? -1 : _rand.Next(0, _dic_variants.Count - 1);

        public static void SetResponse(int IDResponse)
        {
            // если вариант ответа есть и он правильный
            if (GetIndexCorrectOption(IDResponse) != -1)
            {
                // удаляем вопрос из списка вопросов (чтобы больше не показывался)
                _dic_variants.Remove(IDResponse);
            }
        }

        public static void SaveTest(List<Models.Questions> questions, string testName, short TimeToTake, long TG_ID) => MyDapper.SaveTest(questions, testName, TimeToTake, TG_ID);

        public static Dictionary<int, string> ImportResults(long TG_ID) => MyDapper.GetCompletedTests(TG_ID);
    }
}