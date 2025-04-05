namespace OOPsl.DocumentFunctions.Commands
{
    public class DeleteCommand : ICommand
    {
        private int index;
        private int length;
        private string currentText;
        private string deletedText;
        public string UpdatedText { get; private set; }
        public DeleteCommand(int index, int length, string currentText)
        {
            this.index = index;
            this.length = length;
            this.currentText = currentText;
        }
        public void Execute()
        {
            deletedText = currentText.Substring(index, length);
            UpdatedText = currentText.Remove(index, length);
        }
        public void UnExecute()
        {
            UpdatedText = currentText.Insert(index, deletedText);
        }
    }
}
