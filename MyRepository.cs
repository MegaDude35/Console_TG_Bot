using Console_TelegramBot.Models;
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

        private static readonly Random _rand = new();

        public int Rand { get; private set; }

        /// <summary>
        /// Запускает тест
        /// </summary>
        /// <param name="key">Ключ для тестирования</param>
        /// <returns>Результат запуска (истина/ложь)</returns>
        public bool StartTest(string key)
        {
            TestKey = key;
            List<Questions> _variants = MyDapper.GetList<Questions>(
                SQLQueries.GetListQuestions,
                MyDapper.GetSingle<TestKeys>(
                    SQLQueries.GetSingleTestKey, key).Test_ID);

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

        /// <summary>
        /// Возвращает заголовок вопроса
        /// </summary>
        /// <param name="i">Порядковый номер вопроса</param>
        /// <returns>Заголовок вопроса</returns>
        public string GetQuestion(int i) => _dic_Variants[i][0];

        /// <summary>
        /// Возвращает варианты ответа
        /// </summary>
        /// <param name="i">Порядковый номер вопроса</param>
        /// <returns>Варианты ответа</returns>
        public IEnumerable<string> GetVariant(int i) => _dic_Variants[i][1..];

        /// <summary>
        /// Возвращает случайный номер вопроса или -1 если вопросы закончились
        /// </summary>
        /// <returns>Случайный номер вопроса или -1 если вопросы закончились</returns>
        public int GetRandom() => Rand = _dic_Variants.Count == 0 ? -1 : _rand.Next(0, _dic_Variants.Count);

        /// <summary>
        /// Проверяет права автора
        /// </summary>
        /// <param name="TG_ID">ID пользователя</param>
        /// <returns>Статус наличия прав автора</returns>
        public static bool GetAuthor(long TG_ID) => MyDapper.GetSingle<Users>(SQLQueries.GetSingleUser, TG_ID).Author;

        /// <summary>
        /// Сохраняет ответ и удаляет вопрос из списка вопросов
        /// </summary>
        /// <param name="responseIndex"></param>
        /// <param name="TG_ID">ID пользователя</param>
        /// <param name="variant">Индекс варианта</param>
        public void SetResponse(int responseIndex, long TG_ID, int variant)
        {
            MyDapper.SaveAnswer(TestKey, TG_ID, _dic_Variants[variant][++responseIndex]);
            // удаляем вопрос из списка вопросов чтобы больше не показывался
            _dic_Variants.Remove(variant);
        }

        /// <summary>
        /// Сохраняет спарсенный тест в базу данных
        /// </summary>
        /// <param name="questions">Список вопросов (с заголовками)</param>
        /// <param name="testName">Имя теста</param>
        /// <param name="timeToTake">Время на прохождение</param>
        /// <param name="TG_ID">ID пользователя (автора)</param>
        /// <returns>Количество добавленных строк вопросов теста</returns>
        public static int SaveTest(List<Models.Questions> questions, string testName, short timeToTake, long TG_ID) =>
            MyDapper.SaveTest(questions, testName, timeToTake, TG_ID);

        public static string GetResults(long TG_ID, bool author = false)
        {
            var tmp = MyDapper.GetList<Tests>(author ? SQLQueries.GetListMyCreatedTests : SQLQueries.GetListCompletedTests, TG_ID);
            System.Text.StringBuilder sb = new();
            foreach (var item in tmp)
            {
                sb.AppendLine("ID: " + item.ID);
                sb.AppendLine("Имя: " + item.Name);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Просмотр результата тестирования
        /// </summary>
        /// <param name="TG_ID">ID пользователя</param>
        /// <param name="Test_ID">ID теста</param>
        /// <returns>Строку с текстом вопроса и ответом</returns>
        public static string GetTestResult(long TG_ID, string Test_ID)
        {
            if (!int.TryParse(Test_ID, out int testID))
            {
                return string.Empty;
            }
            var tmp = MyDapper.GetList<Questions>(SQLQueries.GetTestQuestionsResult, TG_ID, testID);
            System.Text.StringBuilder sb = new();
            foreach (var item in tmp)
            {
                sb.Append("Вопрос " + item.Question_Text);
                sb.Append('\t');
                sb.Append("Балл" + item.Question_Ball);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static bool DeleteTest(long TG_ID, string Test_ID)
        {
            if (!int.TryParse(Test_ID, out int testID))
            {
                return false;
            }

            if (MyDapper.GetSingle<Users>(SQLQueries.GetSingleUser, MyDapper.GetSingle<Tests>(SQLQueries.GetSingleTest, testID).Author_ID).TG_ID == TG_ID)
            {
                return MyDapper.DeleteTest(testID) > 0;
            }

            return false;
        }

        public static bool CheckAddUser(long TG_ID) => MyDapper.GetSingle<Users>(SQLQueries.GetSingleUser, TG_ID).TG_ID != 0;

        public static void AddUser(long TG_ID, string Firstname, string Lastname) => MyDapper.AddUser(TG_ID, Firstname, Lastname);

        public static string RandomString(int length = 64) => new(
            Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                      .Select(s => s[_rand.Next(s.Length)])
                      .ToArray());

        public static void SetAuthor(long TG_ID) => MyDapper.SetAuthor(TG_ID);
    }
}