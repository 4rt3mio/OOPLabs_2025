using OOPsl.DocumentFunctions.Managers;

namespace OOPsl.MenuFunctions
{
    public class ConsoleMenu : IMenu
    {
        private UserManager userManager;
        private DocumentManager documentManager;

        public ConsoleMenu(UserManager userManager, DocumentManager documentManager)
        {
            this.userManager = userManager;
            this.documentManager = documentManager;
        }

        public void Display()
        {
            Console.WriteLine("=== Главное меню ===");
            Console.WriteLine("1. Показать текущих пользователей");
            Console.WriteLine("2. Создать нового пользователя");
            Console.WriteLine("3. Выход");
            Console.Write("Выберите пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayUsers();
                    break;
                case "2":
                    CreateUser();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }

        public void DisplayUsers()
        {
            Console.WriteLine("=== Список пользователей ===");
            var users = userManager.GetUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("Пользователей не найдено.");
            }
            else
            {
                foreach (var user in users)
                {
                    Console.WriteLine(user.Name);
                }
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню...");
            Console.ReadKey();
        }

        public void CreateUser()
        {
            Console.Write("Введите имя нового пользователя: ");
            string newUserName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                Console.WriteLine("Имя не может быть пустым.");
            }
            else
            {
                User newUser = new RegularUser(newUserName);
                userManager.AddUser(newUser);
                Console.WriteLine($"Пользователь \"{newUserName}\" успешно создан.");
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню...");
            Console.ReadKey();
        }
    }
}
