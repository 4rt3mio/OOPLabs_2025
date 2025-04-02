using OOPsl.DocumentFunctions.Managers;
using OOPsl.UserFunctions;

namespace OOPsl.MenuFunctions
{
    public class ConsoleMenu : IMenu
    {
        private UserManager userManager;
        private DocumentManager documentManager;
        private DocumentAccessManager accessManager;

        public ConsoleMenu(UserManager userManager, DocumentManager documentManager, DocumentAccessManager accessManager)
        {
            this.userManager = userManager;
            this.documentManager = documentManager;
            this.accessManager = accessManager;
        }

        public int Display()
        {
            Console.Clear();
            Console.WriteLine("=== Главное меню пользователей ===");
            Console.WriteLine("1. Показать текущих пользователей");
            Console.WriteLine("2. Создать нового пользователя");
            Console.WriteLine("3. Выбрать пользователя");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите пункт: ");
            if (int.TryParse(Console.ReadLine(), out int option))
            {
                return option;
            }
            return -1;
        }

        public void DisplayUsers()
        {
            Console.Clear();
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
            Console.WriteLine("Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        public void CreateUser()
        {
            Console.Clear();
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
                // Добавляем новому пользователю роль Viewer во всех существующих документах
                accessManager.AddUserToAllDocuments(newUser, documentManager.GetAllDocuments());
                Console.WriteLine($"Пользователь \"{newUserName}\" успешно создан.");
            }
        }

        public void SelectUser()
        {
            Console.Clear();
            Console.WriteLine("=== Выбор пользователя ===");
            var users = userManager.GetUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("Пользователей не найдено. Сначала создайте нового пользователя.");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Name}");
            }
            Console.Write("Выберите пользователя по номеру: ");
            if (int.TryParse(Console.ReadLine(), out int userIndex) && userIndex > 0 && userIndex <= users.Count)
            {
                User selectedUser = users[userIndex - 1];
                UserActionsMenu userActionsMenu = new UserActionsMenu(selectedUser, documentManager, accessManager, userManager);
                userActionsMenu.Display();
            }
            else
            {
                Console.WriteLine("Неверный выбор. Нажмите любую клавишу для возврата...");
                Console.ReadKey();
            }
        }
    }
}