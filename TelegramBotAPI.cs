using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using Microsoft.VisualBasic;
using System.IO.Pipes;

namespace Console_TelegramBot
{
    class TelegramBotAPI
    {
        public TelegramBotAPI()
        {
            ITelegramBotClient bot = new TelegramBotClient(Properties.Resources.TGBotApiKey);
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
            Console.WriteLine(update.Message.Text);
            if (update.Type == UpdateType.PollAnswer)
            {
                MyRepository.SetResponse(update.PollAnswer.OptionIds[0]);
                return;
            }

            if (update.Message.Type == MessageType.Document)
            {
                SaveFile(botClient, update.Message.Document.FileName, update.Message.Document.FileId, update.Message.Chat.Id);
                return;
            }

            if (update.Message.Type == MessageType.Sticker)
            {
                await botClient.SendTextMessageAsync(
                    update.Message.Chat,
                    update.Message.Sticker.Emoji,
                    cancellationToken: cancellationToken);
                return;
            }

            if (update.Message.Text is null)
            {
                return;
            }

            switch (update.Message.Text)
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
                        replyMarkup:
                        MyRepository.GetAuthor(update.Message.Chat.Id) ?
                        new ReplyKeyboardMarkup(new[]
                        {
                            new KeyboardButton[]
                            {
                                "Добавить тест", "Запустить тест", "Посмотреть результаты"
                            }
                        })
                        {
                            ResizeKeyboard = true
                        }
                        :
                        new ReplyKeyboardMarkup(new[]
                        {
                            new KeyboardButton[]
                            {
                                "Запустить тест", "Посмотреть результаты"
                            }
                        })
                        {
                            ResizeKeyboard = true
                        },
                        cancellationToken: cancellationToken
                        );
                        break;
                    }

                case "Запустить тест":
                    {
                        int r = MyRepository.GetRandom();

                        if (r != -1)
                        {
                            await botClient.SendPollAsync(
                            update.Message.Chat.Id,
                            MyRepository.GetQuestion(r),
                            MyRepository.GetVariant(r),
                            false,
                            PollType.Quiz,
                            false,
                            MyRepository.GetIndexCorrectOption(r),
                            cancellationToken: cancellationToken
                            );
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Что-то пошло не так.", cancellationToken: cancellationToken);
                        }
                        else
                        {
                            if (MyRepository.QuestionsEmpty)
                            {
                                // уже прошли все вопросы, которые были.
                                await botClient.SendTextMessageAsync(update.Message.Chat, "Тест окончен.", cancellationToken: cancellationToken);
                            }
                        }
                        break;
                    }
                case "Добавить тест":
                    {
                        if (!MyRepository.GetAuthor(update.Message.Chat.Id))
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Данный функционал недоступен.\nЕсли вы считаете что это неправильно, свяжитесь с администратором", cancellationToken: cancellationToken);
                            break;
                        }
                        await botClient.SendTextMessageAsync(update.Message.Chat, "Отправьте мне файл в формате Aiken для загрузки вопросов", cancellationToken: cancellationToken);
                        break;
                    }
                case "Посмотреть результаты":
                    {
                        return;
                        var tmp = MyRepository.ImportResults(update.Message.Chat.Id);
                        System.Text.StringBuilder sb = new();
                        foreach (var item in tmp)
                        {
                            sb.Append(item.Value + '\n');
                        }
                        await botClient.SendTextMessageAsync(update.Message.Chat, $"Вот все ваши тесты:\n" + sb.ToString(), cancellationToken: cancellationToken);
                        break;
                    }
                default:
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat, "Некорректная фраза. Выберите вариант, либо не нагружайте меня", cancellationToken: cancellationToken);
                        break;
                    }
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.InnerException.ToString() + ' ' + exception.Message);
        }

        private static async void SaveFile(ITelegramBotClient botClient,string fileName, string fileId, long chatId)
        {
            string directoryPath = Directory.CreateDirectory($"files\\{chatId}\\").FullName;   //Создаём папку и получаем её путь

            Telegram.Bot.Types.File file = await botClient.GetFileAsync(fileId);
            FileStream fs = new($"{directoryPath}{fileName}", FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
            Console.WriteLine(
                MyRepository.SaveTest(
                    AikenParser.ParseQuestions(directoryPath + fileName),
                    fileName.Remove(fileName.LastIndexOf('.')),
                    60,
                    chatId));
            await botClient.SendTextMessageAsync(chatId, "OK. Ваш файл успешно сохранён.");
        }

        private static async Task<string> GetJson(Exception obj)
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
        }
    }
}
