namespace OOPsl.DocumentFunctions.Formats
{
    public class MarkdownDocument : Document
    {
        public MarkdownDocument(string fileName)
        {
            FileName = fileName;
            VersionHistory.Add(fileName);
        }

        public override void Create()
        {
            // Реализация создания Markdown документа 
        }

        public override void Open()
        {
            // Реализация открытия Markdown документа 
        }

        public override void Edit()
        {
            // Реализация редактирования Markdown документа 
        }

        public override void Delete()
        {
            // Реализация удаления Markdown документа 
        }

        public override void Save()
        {
            // Реализация сохранения Markdown документа (при вызове из DocumentManager) 
        }

        public override void Load()
        {
            // Реализация загрузки Markdown документа 
        }
    }
}
