namespace OOPfl
{
    public class AddShapeCommand : ShapeCommand
    {
        private Shape shape;

        public AddShapeCommand(Canvas canvas, int shapeId) : base(canvas, shapeId)
        {
            shape = canvas.GetShapeById(shapeId);
        }

        public override void Do()
        {
            shape.IsVisible = true;
        }

        public override void Undo()
        {
            shape.IsVisible = false;
        }
    }
}