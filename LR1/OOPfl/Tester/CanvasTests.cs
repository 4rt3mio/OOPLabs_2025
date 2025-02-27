using OOPfl;

namespace Tester
{
    public class CanvasTests
    {
        [Fact]
        public void AddShape_ShouldAddShapeToCanvas()
        {
            var canvas = new Canvas(20, 20, '.');
            var shape = new Rectangle(1, 5, 5, '#', new Coordinates(2, 2));

            canvas.AddShape(shape);

            Assert.Contains(shape, canvas.GetShapes());
        }

        [Fact]
        public void Clear_ShouldRemoveAllShapes()
        {
            var canvas = new Canvas(20, 20, '.');
            canvas.AddShape(new Rectangle(1, 5, 5, '#', new Coordinates(2, 2)));
            canvas.AddShape(new Circle(2, 3, '*', new Coordinates(4, 4)));

            canvas.Clear();

            Assert.Empty(canvas.GetShapes());
        }
    }
}