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
            // TODO Логика отрисовки круга
        }

        private void DrawRectangle(Rectangle rectangle)
        {
            // TODO Логика отрисовки прямоугольника
        }

        private void DrawTriangle(Triangle triangle)
        {
            // TODO Логика отрисовки треугольника
        }
    }
}