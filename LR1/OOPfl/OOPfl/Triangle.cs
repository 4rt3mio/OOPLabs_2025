namespace OOPfl
{
    public class Triangle : Shape
    {
        public Coordinates PointA { get; private set; }
        public Coordinates PointB { get; private set; }
        public Coordinates PointC { get; private set; }

        public Triangle(int id, Coordinates pointA, Coordinates pointB, Coordinates pointC, char fillChar)
            : base(id, fillChar, pointA)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;
        }

        public override string Info()
        {
            return $"[ID {Id}] Треугольник A({PointA.X}, {PointA.Y}) B({PointB.X}, {PointB.Y}) C({PointC.X}, {PointC.Y}), '{FillChar}', {(IsVisible ? "Видим" : "Скрыт")}";
        }
    }
}