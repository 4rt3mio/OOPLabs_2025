using OOPtl.Application.Interfaces;
using OOPtl.Application.Services;

namespace OOPtl.Presentation.Commands
{
    public class ViewStudentsCommand : ICommand
    {
        private readonly StudentService _service;

        public ViewStudentsCommand(StudentService service)
        {
            _service = service;
        }

        public Task ExecuteAsync()
        {
            var students = _service.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("No students in records");
                return Task.CompletedTask;
            }

            Console.WriteLine("\nStudent List:");
            foreach (var student in students)
            {
                Console.WriteLine($"- {student.Name} (Grade: {student.Grade})");
            }
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
