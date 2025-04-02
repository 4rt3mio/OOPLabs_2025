using OOPsl.DocumentFunctions;

namespace OOPsl.DocumentFunctions.Storage
{
    public interface IStorageStrategy
    {
        void Save(Document document);
        Document Load(string fileName);
    }
}
