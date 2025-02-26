namespace OOPfl
{
    public abstract class Shape
    {
        public Coordinates StartPosition { get; set; }
        public char FillChar { get; set; }
        public int Id { get; private set; }
        public bool IsVisible { get; set; }

        public Shape(int id, char fillChar, Coordinates startPosition)
        {
            Id = id;
            FillChar = fillChar;
            StartPosition = startPosition;
            IsVisible = true;
        }

        public abstract string Info();
    }
}
