using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    // Пешка
    public class PawnFigure : Figure
    {
        public PawnFigure(Side side) : base(side, Figures.Pawn)
        {
        }
    }
}
