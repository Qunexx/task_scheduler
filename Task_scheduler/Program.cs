using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace Task_scheduler
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("Сюда пихать токен своего бота");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message) //Проверка на то, сообщение ли было отправлено
            {
                var message = update.Message; //Вся инфа о сообщении пользователя
                if (message.Text.ToLower() == "/start") //Если старт, то пишем здрасьте
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    await botClient.SendTextMessageAsync(message.Chat, "Список моих команд:");
                    await botClient.SendTextMessageAsync(message.Chat, "1./......");
                    await botClient.SendTextMessageAsync(message.Chat, "2./......");
                    await botClient.SendTextMessageAsync(message.Chat, "3./......");
                    await botClient.SendTextMessageAsync(message.Chat, "4./......");
                    return;
                }
                var username = update.Message.From.Username; //Получаем имя пользователя
                await botClient.SendTextMessageAsync(message.Chat, "Сообщение получено " + username);
             
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception)); //Выводим сообщение пользователя в консоль(Далее его можно будет сохранить в бд)
        }


        static void Main(string[] args) //работа самого клиента(консоли)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}