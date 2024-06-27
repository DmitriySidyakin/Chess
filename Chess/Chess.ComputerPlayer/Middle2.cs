using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class Middle2 : AbstarctMiddle
    {
        public override string Name => "Middle 2";

        public Middle2(Board board) : base(board) { }

        public override Step MakeStep(int deep)
        {
            //TODO: Реализовать какой-нибудь алгоритм с консультацией заядлого шахматиста.

            // В случает отсутствия стратегии сделать простой шаг.
            return base.MakeStep(deep);
        }

    }
}
