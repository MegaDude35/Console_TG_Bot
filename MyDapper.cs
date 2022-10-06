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
                where TG_ID = @TG_ID;",
                new { TG_ID }).FirstOrDefault();
                conn.Close();
                return result;
            }

            public static Questions GetQuestion(string Question_Text)
            {
                conn.Open();
                Questions result = conn.Query<Questions>(@"
                select top(1) *
                from Questions
                where Question_Text like '" + Question_Text + "';").FirstOrDefault();
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
                order by Question_Group asc;",
                new { testID }).ToList();
                conn.Close();
                return result;
            }

            public static Dictionary<int, string> GetCompletedTests(long TG_ID)
            {
                conn.Open();
                Dictionary<int, string> result = new();
                List<Tests> tests = conn.Query<Tests>(@"
                select *
                from Tests
                join Test_Keys on Tests.ID = Test_Keys.Test_ID
                join Answers on Answers.Test_Key_ID = Test_Keys.ID
                join Users on Users.ID = Answers.User_ID
                where Users.TG_ID = @TG_ID
                group by Answers.Test_Key_ID;",
                new { TG_ID }).ToList();
                conn.Close();
                foreach (Tests test in tests)
                {
                    result.Add(test.Id, test.Name);
                }
                return result;
            }

            public static int SaveTest(List<Questions> questions, string testName, short TimeToTake, long TG_ID)
            {
                conn.Open();
                int testID = conn.ExecuteScalar<int>(@"
                    insert into Tests(Name, Time_to_Take, Author_ID)
                    output inserted.ID
                    values(@testName, @TimeToTake, @Id);",
                        new { testName, TimeToTake, GetUser(TG_ID).Id });

                int result = 0;
                foreach (Questions question in questions)
                {
                    result += conn.Execute(@"
                    insert into Questions(Test_ID, Question_Group, Question_Type, Question_Text, Question_Ball)
                    values (@testID, @Question_Group, @Question_Type, @Question_Text, @Question_Ball);",
                            new
                            {
                                testID,
                                question.Question_Group,
                                question.Question_Type,
                                question.Question_Text,
                                question.Question_Ball
                            });
                }
                conn.Close();
                return result;
            }

            public static int SaveAnswer(string TestKey, long TG_ID, string Question_Text)
            {
                int result;
                conn.Open();
                result = conn.Execute(@"
                    insert into Answers(Test_Key_ID, User_ID, Question_ID, Try
                    values (@Test_Key_ID, @User_ID, @Question_ID, DEFAULT);",
                    new
                    {
                        Test_Key_ID = GetTestKey(TestKey).Id,
                        User_ID = GetUser(TG_ID).Id,
                        Question_ID = GetQuestion(Question_Text).Id
                    });
                conn.Close();
                return result;
            }

            public static void AddUser(long TG_ID, string Firstname, string Lastname)
            {
                conn.Open();
                System.Console.WriteLine(conn.Execute(@"
                    insert into Users(Firstname, Lastname, TG_ID, Author)
                    values (@Firstname, @Lastname, @TG_ID, DEFAULT);",
                    new { Firstname, Lastname, TG_ID }));
                conn.Close();
            }
        }
    }
}