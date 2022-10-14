namespace Console_TelegramBot
{
    public partial class MyRepository
    {
        private static class SQLQueries
        {
            public static string GetSingleTestKey => @"
                select top 1 *
                from Test_Keys
                where Test_Key like @param1;";

            public static string GetSingleTest => @"
                select top 1 *
                from Tests
                where ID = @param1;";

            public static string GetSingleUser => @"
                select top 1 *
                from Users
                where TG_ID = @param1;";

            public static string GetSingleQuestion => @"
                select top 1 *
                from Questions
                where Question_Text like @param1;";

            public static string GetListTests => @"
                select *
                from Tests;";

            public static string GetListTestsByName => @"
                select *
                from Tests
                where Name like @param1;";

            public static string GetListQuestions => @"
                select *
                from Questions
                where Test_Id = @param1
                order by Question_Group asc;";

            public static string GetListCompletedTests => @"
                select *
                from Tests
                where ID in (select Test_Keys.Test_ID
                            from Test_Keys
                            join Answers on Answers.Test_Key_ID = Test_Keys.ID
                            where Answers.User_ID = @param1);";

            public static string GetListMyCreatedTests => @"
                select *
                from Tests
                where Tests.Author_ID = @param1;";

            public static string GetTestQuestionsResult => @"
                select *
                from Questions
                join Answers on Answers.Question_ID = Questions.ID
                where Answers.User_ID = @param1 and Questions.Test_ID = @param2;";
        }
    }
}
