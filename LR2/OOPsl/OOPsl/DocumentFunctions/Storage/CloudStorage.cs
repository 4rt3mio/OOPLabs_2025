using OOPsl.DocumentFunctions.Formats;

namespace OOPsl.DocumentFunctions.Storage
{
    public class CloudStorage : IStorageStrategy
    {
        // Абсолютный путь к "облачному" хранилищу (имитация Google Drive)
        private readonly string cloudFolder = @"D:\OOP\LR2\OOPsl\OOPsl\Files\CloudFiles";

        public CloudStorage()
        {
            if (!Directory.Exists(cloudFolder))
            {
                Directory.CreateDirectory(cloudFolder);
            }
        }

        public void Save(Document document)
        {
            try
            {
                string fullPath = Path.Combine(cloudFolder, Path.GetFileName(document.FileName));
                File.WriteAllText(fullPath, document.Content);
                Console.WriteLine($"Документ сохранён в облаке по адресу: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в облако: {ex.Message}");
            }
        }

        public Document Load(string fileName)
        {
            try
            {
                string fullPath = Path.Combine(cloudFolder, fileName);
                if (File.Exists(fullPath))
                {
                    var doc = new PlainTextDocument(fullPath);
                    doc.Content = File.ReadAllText(fullPath);
                    return doc;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке из облака: {ex.Message}");
            }
            return null;
        }
    }
}
