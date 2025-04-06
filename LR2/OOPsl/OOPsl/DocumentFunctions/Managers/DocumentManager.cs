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
            int version = document.VersionHistory.Count + 1;
            string baseName = Path.GetFileNameWithoutExtension(document.FileName);
            string ext = Path.GetExtension(document.FileName);
            string newFileName = $"{baseName}_v{version}{ext}";
            string fullPath = Path.Combine(documentsFolder, newFileName);
            document.FileName = fullPath;
            document.VersionHistory.Add(fullPath);
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

        public void RemoveDocument(Document document)
        {
            documents.Remove(document);
            if (File.Exists(document.FileName))
            {
                File.Delete(document.FileName);
            }
        }
        private void LoadDocumentsFromStorage(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            var groups = files.GroupBy(f => GetBaseName(f)).ToList();

            foreach (var group in groups)
            {
                var sortedFiles = group.OrderBy(f => GetVersionNumber(f)).ToList();
                string latestFile = sortedFiles.Last();
                string ext = Path.GetExtension(latestFile).ToLower();
                Document doc = null;
                if (ext == ".txt")
                {
                    doc = new PlainTextDocument(latestFile);
                }
                else if (ext == ".md")
                {
                    doc = new MarkdownDocument(latestFile);
                }
                else if (ext == ".rtf")
                {
                    doc = new RichTextDocument(latestFile);
                }
                if (doc != null)
                {
                    try
                    {
                        doc.Content = File.ReadAllText(latestFile);
                        doc.VersionHistory = sortedFiles.ToList();
                        documents.Add(doc);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки файла {latestFile}: {ex.Message}");
                    }
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

        private string GetBaseName(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int index = fileName.LastIndexOf("_v");
            if (index > 0)
            {
                return fileName.Substring(0, index);
            }
            return fileName;
        }

        private int GetVersionNumber(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            int index = fileName.LastIndexOf("_v");
            if (index > 0 && int.TryParse(fileName.Substring(index + 2), out int version))
            {
                return version;
            }
            return 1;
        }
    }
}