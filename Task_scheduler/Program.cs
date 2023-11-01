using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Task_scheduler
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("Сюда пихать токен");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            try
            {
                
            switch (update.Type) //обработка типов приходищих данных
            {
                case UpdateType.Message:
                {
                    var message = update.Message; //Вся инфа о сообщении пользователя
                    var chat = message.Chat; // Вся инфа о чате

                    await botClient.SendTextMessageAsync(message.Chat, "Напишите /start чтоб посмотреть команды");
                    if (message.Text.ToLower() == "/start") //Если старт, то пишем здрасьте
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                        

                        // инициализируем клаву
                        var inlineKeyboard = new InlineKeyboardMarkup(
                            new
                                List<InlineKeyboardButton[]>() // список, cодержащщий массив кнопок
                                {
                                    // Каждый новый массив - это дополнительные строки,
                                    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                    new InlineKeyboardButton[] // тут создаем массив кнопок
                                    {
                                        InlineKeyboardButton.WithUrl("Это кнопка с сайтом", "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley"),
                                        InlineKeyboardButton.WithCallbackData("Команда 1", "button1"),
                                    },
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Команда 2", "button2"),
                                        InlineKeyboardButton.WithCallbackData("Команда 3", "button3"),
                                    },
                                });

                        await botClient.SendTextMessageAsync(
                            chat.Id,
                            "Это клавиатура команд!",
                            replyMarkup: inlineKeyboard); // Все клавиатуры передаются в параметр replyMarkup
                    }
                    

                    var username = update.Message.From.Username; //Получаем имя пользователя
                    await botClient.SendTextMessageAsync(message.Chat, "Сообщение получено " + username);
                    return;
                }

                    case UpdateType.CallbackQuery: //Если запрос от нажатия кнопки
                    {
                        // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                        var callbackQuery = update.CallbackQuery;
                        var user = callbackQuery.From;
                        // Выводим на экран нажатие кнопки
                        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                        
                        var chatt = callbackQuery.Message.Chat;

                        // Проверка(индентификация нажатой кнопки)
                        switch (callbackQuery.Data)
                        {

                            case "button1":
                            {
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);// запрос для тг, что мы нажали кнопку
                                

                                await botClient.SendTextMessageAsync(
                                    chatt.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }

                            case "button2":
                            {
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут может быть ваш текст!"); //выводит уведомление на экран

                                await botClient.SendTextMessageAsync(
                                    chatt.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }

                            case "button3":
                            {
                                
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "А это полноэкранный текст!",
                                    showAlert: true); //выводит полноценное окно внутри тг, параметр showalert: true позволяет это сделать

                                await botClient.SendTextMessageAsync(
                                    chatt.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }
                        }


                        return;
                    }

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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