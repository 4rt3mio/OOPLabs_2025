using Newtonsoft.Json;
using OOPsl.UserFunctions;
using System.Xml;

namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentAccessManager
    {
        // Ключ – логическое имя документа (базовое имя без суффикса версии),
        // значение – список записей доступа.
        private Dictionary<string, List<AccessControlEntry>> accessMapping = new Dictionary<string, List<AccessControlEntry>>();
        private const string AccessDataFile = @"D:\OOP\LR2\OOPsl\OOPsl\Files\access_manager.json";

        public DocumentAccessManager()
        {
            LoadAccessData();
        }

        // Вычисляем ключ для документа (базовое имя без версии)
        private string GetKey(Document document)
        {
            string fileName = Path.GetFileNameWithoutExtension(document.FileName);
            int index = fileName.LastIndexOf("_v");
            return index > 0 ? fileName.Substring(0, index) : fileName;
        }

        // Добавляем документ с доступом: создатель получает роль Admin, остальные – Viewer.
        public void AddDefaultAccess(Document document, User creator, IEnumerable<User> allUsers)
        {
            string docKey = GetKey(document);
            var list = new List<AccessControlEntry>
            {
                new AccessControlEntry(creator, DocumentRole.Admin)
            };

            foreach (var user in allUsers)
            {
                if (!user.Name.Equals(creator.Name, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new AccessControlEntry(user, DocumentRole.Viewer));
                }
            }

            accessMapping[docKey] = list;
            SaveAccessData();
        }

        // Метод для изменения роли пользователя для конкретного документа.
        public void SetUserRole(Document document, User targetUser, DocumentRole newRole, User actionUser)
        {
            string docKey = GetKey(document);
            if (!accessMapping.ContainsKey(docKey) ||
                !accessMapping[docKey].Any(ace => ace.User.Name.Equals(actionUser.Name, StringComparison.OrdinalIgnoreCase) && ace.Role == DocumentRole.Admin))
            {
                throw new UnauthorizedAccessException("Только администратор документа может изменять роли.");
            }

            var entry = accessMapping[docKey].FirstOrDefault(ace => ace.User.Name.Equals(targetUser.Name, StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                entry.Role = newRole;
            }
            else
            {
                accessMapping[docKey].Add(new AccessControlEntry(targetUser, newRole));
            }
            SaveAccessData();
        }

        // Возвращает список записей доступа для указанного документа.
        public List<AccessControlEntry> GetAccessList(Document document)
        {
            string docKey = GetKey(document);
            return accessMapping.ContainsKey(docKey) ? accessMapping[docKey] : new List<AccessControlEntry>();
        }

        // Добавляет нового пользователя ко всем существующим документам с ролью Viewer, если записи ещё нет.
        public void AddUserToAllDocuments(User newUser, IEnumerable<Document> allDocuments)
        {
            foreach (var doc in allDocuments)
            {
                string docKey = GetKey(doc);
                if (!accessMapping.ContainsKey(docKey))
                {
                    accessMapping[docKey] = new List<AccessControlEntry>();
                }
                if (!accessMapping[docKey].Any(ace => ace.User.Name.Equals(newUser.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    accessMapping[docKey].Add(new AccessControlEntry(newUser, DocumentRole.Viewer));
                }
            }
            SaveAccessData();
        }

        // Сохранение данных доступа в JSON-файл.
        public void SaveAccessData()
        {
            try
            {
                string dir = Path.GetDirectoryName(AccessDataFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // Преобразуем каждую запись доступа в объект с полями Name и Role.
                var serializableMapping = accessMapping.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(ace => new { Name = ace.User.Name, ace.Role }).ToList()
                );
                string json = JsonConvert.SerializeObject(serializableMapping, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(AccessDataFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сохранения данных доступа: " + ex.Message);
            }
        }

        // Загрузка данных доступа из JSON-файла.
        public void LoadAccessData()
        {
            if (File.Exists(AccessDataFile))
            {
                try
                {
                    string json = File.ReadAllText(AccessDataFile);
                    var tempMapping = JsonConvert.DeserializeObject<Dictionary<string, List<TempAce>>>(json)
                                      ?? new Dictionary<string, List<TempAce>>();
                    // Преобразуем во внутреннее представление, создавая DummyUser.
                    accessMapping = tempMapping.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Select(ace => new AccessControlEntry(new DummyUser(ace.Name), ace.Role)).ToList()
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка загрузки данных доступа: " + ex.Message);
                    accessMapping = new Dictionary<string, List<AccessControlEntry>>();
                }
            }
        }

        // Метод для восстановления привязки документов к реальным объектам пользователей.
        public void RestoreUserDocuments(UserManager userManager, DocumentManager documentManager)
        {
            foreach (var kvp in accessMapping)
            {
                string docKey = kvp.Key;
                Document doc = documentManager.GetAllDocuments()
                                  .FirstOrDefault(d => GetKey(d).Equals(docKey, StringComparison.OrdinalIgnoreCase));
                if (doc == null) continue;

                foreach (var ace in kvp.Value)
                {
                    User realUser = userManager.FindUser(ace.User.Name);
                    if (realUser != null)
                    {
                        ace.User = realUser;
                        if (ace.Role == DocumentRole.Admin)
                        {
                            if (!realUser.OwnedDocuments.Contains(doc))
                                realUser.OwnedDocuments.Add(doc);
                        }
                        else if (ace.Role == DocumentRole.Editor)
                        {
                            if (!realUser.EditableDocuments.Contains(doc))
                                realUser.EditableDocuments.Add(doc);
                        }
                    }
                }
            }
        }

        // Вспомогательный класс для десериализации.
        private class TempAce
        {
            public string Name { get; set; }
            public DocumentRole Role { get; set; }
        }
    }

    public class DummyUser : User
    {
        public DummyUser(string name) : base(name) { }
        public override Document CreateDocument(string fileName, string initialContent) { return null; }
        public override void DeleteDocument(Document document, DocumentManager documentManager) { }
        public override void ChangeDocumentUserRole(Document document, User targetUser, DocumentRole newRole, DocumentAccessManager accessManager) { }
        public override void OpenDocument(Document document) { }
    }
}