namespace OOPfl
{
    public class InputManager
    {
        private FileManager fileManager;
        private CommandManager commandManager;

        public InputManager(FileManager fileManager, CommandManager commandManager)
        {
            this.fileManager = fileManager;
            this.commandManager = commandManager;
        }

        public void ProcessInput(string input)
        {
            // TODO: Разбор строки `input` и вызов нужного менеджера
        }
    }
}