namespace OOPfl
{
    public class FillBackgroundCommand : ShapeCommand
    {
        private char newSymbol;
        private char oldSymbol;
        private Shape shape;

        public FillBackgroundCommand(Canvas canvas, int shapeId, char symbol, char oldSymbol) : base(canvas, shapeId)
        {
            this.newSymbol = symbol;
            this.oldSymbol = oldSymbol;
            shape = canvas.GetShapeById(shapeId);
        }

        public override void Do()
        {
            shape.FillChar = newSymbol;
        }

        public override void Undo()
        {
            shape.FillChar = oldSymbol;
        }
    }
}