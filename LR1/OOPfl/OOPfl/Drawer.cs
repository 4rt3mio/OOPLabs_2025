namespace OOPfl
{
    public class Drawer
    {
        private Canvas canvas;

        public Drawer(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void Draw()
        {
            canvas.FillBackground();
            foreach (var shape in canvas.GetShapes())
            {
                if (shape.IsVisible)
                {
                    DrawShape(shape);
                }
            }
            char[,] grid = canvas.GetGrid();
            Console.Clear();

            Console.Write("┌");
            for (int x = 0; x < canvas.Width * 2; x++)
                Console.Write("─");
            Console.WriteLine("┐");

            for (int y = 0; y < canvas.Height; y++)
            {
                Console.Write("│");
                for (int x = 0; x < canvas.Width; x++)
                {
                    Console.Write(grid[y, x]);
                    Console.Write(grid[y, x]);
                }
                Console.WriteLine("│");
            }

            Console.Write("└");
            for (int x = 0; x < canvas.Width * 2; x++)
                Console.Write("─");
            Console.WriteLine("┘");
        }

        private void DrawShape(Shape shape)
        {
            switch (shape)
            {
                case Circle circle:
                    DrawCircle(circle);
                    break;
                case Rectangle rectangle:
                    DrawRectangle(rectangle);
                    break;
                case Triangle triangle:
                    DrawTriangle(triangle);
                    break;
            }
        }

        private void DrawCircle(Circle circle)
        {
            int centerX = circle.StartPosition.X;
            int centerY = circle.StartPosition.Y;
            int radiusX = circle.Radius; 
            int radiusY = circle.Radius;
            char fillChar = circle.FillChar;

            for (int y = -radiusY; y <= radiusY; y++)
            {
                for (int x = -radiusX; x <= radiusX; x++)
                {
                    if ((x * x + y * y) <= (radiusY * radiusY))
                    {
                        canvas.SetElement(centerX + x, centerY + y, fillChar);
                    }
                }
            }
        }

        private void DrawRectangle(Rectangle rectangle)
        {
            int startX = rectangle.StartPosition.X;
            int startY = rectangle.StartPosition.Y;
            int width = rectangle.Width; 
            int height = rectangle.Height;
            char fillChar = rectangle.FillChar;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    canvas.SetElement(startX + x, startY + y, fillChar);
                }
            }
        }

        private void DrawTriangle(Triangle triangle)
        {
            var (ax, ay) = (triangle.PointA.X, triangle.PointA.Y);
            var (bx, by) = (triangle.PointB.X, triangle.PointB.Y);
            var (cx, cy) = (triangle.PointC.X, triangle.PointC.Y);
            char fillChar = triangle.FillChar;

            List<(int x, int y)> borderPoints = new List<(int, int)>();

            borderPoints.AddRange(DrawBresenhamLine(ax, ay, bx, by, fillChar));
            borderPoints.AddRange(DrawBresenhamLine(bx, by, cx, cy, fillChar));
            borderPoints.AddRange(DrawBresenhamLine(cx, cy, ax, ay, fillChar));

            FillTriangle(borderPoints, fillChar);
        }

        private List<(int x, int y)> DrawBresenhamLine(int x1, int y1, int x2, int y2, char fillChar)
        {
            List<(int, int)> points = new List<(int, int)>();

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                canvas.SetElement(x1, y1, fillChar);
                points.Add((x1, y1));

                if (x1 == x2 && y1 == y2)
                    break;

                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }

            return points;
        }

        private void FillTriangle(List<(int x, int y)> borderPoints, char fillChar)
        {
            var groupedByY = borderPoints.GroupBy(p => p.y).OrderBy(g => g.Key);

            foreach (var group in groupedByY)
            {
                int y = group.Key;
                int minX = group.Min(p => p.x);
                int maxX = group.Max(p => p.x);

                for (int x = minX + 1; x < maxX; x++)
                {
                    canvas.SetElement(x, y, fillChar);
                }
            }
        }
    }
}