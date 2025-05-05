namespace OOPtl.Domain.Validators
{
    public static class StudentValidator
    {
        public static void Validate(string name, int grade)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            if (grade < 0 || grade > 100)
                throw new ArgumentException("Grade must be between 0 and 100");
        }
    }
}
