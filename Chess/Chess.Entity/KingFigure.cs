using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Король
    public class KingFigure : Figure
    {
        public KingFigure(Side side) : base(side, Figures.King)
        {
        }
    }
}
