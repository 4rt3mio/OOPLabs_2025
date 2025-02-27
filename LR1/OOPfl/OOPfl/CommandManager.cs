namespace OOPfl
{
    public class CommandManager
    {
        public Stack<ShapeCommand> executedCommands { set; get; }  
        public Stack<ShapeCommand> undoneCommands { set; get; }

        public CommandManager()
        {
            executedCommands = new Stack<ShapeCommand>();
            undoneCommands = new Stack<ShapeCommand>();
        }
        public void ClearHistory()
        {
            executedCommands.Clear();
            undoneCommands.Clear();
        }

        public void ExecuteCommand(ShapeCommand command)
        {
            command.Do();
            executedCommands.Push(command);
            undoneCommands.Clear(); 
        }

        public void Undo()
        {
            if (executedCommands.Count > 0)
            {
                ShapeCommand command = executedCommands.Pop();
                command.Undo();
                undoneCommands.Push(command);
            }
        }

        public void Redo()
        {
            if (undoneCommands.Count > 0)
            {
                ShapeCommand command = undoneCommands.Pop();
                command.Do();
                executedCommands.Push(command);
            }
        }
    }
}