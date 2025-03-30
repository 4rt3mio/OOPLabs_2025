namespace OOPsl
{
    public abstract class User : IObserver
    {
        public string Name { get; set; }
        public IUserRoleStrategy RoleStrategy { get; set; }

        public User(string name, IUserRoleStrategy roleStrategy)
        {
            Name = name;
            RoleStrategy = roleStrategy;
        }

        public void OpenDocument(Document document)
        {
            RoleStrategy.OpenDocument(document);
        }

        public void Update(Document document) { }
    }
}
