using OOPsl.DocumentFunctions.Formats;
using OOPsl.DocumentFunctions.Storage;
using OOPsl.UserFunctions;

namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentManager
    {
        private List<Document> documents = new List<Document>();
        private DocumentAccessManager accessManager;
        // Абсолютный путь к локальному хранилищу документов
        private string documentsFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\LocalFiles";

        public DocumentManager(DocumentAccessManager accessManager)
        {
            this.accessManager = accessManager;
            if (!Directory.Exists(documentsFolder))
            {
                Directory.CreateDirectory(documentsFolder);
            }
            LoadDocumentsFromStorage(documentsFolder);
        }

        // Создание нового документа.
        // allUsers – список всех пользователей, для установки доступа.
        public void CreateDocument(Document document, User creator, List<User> allUsers)
        {
            document.Create();
            documents.Add(document);
            accessManager.AddDefaultAccess(document, creator, allUsers);
            creator.OwnedDocuments.Add(document);

            // Сохраняем документ локально
            IStorageStrategy localStorage = new Storage.LocalFileStorage();
            localStorage.Save(document);
        }

        // Сохранение новой версии документа.
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

        public void RemoveDocument(Document document)
        {
            documents.Remove(document);
            if (File.Exists(document.FileName))
            {
                File.Delete(document.FileName);
            }
        }

        // Метод загрузки всех документов из указанной папки
        private void LoadDocumentsFromStorage(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            // Группируем файлы по базовому имени (без версии) и расширению
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
                    // Предполагается, что у вас есть класс RichTextDocument
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