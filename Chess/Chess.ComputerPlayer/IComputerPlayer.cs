using Chess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public interface IComputerPlayer
    {
        public string Name { get; }

        public Board CurrentBoard { get; set; }

        public Side CurrentStepSide { get; set; }

        public Step MakeStep(int deep);
    }
}
