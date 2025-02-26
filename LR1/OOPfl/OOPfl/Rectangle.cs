namespace OOPfl
{
    public class Rectangle : Shape
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Rectangle(int id, int width, int height, char fillChar) : base(id, fillChar)
        {
            Width = width;
            Height = height;
        }

        public override string Info()
        {
            return $"[ID {Id}] Прямоугольник ({StartPosition.X}, {StartPosition.Y}), {Width}x{Height}, '{FillChar}', {(IsVisible ? "Видим" : "Скрыт")}";
        }
    }
}