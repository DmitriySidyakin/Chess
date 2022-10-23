using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class EmptyCell : Figure
    {
        public EmptyCell() : base(Side.White, Figures.Empty)
        {
        }
    }
}
