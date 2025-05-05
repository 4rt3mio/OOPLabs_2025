using OOPtl.Application.DTOs;
using OOPtl.Application.Interfaces;
using OOPtl.Application.Services;

namespace OOPtl.Presentation.Commands
{
    public class EditStudentCommand : ICommand
    {
        private readonly StudentService _service;

        public EditStudentCommand(StudentService service)
        {
            _service = service;
        }

        public async Task ExecuteAsync()
        {
            var students = _service.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("No students to edit");
                return;
            }

            Console.WriteLine("Select student to edit:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"[{i}] {students[i].Name} - Grade: {students[i].Grade}");
            }

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= students.Count)
            {
                Console.WriteLine("Invalid index");
                return;
            }

            Console.Write("New name: ");
            var newName = Console.ReadLine();

            Console.Write("New grade: ");
            if (!int.TryParse(Console.ReadLine(), out int newGrade))
            {
                Console.WriteLine("Invalid grade");
                return;
            }

            try
            {
                _service.UpdateStudent(index, new StudentDTO(newName, newGrade));
                Console.WriteLine("Student updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
