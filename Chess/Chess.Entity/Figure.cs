using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Figure
    {
        public Side Side { get; init; }

        public Figures Man { get; init; }

        public SideFigures SideMan { get; }

        public Figure(Side side, Figures man)
        {
            Side = side;
            Man = man;

            SideMan = man == Figures.Empty ? SideFigures.Empty : (SideFigures)((((byte)man) << 1 | (byte)side) - 1);
        }
    }
}
