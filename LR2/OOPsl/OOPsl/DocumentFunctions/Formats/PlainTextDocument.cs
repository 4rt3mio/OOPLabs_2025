namespace OOPsl.DocumentFunctions.Formats
{
    public class PlainTextDocument : Document
    {
        public PlainTextDocument(string fileName)
        {
            FileName = fileName;
            VersionHistory.Add(fileName);
        }

        public override void Create()
        {
            // Реализация создания документа
        }

        public override void Open()
        {
            // Реализация открытия документа
        }

        public override void Edit()
        {
            // Реализация редактирования документа
        }

        public override void Delete()
        {
            // Реализация удаления документа
        }

        public override void Save()
        {
            // Реализация сохранения документа
        }

        public override void Load()
        {
            // Реализация загрузки документа
        }
    }
}
