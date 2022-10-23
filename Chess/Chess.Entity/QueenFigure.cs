using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Королева
    public class QueenFigure : Figure
    {
        public QueenFigure(Side side) : base(side, Figures.Queen)
        {
        }
    }
}
