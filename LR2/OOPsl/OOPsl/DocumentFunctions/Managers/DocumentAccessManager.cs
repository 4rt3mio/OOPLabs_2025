namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentAccessManager
    {
        private Dictionary<Document, List<AccessControlEntry>> accessMapping;

        // Конструктор по умолчанию. Здесь можно выполнить начальную инициализацию.
        public DocumentAccessManager()
        {
            accessMapping = new Dictionary<Document, List<AccessControlEntry>>();
        }

        // Добавление нового документа с первоначальным доступом: создатель становится Admin.
        public void AddDocument(Document document, User creator)
        {
            accessMapping[document] = new List<AccessControlEntry>
            {
                new AccessControlEntry(creator, DocumentRole.Admin)
            };
        }

        // Метод для изменения роли пользователя для конкретного документа.
        // Доступно только если вызывающий пользователь имеет роль Admin для данного документа.
        public void SetUserRole(Document document, User targetUser, DocumentRole newRole, User actionUser)
        {
            if (!accessMapping.ContainsKey(document) ||
                !accessMapping[document].Any(ace => ace.User == actionUser && ace.Role == DocumentRole.Admin))
            {
                throw new UnauthorizedAccessException("Только администратор документа может изменять роли.");
            }

            var entry = accessMapping[document].FirstOrDefault(ace => ace.User == targetUser);
            if (entry != null)
            {
                entry.Role = newRole;
            }
            else
            {
                accessMapping[document].Add(new AccessControlEntry(targetUser, newRole));
            }
        }

        // Получение списка доступа для документа.
        public List<AccessControlEntry> GetAccessList(Document document)
        {
            if (accessMapping.ContainsKey(document))
            {
                return accessMapping[document];
            }
            return new List<AccessControlEntry>();
        }
    }
}