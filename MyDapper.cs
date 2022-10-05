using Console_TelegramBot.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        public static class MyDapper
        {
            static readonly SqlConnection conn = new(Properties.Resources.ConnectionString);
            public static TestKeys GetTestKey(string key)
            {
                conn.Open();
                TestKeys result = conn.Query<TestKeys>(@"
                select top(1) *
                from Test_Keys
                where Test_Key like '" + key + "';").FirstOrDefault();
                conn.Close();
                return result;
            }

            public static Tests GetTest(int testID)
            {

                conn.Open();
                Tests result = conn.Query<Tests>(@"
                select top(1) *
                from Tests
                where ID = @testID;",
                    new { testID }).FirstOrDefault();
                conn.Close();
                return result;
            }

            public static Users GetUser(long TG_ID)
            {
                conn.Open();
                Users result = conn.Query<Users>(@"
                select top(1) *
                from Users
                where TG_ID = @TG_ID;", new { TG_ID }).FirstOrDefault();
                conn.Close();
                return result;
            }

            public static List<Questions> GetQuestions(int testID)
            {

                conn.Open();
                List<Questions> result = conn.Query<Questions>(@"
                select *
                from Questions
                where Test_Id = @testID
                order by Question_Group asc;", new { testID }).ToList();
                conn.Close();
                return result;
            }

            public static int SaveTest(List<Questions> questions, string testName, short TimeToTake, long TG_ID)
            {

                int testID = conn.ExecuteScalar<int>(@"
                    insert into Tests(Name, Time_to_Take, AuthorID)
                    output inserted.ID
                    values(@testName, @TimeToTake, @AuthorID);",
                        new { testName, TimeToTake, GetUser(TG_ID).Id });

                int result = 0;
                foreach (Questions question in questions)
                {
                    result += conn.Execute(@"
                    insert into Questions(Id_Test, Question_Group, Question_Text, Question_Ball)
                    values (@testID, @Question_Group, @Question_Text, @Question_Ball);",
                            new
                            {
                                testID,
                                question.Question_Group,
                                question.Question_Text,
                                question.Question_Ball
                            });
                }
                return result;
            }

            public static Dictionary<int, string> GetCompletedTests(long TG_ID)
            {
                conn.Open();
                Dictionary<int, string> result = new();
                List<Tests> tests = conn.Query<Tests>(@"
                select *
                from Tests
                join Test_Keys on Tests.ID = Test_Keys.Id_Test
                join Answers on Answers.IdTest_Key = Test_Keys.ID
                join Users on Users.ID = Answers.Id_User
                where Users.TG_ID = @TG_ID
                group by Answers.IdTest_Key;", new { TG_ID }).ToList();
                conn.Close();
                foreach (Tests test in tests)
                {
                    result.Add(test.Id, test.Name);
                }
                return result;
            }
        }
    }
}