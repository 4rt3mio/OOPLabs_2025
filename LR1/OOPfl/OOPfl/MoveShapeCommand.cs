namespace OOPfl
{
    public class MoveShapeCommand : ShapeCommand
    {

        private Coordinates newPosition;
        public MoveShapeCommand(Canvas canvas, int shapeId, Coordinates newPosition) : base(canvas, shapeId)
        {
            this.newPosition = newPosition;
            //TODO
        }

        public override void Do()
        {
            // TODO
        }

        public override void Undo()
        {
            // TODO
        }
    }
}