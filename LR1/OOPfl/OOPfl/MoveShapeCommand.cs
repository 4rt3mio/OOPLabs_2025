namespace OOPfl
{
    public class MoveShapeCommand : ShapeCommand
    {
        private Coordinates startPosition;
        private Coordinates newPosition;
        private Shape shape;
        public MoveShapeCommand(Canvas canvas, int shapeId, Coordinates newPosition) : base(canvas, shapeId)
        {
            this.newPosition = newPosition;
            shape = canvas.GetShapeById(shapeId);
            startPosition = shape.StartPosition;
        }

        public override void Do()
        {
            shape.StartPosition.X = startPosition.X + newPosition.X;
            shape.StartPosition.Y = startPosition.Y + newPosition.Y;
        }

        public override void Undo()
        {
            shape.StartPosition.X = startPosition.X - newPosition.X;
            shape.StartPosition.Y = startPosition.Y - newPosition.Y;
        }
    }
}