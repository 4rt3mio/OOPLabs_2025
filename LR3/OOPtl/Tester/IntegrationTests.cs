using System.Text.Json;
using OOPtl.Application.DTOs;
using OOPtl.Application.Services;
using OOPtl.DataAccess.Api;
using OOPtl.DataAccess.Repositories;
using OOPtl.Domain.Entities;
using OOPtl.Domain.Factories;
using OOPtl.Domain.Validators;
using OOPtl.Presentation.Commands;

namespace Tester
{
    public class IntegrationTests
    {
        private const string TestFilePath = "test_students.json";

        // 1. Тесты валидации
        [Theory]
        [InlineData("John Doe", 85)]
        [InlineData("Alice", 0)]
        [InlineData("Bob", 100)]
        public void Validate_WithValidData_DoesNotThrow(string name, int grade)
        {
            var exception = Record.Exception(() => StudentValidator.Validate(name, grade));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("", 100, "Name cannot be empty")]
        [InlineData("Jane Smith", -5, "Grade must be between")]
        [InlineData("Bob", 101, "Grade must be between")]
        [InlineData(null, 50, "Name cannot be empty")]
        public void Validate_WithInvalidData_ThrowsException(string name, int grade, string errorMessage)
        {
            var ex = Assert.Throws<ArgumentException>(() => StudentValidator.Validate(name, grade));
            Assert.Contains(errorMessage, ex.Message);
        }

        [Fact]
        public void Validate_WithNullName_ThrowsException()
        {
            string name = null;
            int grade = 75;

            Assert.Throws<ArgumentException>(() => StudentValidator.Validate(name, grade));
        }

        [Theory]
        [InlineData("  ", 50)]
        [InlineData("\t", 80)]
        public void Validate_WithWhitespaceName_ThrowsException(string name, int grade)
        {
            var ex = Assert.Throws<ArgumentException>(() => StudentValidator.Validate(name, grade));
            Assert.Contains("Name cannot be empty", ex.Message);
        }

        [Fact]
        public void Validate_WithMaxGrade_DoesNotThrow()
        {
            Record.Exception(() => StudentValidator.Validate("Test", 100));
        }

        [Fact]
        public void Validate_WithMinGrade_DoesNotThrow()
        {
            Record.Exception(() => StudentValidator.Validate("Test", 0));
        }

        // 2. Тесты репозитория
        [Fact]
        public void AddStudent_ToEmptyRepository_ShouldContainOneItem()
        {
            CleanupFile();
            var repo = new StudentRepository(TestFilePath);

            repo.Add(new Student("Test", 80));
            var students = repo.GetAll();

            Assert.Single(students);
            CleanupFile();
        }

        [Fact]
        public void UpdateStudent_WithValidIndex_ShouldChangeData()
        {
            CleanupFile();
            var repo = new StudentRepository(TestFilePath);
            repo.Add(new Student("Old", 50));

            repo.Update(0, new Student("New", 90));
            var student = repo.GetAll().First();

            Assert.Equal("New", student.Name);
            CleanupFile();
        }

        [Fact]
        public void UpdateStudent_WithInvalidIndex_ShouldThrowException()
        {
            CleanupFile();
            var repo = new StudentRepository(TestFilePath);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                repo.Update(-1, new Student("Test", 0)));
            CleanupFile();
        }

        // 3. Тесты API
        [Fact]
        public async Task GetRandomQuote_ShouldReturnValidContent()
        {
            var adapter = new QuoteApiAdapter();

            var quote = await adapter.GetRandomQuoteAsync();

            Assert.NotNull(quote.Content);
            Assert.NotEqual("No quote available", quote.Content);
        }

        // 4. Тесты фабрики
        [Fact]
        public void CreateStudent_WithValidData_ShouldMatchInput()
        {
            const string name = "Alice";
            const int grade = 90;

            var student = StudentFactory.Create(name, grade);

            Assert.Equal(name, student.Name);
            Assert.Equal(grade, student.Grade);
        }

        [Theory]
        [InlineData("", 50)]
        [InlineData("Bob", 101)]
        [InlineData(null, 75)]
        public void CreateStudent_WithInvalidData_ShouldThrowException(string name, int grade)
        {
            Assert.Throws<ArgumentException>(() => StudentFactory.Create(name, grade));
        }

        // 5. Тесты сериализации
        [Fact]
        public void Student_Serialization_ShouldPreserveData()
        {
            var student = new Student("Test", 85);

            var json = JsonSerializer.Serialize(student);
            var deserialized = JsonSerializer.Deserialize<Student>(json);

            Assert.Equal(student.Name, deserialized.Name);
            Assert.Equal(student.Grade, deserialized.Grade);
        }

        [Fact]
        public void Repository_FileCreation_ShouldWorkWithNewFile()
        {
            CleanupFile();
            var repo = new StudentRepository(TestFilePath);

            repo.Add(new Student("Test", 75));
            var exists = File.Exists(TestFilePath);

            Assert.True(exists);
            CleanupFile();
        }

        // 6. Интеграционные тесты
        [Fact]
        public async Task FullFlow_AddStudent_ShouldSaveAndShow()
        {
            CleanupFile();
            var repo = new StudentRepository(TestFilePath);
            var api = new QuoteApiAdapter();
            var service = new StudentService(repo, api);

            await service.AddStudentAsync(new StudentDTO("Integration Test", 95));
            var students = service.GetAllStudents();

            Assert.Single(students);
            Assert.Equal("Integration Test", students.First().Name);
            CleanupFile();
        }

        private void CleanupFile()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }
    }
}