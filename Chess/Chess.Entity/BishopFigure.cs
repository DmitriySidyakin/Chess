using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Офицер
    public class BishopFigure : Figure
    {
        public BishopFigure(Side side) : base(side, Figures.Bishop)
        {
        }
    }
}
