using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;

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
            if (update.Type == UpdateType.PollAnswer)
            {
                MyRepository.SetResponse(update.PollAnswer.OptionIds[0]);
            }

            if (update.Message.Type == MessageType.Document)
            {
                await using FileStream fileStream = System.IO.File.OpenWrite(update.Message.Document.FileName);
                await botClient.DownloadFileAsync(fileStream.Name, fileStream, cancellationToken);
                MyRepository.SaveTest(
                    AikenParser.ParseQuestions(fileStream.Name), 
                    fileStream.Name.Remove(fileStream.Name.LastIndexOf('.')),
                    60, update.Message.Chat.Id);
                //System.IO.File.Delete(fileStream.Name);
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

            if (update.Message is null ||
                update.Message.Text is null)
            {
                return;
            }
            var message = update.Message;
            var chatId = message.Chat.Id;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] {"Добавить тест", "Запустить тест", "Посмотреть результаты"},
            })

            {
                ResizeKeyboard = true
            };

            var argMsg = update.Message.Text[0];

            switch (argMsg)
            {
                case '/':
                    {
                        await botClient.SendTextMessageAsync(
                        chatId,
                        "Привет, что делаем?",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken,
                        disableNotification: false
                        );
                        break;
                    }

                case 'З':
                    {
                        int r = MyRepository.GetRandom();

                        if (r != -1)
                        {
                            try
                            {
                                await botClient.SendPollAsync(
                                chatId: chatId,
                                question: MyRepository.GetQuestion(r),
                                options: MyRepository.GetVariant(r),
                                type: PollType.Quiz,
                                cancellationToken: cancellationToken,
                                correctOptionId: MyRepository.GetIndexCorrectOption(r),
                                isAnonymous: false
                                );
                            }
                            catch
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Что-то пошло не так.", cancellationToken: cancellationToken);
                            }
                        }
                        else
                        {
                            if (!MyRepository.AnswersEmpty)
                            {
                                // уже прошли все вопросы, которые были.
                                await botClient.SendTextMessageAsync(message.Chat, "Тест окончен.", cancellationToken: cancellationToken);
                            }
                        }
                        break;
                    }
                case 'Д':
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Отправьте мне файл в формате Aiken для загрузки вопросов", cancellationToken: cancellationToken);
                        break;
                    }
                case 'П':
                    {
                        var tmp = MyRepository.ImportResults(update.Message.Chat.Id);
                        System.Text.StringBuilder sb = new();
                        foreach (var item in tmp)
                        {
                            sb.Append(item.Value + '\n');
                        }
                        await botClient.SendTextMessageAsync(message.Chat, $"Вот все ваши тесты:\n" + sb.ToString() , cancellationToken: cancellationToken);
                        break;
                    }
                default:
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Некорректная фраза. Выберите вариант либо не нагружайте меня", cancellationToken: cancellationToken);
                        break;
                    }
            }
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(await GetJson(exception));
        }


        private static async Task<string> GetJson (Exception obj)
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
