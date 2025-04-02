using Newtonsoft.Json;
using System.Text.Json;

namespace OOPsl.UserFunctions
{
    public class UserManager
    {
        private List<User> users = new List<User>();
        private const string UsersFile = @"D:\OOP\LR2\OOPsl\OOPsl\Files\users.json";

        public UserManager()
        {
            LoadUsers();
        }

        public void AddUser(User user)
        {
            users.Add(user);
            SaveUsers();
        }

        public void RemoveUser(User user)
        {
            users.Remove(user);
            SaveUsers();
        }

        public User FindUser(string userName)
        {
            return users.FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public List<User> GetUsers()
        {
            return users;
        }

        private void SaveUsers()
        {
            try
            {
                string dir = Path.GetDirectoryName(UsersFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // Сохраняем типы пользователей с помощью TypeNameHandling
                string json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                File.WriteAllText(UsersFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сохранения пользователей: " + ex.Message);
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(UsersFile))
            {
                try
                {
                    string json = File.ReadAllText(UsersFile);
                    users = JsonConvert.DeserializeObject<List<User>>(json,
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) ?? new List<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка загрузки пользователей: " + ex.Message);
                    users = new List<User>();
                }
            }
        }
    }
}