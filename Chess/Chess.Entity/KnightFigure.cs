using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Конь
    public class KnightFigure : Figure
    {
        public KnightFigure(Side side) : base(side, Figures.Knight)
        {
        }
    }
}
