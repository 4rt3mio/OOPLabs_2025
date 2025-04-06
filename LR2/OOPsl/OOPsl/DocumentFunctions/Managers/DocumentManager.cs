using OOPsl.DocumentFunctions.Formats;
using OOPsl.DocumentFunctions.Storage;
using OOPsl.UserFunctions;
using System.Reflection.Metadata;

namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentManager
    {
        private List<Document> documents = new List<Document>();
        private DocumentAccessManager accessManager;
        private string documentsFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\LocalFiles";

        public DocumentManager(DocumentAccessManager accessManager)
        {
            this.accessManager = accessManager;
            if (!Directory.Exists(documentsFolder))
            {
                Directory.CreateDirectory(documentsFolder);
            }
            LoadDocumentsFromStorage(documentsFolder);
            LoadDocumentsFromCloud();
        }

        public void CreateDocument(Document document, User creator, List<User> allUsers)
        {
            document.Create();
            documents.Add(document);
            accessManager.AddDefaultAccess(document, creator, allUsers);
            creator.OwnedDocuments.Add(document);

            IStorageStrategy localStorage = new LocalFileStorage();
            localStorage.Save(document);
        }

        public void SaveDocument(Document document, IStorageStrategy storageStrategy)
        {
            document.VersionHistory.Add(document.Content);
            storageStrategy.Save(document);
        }

        public Document LoadDocument(string fileName)
        {
            return documents.Find(d => d.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        public List<Document> GetAllDocuments()
        {
            return documents;
        }

        public List<Document> GetLocalDocuments()
        {
            return documents.Where(d => d.FileName.StartsWith(documentsFolder, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Document> GetCloudDocuments()
        {
            return documents.Where(d => !d.FileName.StartsWith(documentsFolder, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void RemoveDocument(Document document, IStorageStrategy storageStrategy)
        {
            documents.Remove(document);
            storageStrategy.Delete(document);
        }

        private void LoadDocumentsFromStorage(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                try
                {
                    string content = File.ReadAllText(file);
                    Document doc = new PlainTextDocument(file);
                    doc.Content = content;
                    documents.Add(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки файла {file}: {ex.Message}");
                }
            }
        }

        private void LoadDocumentsFromCloud()
        {
            try
            {
                GoogleDriveStorage driveStorage = new GoogleDriveStorage();
                var driveDocs = driveStorage.GetAllDocumentsFromDrive();
                if (driveDocs != null && driveDocs.Count > 0)
                {
                    documents.AddRange(driveDocs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки файлов из облака: " + ex.Message);
            }
        }
    }
}