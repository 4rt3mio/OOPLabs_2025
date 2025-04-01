namespace OOPsl
{
    public class AccessControlEntry
    {
        public User User { get; set; }
        public DocumentRole Role { get; set; }

        public AccessControlEntry(User user, DocumentRole role)
        {
            User = user;
            Role = role;
        }
    }
}
