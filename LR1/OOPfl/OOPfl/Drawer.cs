namespace OOPfl
{
    public class Drawer
    {
        private Canvas canvas;
        private const double aspectRatio = 1.0; 

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
            canvas.Redraw();
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
            int radiusX = (int)(circle.Radius * aspectRatio); 
            int radiusY = circle.Radius;
            char fillChar = circle.FillChar;

            for (int y = -radiusY; y <= radiusY; y++)
            {
                for (int x = -radiusX; x <= radiusX; x++)
                {
                    if (((x * x) / (aspectRatio * aspectRatio) + y * y) <= (radiusY * radiusY))
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
            int width = (int)(rectangle.Width * aspectRatio); 
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
            int startX = triangle.StartPosition.X;
            int startY = triangle.StartPosition.Y;
            int baseLength = (int)(triangle.BaseLength * aspectRatio);
            int height = triangle.Height;
            char fillChar = triangle.FillChar;

            List<(int x, int y)> borderPoints = new List<(int, int)>();

            for (int i = 0; i < height; i++)
            {
                canvas.SetElement(startX, startY + i, fillChar);
                borderPoints.Add((startX, startY + i));
            }

            for (int i = 0; i < baseLength; i++)
            {
                canvas.SetElement(startX + i, startY + height - 1, fillChar);
                borderPoints.Add((startX + i, startY + height - 1));
            }


            int x1 = startX, y1 = startY;
            int x2 = startX + baseLength - 1, y2 = startY + height - 1;
            borderPoints.AddRange(DrawBresenhamLine(x1, y1, x2, y2, fillChar));

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