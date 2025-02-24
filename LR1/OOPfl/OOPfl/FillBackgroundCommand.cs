namespace OOPfl
{
    public class FillBackgroundCommand : ShapeCommand
    {
        private char newSymbol;

        public FillBackgroundCommand(Canvas canvas, int shapeId, char symbol) : base(canvas, shapeId)
        {
            this.newSymbol = symbol;
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