namespace OOPfl
{
    public class InputManager
    {
        private FileManager fileManager;
        private CommandManager commandManager;
        private Canvas canvas;

        public InputManager(FileManager fileManager, CommandManager commandManager, Canvas canvas)
        {
            this.fileManager = fileManager;
            this.commandManager = commandManager;
            this.canvas = canvas;
        }

        public void ProcessInput(string input)
        {
            // TODO: Разбор строки `input` и вызов нужного менеджера
        }
    }
}