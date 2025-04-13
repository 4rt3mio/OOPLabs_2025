using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Managers;
using OOPsl.DocumentFunctions.Storage;
using OOPsl.MenuFunctions;
using OOPsl.UserFunctions;
using System.Reflection;

namespace Tester
{
    public class DuplicateDocumentException : Exception
    {
        public DuplicateDocumentException(string message) : base(message) { }
    }

    public class DocumentOperationsTests : IDisposable
    {
        private readonly string testLocalFilesFolder = Path.Combine(Path.GetTempPath(), "OOPslTestLocalFiles");
        private readonly string testLocalHistoryFolder = Path.Combine(Path.GetTempPath(), "OOPslTestLocalHistory");

        private readonly DocumentAccessManager accessManager;
        private readonly DocumentManager documentManager;
        private readonly UserManager userManager;
        private readonly RegularUser testUser;

        public DocumentOperationsTests()
        {
            Directory.CreateDirectory(testLocalFilesFolder);
            Directory.CreateDirectory(testLocalHistoryFolder);
            accessManager = new DocumentAccessManager();
            userManager = new UserManager();
            documentManager = new DocumentManager(accessManager);
            testUser = new RegularUser("TestUser");
            userManager.AddUser(testUser);
        }

        public void Dispose()
        {
            if (Directory.Exists(testLocalFilesFolder))
                Directory.Delete(testLocalFilesFolder, true);
            if (Directory.Exists(testLocalHistoryFolder))
                Directory.Delete(testLocalHistoryFolder, true);
        }

        [Fact]
        public void CreateDocument_ShouldAddDocumentToManager()
        {
            // Arrange
            string fileName = "testdocument.txt";
            string fullPath = Path.Combine(testLocalFilesFolder, fileName);
            Document doc = new Document
            {
                FileName = fullPath,
                Content = "Hello World"
            };

            // Act
            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());

