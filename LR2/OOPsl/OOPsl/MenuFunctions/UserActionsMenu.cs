using OOPsl.DocumentFunctions.Managers;
using OOPsl.UserFunctions;
using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Formats;
using System.Reflection.Metadata;
using Document = OOPsl.DocumentFunctions.Document;
using OOPsl.DocumentFunctions.Storage;
using System.Xml.Linq;

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
                Console.WriteLine("5. Посмотреть историю файл");
                Console.WriteLine("6. Изменить роли для файла");
                Console.WriteLine("7. Показать уведомления");
                Console.WriteLine("8. Вернуться к выбору пользователя");
                Console.WriteLine("9. Выход из приложения");
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
                        ViewDocumentHistory();
                        break;
                    case "6":
                        ChangeRolesForFile();
                        break;
                    case "7":
                        ShowNotifications();
                        break;
                    case "8":
                        exitMenu = true;
                        break;
                    case "9":
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

            documentManager.CreateDocument(newDoc, currentUser, userManager.GetUsers());
            foreach (var user in userManager.GetUsers()) 
            {
                newDoc.Attach(user);
            }
            Console.WriteLine($"Документ \"{fileName}\" успешно создан.");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void ShowAllFiles()
        {
            Console.Clear();
            var localDocs = documentManager.GetLocalDocuments();
            var cloudDocs = documentManager.GetCloudDocuments();

            Console.WriteLine("=== Локальные документы ===");
            if (localDocs.Count == 0)
            {
                Console.WriteLine("Нет локальных документов.");
            }
            else
            {
                foreach (var doc in localDocs)
                {
                    Console.WriteLine($"Файл: {doc.FileName}");
                    var accessList = accessManager.GetAccessList(doc);
                    if (accessList.Count == 0)
                        Console.WriteLine("  Роли не назначены.");
                    else
                    {
                        foreach (var ace in accessList)
                            Console.WriteLine($"  Пользователь: {ace.User.Name}, Роль: {ace.Role}");
                    }
                }
            }
            Console.WriteLine("\n=== Документы с Google Drive ===");
            if (cloudDocs.Count == 0)
            {
                Console.WriteLine("Нет документов из облака.");
            }
            else
            {
                foreach (var doc in cloudDocs)
                {
                    Console.WriteLine($"Файл: {doc.FileName}");
                    var accessList = accessManager.GetAccessList(doc);
                    if (accessList.Count == 0)
                        Console.WriteLine("  Роли не назначены.");
                    else
                    {
                        foreach (var ace in accessList)
                            Console.WriteLine($"  Пользователь: {ace.User.Name}, Роль: {ace.Role}");
                    }
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void OpenFile()
        {
            Console.Clear();
            Console.WriteLine("Выберите источник файла:");
            Console.WriteLine("1. Локальные файлы");
            Console.WriteLine("2. Google Drive");
            Console.Write("Ваш выбор: ");
            string sourceChoice = Console.ReadLine();

            List<Document> docs;
            if (sourceChoice == "1")
            {
                docs = documentManager.GetLocalDocuments();
            }
            else if (sourceChoice == "2")
            {
                docs = documentManager.GetCloudDocuments();
            }
            else
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите имя файла для открытия: ");
            string inputName = Console.ReadLine();
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
                editor.Run();
                Console.WriteLine("Сохранить файл:");
                Console.WriteLine("1. Локально");
                Console.WriteLine("2. В облако (Google Drive)");
                Console.Write("Выберите опцию (1 или 2): ");
                var key = Console.ReadKey();
                Console.WriteLine();
                IStorageStrategy storageStrategy = null;
                if (key.KeyChar == '1')
                    storageStrategy = new LocalFileStorage();
                else if (key.KeyChar == '2')
                    storageStrategy = new GoogleDriveStorage();
                else
                {
                    Console.WriteLine("Неверный выбор. Сохранение отменено.");
                    Console.WriteLine("Нажмите любую клавишу для возврата...");
                    Console.ReadKey();
                    return;
                }
                documentManager.SaveDocument(docToOpen, storageStrategy);
                docToOpen.Notify();
                Console.WriteLine("Файл сохранён. Нажмите любую клавишу для возврата...");
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
            IStorageStrategy storageStrategy;
            Console.Clear();
            Console.WriteLine("Выберите источник файла для удаления:");
            Console.WriteLine("1. Локальные документы");
            Console.WriteLine("2. Документы с Google Drive");
            string sourceChoice = Console.ReadLine();
            List<Document> docs;
            if (sourceChoice == "1")
            {
                docs = documentManager.GetLocalDocuments();
                storageStrategy = new LocalFileStorage();
            }
            else if (sourceChoice == "2")
            {
                docs = documentManager.GetCloudDocuments();
                storageStrategy = new GoogleDriveStorage();
            }
            else
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите имя файла для удаления: ");
            string inputName = Console.ReadLine();
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
                bool isAdmin = accessList.Any(ace =>
                    ace.User.Name.Equals(currentUser.Name, StringComparison.OrdinalIgnoreCase) &&
                    ace.Role == DocumentRole.Admin);

                if (!isAdmin)
                {
                    Console.WriteLine("Вы не являетесь администратором этого документа, удаление невозможно.");
                }
                else
                {
                    docToDelete.Delete();
                    documentManager.RemoveDocument(docToDelete, storageStrategy);
                    currentUser.OwnedDocuments.Remove(docToDelete);
                    Console.WriteLine($"Документ \"{docToDelete.FileName}\" успешно удалён.");
                }
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void ViewDocumentHistory()
        {
            Console.Clear();
            Console.WriteLine("Выберите источник документа:");
            Console.WriteLine("1. Локальные документы");
            Console.WriteLine("2. Документы с Google Drive");
            string sourceChoice = Console.ReadLine();

            IStorageStrategy storageStrategy;
            List<Document> docs;

            if (sourceChoice == "1")
            {
                storageStrategy = new LocalFileStorage();
                docs = documentManager.GetLocalDocuments();
            }
            else if (sourceChoice == "2")
            {
                storageStrategy = new GoogleDriveStorage();
                docs = documentManager.GetCloudDocuments();
            }
            else
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите имя документа: ");
            string inputName = Console.ReadLine();

            Document doc = docs.FirstOrDefault(d =>
                d.FileName.Equals(inputName, StringComparison.OrdinalIgnoreCase) ||
                System.IO.Path.GetFileName(d.FileName).Equals(inputName, StringComparison.OrdinalIgnoreCase));

            if (doc == null)
            {
                Console.WriteLine("Документ не найден.");
                Console.ReadKey();
                return;
            }

            var history = storageStrategy.LoadHistory(doc);
            if (history == null || history.Count == 0)
            {
                Console.WriteLine("История изменений не найдена.");
            }
            else
            {
                Console.WriteLine($"Всего версий: {history.Count}");
                Console.Write("Введите номер версии для просмотра (1 - самая старая, {0} - последняя): ", history.Count);
                if (int.TryParse(Console.ReadLine(), out int versionNumber) &&
                    versionNumber >= 1 && versionNumber <= history.Count)
                {
                    Console.Clear();
                    Console.WriteLine($"=== Версия {versionNumber} документа \"{doc.FileName}\" ===\n");
                    Console.WriteLine(history[versionNumber - 1]);
                }
                else
                {
                    Console.WriteLine("Некорректный номер версии.");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void ChangeRolesForFile()
        {
            Console.Clear();
            Console.WriteLine("Выберите источник файла для изменения ролей:");
            Console.WriteLine("1. Локальные документы");
            Console.WriteLine("2. Документы с Google Drive");
            string sourceChoice = Console.ReadLine();
            List<Document> docs;
            if (sourceChoice == "1")
                docs = documentManager.GetLocalDocuments();
            else if (sourceChoice == "2")
                docs = documentManager.GetCloudDocuments();
            else
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите имя файла для изменения ролей: ");
            string inputName = Console.ReadLine();
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
                bool isAdmin = accessList.Any(ace =>
                    ace.User.Name.Equals(currentUser.Name, StringComparison.OrdinalIgnoreCase) &&
                    ace.Role == DocumentRole.Admin);

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

        private void ShowNotifications()
        {
            Console.Clear();
            Console.WriteLine("=== Уведомления пользователя ===");

            if (currentUser.Notifications.Count == 0)
            {
                Console.WriteLine("У вас нет новых уведомлений.");
            }
            else
            {
                int count = 1;
                foreach (var notification in currentUser.Notifications)
                {
                    Console.WriteLine($"{count++}. {notification}");
                }
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}