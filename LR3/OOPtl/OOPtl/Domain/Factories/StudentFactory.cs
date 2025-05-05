using OOPtl.Domain.Entities;
using OOPtl.Domain.Validators;

namespace OOPtl.Domain.Factories
{
    public static class StudentFactory
    {
        public static Student Create(string name, int grade)
        {
            StudentValidator.Validate(name, grade);
            return new Student(name, grade);
        }
    }
}