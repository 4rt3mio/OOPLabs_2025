using OOPsl.DocumentFunctions.Formats;
using OOPsl.DocumentFunctions.Managers;
using System.Text.Json.Serialization;

namespace OOPsl
{
    public class RegularUser : User
    {
        public RegularUser(string name) : base(name) { }

        public override Document CreateDocument(string fileName, string initialContent)
        {
            // Здесь можно добавить логику выбора типа документа.
            return new MarkdownDocument { FileName = fileName, Content = initialContent };
        }

        public override void DeleteDocument(Document document, DocumentManager documentManager)
        {
            // Удалять документ можно только если пользователь является его владельцем (Admin).
            if (!OwnedDocuments.Contains(document))
            {
                throw new UnauthorizedAccessException("Удалять можно только документы, которыми вы владеете.");
            }
            document.Delete();
            // Дополнительно можно удалить документ из DocumentManager.
        }

        public override void ChangeDocumentUserRole(Document document, User targetUser, DocumentRole newRole, DocumentAccessManager accessManager)
        {
            accessManager.SetUserRole(document, targetUser, newRole, this);
        }

        public override void OpenDocument(Document document)
        {
            document.Open();
        }
    }
}
