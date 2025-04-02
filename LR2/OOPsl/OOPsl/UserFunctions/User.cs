using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Managers;

namespace OOPsl.UserFunctions
{
    public abstract class User : IObserver
    {
        public string Name { get; set; }
        public List<Document> OwnedDocuments { get; set; } = new List<Document>();
        public List<Document> EditableDocuments { get; set; } = new List<Document>();

        public User(string name)
        {
            Name = name;
        }

        public abstract Document CreateDocument(string fileName, string initialContent);

        // Возможность удаления документа, которым пользователь владеет.
        public abstract void DeleteDocument(Document document, DocumentManager documentManager);

        // Изменение прав доступа к документу (выдача роли Editor и т.д.).
        public abstract void ChangeDocumentUserRole(Document document, User targetUser, DocumentRole newRole, DocumentAccessManager accessManager);

        // Метод для открытия документа.
        public abstract void OpenDocument(Document document);

        // Получение уведомлений об изменениях в документе.
        public void Update(Document document) { }
    }
}