using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPfl
{
    public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(int id, int width, int height) : base(id)
        {
            Width = width;
            Height = height;
        }
    }
}