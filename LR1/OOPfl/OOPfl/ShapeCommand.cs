using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPfl
{
    public abstract class ShapeCommand
    {
        protected Canvas canvas;
        protected int shapeId;

        public ShapeCommand(Canvas canvas, int shapeId)
        {
            this.canvas = canvas;
            this.shapeId = shapeId;
        }

        public abstract void Do();

        public abstract void Undo();
    }
}