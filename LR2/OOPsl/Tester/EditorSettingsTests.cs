using OOPsl.DocumentFunctions.Managers;
using OOPsl.MenuFunctions;
using OOPsl.UserFunctions;
using OOPsl;

namespace Tester
{
    public class EditorSettingsTests : IDisposable
    {
        private readonly TextReader originalIn;
        private readonly TextWriter originalOut;

        public EditorSettingsTests()
        {
            originalIn = Console.In;
            originalOut = Console.Out;
        }

        public void Dispose()
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        [Fact]
        public void ChangeSettings_ShouldUpdateThemeToLight()
        {
            string simulatedInput = "Light" + Environment.NewLine;
            using (var sr = new StringReader(simulatedInput))
            using (var sw = new StringWriter())
            {
                Console.SetIn(sr);
                Console.SetOut(sw);

                var userMgr = new UserManager();
                var accessMgr = new DocumentAccessManager();
                var docMgr = new DocumentManager(accessMgr);
                var menu = new ConsoleMenu(userMgr, docMgr, accessMgr);

                menu.ChangeSettings();

                Assert.Equal("Dark", EditorSettings.Instance.Theme);
            }
        }

        [Fact]
        public void ChangeSettings_ShouldUpdateThemeToDark()
        {
            string simulatedInput = "Dark" + Environment.NewLine;
            using (var sr = new StringReader(simulatedInput))
            using (var sw = new StringWriter())
            {
                Console.SetIn(sr);
                Console.SetOut(sw);

                EditorSettings.Instance.Theme = "Dark";

                var userMgr = new UserManager();
                var accessMgr = new DocumentAccessManager();
                var docMgr = new DocumentManager(accessMgr);
                var menu = new ConsoleMenu(userMgr, docMgr, accessMgr);

                menu.ChangeSettings();

                Assert.Equal("Dark", EditorSettings.Instance.Theme);
            }
        }
    }
}