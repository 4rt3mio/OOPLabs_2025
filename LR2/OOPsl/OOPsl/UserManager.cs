using System.Text.Json;

namespace OOPsl
{
    public class UserManager
    {
        private List<User> users = new List<User>();

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public User FindUser(string userName)
        {
            return users.FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public List<User> GetUsers()
        {
            return users;
        }
    }
}