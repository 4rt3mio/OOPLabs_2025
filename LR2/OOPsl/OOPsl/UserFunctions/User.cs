using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Managers;

namespace OOPsl.UserFunctions
{
    public abstract class User : IObserver
    {
        public string Name { get; set; }
        public List<Document> OwnedDocuments { get; set; } = new List<Document>();
        public List<Document> EditableDocuments { get; set; } = new List<Document>();

        public List<string> Notifications { get; private set; } = new List<string>();

        public User(string name)
        {
            Name = name;
        }

        public abstract Document CreateDocument(string fileName, string initialContent);

        public abstract void DeleteDocument(Document document, DocumentManager documentManager);

        public abstract void ChangeDocumentUserRole(Document document, User targetUser, DocumentRole newRole, DocumentAccessManager accessManager);

        public abstract void OpenDocument(Document document);

        public virtual void Update(Document document)
        {
            Notifications.Add($"Документ \"{document.FileName}\" был обнавлён.");
        }
    }
}