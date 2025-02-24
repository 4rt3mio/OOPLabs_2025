namespace OOPfl
{
    public class Application
    {
        private Canvas canvas { set; get; }
        private InputManager inputManager;

        public Application(int width, int height, char backgroundChar = ' ')
        {
            canvas = new Canvas(width, height, backgroundChar);
            FileManager fileManager = new FileManager(canvas);
            CommandManager commandManager = new CommandManager();
            inputManager = new InputManager(fileManager, commandManager);
        }

        public void Start()
        {
            // TODO: Основной цикл приложения
        }
    }
}