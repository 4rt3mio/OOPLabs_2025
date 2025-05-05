using OOPtl.Application.DTOs;
using OOPtl.DataAccess.Api;
using OOPtl.DataAccess.Repositories;
using OOPtl.Domain.Entities;
using OOPtl.Domain.Factories;

namespace OOPtl.Application.Services
{
    public class StudentService
    {
        private readonly StudentRepository _repository;
        private readonly QuoteApiAdapter _quoteAdapter;

        public StudentService(StudentRepository repository, QuoteApiAdapter quoteAdapter)
        {
            _repository = repository;
            _quoteAdapter = quoteAdapter;
        }

        public async Task<(StudentDTO student, QuoteDTO quote)> AddStudentAsync(StudentDTO studentDto)
        {
            var student = StudentFactory.Create(studentDto.Name, studentDto.Grade);
            _repository.Add(student);

            var quote = await _quoteAdapter.GetRandomQuoteAsync();
            return (studentDto, quote);
        }

        public List<StudentDTO> GetAllStudents()
        {
            return _repository.GetAll()
                .Select(s => new StudentDTO(s.Name, s.Grade))
                .ToList();
        }

        public void UpdateStudent(int index, StudentDTO studentDto)
        {
            var student = StudentFactory.Create(studentDto.Name, studentDto.Grade);
            _repository.Update(index, student);
        }
    }
}
