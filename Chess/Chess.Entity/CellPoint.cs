﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class CellPoint : ICloneable
    {
        public sbyte X { get; set; }
        public sbyte Y { get; set; }

        private static CellPoint unexisted  = new CellPoint() { X = -1, Y = -1 };
        public static CellPoint Unexisted { get => unexisted; }

        public object Clone()
        {
            return new CellPoint() { X = X, Y = Y };
        }
    }
}
