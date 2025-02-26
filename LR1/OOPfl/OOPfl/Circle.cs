namespace OOPfl
{
    public class Circle : Shape
    {
        public int Radius { get; private set; }

        public Circle(int id, int radius, char fillChar, Coordinates startPosition)
        : base(id, fillChar, startPosition)
        {
            Radius = radius;
        }

        public override string Info()
        {
            return $"[ID {Id}] Круг ({StartPosition.X}, {StartPosition.Y}), Радиус: {Radius}, '{FillChar}', {(IsVisible ? "Видим" : "Скрыт")}";
        }
    }
}