namespace OOPtl.Domain.Entities
{
    public class Student
    {
        public string Name { get; }
        public int Grade { get; }

        public Student(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }
    }
}
