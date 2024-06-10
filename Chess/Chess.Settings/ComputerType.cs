using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Settings
{
    public enum ComputerType : byte
    {
        ShortestPath = 0, // Impossible by hardness of computation
        ForKids = 1,
        SimpleComputerPlayer = 2,
    }
}
