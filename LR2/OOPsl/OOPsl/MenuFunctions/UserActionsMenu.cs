using OOPsl.DocumentFunctions.Managers;
using OOPsl.UserFunctions;
using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Formats;

namespace OOPsl.MenuFunctions
{
    public class UserActionsMenu
    {
        private User currentUser;
        private DocumentManager documentManager;
        private DocumentAccessManager accessManager;
        private UserManager userManager;

        public UserActionsMenu(User user, DocumentManager documentManager, DocumentAccessManager accessManager, UserManager userManager)
        {
            currentUser = user;
            this.documentManager = documentManager;
            this.accessManager = accessManager;
            this.userManager = userManager;
        }

        public void Display()
        {
            bool exitMenu = false;
            while (!exitMenu)
            {
                Console.Clear();
                Console.WriteLine($"=== Действия для пользователя: {currentUser.Name} ===");
                Console.WriteLine("1. Создать новый файл");
                Console.WriteLine("2. Показать все файлы (с ролями)");
                Console.WriteLine("3. Открыть файл");
                Console.WriteLine("4. Удалить файл");
                Console.WriteLine("5. Изменить роли для файла");
                Console.WriteLine("6. Вернуться к выбору пользователя");
                Console.WriteLine("7. Выход из приложения");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateNewFile();
                        break;
                    case "2":
                        ShowAllFiles();
                        break;
                    case "3":
                        OpenFile();
                        break;
                    case "4":
                        DeleteFile();
                        break;
                    case "5":
                        ChangeRolesForFile();
                        break;
                    case "6":
                        exitMenu = true;
                        break;
                    case "7":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для повторного ввода...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void CreateNewFile()
        {
            Console.Clear();
            Console.Write("Введите имя файла: ");
            string fileName = Console.ReadLine();
            Console.Write("1. PlainText(.txt) \n2. RichText(.rtf) \n3. Markdown(.md)\nВведите формат файла: ");
            int format = 0;
            bool validInput = false;

            while (!validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out format);

                if (!validInput || format < 1 || format > 3)
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите 1, 2 или 3.");
                    Console.Write("1. PlainText(.txt) \n2. RichText(.rtf) \n3. Markdown(.md)\nВведите формат файла: ");
                    validInput = false;
                }
            }

            Document newDoc;
            switch (format)
            {
                case 1:
                    newDoc = new PlainTextDocument(fileName);
                    break;
                case 2:
                    newDoc = new RichTextDocument(fileName);
                    break;
                case 3:
                default:
                    newDoc = new MarkdownDocument(fileName);
                    break;
            }

            // При создании файла передаем список всех пользователей для установки доступа.
            documentManager.CreateDocument(newDoc, currentUser, userManager.GetUsers());
            Console.WriteLine($"Документ \"{fileName}\" успешно создан.");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void ShowAllFiles()
        {
            Console.Clear();
            var docs = documentManager.GetAllDocuments();
            if (docs.Count == 0)
            {
                Console.WriteLine("Нет созданных документов.");
            }
            else
            {
                Console.WriteLine("=== Список документов и роли пользователей ===");
                foreach (var doc in docs)
                {
                    Console.WriteLine($"Файл: {doc.FileName}");
                    var accessList = accessManager.GetAccessList(doc);
                    if (accessList.Count == 0)
                    {
                        Console.WriteLine("  Роли не назначены.");
                    }
                    else
                    {
                        foreach (var ace in accessList)
                        {
                            Console.WriteLine($"  Пользователь: {ace.User.Name}, Роль: {ace.Role}");
                        }
                    }
                }
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void OpenFile()
        {
            Console.Clear();
            Console.Write("Введите имя файла для открытия: ");
            string inputName = Console.ReadLine();

            var docs = documentManager.GetAllDocuments();
            // Ищем документ, сравнивая абсолютное имя или базовое имя файла
            Document docToOpen = docs.FirstOrDefault(d =>
                d.FileName.Equals(inputName, StringComparison.OrdinalIgnoreCase) ||
                System.IO.Path.GetFileName(d.FileName).Equals(inputName, StringComparison.OrdinalIgnoreCase));

            if (docToOpen == null)
            {
                Console.WriteLine("Документ не найден.");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            // Проверяем права текущего пользователя для данного документа
            var accessList = accessManager.GetAccessList(docToOpen);
            var currentUserAccess = accessList.FirstOrDefault(a =>
                a.User.Name.Equals(currentUser.Name, StringComparison.OrdinalIgnoreCase));

            if (currentUserAccess == null)
            {
                Console.WriteLine("У вас нет доступа к этому документу.");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            if (currentUserAccess.Role == DocumentRole.Admin || currentUserAccess.Role == DocumentRole.Editor)
            {
                Console.WriteLine("Открывается режим редактирования. Нажмите Escape для выхода из редактора.");
                TextEditor editor = new TextEditor(docToOpen);
                editor.Run(); // Внутри TextEditor реализована обработка Escape для выхода из редактора
                Console.WriteLine("Вы вышли из режима редактирования.");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
            }
            else if (currentUserAccess.Role == DocumentRole.Viewer)
            {
                Console.WriteLine("У вас права только на просмотр. Открывается режим просмотра.");
                try
                {
                    string content = System.IO.File.ReadAllText(docToOpen.FileName);
                    Console.Clear();
                    Console.WriteLine(content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                }
                Console.WriteLine("\nНажмите любую клавишу для возврата...");
                Console.ReadKey();
            }
        }

        private void DeleteFile()
        {
            Console.Clear();
            Console.Write("Введите имя файла для удаления: ");
            string inputName = Console.ReadLine();

            var docs = documentManager.GetAllDocuments();
            // Ищем документ по абсолютному или базовому имени
            Document docToDelete = docs.FirstOrDefault(d =>
                d.FileName.Equals(inputName, StringComparison.OrdinalIgnoreCase) ||
                System.IO.Path.GetFileName(d.FileName).Equals(inputName, StringComparison.OrdinalIgnoreCase));

            if (docToDelete == null)
            {
                Console.WriteLine("Документ не найден.");
            }
            else
            {
                var accessList = accessManager.GetAccessList(docToDelete);
                bool isAdmin = accessList.Any(ace => ace.User.Name.Equals(currentUser.Name, StringComparison.OrdinalIgnoreCase) && ace.Role == DocumentRole.Admin);

                if (!isAdmin)
                {
                    Console.WriteLine("Вы не являетесь администратором этого документа, удаление невозможно.");
                }
                else
                {
                    docToDelete.Delete();
                    documentManager.RemoveDocument(docToDelete);
                    currentUser.OwnedDocuments.Remove(docToDelete);
                    Console.WriteLine($"Документ \"{docToDelete.FileName}\" успешно удалён.");
                }
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void ChangeRolesForFile()
        {
            Console.Clear();
            Console.Write("Введите имя файла для изменения ролей: ");
            string inputName = Console.ReadLine();

            var docs = documentManager.GetAllDocuments();
            // Ищем документ по абсолютному или базовому имени
            Document docToChange = docs.FirstOrDefault(d =>
                d.FileName.Equals(inputName, StringComparison.OrdinalIgnoreCase) ||
                System.IO.Path.GetFileName(d.FileName).Equals(inputName, StringComparison.OrdinalIgnoreCase));

            if (docToChange == null)
            {
                Console.WriteLine("Документ не найден.");
            }
            else
            {
                var accessList = accessManager.GetAccessList(docToChange);
                bool isAdmin = accessList.Any(ace => ace.User.Name.Equals(currentUser.Name, StringComparison.OrdinalIgnoreCase) && ace.Role == DocumentRole.Admin);

                if (!isAdmin)
                {
                    Console.WriteLine("Вы не являетесь администратором этого документа, изменение ролей невозможно.");
                }
                else
                {
                    Console.WriteLine("Введите имена пользователей через запятую, для которых нужно поменять роль (Viewer <-> Editor): ");
                    string input = Console.ReadLine();
                    var userNames = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(name => name.Trim());

                    foreach (var userName in userNames)
                    {
                        User targetUser = userManager.FindUser(userName);
                        if (targetUser == null)
                        {
                            Console.WriteLine($"Пользователь \"{userName}\" не найден.");
                        }
                        else
                        {
                            var ace = accessList.FirstOrDefault(a => a.User.Name.Equals(targetUser.Name, StringComparison.OrdinalIgnoreCase));
                            if (ace == null)
                            {
                                Console.WriteLine($"Пользователь \"{userName}\" не имеет доступа к данному документу.");
                            }
                            else if (ace.Role == DocumentRole.Viewer)
                            {
                                accessManager.SetUserRole(docToChange, targetUser, DocumentRole.Editor, currentUser);
                                Console.WriteLine($"Роль пользователя \"{userName}\" изменена с Viewer на Editor.");
                            }
                            else if (ace.Role == DocumentRole.Editor)
                            {
                                accessManager.SetUserRole(docToChange, targetUser, DocumentRole.Viewer, currentUser);
                                Console.WriteLine($"Роль пользователя \"{userName}\" изменена с Editor на Viewer.");
                            }
                            else if (ace.Role == DocumentRole.Admin)
                            {
                                Console.WriteLine($"Пользователь \"{userName}\" является администратором, его роль не изменяется.");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}