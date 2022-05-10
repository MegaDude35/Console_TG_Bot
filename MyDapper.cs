using Npgsql;
using System.Linq;
using System;
using Dapper;
using System.Collections.Generic;

namespace Console_TelegramBot
{
    public class MyDapper
    {

         static public void importData()
        {
            var list_answers = new List<answers>();
            var list_string = new List<string>();
            var list_questions = new List<questions>();
            using (var connection = new NpgsqlConnection(Config.SqlConnectionString))
            {

                string sql = @"select * from  questions ";
                connection.Open();
                var res_questions = connection.Query<questions>(sql);
                list_questions = res_questions.ToList();


                foreach (questions r in list_questions)
                {
                    sql = @"select * from  answers where idquestions = @idquestions";

                    var connection2 = new NpgsqlConnection(Config.SqlConnectionString);
                    connection2.Open();
                    var res_answers = connection2.Query<answers>(sql,new { idquestions = r.id });
                    list_answers = res_answers.ToList();

             
                    string[] array_string = new string[list_answers.Count()];

                    for (int i = 0; i < list_answers.Count(); i++)
                    {
                        array_string[i] = list_answers[i].name;
                        if (list_answers[i].mark == 1)
                        {
                            MyRepository._dic_answer.TryAdd(r.name, list_answers[i].name);
                        }
                    }

                    bool v = MyRepository._dic_variants.TryAdd(r.name, array_string);
                }
            }
        }


        static public void saveData()
        {


            // и пишем в табличку базы данных.
            using (var connection = new NpgsqlConnection(Config.SqlConnectionString))
            {
                // наполняем наши классы,
                for (int i = 0; i < MyRepository._dic_answer.Count(); i++)
                {
                    var question = new questions();
                    question.id = Guid.NewGuid().ToString();
                    question.name = MyRepository.GetQuestion(i);

                    var query = @"insert into questions(id,name) 
                                values (@id, @name)";
                    var list = connection.Execute(query, new { id = question.id, name = question.name });

                    string[] variant = MyRepository.GetVariant(i);
                    MyRepository._dic_answer.TryGetValue(question.name, out string correctValue);
                    foreach (string var in variant)
                    {
                        var answer = new answers();
                        answer.id = Guid.NewGuid().ToString();
                        answer.name = var;
                        answer.idquestions = question.id;
                        if (correctValue == var)
                        {
                            answer.mark = 1;
                        }

                        query = @"insert into answers(id,idquestions,name,mark) 
                                values (@id, @idquestions,@name,@mark)";
                        var list2 = connection.Execute(query, new { 
                            id = answer.id
                            , idquestions = answer.idquestions
                            , name = answer.name
                            , mark = answer.mark
                        });
                    }
                }

            }
        }
    }
}


/*
--CREATE DATABASE quiz_show;

--*******************
--далее, переходим в эту базу данных и выполняем создание таблиц:
--*******************

--CREATE DATABASE quiz_show;

--*******************
-- далее, переходим в эту базу данных и выполняем создание таблиц:
--*******************

CREATE TABLE person
(
    id CHARACTER VARYING(50) PRIMARY KEY
    ,firstName CHARACTER VARYING(50)
    ,lastName CHARACTER VARYING(50)
	,languageCode CHARACTER VARYING(10)
);

CREATE TABLE questions 
(
    id CHARACTER VARYING(50) PRIMARY KEY
	,name CHARACTER VARYING(1000)
);

CREATE TABLE answers 
(
    id CHARACTER VARYING(50) PRIMARY KEY
	,idQuestions CHARACTER VARYING(50) REFERENCES questions (id)
	,name CHARACTER VARYING(1000)
	,mark int
);

CREATE TABLE inspection
(
   	id CHARACTER VARYING(50) PRIMARY KEY
    ,idPerson CHARACTER VARYING(50) REFERENCES person (id)
	    ON DELETE RESTRICT
    	ON UPDATE CASCADE
	,idQuestion CHARACTER VARYING(50) REFERENCES questions (id)
	    ON DELETE RESTRICT
    	ON UPDATE CASCADE
    ,result INTEGER
	,data Date
);

create index idx_person
 on inspection (id);


 */