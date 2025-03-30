namespace OOPsl
{
    public abstract class Document : ISubject
    {
        public string FileName { get; set; }
        public string Content { get; set; } = string.Empty;
        public abstract void Create();
        public abstract void Open();
        public abstract void Edit();
        public abstract void Delete();
        public abstract void Save();
        public abstract void Load();

        // Реализация паттерна Observer (Subject)
        private List<IObserver> observers = new List<IObserver>();
        public void Attach(IObserver observer) { /* Подписка на уведомления */ }
        public void Detach(IObserver observer) { /* Отписка от уведомлений */ }
        public void Notify() { /* Оповещение подписчиков об изменениях */ }
    }
}