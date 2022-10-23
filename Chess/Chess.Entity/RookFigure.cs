using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Тура
    public class RookFigure : Figure
    {
        public RookFigure(Side side) : base(side, Figures.Rook)
        {
        }
    }
}
