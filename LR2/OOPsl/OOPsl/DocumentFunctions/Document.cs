using OOPsl.DocumentFunctions.Commands;
using OOPsl.UserFunctions;

namespace OOPsl.DocumentFunctions
{
    public abstract class Document : ISubject
    {
        public string FileName { get; set; }
        public string Content { get; set; } = string.Empty;

        // Список версий – каждый элемент содержит полное имя файла версии
        public List<string> VersionHistory { get; set; } = new List<string>();

        public CommandManager CommandManager { get; set; } = new CommandManager();

        public abstract void Create();
        public abstract void Open();
        public abstract void Edit();
        public abstract void Delete();
        public abstract void Save();
        public abstract void Load();

        private List<IObserver> observers = new List<IObserver>();
        public void Attach(IObserver observer) { observers.Add(observer); }
        public void Detach(IObserver observer) { observers.Remove(observer); }
        public void Notify() { foreach (var obs in observers) obs.Update(this); }
    }
}