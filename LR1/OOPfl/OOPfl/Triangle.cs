using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OOPfl
{
    public class Triangle : Shape
    {
        public int BaseLength { get; set; }
        public int Height { get; set; }

        public Triangle(int id, int baseLength, int height) : base(id)
        {
            BaseLength = baseLength;
            Height = height;
        }
    }
}