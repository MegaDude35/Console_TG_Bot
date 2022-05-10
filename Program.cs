using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;

namespace Console_TelegramBot
{
    class Program
    {

        static ITelegramBotClient bot = new TelegramBotClient("TOKEN");
        private static PollType? keyboard;
 


        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.PollAnswer)
            {
                MyRepository.setResponse(update.PollAnswer.OptionIds[0]);
            }

            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message|| update.Message==null || update.Message.Text==null)
                return;

            if (update.Message.Type == MessageType.Sticker)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, update.Message.Sticker.Emoji);
                return;
            }



            var message = update.Message;
            var chatId = message.Chat.Id;
            var messageText = message.Text;


            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] {"Добавить вопрос", "Вызвать опрос"},
                new KeyboardButton[] {"Сохранить в БД", "Загрузить из БД" },

            })
            
            {
                ResizeKeyboard = true
            };



            var argMsg = update.Message.Text.Split(" ");
            switch (argMsg[0])
            {

                case "/start":
                {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Привет, что делаем? ",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken,
                        disableNotification: false
                        );
                        
                        //await botClient.SendTextMessageAsync(message.From.Id, command_List);
                        break;
                }

                case "Вызвать":
                    {
                        var r = MyRepository.GetRundom();
                        //Personal.setCommand(chatId, argMsg[0]);

                        if (r != -1)
                        {
                            try
                            {
                                Message pollMessage = await botClient.SendPollAsync(
                                chatId: chatId,
                                question: MyRepository.GetQuestion(r),
                                options: MyRepository.GetVariant(r),
                                type: PollType.Quiz,
                                cancellationToken: cancellationToken,
                                correctOptionId: MyRepository.GetIndexCorrectOption(r),
                                replyToMessageId: 0,
                                isAnonymous: false
                                );
                            }
                            catch
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Что-то пошло не так. ");
                            }
                        }
                        else
                        {
                            if (MyRepository._dic_answerPerson.Count != 0)
                            {
                                // уже прошли все вопросы, которые были.
                                await botClient.SendTextMessageAsync(message.Chat, "Вопросов больше не осталось.    " +
                                "Нужно создать вопросы (или загрузить из БД) ");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Пустой стек вопросов.    " +
                                    "Нужно создать вопросы (или загрузить из БД) ");
                            }


                        }

                        break;
                    }
                case "Добавить":
                    {
                        Personal.setCommand(chatId, argMsg[0]);
                        break;
                    }
                case "Сохранить":
                    {
                        int numberOfQuestions = MyRepository._dic_answer.Count;
                        MyDapper.saveData();
                        await botClient.SendTextMessageAsync(message.Chat, $"Сохранено {numberOfQuestions} вопросов ");
                        break;
                    }
                case "Загрузить":
                    {
                        int numberOfQuestions = MyRepository._dic_answer.Count;
                        MyDapper.importData();
                        int numberOfQuestions2 = MyRepository._dic_answer.Count - numberOfQuestions;
                        string ending;
                        if (numberOfQuestions2 < 5)
                        { ending = "а"; }
                        else
                        { ending = "ов"; }

                        await botClient.SendTextMessageAsync(message.Chat, $"Загружено {numberOfQuestions2} вопрос{ending}");
                        break;
                    }
                default:
                    {
                        if (Personal.lastCommand.ContainsKey(chatId))
                        {
                            if (Personal.lastCommand[chatId] == "Добавить")
                            { 
                                if (MyRepository.AddWord(update.Message.Text)==false)
                                {
                                    await botClient.SendTextMessageAsync(message.Chat, "Не корректная фраза. Должны присутствовать <?> разделитель вариантов <;> правильный ответ помечаем <*>" +
                                        "например : Какой-то вопрос? Ответ1; *Ответ2; Ответ3 ");
                                }
                            }
                        }
                        break;
                    }

            }
            //await botClient.SendTextMessageAsync(message.Chat, update.Message.Text);


        }

  



        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            bot = new TelegramBotClient(Config.apiKey);

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);


            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var allowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.PollAnswer };
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = allowedUpdates 
            };
            
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );




            Console.ReadLine();
            cts.Cancel();
        }


    }
}
