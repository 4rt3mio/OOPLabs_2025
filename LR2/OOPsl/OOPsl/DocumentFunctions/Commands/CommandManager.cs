namespace OOPsl.DocumentFunctions.Commands
{
    public class CommandManager
    {
        private Stack<ICommand> executedCommands = new Stack<ICommand>();
        private Stack<ICommand> undoneCommands = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            executedCommands.Push(command);
        }

        public void Undo() { }
        public void Redo() { }
    }
}
