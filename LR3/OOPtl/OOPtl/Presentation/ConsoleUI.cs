using OOPtl.Presentation.Commands;

namespace OOPtl.Presentation
{
    public class ConsoleUI
    {
        private readonly AddStudentCommand _addCommand;
        private readonly EditStudentCommand _editCommand;
        private readonly ViewStudentsCommand _viewCommand;

        public ConsoleUI(
            AddStudentCommand addCommand,
            EditStudentCommand editCommand,
            ViewStudentsCommand viewCommand)
        {
            _addCommand = addCommand;
            _editCommand = editCommand;
            _viewCommand = viewCommand;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("\n1. Add Student\n2. Edit Student\n3. View Students\n4. Exit");
                Console.Write("Choose option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await _addCommand.ExecuteAsync();
                        break;
                    case "2":
                        await _editCommand.ExecuteAsync();
                        break;
                    case "3":
                        await _viewCommand.ExecuteAsync();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
        }
    }
}
