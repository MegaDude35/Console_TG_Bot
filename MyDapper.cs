using Console_TelegramBot.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private static class MyDapper
        {
            static readonly SqlConnection conn = new(System.Configuration.ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString);

            public static T GetSingle<T>(string query, dynamic param1 = null, dynamic param2 = null)
            {
                conn.Open();
                T result = conn.Query<T>(query,
                new { param1, param2 }).FirstOrDefault();
                conn.Close();
                return result;
            }

            public static List<T> GetList<T>(string query, dynamic param1 = null, dynamic param2 = null)
            {
                conn.Open();
                List<T> result = conn.Query<T>(query,
                new {param1, param2}).ToList();
                conn.Close();
                return result;
            }

            public static int SaveTest(List<Questions> questions, string testName, short TimeToTake, long TG_ID)
            {
                conn.Open();
                int testID = conn.ExecuteScalar<int>(@"
                    insert into Tests(Name, Time_to_Take, Author_ID)
                    output inserted.ID
                    values(@testName, @TimeToTake, @TG_ID);",
                        new { testName, TimeToTake, TG_ID });

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

            public static int SaveAnswer(string TestKey, long TG_ID, string Question_Text, int Try = 1)
            {
                var Test_Key_ID = GetSingle<TestKeys>(SQLQueries.GetSingleTestKey, TestKey).ID;
                var Question_ID = GetSingle<Questions>(SQLQueries.GetSingleQuestion, Question_Text).ID;
                conn.Open();
                int result = conn.Execute(@"
                    insert into Answers(Test_Key_ID, User_ID, Question_ID, Try)
                    values (@Test_Key_ID, @TG_ID, @Question_ID, @Try);",
                    new
                    {
                        Test_Key_ID,
                        TG_ID,
                        Question_ID,
                        Try
                    });
                conn.Close();
                return result;
            }

            public static void AddUser(long TG_ID, string Firstname, string Lastname)
            {
                conn.Open();
                conn.Execute(@"
                    insert into Users(Firstname, Lastname, TG_ID, Author)
                    values (@Firstname, @Lastname, @TG_ID, DEFAULT);",
                    new { Firstname, Lastname, TG_ID });
                conn.Close();
            }

            public static void SetAuthor(long TG_ID, bool flag = true)
            {
                conn.Open();
                conn.Execute(@"
                alter table Users
                set Author = @flag
                where TG_ID = @TG_ID;",
                new { flag, TG_ID });
                conn.Close();
            }

            public static int DeleteTest(int testID)
            {
                conn.Open();
                int result = conn.Execute(@"
                delete from tests
                where id = @testID",
                new { testID });
                conn.Close();
                return result;
            }

        }
    }
}