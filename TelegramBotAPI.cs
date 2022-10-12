using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Newtonsoft.Json;

namespace Console_TelegramBot
{
    class TelegramBotAPI
    {
        private readonly static Dictionary<long, MyRepository> testUsers = new();
        public TelegramBotAPI()
        {
            ITelegramBotClient bot = new TelegramBotClient(System.Configuration.ConfigurationManager.AppSettings["BotKey"]);
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions()
                {
                    AllowedUpdates = new UpdateType[]
                    {
                        UpdateType.Message,
                        UpdateType.PollAnswer
                    }
                },
                new CancellationTokenSource().Token
            );
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.PollAnswer)
            {
                if (!testUsers.ContainsKey(update.PollAnswer.User.Id))
                {
                    return;
                }
                testUsers[update.PollAnswer.User.Id].SetResponse(update.PollAnswer.OptionIds[0], update.PollAnswer.User.Id, testUsers[update.PollAnswer.User.Id].Rand);
                StartTest(botClient, testUsers[update.PollAnswer.User.Id], update.PollAnswer.User.Id, cancellationToken);
                return;
            }

            if (update.Message.Type == MessageType.Document)
            {
                if (MyRepository.GetAuthor(update.Message.Chat.Id))
                {
                    SaveFile(botClient, update.Message.Document.FileName, update.Message.Document.FileId, update.Message.Chat.Id);
                }
                else
                {
                    await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId, cancellationToken);
                    await botClient.SendTextMessageAsync(update.Message.Chat, "У вас нет прав на загрузку файла.\nЕсли вы считаете что это неправильно, свяжитесь с администратором", cancellationToken: cancellationToken);

                }
                return;
            }

            if (update.Message.Type != MessageType.Text)
            {
                return;
            }

            Console.WriteLine(update.Message.Text);
            var command = update.Message.Text.Split(' ');
            switch (command[0])
            {
                case "/start":
                    {
                        if (!MyRepository.CheckAddUser(update.Message.Chat.Id))
                        {
                            MyRepository.AddUser(
                                update.Message.Chat.Id,
                                update.Message.Chat.FirstName,
                                update.Message.Chat.LastName);
                        }
                        await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Привет, что делаем?",
                        cancellationToken: cancellationToken
                        );
                        break;
                    }
                case "/get_results":
                    {
                        if (command.Length == 2)
                        {
                            if (int.TryParse(command[1], out int Test_ID))
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat, "Ваш результат по тесту:\n" + MyRepository.GetTestResult(update.Message.Chat.Id, Test_ID), cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(update.Message.Chat, "Неопознанная команда. Введитке команду, либо не нагружайте меня", cancellationToken: cancellationToken);
                            }
                        }
                        break;
                    }
                case "/start_test":
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat, "Отправьте мне ключ и я постараюсь найти ваш тест.", cancellationToken: cancellationToken);
                        break;
                    }
                case "/schedule_test":
                    {
                        if (MyRepository.GetAuthor(update.Message.Chat.Id))
                        {
                            //MyRepository.SheduleTest();
                        }
                        else
                        {

                        }
                        break;
                    }
                case "/add_test":
                    {
                        if (MyRepository.GetAuthor(update.Message.Chat.Id))
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Отправьте мне файл в формате Aiken для загрузки вопросов", cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Данный функционал недоступен.\nЕсли вы считаете что это неправильно, свяжитесь с администратором", cancellationToken: cancellationToken);
                        }
                        break;
                    }
                case "/get_my_tests":
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat, $"Вот все ваши тесты:\n{MyRepository.GetResults(update.Message.Chat.Id)}Для просмотра результатов по конкретному тесту, отправьте команду /get_results", cancellationToken: cancellationToken);
                        break;
                    }
                default:
                    {
                        if (command[0].Length == 32)
                        {
                            if (!testUsers.ContainsKey(update.Message.Chat.Id))
                            {
                                testUsers.Add(update.Message.Chat.Id, new MyRepository());
                            }
                            StartTest(botClient, testUsers[update.Message.Chat.Id], update.Message.Chat.Id, cancellationToken, command[0]);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Неопознанная команда. Введитке команду, либо не нагружайте меня", cancellationToken: cancellationToken);
                        }
                        break;
                    }
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception));
            return Task.CompletedTask;
        }

        private static async void SaveFile(ITelegramBotClient botClient, string fileName, string fileId, long chatId)
        {
            string directoryPath = System.IO.Directory.CreateDirectory($"files\\{chatId}\\").FullName;   //Создаём папку и получаем её путь

            File file = await botClient.GetFileAsync(fileId);
            System.IO.FileStream fs = new($"{directoryPath}{fileName}", System.IO.FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
            Console.WriteLine(
                MyRepository.SaveTest(
                    AikenParser.ParseQuestions(directoryPath + fileName),
                    fileName,
                    60,
                    chatId));
            await botClient.SendTextMessageAsync(chatId, "OK. Ваш файл успешно сохранён.");
        }

        private static async void StartTest(ITelegramBotClient botClient, MyRepository user, ChatId chatId, CancellationToken cancellationToken, string key = null)
        {
            if (key is not null)
                if (!user.StartTest(key))
                {
                    {
                        await botClient.SendTextMessageAsync(chatId, "Тестов по данному ключу не найдено.\nПовторите попытку.", cancellationToken: cancellationToken);
                        return;
                    }
                }
            int r = user.GetRandom();

            if (r != -1)
            {
                await botClient.SendPollAsync(
                chatId,
                user.GetQuestion(r),
                user.GetVariant(r),
                false,
                PollType.Quiz,
                false,
                cancellationToken: cancellationToken
                );
            }
            else
            {
                // уже прошли все вопросы, которые были.
                await botClient.SendTextMessageAsync(chatId, "По данному ключу тест завершён.", cancellationToken: cancellationToken);
            }
        }
        /*    private static async void GetJson(Exception obj)
            {
                string json;
                using (var stream = new MemoryStream())
                {
                    await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj);
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);
                    json = await reader.ReadToEndAsync();
                }
                return json;
            }*/
    }
}