            // Assert: проверяем, что документ присутствует, сравниваем базовые имена (без расширения)
            var allDocs = documentManager.GetAllDocuments();
            Assert.Contains(allDocs, d =>
                string.Equals(
                    Path.GetFileNameWithoutExtension(d.FileName),
                    Path.GetFileNameWithoutExtension(fileName),
                    StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void DeleteDocument_ShouldRemoveDocumentAndHistory()
        {
            // Arrange
            string fileName = "tobedeleted.txt";
            string fullPath = Path.Combine(testLocalFilesFolder, fileName);
            Document doc = new Document
            {
                FileName = fullPath,
                Content = "To be deleted"
            };
            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());
            IStorageStrategy storage = new LocalFileStorage();

            // Act
            documentManager.RemoveDocument(doc, storage);

            // Assert: документ должен отсутствовать в локальных документах (проверяем по базовому имени)
            var localDocs = documentManager.GetLocalDocuments();
            Assert.DoesNotContain(localDocs, d =>
                string.Equals(
                    Path.GetFileNameWithoutExtension(d.FileName),
                    Path.GetFileNameWithoutExtension(fileName),
                    StringComparison.OrdinalIgnoreCase));

            // Проверяем, что файл истории также удалён
            string historyPath = Path.Combine(testLocalHistoryFolder,
                Path.GetFileNameWithoutExtension(fileName) + "_history.json");
            Assert.False(File.Exists(historyPath));
        }

        [Fact]
        public void SubscribeToDocumentChanges_ShouldAddNotification()
        {
            // Arrange
            string fileName = "notif.txt";
            string fullPath = Path.Combine(testLocalFilesFolder, fileName);
            Document doc = new Document
            {
                FileName = fullPath,
                Content = "Initial Content"
            };
            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());
            doc.Attach(testUser);

            // Act
            doc.Content = "Updated Content";
            doc.Notify();

            // Assert: проверяем, что уведомление содержит имя файла
            Assert.Contains(testUser.Notifications, note =>
                note.Contains(Path.GetFileName(fileName), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void CreateDocument_WithUniqueBaseName_ShouldAddDocument()
        {
            // Arrange
            string fileName = "uniqueDoc.txt";
            string fullPath = Path.Combine(testLocalFilesFolder, fileName);
            Document doc = new Document
            {
                FileName = fullPath,
                Content = "Unique content"
            };

            // Act
            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());


            var allDocs = documentManager.GetAllDocuments();
            Assert.Contains(allDocs, d =>
                string.Equals(
                    Path.GetFileNameWithoutExtension(d.FileName),
                    Path.GetFileNameWithoutExtension(fileName),
                    StringComparison.OrdinalIgnoreCase));
        }

        //[Fact]
        //public void CreateDocument_WithInvalidExtension_ShouldShowErrorMessage_Simple()
        //{

        //    string invalidFileName = "badformat.exe";
        //    string simulatedInput = invalidFileName + Environment.NewLine;
        //    using (var sr = new StringReader(simulatedInput))
        //    using (var sw = new StringWriter())
        //    {
        //        Console.SetIn(sr);
        //        Console.SetOut(sw);

        //        var userActionsMenu = new TesterHelperUserActionsMenu(testUser, documentManager, accessManager, userManager);
        //        try
        //        {
        //            userActionsMenu.CreateNewFile();
        //        }
        //        catch (IOException)
        //        {
        //        }

        //        string output = sw.ToString();
        //        Assert.Contains("Неверное расширение файла. Допустимые расширения: .txt, .md, .rtf, .json, .xml", output);
        //    }
        //}

        //[Fact]
        //public void CreateDocument_WithExistingBaseName_ShouldShowErrorMessage_Simple()
        //{
        //    string fileName1 = "duplicate.txt";
        //    string simulatedInput1 = fileName1 + Environment.NewLine;
        //    using (var sr1 = new StringReader(simulatedInput1))
        //    using (var sw1 = new StringWriter())
        //    {
        //        Console.SetIn(sr1);
        //        Console.SetOut(sw1);
        //        var menu = new TesterHelperUserActionsMenu(testUser, documentManager, accessManager, userManager);
        //        try
        //        {
        //            menu.CreateNewFile();
        //        }
        //        catch (IOException)
        //        {
                    
        //        }
        //    }

        //    string fileName2 = "duplicate.md";
        //    string simulatedInput2 = fileName2 + Environment.NewLine;
        //    using (var sr2 = new StringReader(simulatedInput2))
        //    using (var sw2 = new StringWriter())
        //    {
        //        Console.SetIn(sr2);
        //        Console.SetOut(sw2);
        //        var menu = new TesterHelperUserActionsMenu(testUser, documentManager, accessManager, userManager);
        //        try
        //        {
        //            menu.CreateNewFile();
        //        }
        //        catch (IOException)
        //        {
        //        }
        //        string output = sw2.ToString();
        //        Assert.Contains("Документ с таким базовым именем уже существует.", output);
        //    }
        //}

        [Fact]
        public void ImportLocalFile_WithValidData_ShouldAddDocument_Simple()
        {
            string tempFileName = "import_test.txt";
            string tempFilePath = Path.Combine(testLocalFilesFolder, tempFileName);
            File.WriteAllText(tempFilePath, "Test content");
            Document doc = new Document
            {
                FileName = tempFileName,
                Content = File.ReadAllText(tempFilePath)
            };

            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());

            var importedDoc = documentManager.GetLocalDocuments()
                .FirstOrDefault(d =>
                    string.Equals(
                        Path.GetFileNameWithoutExtension(d.FileName),
                        Path.GetFileNameWithoutExtension(tempFileName),
                        StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(importedDoc);
            Assert.Equal("Test content", importedDoc.Content);
        }

        [Fact]
        public void ImportLocalFile_WithValidData_ShouldAddDocument()
        {
            string tempFileName = "import_test.txt";
            string tempFilePath = Path.Combine(testLocalFilesFolder, tempFileName);
            File.WriteAllText(tempFilePath, "Test content");
            Document doc = new Document
            {
                FileName = tempFileName,
                Content = File.ReadAllText(tempFilePath)
            };

            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());

            var importedDoc = documentManager.GetLocalDocuments()
                .FirstOrDefault(d =>
                    string.Equals(
                        Path.GetFileNameWithoutExtension(d.FileName),
                        Path.GetFileNameWithoutExtension(tempFileName),
                        StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(importedDoc);
            Assert.Equal("Test content", importedDoc.Content);
        }

        [Fact]
        public void ImportLocalFile_WithValidData_ShouldAddDoc()
        {
            string tempFileName = "import_test.txt";
            string tempFilePath = Path.Combine(testLocalFilesFolder, tempFileName);
            File.WriteAllText(tempFilePath, "Test content");
            Document doc = new Document
            {
                FileName = tempFileName,
                Content = File.ReadAllText(tempFilePath)
            };

            documentManager.CreateDocument(doc, testUser, userManager.GetUsers());

            var importedDoc = documentManager.GetLocalDocuments()
                .FirstOrDefault(d =>
                    string.Equals(
                        Path.GetFileNameWithoutExtension(d.FileName),
                        Path.GetFileNameWithoutExtension(tempFileName),
                        StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(importedDoc);
            Assert.Equal("Test content", importedDoc.Content);
        }
    }
    public class TesterHelperUserActionsMenu : UserActionsMenu
    {
        public TesterHelperUserActionsMenu(User user, DocumentManager docManager, DocumentAccessManager accessMgr, UserManager userMgr)
            : base(user, docManager, accessMgr, userMgr)
        {
        }

        public void CreateNewFile()
        {
            MethodInfo method = typeof(UserActionsMenu)
                .GetMethod("CreateNewFile", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(this, null);
        }
    }
}
