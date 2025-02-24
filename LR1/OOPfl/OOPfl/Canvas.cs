namespace OOPfl
{
    public class Canvas
    {
        public int Width { get; }
        public int Height { get; }

        private char backgroundChar;

        private char[,] grid;

        private List<Shape> shapes;

        public Canvas(int width, int height, char backgroundChar = ' ')
        {
            Width = width;
            Height = height;
            this.backgroundChar = backgroundChar;
            grid = new char[height, width];
            shapes = new List<Shape>();
            FillBackground(); 
        }

        public void FillBackground()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    grid[i, j] = backgroundChar;
                }
            }
        }

        public void Redraw()
        {
            //TODO
        }

        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }

        public void SetElement(int x, int y, char symbol)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                grid[y, x] = symbol;
            }
        }
    }
}