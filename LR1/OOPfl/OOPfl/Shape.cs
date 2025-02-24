namespace OOPfl
{
    public abstract class Shape
    {
        public Coordinates StartPosition { get; set; }
        public char FillChar { get; set; }
        public int Id { get; private set; } 
        public bool IsVisible { get; set; }

        public Shape(int id)
        {
            Id = id;
            IsVisible = true; 
        }
    }
}
