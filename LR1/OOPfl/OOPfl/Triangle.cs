namespace OOPfl
{
    public class Triangle : Shape
    {
        public int BaseLength { get; private set; }
        public int Height { get; private set; }

        public Triangle(int id, int baseLength, int height, char fillChar) : base(id, fillChar)
        {
            BaseLength = baseLength;
            Height = height;
        }

        public override string Info()
        {
            return $"[ID {Id}] Треугольник ({StartPosition.X}, {StartPosition.Y}), Основание: {BaseLength}, Высота: {Height}, '{FillChar}', {(IsVisible ? "Видим" : "Скрыт")}";
        }
    }
}