using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class Middle1 : AbstarctMiddle
    {
        public override string Name => "Middle 1";

        public Middle1(Board board) : base(board) {  }


        public override Step MakeStep(int deep)
        {
            //TODO: Сделать давку офицерами и конями. В конце игра загон под мат/(шах и мат).

            // В случает отсутствия стратегии сделать простой шаг.
            return base.MakeStep(deep);
        }
    }
}
