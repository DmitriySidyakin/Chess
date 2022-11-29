using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Figure : ICloneable
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

        public Figure(byte figureCode)
        {
            if (figureCode == 0)
            {
                var emptyCell = new EmptyCell();

                Side = emptyCell.Side;
                Man = emptyCell.Man;
                SideMan = emptyCell.SideMan;
            }
            else
            {
                Man = (Figures)((figureCode + 1) >> 1);
                Side = (Side)((figureCode + 1) & 1);
                SideMan = (SideFigures)((((byte)Man) << 1 | (byte)Side) - 1);
            }
        }

        public override string ToString()
        {
           string? name = Enum.GetName<SideFigures>(SideMan);

           return name ?? "[Error]";
        }

        public object Clone()
        {
            return new Figure(Side, Man);
        }
    }
}
