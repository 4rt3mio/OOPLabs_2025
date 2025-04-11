using OOPsl.UserFunctions;

namespace Tester
{
    public class UserManagerTests
    {
        private readonly string testUsersFile = Path.Combine(Path.GetTempPath(), "test_users.json");

        private UserManager CreateUserManagerForTest()
        {
            if (File.Exists(testUsersFile))
            {
                File.Delete(testUsersFile);
            }
            return new UserManager(testUsersFile);
        }

        [Fact]
        public void AddUser_ShouldAddUserToUserManager()
        {
            var userManager = CreateUserManagerForTest();
            var user = new RegularUser("TestUser");

            userManager.AddUser(user);

            var users = userManager.GetUsers();
            Assert.Contains(user, users);
        }

        [Fact]
        public void RemoveUser_ShouldRemoveUserFromUserManager()
        {
            var userManager = CreateUserManagerForTest();
            var user = new RegularUser("TestUser");
            userManager.AddUser(user);

            userManager.RemoveUser(user);

            var users = userManager.GetUsers();
            Assert.DoesNotContain(user, users);
        }
    }
}