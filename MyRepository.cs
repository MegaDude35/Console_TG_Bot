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

        private static Random _rand = new Random();

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

            var r = _rand.Next(0, _dic_variants.Count-1);
            return r;
        }

    }
}

