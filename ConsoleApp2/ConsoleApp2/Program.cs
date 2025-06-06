using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    private static List<User> users = new List<User>();
    private static User currentUser = null;
    private const string UsersFile = "users.txt";
    private const string TasksFilePrefix = "tasks_";

    public static async Task Main()
    {
        Console.WriteLine("Добро пожаловать в систему управления задачами!");
        await LoadUsersAsync();

        while (true)
        {
            if (currentUser == null)
            {
                await ShowAuthMenuAsync();
            }
            else
            {
                await ShowMainMenuAsync();
            }
        }
    }

    private static async Task ShowAuthMenuAsync()
    {
        Console.WriteLine("\n1. Войти");
        Console.WriteLine("2. Зарегистрироваться");
        Console.WriteLine("3. Выйти");
        Console.Write("Выберите действие: ");

        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            switch (choice)
            {
                case 1:
                    await LoginAsync();
                    break;
                case 2:
                    await RegisterAsync();
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Неверный ввод.");
        }
    }

    private static async Task ShowMainMenuAsync()
    {
        Console.WriteLine($"\nДобро пожаловать, {currentUser.Username}!");
        Console.WriteLine("1. Просмотреть задачи");
        Console.WriteLine("2. Добавить задачу");
        Console.WriteLine("3. Редактировать задачу");
        Console.WriteLine("4. Удалить задачу");
        Console.WriteLine("5. Выйти из системы");
        Console.Write("Выберите действие: ");

        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            switch (choice)
            {
                case 1:
                    await ViewTasksAsync();
                    break;
                case 2:
                    await AddTaskAsync();
                    break;
                case 3:
                    await EditTaskAsync();
                    break;
                case 4:
                    await DeleteTaskAsync();
                    break;
                case 5:
                    currentUser = null;
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Неверный ввод.");
        }
    }

    private static async Task LoadUsersAsync()
    {
        try
        {
            if (File.Exists(UsersFile))
            {
                var lines = await File.ReadAllLinesAsync(UsersFile);
                users = lines.Select(line =>
                {
                    var parts = line.Split('|');
                    return new User(parts[0], parts[1]);
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке пользователей: {ex.Message}");
        }
    }

    private static async Task SaveUsersAsync()
    {
        try
        {
            var lines = users.Select(u => $"{u.Username}|{u.Password}");
            await File.WriteAllLinesAsync(UsersFile, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении пользователей: {ex.Message}");
        }
    }

    private static async Task RegisterAsync()
    {
        Console.Write("Введите имя пользователя: ");
        string username = Console.ReadLine();

        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Пользователь с таким именем уже существует.");
            return;
        }

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        users.Add(new User(username, password));
        await SaveUsersAsync();

        Console.WriteLine("Регистрация успешно завершена.");
    }

    private static async Task LoginAsync()
    {
        Console.Write("Введите имя пользователя: ");
        string username = Console.ReadLine();

        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            currentUser = user;
            Console.WriteLine("Вход выполнен успешно.");
        }
        else
        {
            Console.WriteLine("Неверное имя пользователя или пароль.");
        }
    }

    private static string GetTasksFileName()
    {
        return $"{TasksFilePrefix}{currentUser.Username}.txt";
    }

    private static async Task<List<TaskItem>> LoadTasksAsync()
    {
        var tasks = new List<TaskItem>();
        var fileName = GetTasksFileName();

        try
        {
            if (File.Exists(fileName))
            {
                var lines = await File.ReadAllLinesAsync(fileName);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        tasks.Add(new TaskItem
                        {
                            Title = parts[0],
                            Description = parts[1],
                            Priority = (Priority)Enum.Parse(typeof(Priority), parts[2]),
                            Status = (Status)Enum.Parse(typeof(Status), parts[3])
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
        }

        return tasks;
    }

    private static async Task SaveTasksAsync(List<TaskItem> tasks)
    {
        var fileName = GetTasksFileName();

        try
        {
            var lines = tasks.Select(t => $"{t.Title}|{t.Description}|{t.Priority}|{t.Status}");
            await File.WriteAllLinesAsync(fileName, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
        }
    }

    private static async Task ViewTasksAsync()
    {
        var tasks = await LoadTasksAsync();

        if (tasks.Count == 0)
        {
            Console.WriteLine("У вас нет задач.");
            return;
        }

        Console.WriteLine("\nСписок задач:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
        }
    }

    private static async Task AddTaskAsync()
    {
        Console.Write("Введите заголовок задачи: ");
        string title = Console.ReadLine();

        Console.Write("Введите описание задачи: ");
        string description = Console.ReadLine();

        Console.WriteLine("Выберите приоритет:");
        foreach (Priority priority in Enum.GetValues(typeof(Priority)))
        {
            Console.WriteLine($"{(int)priority}. {priority}");
        }
        Priority selectedPriority = (Priority)int.Parse(Console.ReadLine());

        Console.WriteLine("Выберите статус:");
        foreach (Status status in Enum.GetValues(typeof(Status)))
        {
            Console.WriteLine($"{(int)status}. {status}");
        }
        Status selectedStatus = (Status)int.Parse(Console.ReadLine());

        var tasks = await LoadTasksAsync();
        tasks.Add(new TaskItem
        {
            Title = title,
            Description = description,
            Priority = selectedPriority,
            Status = selectedStatus
        });

        await SaveTasksAsync(tasks);
        Console.WriteLine("Задача успешно добавлена.");
    }

    private static async Task EditTaskAsync()
    {
        var tasks = await LoadTasksAsync();

        if (tasks.Count == 0)
        {
            Console.WriteLine("У вас нет задач для редактирования.");
            return;
        }

        await ViewTasksAsync();
        Console.Write("Выберите номер задачи для редактирования: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
        {
            var task = tasks[taskNumber - 1];

            Console.Write("Введите новый заголовок (оставьте пустым, чтобы не изменять): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTitle))
            {
                task.Title = newTitle;
            }

            Console.Write("Введите новое описание (оставьте пустым, чтобы не изменять): ");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription))
            {
                task.Description = newDescription;
            }

            Console.WriteLine("Выберите новый приоритет (оставьте пустым, чтобы не изменять):");
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                Console.WriteLine($"{(int)priority}. {priority}");
            }
            string priorityInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(priorityInput) && Enum.TryParse(priorityInput, out Priority newPriority))
            {
                task.Priority = newPriority;
            }

            Console.WriteLine("Выберите новый статус (оставьте пустым, чтобы не изменять):");
            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                Console.WriteLine($"{(int)status}. {status}");
            }
            string statusInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(statusInput) && Enum.TryParse(statusInput, out Status newStatus))
            {
                task.Status = newStatus;
            }

            await SaveTasksAsync(tasks);
            Console.WriteLine("Задача успешно обновлена.");
        }
        else
        {
            Console.WriteLine("Неверный номер задачи.");
        }
    }

    private static async Task DeleteTaskAsync()
    {
        var tasks = await LoadTasksAsync();

        if (tasks.Count == 0)
        {
            Console.WriteLine("У вас нет задач для удаления.");
            return;
        }

        await ViewTasksAsync();
        Console.Write("Выберите номер задачи для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count)
        {
            tasks.RemoveAt(taskNumber - 1);
            await SaveTasksAsync(tasks);
            Console.WriteLine("Задача успешно удалена.");
        }
        else
        {
            Console.WriteLine("Неверный номер задачи.");
        }
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class TaskItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }

    public override string ToString()
    {
        return $"{Title} ({Priority} приоритет) - {Status}\n   {Description}";
    }
}

public enum Priority
{
    Низкий = 1,
    Средний = 2,
    Высокий = 3
}

public enum Status
{
    Не_начата = 1,
    В_процессе = 2,
    Завершена = 3
}