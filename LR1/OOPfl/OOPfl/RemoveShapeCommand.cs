namespace OOPfl
{
    public class RemoveShapeCommand : ShapeCommand
    {
        private Shape shape;
        public RemoveShapeCommand(Canvas canvas, int shapeId) : base(canvas, shapeId)
        {
            shape = canvas.GetShapeById(shapeId);
        }

        public override void Do()
        {
            shape.IsVisible = false;
        }

        public override void Undo()
        {
            shape.IsVisible = true;
        }
    }
}