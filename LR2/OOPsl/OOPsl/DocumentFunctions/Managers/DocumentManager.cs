using Newtonsoft.Json;
using OOPsl.DocumentFunctions.Storage;
using OOPsl.UserFunctions;

namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentManager
    {
        private List<Document> documents = new List<Document>();
        private DocumentAccessManager accessManager;
        private readonly string documentsFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\LocalFiles";
        private readonly string historyFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\LocalHistory";

        public DocumentManager(DocumentAccessManager accessManager)
        {
            this.accessManager = accessManager;
            Directory.CreateDirectory(documentsFolder);
            Directory.CreateDirectory(historyFolder);

            LoadDocumentsFromStorage(documentsFolder);
            LoadDocumentsFromCloud();
        }

        public void CreateDocument(Document document, User creator, List<User> allUsers)
        {
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
            storageStrategy.DeleteHistory(document);
        }

        private void LoadDocumentsFromStorage(string folderPath)
        {
            IStorageStrategy localStorage = new LocalFileStorage();
            string[] files = Directory.GetFiles(folderPath);

            foreach (var file in files)
            {
                try
                {
                    string content = File.ReadAllText(file);
                    Document doc = new Document()
                    {
                        Content = content,
                        FileName = file
                    };
                    doc.VersionHistory = localStorage.LoadHistory(doc);
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

                foreach (var doc in driveDocs)
                {
                    doc.VersionHistory = driveStorage.LoadHistory(doc);
                    documents.Add(doc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки файлов из облака: " + ex.Message);
            }
        }
    }
}