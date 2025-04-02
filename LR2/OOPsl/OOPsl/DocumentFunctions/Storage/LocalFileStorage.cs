using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Formats;

namespace OOPsl.DocumentFunctions.Storage
{
    public class LocalFileStorage : IStorageStrategy
    {
        // Абсолютный путь к локальному хранилищу
        private readonly string localFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\LocalFiles";

        public LocalFileStorage()
        {
            if (!Directory.Exists(localFolder))
            {
                Directory.CreateDirectory(localFolder);
            }
        }

        public void Save(Document document)
        {
            try
            {
                // Формируем полный путь, используя имя файла документа
                string fullPath = Path.Combine(localFolder, Path.GetFileName(document.FileName));
                File.WriteAllText(fullPath, document.Content);
                Console.WriteLine($"Документ сохранён локально по адресу: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении локально: {ex.Message}");
            }
        }

        public Document Load(string fileName)
        {
            try
            {
                string fullPath = Path.Combine(localFolder, fileName);
                if (File.Exists(fullPath))
                {
                    // Для примера создаём простой PlainTextDocument
                    var doc = new PlainTextDocument(fullPath);
                    doc.Content = File.ReadAllText(fullPath);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке локально: {ex.Message}");
            }
            return null;
        }
    }
}
