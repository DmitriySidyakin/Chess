using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entity
{
    public class Step
    {
        public CellPoint Start { get; }
        public CellPoint End { get; }

        public Step(CellPoint start, CellPoint end)
        {
            Start = start;
            End = end;
        }
    }
}
