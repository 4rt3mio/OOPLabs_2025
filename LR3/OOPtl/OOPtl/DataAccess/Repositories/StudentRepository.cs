using OOPtl.Domain.Entities;
using System.Text.Json;

namespace OOPtl.DataAccess.Repositories
{

    public class StudentRepository
    {
        private readonly string _filePath;
        private List<Student> _students;

        public StudentRepository(string filePath)
        {
            _filePath = filePath;
            LoadStudents();
        }

        public void Add(Student student)
        {
            _students.Add(student);
            SaveStudents();
        }

        public List<Student> GetAll() => new List<Student>(_students);

        public void Update(int index, Student student)
        {
            if (index < 0 || index >= _students.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _students[index] = student;
            SaveStudents();
        }

        private void LoadStudents()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _students = JsonSerializer.Deserialize<List<Student>>(json);
            }
            else
            {
                _students = new List<Student>();
            }
        }

        private void SaveStudents()
        {
            var json = JsonSerializer.Serialize(_students);
            File.WriteAllText(_filePath, json);
        }
    }
}
