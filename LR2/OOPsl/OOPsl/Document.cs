namespace OOPsl
{
    public abstract class Document : ISubject
    {
        public string FileName { get; set; }
        public string Content { get; set; } = string.Empty;

        // Менеджер команд для данного документа
        public CommandManager CommandManager { get; set; } = new CommandManager();

        public abstract void Create();
        public abstract void Open();
        public abstract void Edit();
        public abstract void Delete();
        public abstract void Save();
        public abstract void Load();

        // Реализация Observer (Subject)
        private List<IObserver> observers = new List<IObserver>();
        public void Attach(IObserver observer) { /* Подписка */ }
        public void Detach(IObserver observer) { /* Отписка */ }
        public void Notify() { /* Оповещение подписчиков */ }
    }
}