using System.Text;
using Npgsql;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Task_scheduler
{

    class Program
    {
        
        static ITelegramBotClient bot = new TelegramBotClient("Сюда токен пихать");
        static String connectionString = "Host=хост;Port=порт;Username=юзернейм;Password=пароль;Database=Название бд";

        enum UserState
        {
            None,
            AwaitingTaskDescription,
            AwaitingTaskDelete,
            AwaitingTaskIdForUpdate,
            AwaitingTaskUpdate
        }

        static Dictionary<long, UserState> userStates = new Dictionary<long, UserState>(); //хранение всех состояний пользователя
        static Dictionary<int, int> userTaskIds = new Dictionary<int, int>();  //промежуточное хранение выбора пользователя для смены состояния апдейта
        
       
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
                    var chatId = message.Chat.Id; //чатId
                    // if (message.Text.ToLower() == "/start") //Если старт, то пишем здрасьте
                    // {
                    //     await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    //     
                    //
                    //     // инициализируем клаву
                    //     var inlineKeyboard = new InlineKeyboardMarkup(
                    //         new
                    //             List<InlineKeyboardButton[]>() // список, cодержащщий массив кнопок
                    //             {
                    //                 // Каждый новый массив - это дополнительные строки,
                    //                 // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда
                    //
                    //                 new InlineKeyboardButton[] // тут создаем массив кнопок
                    //                 {
                    //                     InlineKeyboardButton.WithUrl("Это кнопка с сайтом", "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley"),
                    //                     InlineKeyboardButton.WithCallbackData("Команда 1", "button1"),
                    //                 },
                    //                 new InlineKeyboardButton[]
                    //                 {
                    //                     InlineKeyboardButton.WithCallbackData("Команда 2", "button2"),
                    //                     InlineKeyboardButton.WithCallbackData("Команда 3", "button3"),
                    //                 },
                    //            });
                    //
                    //     await botClient.SendTextMessageAsync(
                    //         chat.Id,
                    //         "Это клавиатура команд!",
                    //         replyMarkup: inlineKeyboard); // Все клавиатуры передаются в параметр replyMarkup
                    // }

                    int userId = (int)message.From.Id;
                    // 

    
                    // Установка начального состояния пользователя, если оно не определено
                    if (!userStates.TryGetValue(userId, out UserState currentState))
                    {
                        currentState = UserState.None;
                        userStates[userId] = currentState; 
                    }

                    string? username;
                    switch (currentState)
                    {
                        case UserState.None:
                            
                            if (message.Text.ToLower() == "/start")
                            {
                                username = update.Message.From.Username;
                                await botClient.SendTextMessageAsync(message.Chat, "Привет, " + username + "! Я Task Scheduler бот, вот список моих команд:");
                                Console.WriteLine($"Текущее состояние: {currentState}");
                                await botClient.SendTextMessageAsync(chatId, "Список команд: \n1./addtask \n2./showtasks \n3./deletetask \n4./updatetask\n.....");
                                userId = (int)message.From.Id;
                                userStates[userId] = UserState.None;
                            }
                            if (message.Text.ToLower() == "/addtask")
                            {
                                Console.WriteLine($"Текущее состояние: {currentState}");
                                await botClient.SendTextMessageAsync(chatId, "Введите описание задачи:");
                                userId = (int)message.From.Id;
                                userStates[userId] = UserState.AwaitingTaskDescription;
                            }
                            if (message.Text.ToLower() == "/showtasks")
                            {
                                Console.WriteLine($"Текущее состояние: {currentState}");
                                userId = (int)message.From.Id;
                                await GetTasks(userId);
                                await DisplayTasks(botClient,chatId,userId);
                                userStates[userId] = UserState.None;
                            }
                            if(message.Text.ToLower() == "/deletetask")
                            {
                                userId = (int)message.From.Id;
                                Console.WriteLine($"Текущее состояние: {currentState}");
                                await GetTasks(userId);
                                await DisplayTasks(botClient,chatId,userId);
                                await botClient.SendTextMessageAsync(chatId, "Выберите id задачи для удаления:");
                                userStates[userId] = UserState.AwaitingTaskDelete;
                                
                            }

                            if (message.Text.ToLower() == "/updatetask")
                            {
                                userId = (int)message.From.Id;
                                Console.WriteLine($"Текущее состояние: {currentState}");
                                await GetTasks(userId);
                                await DisplayTasks(botClient,chatId,userId);
                                await botClient.SendTextMessageAsync(chatId, "Выберите id задачи для обновления:");
                                userStates[userId] = UserState.AwaitingTaskIdForUpdate;
                                
                            }
                            
                            break;
                        
                        
                        case UserState.AwaitingTaskDescription:
                            userId = (int)message.From.Id;
                            Console.WriteLine($"Текущее состояние: {currentState}");
                            string description = message.Text;
                            

                            // Вызов метода добавления задачи
                            await AddTask(description, userId);
                            await botClient.SendTextMessageAsync(chatId, "Задача добавлена!");

                            // Возврат пользователя в начальное состояние
                            userStates[userId] = UserState.None;
                            break;
                        
                        case UserState.AwaitingTaskDelete:
                            
                            userId = (int)message.From.Id;
                            Console.WriteLine($"Текущее состояние: {currentState}");
                            int choice = Convert.ToInt32(message.Text);

                            if (await IsUsersTask(choice, userId))
                            {
                                await DeleteTask(choice);
                                await botClient.SendTextMessageAsync(chatId, "Задача удалена!");
                                userStates[userId] = UserState.None;
                                break;
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId, "У вас нет прав на удаление этой задачи, вы можете удалить только одну из представленных выше.");
                                break;
                            }

                        case UserState.AwaitingTaskIdForUpdate:
                            userId = (int)message.From.Id;
                            Console.WriteLine($"Текущее состояние: {currentState}");
                            int taskid = Convert.ToInt32(message.Text); 

                            if (await IsUsersTask(taskid, userId))
                            {
                                userTaskIds[userId] = taskid; // Сохраненяем taskid для конкретного userid
                                await botClient.SendTextMessageAsync(chatId, "Введите новое описание задачи:");
                                userStates[userId] = UserState.AwaitingTaskUpdate;
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId, "У вас нет прав на изменение этой задачи.");
                                break;
                            }
                            break;

                            

                        case UserState.AwaitingTaskUpdate:
                            userId = (int)message.From.Id;
                            Console.WriteLine($"Текущее состояние: {currentState}");
                            Console.WriteLine($"Полученный userId: {userId}");

                            string newDescription = message.Text;

                            if (userTaskIds.TryGetValue(userId, out int taskId))
                            {
                               
                                Console.WriteLine($"Найденный taskId: {taskId}");
                                await UpdateTask(taskId, newDescription, userId);
                                await botClient.SendTextMessageAsync(chatId, "Задача обновлена!");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId, "Произошла ошибка при обновлении задачи, попробуйте ещё");
                            }

                            userTaskIds.Remove(userId); // Очищаем userrTaskIds
                            userStates[userId] = UserState.None;
                            break;

                    }
                
                    

                    username = update.Message.From.Username;
                    //await botClient.SendTextMessageAsync(message.Chat, "Сообщение получено " + username);
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

                    
                        async Task AddTask(string description, int userId)
                        {
                            using (var connection = new NpgsqlConnection(connectionString))
                            {
                                await connection.OpenAsync();

                                var cmd = new NpgsqlCommand("INSERT INTO tasks (Description, UserId) VALUES (@description, @userId)", connection);
                                cmd.Parameters.AddWithValue("description", description);
                                cmd.Parameters.AddWithValue("userId", userId);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        
                        async Task<List<TaskModel>> GetTasks(int userId)
                        {
                            var tasks = new List<TaskModel>(); 

                            using (var connection = new NpgsqlConnection(connectionString))
                            {
                                await connection.OpenAsync();
                                var cmd = new NpgsqlCommand("SELECT taskid, description FROM tasks WHERE userid = @userid", connection);
                                cmd.Parameters.AddWithValue("userid", userId);

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        // Создание нового объекта TaskModel и заполнение его свойств
                                        var task = new TaskModel
                                        {
                                            TaskId = reader.GetInt32(reader.GetOrdinal("taskid")), // 
                                            Description = reader.GetString(reader.GetOrdinal("description")),
                                        };
                                        tasks.Add(task);
                                    }
                                }
                            }

                            return tasks;
                        }
                        
                        async Task DisplayTasks(ITelegramBotClient botClient, long chatId, int userId)
                        {
                            var tasks = await GetTasks(userId);

                            if (tasks.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(chatId, "У вас нет задач.");
                                return;
                            }

                            var messageBuilder = new StringBuilder("Ваши задачи:\n");
                            foreach (var task in tasks)
                            {
                                messageBuilder.AppendLine($"ID: {task.TaskId}, Описание: {task.Description}");
                            }

                            await botClient.SendTextMessageAsync(chatId, messageBuilder.ToString());
                        }
                        
                        async Task UpdateTask(int taskid, string description, int userid)
                        {
                            using (var connection = new NpgsqlConnection(connectionString))
                            {
                                await connection.OpenAsync();

                                var cmd = new NpgsqlCommand("UPDATE tasks SET description = @description WHERE taskid = @taskid", connection);
                                cmd.Parameters.AddWithValue("description", description);
                                cmd.Parameters.AddWithValue("taskid", taskid);
                                cmd.Parameters.AddWithValue("userId", userid);

                                await cmd.ExecuteNonQueryAsync();
                                
                               
                                
                            }
                        }
                        
                        
                        async Task DeleteTask(int taskid)
                        {
                            using (var connection = new NpgsqlConnection(connectionString))
                            {
                                await connection.OpenAsync();
                                var cmd = new NpgsqlCommand("DELETE FROM tasks WHERE taskid = @taskid", connection);
                                cmd.Parameters.AddWithValue("taskid", taskid);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        async Task<bool> IsUsersTask(int taskid,int userid)
                        {
                            using (var connection = new NpgsqlConnection(connectionString)) 
                            {
                                await connection.OpenAsync();

                                var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM tasks WHERE taskid = @taskid AND userid = @userid", connection);
                                cmd.Parameters.AddWithValue("taskid", taskid);
                                cmd.Parameters.AddWithValue("userid", userid);

                                var count = (long)await cmd.ExecuteScalarAsync();
                                return count > 0;
                            }
                            
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
            var connection = new NpgsqlConnection(connectionString);
            if(connection != null){Console.WriteLine("Connection successful");}
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