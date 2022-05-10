using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_TelegramBot
{
    public class MyRepository
    {
        public static ConcurrentDictionary<string, string[]> _dic_variants = new ConcurrentDictionary<string, string[]>();
        
        public static ConcurrentDictionary<string, string> _dic_answer = new ConcurrentDictionary<string, string>();

        public static ConcurrentDictionary<string, string> _dic_answerPerson = new ConcurrentDictionary<string, string>();

        private static Random _rand = new Random();

        private static int currentIssue = -1;

        public int id;



        public static bool AddWord( string val)
        {
            if (!val.Contains("?") || !val.Contains(";") || !val.Contains("*"))
            {
                return false;
            }

            try 
            {
                string[] _phrase = val.Split("?");
                string _key = _phrase[0] + "?";
                string _val = val.Replace(_phrase[0], "");
                _val = _val.Replace("?", "");

                string[] _variants = _val.Split(";");
                
                int counter = 0;
                foreach (string i in _variants)
                {

                    if (i.Contains("*"))
                    {
                        _variants[counter] = i.Replace("*", "");
                        _dic_answer.TryAdd(_key, _variants[counter]);
                    }
                    if (i=="")
                    {
                        _variants[counter] = i.Replace("", "Другое");
                    }

                    counter++;
                }
                _dic_variants.TryAdd(_key, _variants);
                return true;
            }
            catch 
            {
                return false;
            }
          
        }


        public static string[] GetVariant(int r)
        {
            var keys = new List<string>(_dic_variants.Keys);
            _dic_variants.TryGetValue(keys[r], out string[] value );
            return value;
        }




        public static int GetIndexCorrectOption(int r)
        {
            if (r == -1)
            { return 0; }
            
            var keys = new List<string>(_dic_variants.Keys);
            
            _dic_answer.TryGetValue(keys[r], out string correctValue);
            _dic_variants.TryGetValue(keys[r], out string[] value);


            // теперь ищем индекс правильного ответа
            for (int i= 0; i< value.Length; i++)
            {
                if (value[i] == correctValue)
                    return i;
            }
            return 0;

        }

        public static string GetQuestion(int r)
        {
            var keys = new List<string>(_dic_variants.Keys);
            return keys[r];
        }


        public static int GetRundom()
        {
            if (_dic_variants.Count == 0)
                return -1;

            currentIssue = _rand.Next(0, _dic_variants.Count-1);
            return currentIssue;
        }


        public static int GetCarrentIssuse()
        {
            return currentIssue;
        }


        public static void setResponse(int IDRespons)
        {
            // пришел какой-то ответ, но если мы не проходили опрос - игнорим.
            if (currentIssue == -1)
            { return; }
            
            // ответили правильно.
            if (GetIndexCorrectOption(currentIssue) == IDRespons)
            {
                var answer = GetQuestion(currentIssue);
                // если в таблице ответов он уже был - удаляем вопрос из списка вопросов (чтобы больше не показывался
                if (_dic_answerPerson.ContainsKey(answer))
                {
                    _dic_variants.TryRemove(answer,out string[] val1);
                    _dic_answer.TryRemove(answer, out string val2);
                }
                else
                {
                    // если правильный - и еще не было в таблице правильных ответов пользователя - запишем новую строку ответов пользователя.
                    _dic_answerPerson.TryAdd(answer, answer);
                }

            }
        }

    }
}

