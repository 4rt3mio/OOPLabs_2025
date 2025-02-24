namespace OOPfl
{
    public class Circle : Shape
    {
        public int Radius { get; set; }

        public Circle(int id, int radius) : base(id)
        {
            Radius = radius;
        }
    }
}