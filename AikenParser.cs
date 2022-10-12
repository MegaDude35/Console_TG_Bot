using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Console_TelegramBot
{
    internal class AikenParser
    {
        public static List<Models.Questions> ParseQuestions(string filePath)
        {
            byte groupCount = 0;
            List<Models.Questions> questions = new();
            List<string> strings = new();
            List<byte> questionGroup = new();
            foreach (string line in System.IO.File.ReadAllLines(filePath))
            {
                if (line == "")
                {
                    groupCount++;
                    continue;
                }

                if (Regex.IsMatch(line, @"^\bANSWER\b:\s\w$"))
                {
                    int index = strings.FindLastIndex(x => x[0] == line[8]);
                    strings[index] = '=' + strings[index];
                    continue;
                }
                questionGroup.Add(groupCount);
                strings.Add(line);
            };
            for (int i = 0; i < strings.Count; i++)
            {
                if (Regex.IsMatch(strings[i], @"^=?\w[.)]\s"))
                {
                    questions.Add(new Models.Questions
                    {
                        Question_Ball = strings[i][0] == '=' ? 1 : 0,
                        Question_Text = strings[i][0] == '=' ? strings[i][4..] : strings[i][3..],
                        Question_Group = questionGroup[i],
                        Question_Type = 1
                    });
                }
                else
                {
                    questions.Add(new Models.Questions
                    {
                        Question_Ball = 0,
                        Question_Text = strings[i],
                        Question_Group = questionGroup[i],
                        Question_Type = 1
                    });
                }
            }
            return questions;
        }
    }
}