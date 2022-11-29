using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Logging
{
    public class StepEntity : LogEntity
    {
        public Step Step { get; }

        public Side StartSide { get; }

        public Side EndSide { get; }

        public Figure Figure { get; }

        public bool Eat { get; }

        public bool IsCheck { get; }

        public bool IsMate { get; }

        public bool IsCheckmate { get; }

        public StepEntity(
            Step step,

            Side startSide,
            Side endSide,
            Figure figure,
            bool eat,
            bool isCheck,
            bool isMate,
            bool isCheckmate,
            int id,
            DateTime dateTimeStamp
            ) : base(id, dateTimeStamp)
        {
            Step = (Step)step.Clone();
            StartSide = startSide;
            EndSide = endSide;
            Figure = (Figure)figure.Clone();
            Eat = eat;
            IsCheck = isCheck;
            IsMate = isMate;
            IsCheckmate = isCheckmate;
        }


    }
}
