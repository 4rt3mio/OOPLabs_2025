namespace OOPsl
{
    public interface IStorageStrategy
    {
        void Save(Document document);
        Document Load(string fileName);
    }
}
