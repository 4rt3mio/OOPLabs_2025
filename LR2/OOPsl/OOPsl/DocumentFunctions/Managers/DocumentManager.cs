namespace OOPsl.DocumentFunctions.Managers
{
    public class DocumentManager
    {
        // Словарь, где ключ – идентификатор стратегии хранения (например, "local", "cloud"),
        // а значение – конкретная реализация IStorageStrategy.
        private Dictionary<string, IStorageStrategy> storageStrategies = new Dictionary<string, IStorageStrategy>();
        private List<Document> documents = new List<Document>();
        private DocumentAccessManager accessManager;

        public DocumentManager(DocumentAccessManager accessManager)
        {
            this.accessManager = accessManager;
            // Инициализируем предустановленные стратегии хранения.
            storageStrategies["local"] = new LocalFileStorage();
            storageStrategies["cloud"] = new CloudStorage();
        }

        // Создание нового документа. При создании документ добавляется в общий список,
        // и через DocumentAccessManager создателю присваивается роль Admin.
        public void CreateDocument(Document document, User creator)
        {
            document.Create();
            documents.Add(document);
            accessManager.AddDocument(document, creator);
            creator.OwnedDocuments.Add(document);
        }

        // Сохранение документа по выбранной стратегии (например, "local" или "cloud").
        public void SaveDocument(Document document, string storageType)
        {
            if (storageStrategies.ContainsKey(storageType))
            {
                storageStrategies[storageType].Save(document);
            }
            else
            {
                throw new ArgumentException($"Стратегия хранения '{storageType}' не найдена.");
            }
        }

        // Загрузка документа из выбранного хранилища.
        public Document LoadDocument(string fileName, string storageType)
        {
            if (storageStrategies.ContainsKey(storageType))
            {
                return storageStrategies[storageType].Load(fileName);
            }
            else
            {
                throw new ArgumentException($"Стратегия хранения '{storageType}' не найдена.");
            }
        }

        // Получение списка всех документов.
        public List<Document> GetAllDocuments()
        {
            return documents;
        }

        // Возможность динамически добавить новую стратегию хранения.
        public void AddStorageStrategy(string key, IStorageStrategy strategy)
        {
            storageStrategies[key] = strategy;
        }
    }
}